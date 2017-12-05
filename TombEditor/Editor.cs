﻿using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using TombEditor.Geometry;
using TombLib.Utils;
using System.IO;
using System.Diagnostics;
using System.Threading;
using NLog;

namespace TombEditor
{
    public interface IEditorEvent { };

    public interface IEditorPropertyChangedEvent : IEditorEvent { }

    public interface IEditorCameraEvent : IEditorEvent { }

    public interface IEditorEventCausesUnsavedChanges : IEditorEvent { }

    public interface IEditorRoomChangedEvent : IEditorEvent, IEditorEventCausesUnsavedChanges
    {
        Room Room { get; }
    }

    public enum ObjectChangeType
    {
        Add,
        Remove,
        Change
    }

    public interface IEditorObjectChangedEvent : IEditorEvent, IEditorEventCausesUnsavedChanges
    {
        Room Room { get; }
        ObjectInstance Object { get; }
        ObjectChangeType ChangeType { get; }
    }

    public class Editor : IDisposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public event Action<IEditorEvent> EditorEventRaised;

        public void RaiseEvent(IEditorEvent eventObj)
        {
            EditorEventRaised?.Invoke(eventObj);
        }

        // --- State of the editor ---
        // Unfortunately implementing this pattern is slightly elaborate in C#.
        // On the positive side, this allows us to catch any state changes from all known and unknown components
        // therefore being very flexible to future components and improveing state safety by guaranteed updates.

        public class LevelChangedEvent : IEditorPropertyChangedEvent
        {
            public Level Previous { get; internal set; }
            public Level Current { get; internal set; }
        }

        public class LevelCompilationCompletedEvent : IEditorEvent
        {
            public int Boxes { get; set; }
            public int Overlaps { get; set; }
            public int Textures { get; set; }

            public LevelCompilationCompletedEvent(int boxes, int overlaps, int textures)
            {
                Boxes = boxes;
                Overlaps = overlaps;
                Textures = textures;
            }
        }

        private Level _level;
        public Level Level
        {
            get { return _level; }
            set
            {
                if (value == _level)
                    return;

                // Validate level
                int roomCount = value.Rooms.Count((room) => room != null);
                if (roomCount <= 0)
                    value.Rooms[0] = new Room(value, Room.MaxRoomDimensions, Room.MaxRoomDimensions, "Room 0");

                // Reset state that was related to the old level
                SelectedObject = null;
                ChosenItem = null;
                SelectedSectors = SectorSelection.None;
                Action = EditorAction.None;
                SelectedTexture = TextureArea.None;

                // Delete old level after the new level is set
                using (var previousLevel = Level)
                {
                    _level = value;
                    EditorEventRaised?.Invoke(new LevelChangedEvent { Previous = previousLevel, Current = value });
                }
                RoomListChange();
                SelectedRooms = new Room[] { _level.Rooms.First((room) => room != null) };
                ResetCamera();
                LoadedWadsChange(value.Wad);
                LoadedTexturesChange();
                LoadedImportedGeometriesChange();
                LevelFileNameChange();
                HasUnsavedChanges = false;
            }
        }

        public class ActionChangedEvent : IEditorPropertyChangedEvent
        {
            public EditorAction Previous { get; internal set; }
            public EditorAction Current { get; internal set; }
        }
        private EditorAction _action;
        public EditorAction Action
        {
            get { return _action; }
            set
            {
                if (value == _action)
                    return;
                var previous = _action;
                _action = value;
                RaiseEvent(new ActionChangedEvent { Previous = previous, Current = value });
            }
        }

        public class ChosenItemChangedEvent : IEditorPropertyChangedEvent
        {
            public ItemType? Previous { get; internal set; }
            public ItemType? Current { get; internal set; }
        }
        private ItemType? _chosenItem;
        public ItemType? ChosenItem
        {
            get { return _chosenItem; }
            set
            {
                if (value == _chosenItem)
                    return;
                var previous = _chosenItem;
                _chosenItem = value;
                RaiseEvent(new ChosenItemChangedEvent { Previous = previous, Current = value });
            }
        }

        public class ModeChangedEvent : IEditorPropertyChangedEvent
        {
            public EditorMode Previous { get; internal set; }
            public EditorMode Current { get; internal set; }
        }
        private EditorMode _mode = EditorMode.Geometry;
        public EditorMode Mode
        {
            get { return _mode; }
            set
            {
                if (value == _mode)
                    return;
                var previous = _mode;
                _mode = value;
                RaiseEvent(new ModeChangedEvent { Previous = previous, Current = value });
            }
        }

