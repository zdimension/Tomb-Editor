﻿using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NLog;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using Color = System.Drawing.Color;
using TombLib.IO;

namespace TombEditor.Geometry.IO
{
    public class Prj2Loader
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static Level LoadFromPrj2(string filename, GraphicsDevice device, IWin32Window owner)
        {
            var level = new Level();

            try
            {
                using (var reader = CreatePrjReader(filename))
                {
                    level.FileName = filename;

                    // Read version code (in the future it can be used to have multiple PRJ versions)
                    int versionCode = reader.ReadInt32();

                    // Read resource files
                    ResourceLoader.TryLoadingTexture(level, reader.ReadStringUTF8(), device, owner);
                    ResourceLoader.TryLoadingWad(level, reader.ReadStringUTF8(), device, owner);
                    
                    // Read fillers
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();

                    // Read textures
                    int numTextures = reader.ReadInt32();
                    for (int i = 0; i < numTextures; i++)
                    {
                        var texture = new LevelTexture
                        {
                            Id = reader.ReadInt32(),
                            X = reader.ReadInt16(),
                            Y = reader.ReadInt16(),
                            Width = reader.ReadInt16(),
                            Height = reader.ReadInt16(),
                            Page = reader.ReadInt16()
                        };

                        /*texture.UsageCount =*/
                        reader.ReadInt32();
                        texture.Transparent = reader.ReadBoolean();
                        texture.DoubleSided = reader.ReadBoolean();

                        reader.ReadInt32();
                        reader.ReadInt32();
                        reader.ReadInt32();
                        reader.ReadInt32();
                        reader.ReadInt32();
                        reader.ReadInt32();
                        reader.ReadInt32();
                        reader.ReadInt32();

                        level.TextureSamples.Add(texture.Id, texture);
                    }

                    // Read portals
                    int numPortals = reader.ReadInt32();
                    for (int i = 0; i < numPortals; i++)
                    {
                        var portal = new Portal(0, null)
                        {
                            Id = reader.ReadInt32(),
                            OtherId = reader.ReadInt32(),
                            Room = level.GetOrCreateDummyRoom(reader.ReadInt16()),
                            AdjoiningRoom = level.GetOrCreateDummyRoom(reader.ReadInt16()),
                            Direction = (PortalDirection)reader.ReadByte(),
                            X = reader.ReadByte(),
                            Z = reader.ReadByte(),
                            NumXBlocks = reader.ReadByte(),
                            NumZBlocks = reader.ReadByte()
                        };

                        reader.ReadByte();
                        portal.MemberOfFlippedRoom = reader.ReadBoolean();
                        portal.Flipped = reader.ReadBoolean();
                        portal.OtherIdFlipped = reader.ReadInt32();

                        reader.ReadInt32();
                        reader.ReadInt32();
                        reader.ReadInt32();
                        reader.ReadInt32();

                        level.Portals.Add(portal.Id, portal);
                    }

                    // Read objects
                    int numObjects = reader.ReadInt32();
                    for (int i = 0; i < numObjects; i++)
                    {
                        int objectId = reader.ReadInt32();
                        var objectType = (ObjectInstanceType)reader.ReadByte();
                        var Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        var Room = level.GetOrCreateDummyRoom(reader.ReadInt16());
                        
                        PositionBasedObjectInstance o;
                        switch (objectType)
                        {
                            case ObjectInstanceType.Moveable:
                                var m = new MoveableInstance(objectId, Room);
                                m.WadObjectId = reader.ReadUInt32();
                                m.Ocb = reader.ReadInt16();
                                m.Invisible = reader.ReadBoolean();
                                m.ClearBody = reader.ReadBoolean();
                                m.CodeBits = (byte)(reader.ReadByte() & 0x1f);
                                m.Rotation = reader.ReadSingle();
                                o = m;
                                break;
                            case ObjectInstanceType.Static:
                                var s = new StaticInstance(objectId, Room);
                                s.WadObjectId = reader.ReadUInt32();
                                s.Color = Color.FromArgb(255, reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                                s.Rotation = reader.ReadSingle();
                                o = s;
                                break;
                            case ObjectInstanceType.Camera:
                                var c = new CameraInstance(objectId, Room);
                                c.Fixed = reader.ReadBoolean();
                                o = c;
                                break;
                            case ObjectInstanceType.Sink:
                                var si = new SinkInstance(objectId, Room);
                                si.Strength = reader.ReadInt16();
                                o = si;
                                break;
                            case ObjectInstanceType.SoundSource:
                                var ss = new SoundSourceInstance(objectId, Room);
                                ss.SoundId = reader.ReadInt16();
                                ss.CodeBits = (byte)(reader.ReadByte() & 0x1f);
                                o = ss;
                                break;
                            case ObjectInstanceType.FlyByCamera:
                                var ffc = new FlybyCameraInstance(objectId, Room);
                                ffc.Sequence = reader.ReadByte();
                                ffc.Number = reader.ReadByte();
                                ffc.Timer = reader.ReadUInt16();
                                ffc.Flags = reader.ReadUInt16();
                                ffc.Speed = reader.ReadSingle();
                                ffc.Fov = reader.ReadSingle();
                                ffc.Roll = reader.ReadSingle();
                                ffc.RotationX = reader.ReadSingle();
                                ffc.RotationY = reader.ReadSingle();
                                o = ffc;
                                break;
                            default:
                                logger.Warn("Unknown object type " + objectType + " encountered that can't be loaded.");
                                continue;
                        }
                        
                        reader.ReadInt32();
                        reader.ReadInt32();
                        reader.ReadInt32();
                        reader.ReadInt32();

                        level.Objects.Add(o.Id, o);
                    }

                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();

                    // Read triggers
                    int numTriggers = reader.ReadInt32();
                    for (int i = 0; i < numTriggers; i++)
                    {
                        var o = new TriggerInstance(0, null)
                        {
                            Id = reader.ReadInt32(),
                            X = reader.ReadByte(),
                            Z = reader.ReadByte(),
                            NumXBlocks = reader.ReadByte(),
                            NumZBlocks = reader.ReadByte(),
                            TriggerType = (TriggerType)reader.ReadByte(),
                            TargetType = (TriggerTargetType)reader.ReadByte(),
                            Target = reader.ReadInt32(),
                            Timer = reader.ReadInt16(),
                            OneShot = reader.ReadBoolean(),
                            CodeBits = (byte)(reader.ReadByte() & 0x1f),
                            Room = level.GetOrCreateDummyRoom(reader.ReadInt16())
                        };


                        reader.ReadInt16();
                        reader.ReadInt32();
                        reader.ReadInt32();
                        reader.ReadInt32();

                        level.Triggers.Add(o.Id, o);
                    }

                    // Read rooms
                    int numRooms = reader.ReadInt32();
                    for (int i = 0; i < numRooms; i++)
                    {
                        string roomMagicWord = System.Text.Encoding.ASCII.GetString(reader.ReadBytes(4));

                        bool defined = reader.ReadBoolean();
                        if (!defined)
                        {
                            if (level.Rooms[i] != null)
                                logger.Warn($"Room {i} is used, but is marked as undefined");
                            level.Rooms[i] = null;
                            continue;
                        }

                        var room = level.GetOrCreateDummyRoom(i);
                        room.Name = reader.ReadStringUTF8();
                        room.Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        room.Resize(room.NumXSectors, room.NumZSectors);

                        for (int z = 0; z < room.NumZSectors; z++)
                        {
                            for (int x = 0; x < room.NumXSectors; x++)
                            {
                                var b = new Block(BlockType.Floor, BlockFlags.None)
                                {
                                    Type = (BlockType)reader.ReadByte(),
                                    Flags = (BlockFlags)reader.ReadInt16(),
                                    QAFaces =
                                    {
                                        [0] = reader.ReadInt16(),
                                        [1] = reader.ReadInt16(),
                                        [2] = reader.ReadInt16(),
                                        [3] = reader.ReadInt16()
                                    },
                                    EDFaces =
                                    {
                                        [0] = reader.ReadInt16(),
                                        [1] = reader.ReadInt16(),
                                        [2] = reader.ReadInt16(),
                                        [3] = reader.ReadInt16()
                                    },
                                    WSFaces =
                                    {
                                        [0] = reader.ReadInt16(),
                                        [1] = reader.ReadInt16(),
                                        [2] = reader.ReadInt16(),
                                        [3] = reader.ReadInt16()
                                    },
                                    RFFaces =
                                    {
                                        [0] = reader.ReadInt16(),
                                        [1] = reader.ReadInt16(),
                                        [2] = reader.ReadInt16(),
                                        [3] = reader.ReadInt16()
                                    },
                                    SplitFoorType = reader.ReadByte(),
                                    SplitFloor = reader.ReadBoolean(),
                                    SplitCeilingType = reader.ReadByte(),
                                    SplitCeiling = reader.ReadBoolean(),
                                    RealSplitFloor = reader.ReadByte(),
                                    RealSplitCeiling = reader.ReadByte(),
                                    Climb =
                                    {
                                        [0] = reader.ReadBoolean(),
                                        [1] = reader.ReadBoolean(),
                                        [2] = reader.ReadBoolean(),
                                        [3] = reader.ReadBoolean()
                                    },
                                    FloorOpacity = (PortalOpacity)reader.ReadByte(),
                                    CeilingOpacity = (PortalOpacity)reader.ReadByte(),
                                    WallOpacity = (PortalOpacity)reader.ReadByte(),
                                    FloorPortal = reader.ReadInt32(),
                                    CeilingPortal = reader.ReadInt32(),
                                    WallPortal = reader.ReadInt32(),
                                    IsFloorSolid = reader.ReadBoolean(),
                                    IsCeilingSolid = reader.ReadBoolean(),
                                    NoCollisionFloor = reader.ReadBoolean(),
                                    NoCollisionCeiling = reader.ReadBoolean()
                                };

                                foreach (var f in b.Faces)
                                {
                                    f.Defined = reader.ReadBoolean();
                                    f.Flipped = reader.ReadBoolean();
                                    f.Texture = reader.ReadInt16();
                                    f.Rotation = reader.ReadByte();
                                    f.Transparent = reader.ReadBoolean();
                                    f.DoubleSided = reader.ReadBoolean();
                                    f.Invisible = reader.ReadBoolean();
                                    f.NoCollision = reader.ReadBoolean();
                                    f.TextureTriangle = (TextureTileType)reader.ReadByte();

                                    for (int n = 0; n < 4; n++)
                                        f.RectangleUV[n] = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                                    for (int n = 0; n < 3; n++)
                                        f.TriangleUV[n] = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                                    for (int n = 0; n < 3; n++)
                                        f.TriangleUV2[n] = new Vector2(reader.ReadSingle(), reader.ReadSingle());

                                    reader.ReadInt32();
                                    reader.ReadInt32();
                                    reader.ReadInt32();
                                    reader.ReadInt32();
                                }

                                b.FloorDiagonalSplit = (DiagonalSplit)reader.ReadByte();
                                b.FloorDiagonalSplitType = (DiagonalSplitType)reader.ReadByte();
                                b.CeilingDiagonalSplit = (DiagonalSplit)reader.ReadByte();
                                b.CeilingDiagonalSplitType = (DiagonalSplitType)reader.ReadByte();

                                reader.ReadInt32();
                                reader.ReadInt32();
                                reader.ReadInt32();
                                reader.ReadInt32();

                                room.Blocks[x, z] = b;
                            }
                        }

                        int numLights = reader.ReadInt32();
                        for (int j = 0; j < numLights; j++)
                        {
                            var l = new Light((LightType)reader.ReadByte(), 
                                new Vector3(reader.ReadSingle(), reader.ReadSingle(),  reader.ReadSingle()))
                            {
                                Intensity = reader.ReadSingle(),
                                Color = Color.FromArgb(255, reader.ReadByte(), reader.ReadByte(), reader.ReadByte()),
                                In = reader.ReadSingle(),
                                Out = reader.ReadSingle(),
                                Len = reader.ReadSingle(),
                                Cutoff = reader.ReadSingle(),
                                DirectionX = reader.ReadSingle(),
                                DirectionY = reader.ReadSingle(),
                                Enabled = reader.ReadBoolean(),
                                CastsShadows = reader.ReadBoolean(),
                                IsDynamicallyUsed = reader.ReadBoolean(),
                                IsStaticallyUsed = reader.ReadBoolean()
                            };


                            reader.ReadByte();
                            reader.ReadInt16();

                            room.Lights.Add(l);
                        }

                        room.AmbientLight =
                            Color.FromArgb(255, reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                        room.Flipped = reader.ReadBoolean();
                        room.AlternateRoom = level.GetOrCreateDummyRoom(reader.ReadInt16());
                        room.AlternateGroup = reader.ReadInt16();
                        room.WaterLevel = reader.ReadInt16();
                        room.MistLevel = reader.ReadInt16();
                        room.ReflectionLevel = reader.ReadInt16();
                        room.FlagCold = reader.ReadBoolean();
                        room.FlagDamage = reader.ReadBoolean();
                        room.FlagHorizon = reader.ReadBoolean();
                        room.FlagMist = reader.ReadBoolean();
                        room.FlagOutside = reader.ReadBoolean();
                        room.FlagRain = reader.ReadBoolean();
                        room.FlagReflection = reader.ReadBoolean();
                        room.FlagSnow = reader.ReadBoolean();
                        room.FlagWater = reader.ReadBoolean();
                        room.FlagQuickSand = reader.ReadBoolean();
                        room.Flipped = reader.ReadBoolean();
                        room.BaseRoom = level.GetOrCreateDummyRoom(reader.ReadInt16());
                        room.ExcludeFromPathFinding = reader.ReadBoolean();

                        reader.ReadInt32();
                        reader.ReadInt32();
                        reader.ReadInt32();

                        System.Diagnostics.Debug.Assert(ReferenceEquals(room, level.Rooms[i]));
                    }

                    int numAnimatedSets = reader.ReadInt32();
                    for (int i = 0; i < numAnimatedSets; i++)
                    {
                        var effect = (AnimatexTextureSetEffect)reader.ReadByte();
                        var aSet = new AnimatedTextureSet { Effect = effect };
                        int numAnimatedTextures = reader.ReadInt32();
                        for (int j = 0; j < numAnimatedTextures; j++)
                        {
                            short texturePage = reader.ReadInt16();
                            short textureX = reader.ReadInt16();
                            short textureY = reader.ReadInt16();

                            aSet.Textures.Add(new AnimatedTexture(textureX, textureY, texturePage));
                        }

                        level.AnimatedTextures.Add(aSet);
                    }

                    int numTextureSounds = reader.ReadInt32();
                    for (int i = 0; i < numTextureSounds; i++)
                    {
                        var txtSound = new TextureSound(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16())
                        {
                            Sound = (TextureSounds)reader.ReadByte()
                        };

                        level.TextureSounds.Add(txtSound);
                    }
                    
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                    reader.ReadInt32();
                }

                // Check that there are uninitialized rooms
                foreach (Room room in level.Rooms)
                    if (room != null)
                        if ((room.NumXSectors <= 0) && (room.NumZSectors <= 0))
                            throw new Exception("Room " + level.Rooms.ReferenceIndexOf(room) + " has a sector size of zero. This is invalid. Probably the room was referenced but never initialized.");

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
            
            // Now fill the structures loaded from PRJ2 
            for (int i = 0; i < level.Triggers.Count; i++)
            {
                var trigger = level.Triggers[i];

                for (int x = trigger.X; x < trigger.X + trigger.NumXBlocks; x++)
                {
                    for (int z = trigger.Z; z < trigger.Z + trigger.NumZBlocks; z++)
                    {
                        trigger.Room.Blocks[x, z].Triggers.Add(trigger.Id);
                    }
                }
            }

            foreach (var obj in level.Objects.Values)
            {
                var objectType = obj.Type;
                int objectId = obj.Id;

                switch (objectType)
                {
                    case ObjectInstanceType.Moveable:
                        obj.Room.Moveables.Add(objectId);
                        break;
                    case ObjectInstanceType.Static:
                        obj.Room.Statics.Add(objectId);
                        break;
                    case ObjectInstanceType.Camera:
                        obj.Room.Cameras.Add(objectId);
                        break;
                    case ObjectInstanceType.Sink:
                        obj.Room.Sinks.Add(objectId);
                        break;
                    case ObjectInstanceType.SoundSource:
                        obj.Room.SoundSources.Add(objectId);
                        break;
                    case ObjectInstanceType.FlyByCamera:
                        obj.Room.FlyByCameras.Add(objectId);
                        break;
                }
            }

            // Now build the real geometry and update DirectX buffers
            foreach (var room in level.Rooms.Where(room => room != null))
            {
                room.BuildGeometry();
                room.CalculateLightingForThisRoom();
                room.UpdateBuffers();
            }

            foreach (var portal in level.Portals)
            {
                portal.Value.Room.Portals.Add(portal.Key);
            }

            return level;
        }
        
        private static BinaryReaderEx CreatePrjReader(string filename)
        {
            var reader = new BinaryReaderEx(File.OpenRead(filename));

            // Check file version
            var buffer = reader.ReadBytes(4);
            if (buffer[0] == 0x50 && buffer[1] == 0x52 && buffer[2] == 0x4A && buffer[3] == 0x32)
            {
                // PRJ2 senza compressione
                return reader;
            }
            else if (buffer[0] == 0x5A && buffer[1] == 0x52 && buffer[2] == 0x4A && buffer[3] == 0x32)
            {
                // PRJ2 compresso
                int uncompressedSize = reader.ReadInt32();
                int compressedSize = reader.ReadInt32();
                var projectData = reader.ReadBytes(compressedSize);
                projectData = Utils.DecompressData(projectData);

                var ms = new MemoryStream(projectData);
                ms.Seek(0, SeekOrigin.Begin);

                reader = new BinaryReaderEx(ms);
                reader.ReadInt32();
                return reader;
            }
            else
            {
                throw new NotSupportedException("The header of the *.prj2 file was unrecognizable.");
            }
        }

    }
}