        public class ToolChangedEvent : IEditorPropertyChangedEvent
        {
            public EditorTool Previous { get; internal set; }
            public EditorTool Current { get; internal set; }
        }
        private EditorTool _tool = new EditorTool() { Tool = EditorToolType.Selection, TextureUVFixer = true };
        public EditorTool Tool
        {
            get { return _tool; }
            set
            {
                if (value == _tool)
                    return;
                var previous = _tool;
                _tool = value;
                RaiseEvent(new ToolChangedEvent { Previous = previous, Current = value });
            }
        }

        public class SelectedRoomsChangedEvent : IEditorPropertyChangedEvent
        {
            public IReadOnlyList<Room> Previous { get; internal set; }
            public IReadOnlyList<Room> Current { get; internal set; }
        }
        private Room[] _selectedRooms;
        public IReadOnlyList<Room> SelectedRooms
        {
            get { return _selectedRooms; }
            set
            {
                if (_selectedRooms != null && _selectedRooms.SequenceEqual(value))
                    return;
                if (value.Count < 0)
                    throw new ArgumentException("The selected room list must contain at least 1 room.");
                if (value.Any(room => room == null))
                    throw new ArgumentNullException("The selected room list may not contain null.");
                var roomSet = new HashSet<Room>(new Room[] { null });
                if (value.Any(room => !roomSet.Add(room)))
                    throw new ArgumentNullException("The selected room list may not contain duplicates.");
                var previous = _selectedRooms;
                _selectedRooms = value.ToArray();
                if (previous == null || (previous[0] != _selectedRooms[0]))
                    RaiseEvent(new SelectedRoomChangedEvent(previous, value));
                else
                    RaiseEvent(new SelectedRoomsChangedEvent { Previous = previous, Current = value });
            }
        }
        public bool SelectedRoomsContains(Room room) => Array.IndexOf<Room>(_selectedRooms, room) != -1;

        public class SelectedRoomChangedEvent : SelectedRoomsChangedEvent
        {
            public new Room Previous => base.Previous[0];
            public new Room Current => base.Current[0];
            internal SelectedRoomChangedEvent(IReadOnlyList<Room> previous, IReadOnlyList<Room> current)
            {
                base.Current = current;
                base.Previous = previous;
            }
        }
        public Room SelectedRoom
        {
            get { return _selectedRooms[0]; }
            set
            {
                if (value == _selectedRooms[0])
                    return;
                SelectedRooms = new Room[] { value };
            }
        }

        public bool IsSelectedRoomEvent(IEditorEvent eventObj)
        {
            if (eventObj == null)
                return false;
            var roomEvent = eventObj as IEditorRoomChangedEvent;
            if (roomEvent == null)
                return true;
            return (SelectedRoom != null) && (roomEvent.Room == SelectedRoom);
        }

        public class SelectedObjectChangedEvent : IEditorPropertyChangedEvent
        {
            public ObjectInstance Previous { get; internal set; }
            public ObjectInstance Current { get; internal set; }
        }
        private ObjectInstance _selectedObject;
        public ObjectInstance SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                if (value == _selectedObject)
                    return;
                var previous = _selectedObject;
                _selectedObject = value;
                RaiseEvent(new SelectedObjectChangedEvent { Previous = previous, Current = value });
            }
        }

        public class SelectedSectorsChangedEvent : IEditorPropertyChangedEvent
        {
            public SectorSelection Previous { get; internal set; }
            public SectorSelection Current { get; internal set; }
        }
        private SectorSelection _selectedSectors = SectorSelection.None;
        public SectorSelection SelectedSectors
        {
            get { return _selectedSectors; }
            set
            {
                if (value == _selectedSectors)
                    return;
                var previous = _selectedSectors;
                _selectedSectors = value;
                RaiseEvent(new SelectedSectorsChangedEvent { Previous = previous, Current = value });
            }
        }

        public class SelectedTexturesChangedEvent : IEditorPropertyChangedEvent
        {
            public TextureArea Previous { get; internal set; }
            public TextureArea Current { get; internal set; }
        }
        private TextureArea _selectedTexture = TextureArea.None;
        public TextureArea SelectedTexture
        {
            get { return _selectedTexture; }
            set
            {
                if (value == _selectedTexture)
                    return;
                var previous = _selectedTexture;
                _selectedTexture = value;
                RaiseEvent(new SelectedTexturesChangedEvent { Previous = previous, Current = value });
            }
        }

        public class ConfigurationChangedEvent : IEditorPropertyChangedEvent
        {
            public Configuration Previous { get; internal set; }
            public Configuration Current { get; internal set; }
        }
        private Configuration _Configuration;
        public Configuration Configuration
        {
            get { return _Configuration; }
            set
            {
                if (value == _Configuration)
                    return;
                var previous = _Configuration;
                _Configuration = value;
                RaiseEvent(new ConfigurationChangedEvent { Previous = previous, Current = value });
            }
        }

        public class HasUnsavedChangesChangedEvent : IEditorPropertyChangedEvent
        {
            public bool Previous { get; internal set; }
            public bool Current { get; internal set; }
        }
        private bool _hasUnsavedChanges;
        public bool HasUnsavedChanges
        {
            get { return _hasUnsavedChanges; }
            set
            {
                if (value == _hasUnsavedChanges)
                    return;
                var previous = _hasUnsavedChanges;
                _hasUnsavedChanges = value;
                RaiseEvent(new HasUnsavedChangesChangedEvent { Previous = previous, Current = value });
            }
        }

        // This is invoked if the loaded wads changed for the level.
        public class LoadedWadsChangedEvent : IEditorEvent, IEditorEventCausesUnsavedChanges
        {
            public TombLib.Wad.Wad2 Current { get; internal set; }
        }
        public void LoadedWadsChange(TombLib.Wad.Wad2 wad)
        {
            RaiseEvent(new LoadedWadsChangedEvent { Current = wad });
        }

        // This is invoked if the loaded textures changed for the level.
        public class LoadedTexturesChangedEvent : IEditorEvent, IEditorEventCausesUnsavedChanges { }
        public void LoadedTexturesChange()
        {
            RaiseEvent(new LoadedTexturesChangedEvent { });
        }

        // This is invoked if the loaded imported geometries changed for the level.
        public class LoadedImportedGeometriesChangedEvent : IEditorEvent, IEditorEventCausesUnsavedChanges { }
        public void LoadedImportedGeometriesChange()
        {
            RaiseEvent(new LoadedImportedGeometriesChangedEvent { });
        }

        // This is invoked if the animated texture sets changed for the level.
        public class AnimatedTexturesChanged : IEditorEvent { }
        public void AnimatedTexturesChange()
        {
            RaiseEvent(new AnimatedTexturesChanged { });
        }

        // This is invoked when ever the applied textures in a room change.
        // "null" can be passed, if it is not determinable what room changed.
        public class RoomTextureChangedEvent : IEditorRoomChangedEvent, IEditorEventCausesUnsavedChanges
        {
            public Room Room { get; internal set; }
        }
        public void RoomTextureChange(Room room)
        {
            RaiseEvent(new RoomTextureChangedEvent { Room = room });
        }

        // This is invoked when ever the geometry of the room changed. (eg the room is moved, individual sectors are moved up or down, ...)
        // This is not invoked when other the properties of the room change
        // Textures, room properties like reverbration, objects changed, ...
        public class RoomGeometryChangedEvent : IEditorRoomChangedEvent, IEditorEventCausesUnsavedChanges
        {
            public Room Room { get; internal set; }
        }
        public void RoomGeometryChange(Room room)
        {
            RaiseEvent(new RoomGeometryChangedEvent { Room = room });
        }

        // This is invoked when the level is saved an the file name changed.
        public class LevelFileNameChangedEvent : IEditorEvent { }
        public void LevelFileNameChange()
        {
            RaiseEvent(new LevelFileNameChangedEvent { });
        }

        // This is invoked when the amount of rooms is changed. (Rooms have been added or removed)
        // "null" can be passed, if it is not determinable what room changed.
        public class RoomListChangedEvent : IEditorEvent, IEditorEventCausesUnsavedChanges { }
        public void RoomListChange()
        {
            RaiseEvent(new RoomListChangedEvent { });
        }

        // This is invoked for all changes to room flags, "Reverbration", ...
        // "null" can be passed, if it is not determinable what room changed.
        public class RoomPropertiesChangedEvent : IEditorRoomChangedEvent, IEditorEventCausesUnsavedChanges
        {
            public Room Room { get; internal set; }
        }
        public void RoomPropertiesChange(Room room)
        {
            if (room == null)
                throw new ArgumentNullException();
            RaiseEvent(new RoomPropertiesChangedEvent { Room = room });
        }

        // This is invoked for all changes to sectors. (eg setting a trigger, adding a portal, setting a sector to monkey, ...)
        // "null" can be passed, if it is not determinable what room changed.
        public class RoomSectorPropertiesChangedEvent : IEditorRoomChangedEvent, IEditorEventCausesUnsavedChanges
        {
            public Room Room { get; internal set; }
        }
        public void RoomSectorPropertiesChange(Room room)
        {
            if (room == null)
                throw new ArgumentNullException();
            RaiseEvent(new RoomSectorPropertiesChangedEvent { Room = room });
        }

        // This is invoked for all changes to objects. (eg changing a light, changing a movable, moving a static, ...)
        // "null" can be passed, if it is not determinable what object changed.
        public class ObjectChangedEvent : IEditorObjectChangedEvent, IEditorEventCausesUnsavedChanges
        {
            public Room Room { get; internal set; }
            public ObjectInstance Object { get; internal set; }
            public ObjectChangeType ChangeType { get; internal set; }
        }
        public void ObjectChange(ObjectInstance @object, ObjectChangeType changeType)
        {
            ObjectChange(@object, changeType, @object.Room);
        }
        public void ObjectChange(ObjectInstance @object, ObjectChangeType changeType, Room room)
        {
            if (room == null || @object == null)
                throw new ArgumentNullException();
            RaiseEvent(new ObjectChangedEvent { Room = room, Object = @object, ChangeType = changeType });
        }

        // Move the camera to the center of a specific sector.
        public class MoveCameraToSectorEvent : IEditorCameraEvent
        {
            public DrawingPoint Sector { get; set; }
        }
        public void MoveCameraToSector(DrawingPoint sector)
        {
            RaiseEvent(new MoveCameraToSectorEvent { Sector = sector });
        }

        // Center the camera inside the current room.
        public class ResetCameraEvent : IEditorCameraEvent { }
        public void ResetCamera()
        {
            RaiseEvent(new ResetCameraEvent { });
        }

        // Select a texture and center the view
        public class SelectTextureAndCenterViewEvent : IEditorEvent
        {
            public TextureArea Texture { get; internal set; }
        }
        public void SelectTextureAndCenterView(TextureArea texture)
        {
            RaiseEvent(new SelectTextureAndCenterViewEvent { Texture = texture });
        }

        // Change sector highlights
        public HighlightManager HighlightManager { get; private set; }

        // Notify all components that values of the configuration have changed
        public void ConfigurationChange()
        {
            RaiseEvent(new ConfigurationChangedEvent { Previous = _Configuration, Current = _Configuration });
        }

        // Select a room and center the camera
        public void SelectRoomAndResetCamera(Room newRoom)
        {
            if (SelectedRoom == newRoom)
                return;

            SelectedRoom = newRoom;
            ResetCamera();
        }

        // Select rooms
        public void SelectRooms(IEnumerable<Room> newRooms)
        {
            if (newRooms.FirstOrDefault() != null)
                SelectedRooms = newRooms.ToList();
        }

        // Select rooms and center the camera
        public void SelectRoomsAndResetCamera(IEnumerable<Room> newRooms)
        {
            Room oldRoom = SelectedRoom;
            SelectRooms(newRooms);
            Room newRoom = SelectedRoom;
            if (oldRoom != newRoom)
                ResetCamera();
        }

        // Show an object by going to the room it, selecting it and centering the camera appropriately.
        public void ShowObject(ObjectInstance objectInstance)
        {
            if (SelectedRoom != objectInstance.Room)
                SelectRoomAndResetCamera(objectInstance.Room);
            SelectedObject = objectInstance;
        }

        // Call this methode to easily change the settings of the level.
        // All required update methods will be invoked automatically.
        public void UpdateLevelSettings(LevelSettings newSettings)
        {
            if ((_level == null) || newSettings == null)
                return;

            // Determine what will change when the new settings are applied
            // This has to be done now, because the old state will be lost after the new settings are applied
            bool importedGeometryChanged = !newSettings.ImportedGeometries.SequenceEqual(_level.Settings.ImportedGeometries);
            bool texturesChanged = !newSettings.Textures.SequenceEqual(_level.Settings.Textures);
            bool wadsChanged = newSettings.MakeAbsolute(newSettings.WadFilePath) != _level.Settings.MakeAbsolute(_level.Settings.WadFilePath);
            bool levelFilenameChanged = newSettings.MakeAbsolute(newSettings.LevelFilePath) != _level.Settings.MakeAbsolute(_level.Settings.LevelFilePath);
            bool animatedTexturesChanged = newSettings.AnimatedTextureSets.SequenceEqual(_level.Settings.AnimatedTextureSets);

            // Update the current settings
            _level.ApplyNewLevelSettings(newSettings, (instance) => ObjectChange(instance, ObjectChangeType.Change));

            // Update state
            if (importedGeometryChanged)
                LoadedImportedGeometriesChange();

            if (texturesChanged)
                LoadedTexturesChange();

            if (wadsChanged)
                LoadedWadsChange(_level.Wad);

            if (levelFilenameChanged)
                LevelFileNameChange();

            if (animatedTexturesChanged)
                AnimatedTexturesChange();
        }

        // Configuration
        FileSystemWatcher configurationWatcher = null;
        bool configurationIsLoadedFromFile = false;
        private void ConfigurationWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (Path.GetFullPath(e.FullPath) == Path.GetFullPath(Configuration.FilePath))
            {
                Configuration configuration = Configuration;
                if (!Utils.RetryFor(500, () => configuration = Configuration.Load(Configuration.FilePath)))
                    logger.Warn("Unable to load configuration from '" + Path.GetFullPath(Configuration.FilePath) + "' after it changed.");

                // Update configuration
                SynchronizationContext.Post(o =>
                {
                    try
                    {
                        configurationIsLoadedFromFile = true; // Don't save the configuration again just yet
                        Configuration = configuration;
                    }
                    finally
                    {
                        configurationIsLoadedFromFile = false;
                    }
                }, null);
            }
        }

        private void Editor_EditorEventRaised(IEditorEvent obj)
        {
            // Update configuration watcher
            if (obj is ConfigurationChangedEvent)
            {
                Configuration previous = ((ConfigurationChangedEvent)obj).Previous;
                Configuration current = ((ConfigurationChangedEvent)obj).Current;

                if (!string.Equals(previous?.FilePath ?? "", current?.FilePath ?? "", StringComparison.InvariantCultureIgnoreCase))
                {
                    configurationWatcher?.Dispose();
                    if (!string.IsNullOrEmpty(current?.FilePath))
                    {
                        configurationWatcher = new FileSystemWatcher(Path.GetDirectoryName(current.FilePath), Path.GetFileName(current.FilePath));
                        configurationWatcher.EnableRaisingEvents = true;
                        configurationWatcher.Created += ConfigurationWatcher_Changed;
                        configurationWatcher.Deleted += ConfigurationWatcher_Changed;
                        configurationWatcher.Renamed += ConfigurationWatcher_Changed;
                        configurationWatcher.Changed += ConfigurationWatcher_Changed;
                    }
                }
                if (!configurationIsLoadedFromFile)
                    current?.SaveTry();
            }

            // Update unsaved changes state
            if (obj is IEditorEventCausesUnsavedChanges)
            {
                HasUnsavedChanges = true;
            }

            // Make sure an object that was removed isn't selected
            if ((obj as IEditorObjectChangedEvent)?.ChangeType == ObjectChangeType.Remove)
            {
                SelectedObject = null;
            }
        }

        public void Dispose()
        {
            configurationWatcher?.Dispose();
            HighlightManager?.Dispose();
            Level?.Dispose();
        }

        // Construction
        public SynchronizationContext SynchronizationContext { get; }
        public Editor(SynchronizationContext synchronizationContext, Configuration configuration, Level level)
        {
            if (synchronizationContext == null)
                throw new ArgumentNullException("synchronizationContext");
            SynchronizationContext = synchronizationContext;
            Configuration = configuration;
            Level = level;
            HighlightManager = new HighlightManager(this);

            EditorEventRaised += Editor_EditorEventRaised;
            Editor_EditorEventRaised(new ConfigurationChangedEvent { Current = configuration, Previous = null });
        }

        public Editor(SynchronizationContext synchronizationContext, Configuration configuration)
            : this(synchronizationContext, configuration, Level.CreateSimpleLevel())
        { }

        public static Editor Instance;
    }
}
