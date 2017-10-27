﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NLog;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using TombLib.IO;
using Color = System.Drawing.Color;
using TombLib.Utils;
using System.Threading.Tasks;

namespace TombEditor.Geometry.IO
{
    public class PrjLoader
    {
        private static readonly Encoding _encodingCodepageWindows = Encoding.GetEncoding(1252);
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private struct PrjFace
        {
            public short _txtType;
            public short _txtIndex;
            public byte _txtFlags;
            public byte _txtRotation;
            public byte _txtTriangle;
        }

        private struct PrjBlock
        {
            public PrjFace[] _faces;
            public PortalOpacity _floorOpacity;
            public PortalOpacity _ceilingOpacity;
            public PortalOpacity _wallOpacity;
            public bool _hasNoCollisionFloor;
            public bool _hasNoCollisionCeiling;
        }

        private struct PrjTexInfo
        {
            public byte _x;
            public short _y;
            public byte _width;
            public byte _height;
        }

        private struct PrjRoom
        {
            public PrjBlock[,] _blocks;
            public HashSet<int> _portals;
            public short _flipRoom;
            public short _flipGroup;
        }

        private struct PrjPortal
        {
            //public Room _room;
            public Rectangle _area;
            public PortalDirection _direction;
            public short _thisRoomIndex;
            public short _oppositePortalId;
        }

        public static Level LoadFromPrj(string filename, IProgressReporter progressReporter)
        {
            var level = new Level();

            // Setup paths
            level.Settings.LevelFilePath = Path.ChangeExtension(filename, "prj2");

            string gameDirectory = FindGameDirectory(filename);
            progressReporter.ReportProgress(0, "Game directory: " + gameDirectory);
            level.Settings.GameDirectory = level.Settings.MakeRelative(gameDirectory, VariableType.LevelDirectory);

            try
            {
                // Open file
                using (var reader = new BinaryReaderEx(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    progressReporter.ReportProgress(0, "Begin of PRJ import from " + filename);
                    logger.Debug("Opening Winroomedit PRJ file " + filename);

                    // Check if it's a NGLE PRJ
                    bool ngle = false;
                    reader.BaseStream.Seek(reader.BaseStream.Length - 8, SeekOrigin.Begin);
                    var bytesNgle = reader.ReadBytes(4);
                    if (bytesNgle[0] == 0x4E && bytesNgle[1] == 0x47 && bytesNgle[2] == 0x4C && bytesNgle[3] == 0x45)
                    {
                        progressReporter.ReportProgress(1, "This is a NGLE project");
                        logger.Debug("NGLE Project");
                        ngle = true;
                    }

                    reader.BaseStream.Seek(0, SeekOrigin.Begin);

                    // Version
                    reader.ReadBytes(12);

                    // Number of rooms
                    int numRooms = reader.ReadInt32();
                    logger.Debug("Number of rooms: " + numRooms);

                    // Now read the first info about rooms, at the end of the PRJ there will be another block
                    for (int i = 0; i < Level.MaxNumberOfRooms; ++i)
                        level.Rooms[i] = null;

                    var tempRooms = new Dictionary<int, PrjRoom>();
                    var tempPortals = new Dictionary<int, PrjPortal>();

                    progressReporter.ReportProgress(2, "Number of rooms: " + numRooms);

                    for (int i = 0; i < numRooms; i++)
                    {

                        // Room is defined?
                        short defined = reader.ReadInt16();
                        if (defined == 0x01)
                            continue;

                        // Read room's name
                        byte[] roomNameBytes = reader.ReadBytes(80);
                        int roomNameLength = 0;
                        for (; roomNameLength < 80; ++roomNameLength)
                            if (roomNameBytes[roomNameLength] == 0)
                                break;
                        string roomName = _encodingCodepageWindows.GetString(roomNameBytes, 0, roomNameLength);

                        logger.Debug("Room #" + i);
                        logger.Debug("    Name: " + roomName);

                        // Read position
                        int xPos = reader.ReadInt32();
                        int yPos = reader.ReadInt32();
                        int zPos = reader.ReadInt32();
                        int yPos2 = reader.ReadInt32();

                        reader.ReadBytes(6);

                        short numZBlocks = reader.ReadInt16();
                        short numXBlocks = reader.ReadInt16();
                        short posZBlocks = reader.ReadInt16();
                        short posXBlocks = reader.ReadInt16();

                        reader.ReadBytes(2);

                        // Create room
                        var room = new Room(level, numXBlocks, numZBlocks, roomName);
                        room.Position = new Vector3(posXBlocks, yPos / -256.0f, posZBlocks);
                        var tempRoom = new PrjRoom();

                        // Read portals
                        short numPortals = reader.ReadInt16();
                        var portalThings = new short[numPortals];

                        tempRoom._portals = new HashSet<int>();
                        logger.Debug("    Portals: " + numPortals);
                        for (int j = 0; j < numPortals; j++)
                        {
                            portalThings[j] = reader.ReadInt16();
                            tempRoom._portals.Add(portalThings[j]);
                        }
                        for (int j = 0; j < numPortals; j++)
                        {
                            ushort direction = reader.ReadUInt16();
                            short portalZ = reader.ReadInt16();
                            short portalX = reader.ReadInt16();
                            short portalZBlocks = reader.ReadInt16();
                            short portalXBlocks = reader.ReadInt16();
                            reader.ReadInt16();
                            short thisRoomIndex = reader.ReadInt16();
                            short portalOppositeSlot = reader.ReadInt16();

                            reader.ReadBytes(26);

                            PortalDirection directionEnum;
                            switch (direction)
                            {
                                case 0x0001:
                                    directionEnum = PortalDirection.WallNegativeZ;
                                    break;
                                case 0x0002:
                                    directionEnum = PortalDirection.WallNegativeX;
                                    break;
                                case 0x0004:
                                    directionEnum = PortalDirection.Floor;
                                    break;
                                case 0xfffe:
                                    directionEnum = PortalDirection.WallPositiveZ;
                                    break;
                                case 0xfffd:
                                    directionEnum = PortalDirection.WallPositiveX;
                                    break;
                                case 0xfffb:
                                    directionEnum = PortalDirection.Ceiling;
                                    break;
                                default:
                                    progressReporter.ReportWarn("Unknown portal direction value " + direction + " encountered in room #" + i + " '" + roomName + "'");
                                    continue;
                            }

                            if (thisRoomIndex != i)
                                logger.Debug("Portal in room '" + roomName + "' doesn't refer to it's own room. That's probably ok, if it's a flip room.");

                            if (tempPortals.ContainsKey(portalThings[j]))
                            {
                                logger.Debug("Portal in room '" + roomName + "' was already present in the list.");
                                continue;
                            }

                            tempPortals.Add(portalThings[j], new PrjPortal
                            {
                                _area = GetArea(room, 0, portalX, portalZ, portalXBlocks, portalZBlocks),
                                _direction = directionEnum,
                                _thisRoomIndex = thisRoomIndex,
                                _oppositePortalId = portalOppositeSlot
                            });
                        }

                        // Read objects
                        short numObjects = reader.ReadInt16();
                        var objectsThings = new short[numObjects];

                        for (int j = 0; j < numObjects; j++)
                        {
                            objectsThings[j] = reader.ReadInt16();
                        }

                        logger.Debug("    Objects and Triggers: " + numObjects);

                        for (int j = 0; j < numObjects; j++)
                        {
                            short objectType = reader.ReadInt16();
                            short objPosZ = reader.ReadInt16();
                            short objPosX = reader.ReadInt16();
                            short objSizeZ = reader.ReadInt16();
                            short objSizeX = reader.ReadInt16();
                            short objPosY = reader.ReadInt16();
                            var objRoom = reader.ReadInt16();
                            short objSlot = reader.ReadInt16();
                            short objOcb = reader.ReadInt16();
                            short objOrientation = reader.ReadInt16();

                            int objLongX = reader.ReadInt32();
                            int objLongY = reader.ReadInt32();
                            int objLongZ = reader.ReadInt32();

                            short objUnk = reader.ReadInt16();
                            short objFacing = reader.ReadInt16();
                            short objRoll = reader.ReadInt16();
                            short objTint = reader.ReadInt16();
                            short objTimer = reader.ReadInt16();

                            Vector3 position = new Vector3(objLongX, -objLongY - room.WorldPos.Y, objLongZ);

                            switch (objectType)
                            {
                                case 0x0008:
                                    if (objSlot >= 460 && objSlot <= 464)
                                        continue;

                                    int red = objTint & 0x001f;
                                    int green = (objTint & 0x03e0) >> 5;
                                    int blue = (objTint & 0x7c00) >> 10;
                                    Vector4 color = new Vector4(
                                        (red + (red == 0 ? 0.0f : 0.875f)) / 16.0f,
                                        (green + (green == 0 ? 0.0f : 0.875f)) / 16.0f,
                                        (blue + (blue == 0 ? 0.0f : 0.875f)) / 16.0f, 1.0f);

                                    if (objSlot < (ngle ? 520 : 465)) // TODO: a more flexible way to define this
                                    {
                                        var instance = new MoveableInstance()
                                        {
                                            ScriptId = unchecked((ushort)(objectsThings[j])),
                                            CodeBits = (byte)((objOcb >> 1) & 0x1f),
                                            Invisible = (objOcb & 0x0001) != 0,
                                            ClearBody = (objOcb & 0x0080) != 0,
                                            WadObjectId = unchecked((uint)objSlot),
                                            Position = position,
                                            Ocb = objTimer,
                                            RotationY = objFacing * (360.0f / 65535.0f),
                                            Color = color
                                        };
                                        room.AddObject(level, instance);
                                    }
                                    else
                                    {
                                        var instance = new StaticInstance()
                                        {
                                            ScriptId = unchecked((ushort)(objectsThings[j])),
                                            WadObjectId = unchecked((uint)(objSlot - (ngle ? 520 : 465))),
                                            Position = position,
                                            RotationY = objFacing * (360.0f / 65535.0f),
                                            Color = color
                                        };

                                        room.AddObject(level, instance);
                                    }
                                    break;

                                case 0x0010:
                                    short triggerType = reader.ReadInt16();
                                    short triggerItemNumber = reader.ReadInt16();
                                    short triggerTimer = reader.ReadInt16();
                                    short triggerFlags = reader.ReadInt16();
                                    short triggerItemType = reader.ReadInt16();

                                    TriggerType triggerTypeEnum;
                                    switch (triggerType)
                                    {
                                        case 0:
                                            triggerTypeEnum = TriggerType.Trigger;
                                            break;
                                        case 1:
                                            triggerTypeEnum = TriggerType.Pad;
                                            break;
                                        case 2:
                                            triggerTypeEnum = TriggerType.Switch;
                                            break;
                                        case 3:
                                            triggerTypeEnum = TriggerType.Key;
                                            break;
                                        case 4:
                                            triggerTypeEnum = TriggerType.Pickup;
                                            break;
                                        case 5:
                                            triggerTypeEnum = TriggerType.Heavy;
                                            break;
                                        case 6:
                                            triggerTypeEnum = TriggerType.Antipad;
                                            break;
                                        case 7:
                                            triggerTypeEnum = TriggerType.Combat;
                                            break;
                                        case 8:
                                            triggerTypeEnum = TriggerType.Dummy;
                                            break;
                                        case 9:
                                            triggerTypeEnum = TriggerType.Antitrigger;
                                            break;
                                        case 10:
                                            triggerTypeEnum = TriggerType.HeavySwitch;
                                            break;
                                        case 11:
                                            triggerTypeEnum = TriggerType.HeavyAntritrigger;
                                            break;
                                        case 12:
                                            triggerTypeEnum = TriggerType.Monkey;
                                            break;
                                        default:
                                            progressReporter.ReportWarn("Unknown trigger type " + triggerType + " encountered in room #" + i + " '" + roomName + "'");
                                            continue;
                                    }

                                    TriggerTargetType triggerTargetTypeEnum;
                                    switch (triggerItemType)
                                    {
                                        case 0:
                                            triggerTargetTypeEnum = TriggerTargetType.Object;
                                            break;
                                        case 1:
                                            triggerTargetTypeEnum = TriggerTargetType.Camera;
                                            break;
                                        case 2:
                                            triggerTargetTypeEnum = TriggerTargetType.Sink;
                                            break;
                                        case 3:
                                            triggerTargetTypeEnum = TriggerTargetType.FlipMap;
                                            break;
                                        case 4:
                                            triggerTargetTypeEnum = TriggerTargetType.FlipOn;
                                            break;
                                        case 5:
                                            triggerTargetTypeEnum = TriggerTargetType.FlipOff;
                                            break;
                                        case 6:
                                            triggerTargetTypeEnum = TriggerTargetType.Target;
                                            break;
                                        case 7:
                                            triggerTargetTypeEnum = TriggerTargetType.FinishLevel;
                                            break;
                                        case 8:
                                            triggerTargetTypeEnum = TriggerTargetType.PlayAudio;
                                            break;
                                        case 9:
                                            triggerTargetTypeEnum = TriggerTargetType.FlipEffect;
                                            break;
                                        case 10:
                                            triggerTargetTypeEnum = TriggerTargetType.Secret;
                                            break;
                                        case 11:
                                            triggerTargetTypeEnum = TriggerTargetType.ActionNg;
                                            break;
                                        case 12:
                                            triggerTargetTypeEnum = TriggerTargetType.FlyByCamera;
                                            break;
                                        case 13:
                                            triggerTargetTypeEnum = TriggerTargetType.ParameterNg;
                                            break;
                                        case 14:
                                            triggerTargetTypeEnum = TriggerTargetType.FmvNg;
                                            break;
                                        case 15:
                                            triggerTargetTypeEnum = TriggerTargetType.TimerfieldNg;
                                            break;
                                        default:
                                            triggerTargetTypeEnum = TriggerTargetType.FlipEffect;
                                            progressReporter.ReportWarn("Unknown trigger target type " + triggerItemType + " encountered in room #" + i + " '" + roomName + "'");
                                            continue;
                                    }

                                    var trigger = new TriggerInstance(GetArea(room, 1, objPosX, objPosZ, objSizeX, objSizeZ))
                                    {
                                        TriggerType = triggerTypeEnum,
                                        TargetType = triggerTargetTypeEnum,
                                        CodeBits = (byte)((~triggerFlags >> 1) & 0x1f),
                                        OneShot = (triggerFlags & 0x0001) != 0,
                                        Timer = triggerTimer,
                                        TargetData = triggerItemNumber
                                    };
                                    room.AddObject(level, trigger);
                                    break;

                                default:
                                    progressReporter.ReportWarn("Unknown object (first *.prj array) type " + objectType + " encountered in room #" + i + " '" + roomName + "'");
                                    continue;
                            }
                        }

                        room.AmbientLight = new Vector4(reader.ReadByte() / 128.0f, reader.ReadByte() / 128.0f, reader.ReadByte() / 128.0f, 1.0f);
                        reader.ReadByte();

                        short numObjects2 = reader.ReadInt16();
                        var objectsThings2 = new short[numObjects2];

                        logger.Debug("    Lights and other objects: " + numObjects2);

                        for (int j = 0; j < numObjects2; j++)
                        {
                            objectsThings2[j] = reader.ReadInt16();
                        }

                        for (int j = 0; j < numObjects2; j++)
                        {
                            short objectType = reader.ReadInt16();
                            short objPosZ = reader.ReadInt16();
                            short objPosX = reader.ReadInt16();
                            short objSizeZ = reader.ReadInt16();
                            short objSizeX = reader.ReadInt16();
                            short objPosY = reader.ReadInt16();
                            var objRoom = reader.ReadInt16();
                            short objSlot = reader.ReadInt16();
                            short objTimer = reader.ReadInt16();
                            short objOrientation = reader.ReadInt16();

                            int objLongX = reader.ReadInt32();
                            int objLongY = reader.ReadInt32();
                            int objLongZ = reader.ReadInt32();

                            short objUnk = reader.ReadInt16();
                            short objFacing = reader.ReadInt16();
                            short objRoll = reader.ReadInt16();
                            short objSpeed = reader.ReadInt16();
                            short objOcb = reader.ReadInt16();

                            Vector3 position = new Vector3(objLongX, -objLongY - room.WorldPos.Y, objLongZ);

                            switch (objectType)
                            {
                                case 0x4000:
                                case 0x6000:
                                case 0x4200:
                                case 0x5000:
                                case 0x4100:
                                case 0x4020:
                                    // Light
                                    short lightIntensity = reader.ReadInt16();
                                    float lightIn = reader.ReadSingle();
                                    float lightOut = reader.ReadSingle();
                                    float lightX = reader.ReadSingle();
                                    float lightY = reader.ReadSingle();
                                    float lightLen = reader.ReadSingle();
                                    float lightCut = reader.ReadSingle();
                                    byte lightR = reader.ReadByte();
                                    byte lightG = reader.ReadByte();
                                    byte lightB = reader.ReadByte();
                                    byte lightOn = reader.ReadByte();

                                    LightType lightType;
                                    switch (objectType)
                                    {
                                        case 0x4000:
                                            lightType = LightType.Light;
                                            break;
                                        case 0x6000:
                                            lightType = LightType.Shadow;
                                            break;
                                        case 0x4200:
                                            lightType = LightType.Sun;
                                            break;
                                        case 0x5000:
                                            lightType = LightType.Effect;
                                            break;
                                        case 0x4100:
                                            lightType = LightType.Spot;
                                            break;
                                        case 0x4020:
                                            lightType = LightType.FogBulb;
                                            break;
                                        default:
                                            progressReporter.ReportWarn("Unknown light type " + objectType + " found inside *.prj file.");
                                            continue;
                                    }

                                    var light = new LightInstance(lightType)
                                    {
                                        Position = position,
                                        Color = new Vector3(lightR / 128.0f, lightG / 128.0f, lightB / 128.0f),
                                        Enabled = lightOn == 0x01,
                                        InnerRange = lightIn / 1024.0f,
                                        OuterRange = lightOut / 1024.0f,
                                        Intensity = lightIntensity / 8192.0f,
                                    };

                                    // Import light rotation
                                    light.SetArbitaryRotationsYX(lightY + 180, -lightX);

                                    // Spot light's have the inner and outer range swapped with angle in winroomedit
                                    if (lightType == LightType.Spot)
                                    {
                                        light.InnerRange = lightLen / 1024.0f;
                                        light.OuterRange = lightCut / 1024.0f;
                                        light.OuterAngle = lightOut;
                                        light.InnerAngle = lightIn;
                                    }
                                    room.AddObject(level, light);
                                    break;
                                case 0x4c00:
                                    var sound = new SoundSourceInstance()
                                    {
                                        SoundId = unchecked((ushort)objSlot),
                                        Position = position
                                    };

                                    room.AddObject(level, sound);
                                    break;
                                case 0x4400:
                                    var sink = new SinkInstance()
                                    {
                                        ScriptId = unchecked((ushort)(objectsThings2[j])),
                                        Strength = (short)(objTimer / 2),
                                        Position = position
                                    };

                                    room.AddObject(level, sink);
                                    break;
                                case 0x4800:
                                case 0x4080:
                                    var camera = new CameraInstance()
                                    {
                                        ScriptId = unchecked((ushort)(objectsThings2[j])),
                                        Fixed = (objectType == 0x4080),
                                        Position = position
                                    };

                                    room.AddObject(level, camera);
                                    break;
                                case 0x4040:
                                    var flybyCamera = new FlybyCameraInstance()
                                    {
                                        ScriptId = unchecked((ushort)(objectsThings2[j])),
                                        Timer = unchecked((short)objTimer),
                                        Sequence = (byte)((objSlot & 0xe000) >> 13),
                                        Number = (byte)((objSlot & 0x1f00) >> 8),
                                        Fov = (short)(objSlot & 0x00ff),
                                        Roll = objRoll,
                                        Speed = objSpeed / 655.0f,
                                        Position = position,
                                        RotationX = -objUnk,
                                        RotationY = objFacing + 180,
                                        Flags = unchecked((ushort)objOcb)
                                    };

                                    room.AddObject(level, flybyCamera);
                                    break;
                                default:
                                    progressReporter.ReportWarn("Unknown object (second *.prj array) type " + objectType + " encountered in room #" + i + " '" + roomName + "'");
                                    continue;
                            }
                        }

                        tempRoom._flipRoom = reader.ReadInt16();
                        short flags1 = reader.ReadInt16();
                        byte waterLevel = reader.ReadByte();
                        byte mistOrReflectionLevel = reader.ReadByte();
                        byte reverb = reader.ReadByte();
                        tempRoom._flipGroup = (short)(reader.ReadInt16() & 0xff);

                        room.WaterLevel = (byte)((flags1 & 0x0001) != 0 ? waterLevel + 1 : 0);
                        room.Reverberation = (Reverberation)reverb;
                        room.ReflectionLevel = (flags1 & 0x0200) != 0 ? mistOrReflectionLevel : (byte)0;
                        room.MistLevel = (flags1 & 0x0100) != 0 ? mistOrReflectionLevel : (byte)0;
                        room.FlagQuickSand = (flags1 & 0x0004) != 0;
                        room.FlagHorizon = (flags1 & 0x0008) != 0;
                        room.FlagDamage = (flags1 & 0x0010) != 0;
                        room.FlagOutside = (flags1 & 0x0020) != 0;
                        room.FlagNoLensflare = (flags1 & 0x0080) != 0;
                        room.FlagSnow = (flags1 & 0x0400) != 0;
                        room.FlagRain = (flags1 & 0x0800) != 0;

                        // Read blocks
                        tempRoom._blocks = new PrjBlock[numXBlocks, numZBlocks];
                        for (int x = 0; x < room.NumXSectors; x++)
                            for (int z = 0; z < room.NumZSectors; z++)
                            {
                                short blockType = reader.ReadInt16();
                                short blockFlags1 = reader.ReadInt16();
                                short blockYfloor = reader.ReadInt16();
                                short blockYceiling = reader.ReadInt16();

                                var block = room.Blocks[x, z];
                                switch (blockType)
                                {
                                    case 0x01:
                                    case 0x05:
                                    case 0x07:
                                    case 0x03:
                                        block.Type = BlockType.Floor;
                                        break;
                                    case 0x1e:
                                        block.Type = BlockType.BorderWall;
                                        break;
                                    case 0x0e:
                                        block.Type = BlockType.Wall;
                                        break;
                                    case 0x06:
                                        block.Type = BlockType.BorderWall;
                                        break;
                                    default:
                                        block.Type = BlockType.Floor;
                                        break;
                                }

                                block.QAFaces[Block.FaceXpZn] = (short)(reader.ReadSByte() + blockYfloor);
                                block.QAFaces[Block.FaceXnZn] = (short)(reader.ReadSByte() + blockYfloor);
                                block.QAFaces[Block.FaceXnZp] = (short)(reader.ReadSByte() + blockYfloor);
                                block.QAFaces[Block.FaceXpZp] = (short)(reader.ReadSByte() + blockYfloor);

                                block.WSFaces[Block.FaceXpZp] = (short)(reader.ReadSByte() + blockYceiling);
                                block.WSFaces[Block.FaceXnZp] = (short)(reader.ReadSByte() + blockYceiling);
                                block.WSFaces[Block.FaceXnZn] = (short)(reader.ReadSByte() + blockYceiling);
                                block.WSFaces[Block.FaceXpZn] = (short)(reader.ReadSByte() + blockYceiling);

                                block.EDFaces[Block.FaceXpZn] = (short)(reader.ReadSByte() + blockYfloor);
                                block.EDFaces[Block.FaceXnZn] = (short)(reader.ReadSByte() + blockYfloor);
                                block.EDFaces[Block.FaceXnZp] = (short)(reader.ReadSByte() + blockYfloor);
                                block.EDFaces[Block.FaceXpZp] = (short)(reader.ReadSByte() + blockYfloor);

                                block.RFFaces[Block.FaceXpZp] = (short)(reader.ReadSByte() + blockYceiling);
                                block.RFFaces[Block.FaceXnZp] = (short)(reader.ReadSByte() + blockYceiling);
                                block.RFFaces[Block.FaceXnZn] = (short)(reader.ReadSByte() + blockYceiling);
                                block.RFFaces[Block.FaceXpZn] = (short)(reader.ReadSByte() + blockYceiling);

                                if ((blockFlags1 & 0x4000) != 0)
                                    block.Flags |= BlockFlags.Monkey;
                                if ((blockFlags1 & 0x0020) != 0)
                                    block.Flags |= BlockFlags.Box;
                                if ((blockFlags1 & 0x0010) != 0)
                                    block.Flags |= BlockFlags.DeathFire;
                                if ((blockFlags1 & 0x0200) != 0)
                                    block.Flags |= BlockFlags.ClimbNegativeX;
                                if ((blockFlags1 & 0x0100) != 0)
                                    block.Flags |= BlockFlags.ClimbNegativeZ;
                                if ((blockFlags1 & 0x0080) != 0)
                                    block.Flags |= BlockFlags.ClimbPositiveX;
                                if ((blockFlags1 & 0x0040) != 0)
                                    block.Flags |= BlockFlags.ClimbPositiveZ;

                                // Read temp blocks that contain texturing informations that will be needed later
                                var tempBlock = new PrjBlock { _faces = new PrjFace[14] };
                                for (int j = 0; j < 14; j++)
                                {
                                    tempBlock._faces[j] = new PrjFace
                                    {
                                        _txtType = reader.ReadInt16(),
                                        _txtIndex = reader.ReadByte(),
                                        _txtFlags = reader.ReadByte(),
                                        _txtRotation = reader.ReadByte(),
                                        _txtTriangle = reader.ReadByte()
                                    };
                                    reader.ReadInt16();
                                }

                                if ((x == 0 || z == 0 || x == room.NumXSectors - 1 || z == room.NumZSectors - 1))
                                {
                                    if ((blockFlags1 & 0x0008) != 0)
                                        tempBlock._wallOpacity = (blockFlags1 & 0x1000) != 0 ? PortalOpacity.TraversableFaces : PortalOpacity.SolidFaces;
                                }
                                else
                                {
                                    if ((blockFlags1 & 0x0002) != 0)
                                        tempBlock._floorOpacity = (blockFlags1 & 0x0800) != 0 ? PortalOpacity.TraversableFaces : PortalOpacity.SolidFaces;

                                    if ((blockFlags1 & 0x0004) != 0)
                                        tempBlock._ceilingOpacity = (blockFlags1 & 0x0400) != 0 ? PortalOpacity.TraversableFaces : PortalOpacity.SolidFaces;
                                }

                                // Read more flags
                                short blockFlags2 = reader.ReadInt16();
                                short blockFlags3 = reader.ReadInt16();

                                tempBlock._hasNoCollisionFloor = (blockFlags2 & 0x06) != 0;
                                tempBlock._hasNoCollisionCeiling = (blockFlags2 & 0x18) != 0;

                                if ((blockFlags2 & 0x0040) != 0)
                                    block.Flags |= BlockFlags.Beetle;
                                if ((blockFlags2 & 0x0020) != 0)
                                    block.Flags |= BlockFlags.TriggerTriggerer;
                                block.FloorSplitDirectionToggled = (blockFlags3 & 0x1) != 0;

                                tempRoom._blocks[x, z] = tempBlock;
                            }

                        room.NormalizeRoomY();

                        // Add room
                        tempRooms.Add(i, tempRoom);
                        level.Rooms[i] = room;

                        progressReporter.ReportProgress(i / ((float)numRooms) * 28.0f, "");
                    }
                    progressReporter.ReportProgress(30, "Rooms loaded");

                    // Link portals
                    progressReporter.ReportProgress(31, "Link portals");
                    foreach (var tempRoom in tempRooms)
                    {
                        Room room = level.Rooms[tempRoom.Key];
                        foreach (var portalId in tempRoom.Value._portals)
                        {
                            PrjPortal prjPortal = tempPortals[portalId];

                            // Link to the opposite room
                            if (!tempPortals.ContainsKey(prjPortal._oppositePortalId))
                            {
                                progressReporter.ReportWarn("A portal in room '" + room + "' refers to an invalid opposite portal.");
                                continue;
                            }
                            Room adjoiningRoom = level.Rooms[tempPortals[prjPortal._oppositePortalId]._thisRoomIndex];
                            PortalInstance portal = new PortalInstance(prjPortal._area, prjPortal._direction, adjoiningRoom);

                            // Figure out opacity of the portal
                            portal.Opacity = PortalOpacity.None;
                            switch (portal.Direction)
                            {
                                case PortalDirection.Ceiling:
                                    for (int z = portal.Area.Y; z <= portal.Area.Bottom; z++)
                                        for (int x = portal.Area.X; x <= portal.Area.Right; x++)
                                            if (tempRoom.Value._blocks[x, z]._ceilingOpacity > portal.Opacity)
                                                portal.Opacity = tempRoom.Value._blocks[x, z]._ceilingOpacity;

                                    // Special case in winroomedit. Portals are set to be traversable ignoring the Opacity setting if
                                    // the water flag differs.
                                    if (((room.WaterLevel != 0) != (portal.AdjoiningRoom.WaterLevel != 0)) /*&& (portal.Opacity == PortalOpacity.SolidFaces)*/)
                                        portal.Opacity = PortalOpacity.TraversableFaces;
                                    break;
                                case PortalDirection.Floor:
                                    for (int z = portal.Area.Y; z <= portal.Area.Bottom; z++)
                                        for (int x = portal.Area.X; x <= portal.Area.Right; x++)
                                            if (tempRoom.Value._blocks[x, z]._floorOpacity > portal.Opacity)
                                                portal.Opacity = tempRoom.Value._blocks[x, z]._floorOpacity;

                                    // Special case in winroomedit. Portals are set to be traversable ignoring the Opacity setting if
                                    // the water flag differs.
                                    if (((room.WaterLevel != 0) != (portal.AdjoiningRoom.WaterLevel != 0)) /*&& (portal.Opacity == PortalOpacity.SolidFaces)*/)
                                        portal.Opacity = PortalOpacity.TraversableFaces;
                                    break;
                                default:
                                    for (int z = portal.Area.Y; z <= portal.Area.Bottom; z++)
                                        for (int x = portal.Area.X; x <= portal.Area.Right; x++)
                                            if (tempRoom.Value._blocks[x, z]._wallOpacity > portal.Opacity)
                                                portal.Opacity = tempRoom.Value._blocks[x, z]._wallOpacity;
                                    break;
                            }

                            // Add portal to rooms
                            room.AddObjectAndSingularPortal(level, portal);
                        }
                    }

                    progressReporter.ReportProgress(32, "Portals linked");

                    // Link triggers
                    {
                        progressReporter.ReportProgress(31, "Link triggers");

                        // Build lookup table for IDs
                        Dictionary<ushort, PositionBasedObjectInstance> objectLookup =
                            level.Rooms.Where(room => room != null)
                            .SelectMany(room => room.Objects)
                            .Where(instance => instance is IHasScriptID)
                            .ToDictionary(instance => ((IHasScriptID)instance).ScriptId.Value);

                        // Lookup objects from IDs for all triggers
                        foreach (Room room in level.Rooms.Where(room => room != null))
                            foreach (var instance in room.Triggers.ToList())
                                switch (instance.TargetType)
                                {
                                    case TriggerTargetType.Object:
                                    case TriggerTargetType.Target:
                                    case TriggerTargetType.Camera:
                                    case TriggerTargetType.FlyByCamera:
                                    case TriggerTargetType.Sink:
                                        ushort triggerTargetId = unchecked((ushort)(instance.TargetData));
                                        if (!objectLookup.ContainsKey(triggerTargetId))
                                        {
                                            progressReporter.ReportWarn("Trigger '" + instance + "' in '" + instance.Room + "' refers to an object with ID " + triggerTargetId + " that is unavailable.");
                                            room.RemoveObject(level, instance);
                                            continue;
                                        }

                                        instance.TargetObj = objectLookup[triggerTargetId];
                                        instance.TargetData = 0;

                                        // Sinks and cameras are classified as 'object's most of time for some reason.
                                        // We have to fix that.
                                        if (instance.TargetObj is FlybyCameraInstance)
                                            instance.TargetType = TriggerTargetType.FlyByCamera;
                                        if (instance.TargetObj is SinkInstance)
                                            instance.TargetType = TriggerTargetType.Sink;
                                        if (instance.TargetObj is CameraInstance)
                                            instance.TargetType = TriggerTargetType.Camera;
                                        break;
                                }
                        progressReporter.ReportProgress(35, "Triggers linked");
                    }

                    // Link flip rooms
                    progressReporter.ReportProgress(37, "Link flip rooms");
                    foreach (var tempRoom in tempRooms)
                    {
                        Room room = level.Rooms[tempRoom.Key];

                        if (tempRoom.Value._flipRoom != -1)
                        {
                            Room alternateRoom = level.Rooms[tempRoom.Value._flipRoom];

                            room.AlternateRoom = alternateRoom;
                            room.AlternateGroup = tempRoom.Value._flipGroup;
                            alternateRoom.AlternateBaseRoom = room;
                            alternateRoom.AlternateGroup = tempRoom.Value._flipGroup;
                            alternateRoom.Position = new Vector3(room.Position.X, alternateRoom.Position.Y, room.Position.Z);
                        }
                    }

                    // Transform the no collision tiles into the ForceFloorSolid option.
                    progressReporter.ReportProgress(40, "Convert NoCollision to ForceFloorSolid");
                    foreach (var tempRoom in tempRooms)
                    {
                        Room room = level.Rooms[tempRoom.Key];
                        for (int z = 0; z < room.NumZSectors; ++z)
                            for (int x = 0; x < room.NumXSectors; ++x)
                            {
                                Room.RoomConnectionInfo connectionInfo = room.GetFloorRoomConnectionInfo(new DrawingPoint(x, z));
                                switch (connectionInfo.AnyType)
                                {
                                    case Room.RoomConnectionType.TriangularPortalXnZn:
                                    case Room.RoomConnectionType.TriangularPortalXpZn:
                                    case Room.RoomConnectionType.TriangularPortalXnZp:
                                    case Room.RoomConnectionType.TriangularPortalXpZp:
                                        if (!tempRoom.Value._blocks[x, z]._hasNoCollisionFloor)
                                            room.Blocks[x, z].ForceFloorSolid = true;
                                        break;
                                }
                            }
                    }

                    // Ignore unused things indices
                    int dwNumThings = reader.ReadInt32(); // number of things in the map
                    int dwMaxThings = reader.ReadInt32(); // always 2000
                    reader.ReadBytes(dwMaxThings * 4);

                    int dwNumLights = reader.ReadInt32(); // number of lights in the map
                    reader.ReadBytes(768 * 4);

                    int dwNumTriggers = reader.ReadInt32(); // number of triggers in the map
                    reader.ReadBytes(512 * 4);

                    // Read texture
                    bool isTextureNA;
                    LevelTexture texture;
                    {
                        var stringBuffer = new byte[255];
                        int sb = 0;
                        while (true)
                        {
                            byte s = reader.ReadByte();
                            if (s == 0x20)
                                break;
                            if (s == 0x00)
                                continue;
                            stringBuffer[sb] = s;
                            sb++;
                        }

                        string textureFilename = _encodingCodepageWindows.GetString(stringBuffer);
                        isTextureNA = textureFilename.StartsWith("NA");
                        if (string.IsNullOrEmpty(textureFilename) || isTextureNA)
                            texture = new LevelTexture();
                        else
                            texture = new LevelTexture(level.Settings, level.Settings.MakeRelative(Utils.TryFindAbsolutePath(
                                level.Settings, textureFilename.Trim('\0', ' ')), VariableType.LevelDirectory), true);
                        if (texture.Image.Width != 256)
                            texture.SetConvert512PixelsToDoubleRows(level.Settings, false); // Only use this compatibility thing if actually needed
                        level.Settings.Textures.Add(texture);
                        ResourceLoader.CheckLoadedTexture(level.Settings, texture, progressReporter);
                        progressReporter.ReportProgress(50, "Loaded texture '" + texture.Path + "'");
                    }

                    // Read texture tiles
                    var tempTextures = new List<PrjTexInfo>();
                    if (!isTextureNA)
                    {
                        int numTextures = reader.ReadInt32();

                        progressReporter.ReportProgress(52, "Loading textures");
                        progressReporter.ReportProgress(52, "    Number of textures: " + numTextures);

                        for (int t = 0; t < numTextures; t++)
                        {
                            var tmpTxt = new PrjTexInfo
                            {
                                _x = reader.ReadByte(),
                                _y = reader.ReadInt16()
                            };

                            reader.ReadInt16();
                            tmpTxt._width = reader.ReadByte();
                            reader.ReadByte();
                            tmpTxt._height = reader.ReadByte();

                            tempTextures.Add(tmpTxt);
                        }
                    }

                    // Read WAD path
                    {
                        var stringBuffer = new byte[255];
                        int sb = 0;
                        while (true)
                        {
                            byte s = reader.ReadByte();
                            if (s == 0x20)
                                break;
                            if (s == 0x00)
                                continue;
                            stringBuffer[sb] = s;
                            sb++;
                        }
                        string wadName = _encodingCodepageWindows.GetString(stringBuffer);
                        if (string.IsNullOrEmpty(wadName) || wadName.StartsWith("NA"))
                            level.Settings.WadFilePath = "";
                        else
                            level.Settings.WadFilePath = level.Settings.MakeRelative(Utils.TryFindAbsolutePath(
                                level.Settings, Path.ChangeExtension(wadName.Trim('\0', ' '), "wad")), VariableType.LevelDirectory);
                    }

                    // Setup paths to customized fonts and the skys
                    if (!string.IsNullOrEmpty(level.Settings.WadFilePath))
                    {
                        string objectFilePath = level.Settings.MakeAbsolute(level.Settings.WadFilePath);

                        string fontPcFilePath = Path.Combine(Path.GetDirectoryName(objectFilePath), "Font.pc");
                        if (File.Exists(fontPcFilePath))
                            level.Settings.FontTextureFilePath = level.Settings.MakeRelative(fontPcFilePath, VariableType.LevelDirectory);

                        string wadSkyFilePath = Path.ChangeExtension(objectFilePath, "raw");
                        string genericSkyFilePath = Path.Combine(Path.GetDirectoryName(objectFilePath), "pcsky.raw");
                        if (File.Exists(wadSkyFilePath))
                            level.Settings.SkyTextureFilePath = level.Settings.MakeRelative(wadSkyFilePath, VariableType.LevelDirectory);
                        else if (File.Exists(genericSkyFilePath))
                            level.Settings.SkyTextureFilePath = level.Settings.MakeRelative(genericSkyFilePath, VariableType.LevelDirectory);

                        string soundPath = Path.Combine(Path.GetDirectoryName(objectFilePath), "../../sound/Samples");
                        level.Settings.OldWadSoundPaths[2].Path = level.Settings.MakeRelative(soundPath, VariableType.LevelDirectory);
                    }

                    // Read WAD file
                    ResourceLoader.TryLoadingObjects(level, progressReporter);
                    progressReporter.ReportProgress(60, "Loaded WAD '" + level.Settings.WadFilePath + "'");

                    // Write slots
                    const bool writeSlots = false;
                    using (var writerSlots = writeSlots ? new StreamWriter("slots.txt") : null)
                    {
                        int numSlots = reader.ReadInt32();
                        for (int i = 0; i < numSlots; i++)
                        {
                            short slotType = reader.ReadInt16();
                            if (slotType == 0x00)
                            {
                                writerSlots?.WriteLine(i + "\t" + "NOT DEFINED");
                                continue;
                            }

                            var stringBuffer = new byte[255];
                            int sb = 0;
                            while (true)
                            {
                                byte s = reader.ReadByte();
                                if (s == 0x20)
                                    break;
                                if (s == 0x00)
                                    continue;
                                stringBuffer[sb] = s;
                                sb++;
                            }

                            string slotName = _encodingCodepageWindows.GetString(stringBuffer);
                            slotName = slotName.Replace('\0', ' ').Trim();

                            int objectId = reader.ReadInt32();

                            reader.ReadBytes(108);
                            writerSlots?.WriteLine(i + "\t" + slotName + "\t" + slotType + "\t" + objectId);
                        }
                    }

                    // Read animated textures
                    progressReporter.ReportProgress(61, "Loading animated textures and texture sounds");
                    int numAnimationRanges = reader.ReadInt32();
                    for (int i = 0; i < 40; i++)
                        reader.ReadInt32();
                    for (int i = 0; i < 256; i++)
                        reader.ReadInt32();

                    for (int i = 0; i < 40; i++)
                    {
                        int defined = reader.ReadInt32();
                        int firstTexture = reader.ReadInt32();
                        int lastTexture = reader.ReadInt32();

                        if (defined != 1)
                            continue;

                        int TODO_ANIMATED_TEXTURE_IMPORT;
                        /*var aSet = new AnimatedTextureSet();

                        for (int j = firstTexture; j <= lastTexture; j++)
                        {
                            int txtY = j / 4;
                            int txtX = j % 4;

                            AnimatedTexture aTexture = new AnimatedTexture(txtX, txtY, 64, 64);
                            aSet.Textures.Add(aTexture);
                        }

                        level.AnimatedTextures.Add(aSet);*/
                    }

                    // Read texture sounds
                    texture.ResizeTextureSounds(4, 64);
                    for (int i = 0; i < 256; i++)
                    {
                        TextureSound textureSound = (TextureSound)(reader.ReadByte() & 0xf);
                        texture.SetTextureSound(i % 4, i / 4, textureSound);
                    }

                    // Build geometry
                    progressReporter.ReportProgress(80, "Building geometry");
                    foreach (var room in level.Rooms.Where(room => room != null))
                        room.BuildGeometry();

                    // Build faces
                    progressReporter.ReportProgress(85, "Texturize faces");
                    for (int i = 0; i < level.Rooms.GetLength(0); i++)
                    {
                        var room = level.Rooms[i];
                        if (room == null)
                            continue;


                        for (int z = 0; z < room.NumZSectors; z++)
                            for (int x = 0; x < room.NumXSectors; x++)
                            {
                                var prjBlock = tempRooms[i]._blocks[x, z];

                                // 0: BLOCK_TEX_FLOOR
                                LoadTextureArea(room, x, z, BlockFace.Floor, texture, tempTextures, prjBlock._faces[0]);

                                // 1: BLOCK_TEX_CEILING
                                LoadTextureArea(room, x, z, BlockFace.Ceiling, texture, tempTextures, prjBlock._faces[1]);

                                // 2: BLOCK_TEX_N4 (North QA)
                                if (room.IsFaceDefined(x, z, BlockFace.NegativeX_QA) ||
                                    room.IsFaceDefined(x, z, BlockFace.NegativeX_ED))
                                {
                                    if ((room.IsFaceDefined(x, z, BlockFace.NegativeX_QA) &&
                                        room.IsFaceDefined(x, z, BlockFace.NegativeX_ED)) ||
                                        !IsUndefinedButHasArea(room, x, z, BlockFace.NegativeX_QA))
                                    {
                                        LoadTextureArea(room, x, z, BlockFace.NegativeX_ED, texture, tempTextures, prjBlock._faces[10]);
                                    }
                                }
                                else
                                {
                                    if (x > 0)
                                        if ((room.IsFaceDefined(x - 1, z, BlockFace.PositiveX_QA) &&
                                            room.IsFaceDefined(x- 1, z , BlockFace.PositiveX_ED)) ||
                                            !IsUndefinedButHasArea(room, x- 1, z , BlockFace.PositiveX_QA))
                                        {
                                            LoadTextureArea(room, x - 1, z, BlockFace.PositiveX_ED, texture, tempTextures, prjBlock._faces[10]);
                                        }
                                }


                                // 3: BLOCK_TEX_N1 (North RF)
                                if (room.IsFaceDefined(x, z, BlockFace.NegativeX_RF) ||
                                    room.IsFaceDefined(x, z, BlockFace.NegativeX_WS))
                                {
                                    if (room.IsFaceDefined(x, z, BlockFace.NegativeX_RF) &&
                                        !room.IsFaceDefined(x, z, BlockFace.NegativeX_WS))
                                    {
                                        LoadTextureArea(room, x, z, BlockFace.NegativeX_RF, texture, tempTextures, prjBlock._faces[3]);
                                    }
                                    else if (!room.IsFaceDefined(x, z, BlockFace.NegativeX_RF) &&
                                        room.IsFaceDefined(x, z, BlockFace.NegativeX_WS))
                                    {
                                        LoadTextureArea(room, x, z, BlockFace.NegativeX_WS, texture, tempTextures, prjBlock._faces[3]);
                                    }
                                    else
                                    {
                                        LoadTextureArea(room, x, z, BlockFace.NegativeX_RF, texture, tempTextures, prjBlock._faces[3]);
                                    }
                                }
                                else
                                {
                                    if (x > 0)
                                        if (room.IsFaceDefined(x - 1, z, BlockFace.PositiveX_RF) &&
                                            !room.IsFaceDefined(x - 1, z, BlockFace.PositiveX_WS))
                                        {
                                            LoadTextureArea(room, x - 1, z, BlockFace.PositiveX_RF, texture, tempTextures, prjBlock._faces[3]);
                                        }
                                        else if (!room.IsFaceDefined(x - 1, z, BlockFace.PositiveX_RF) &&
                                            room.IsFaceDefined(x - 1, z, BlockFace.PositiveX_WS))
                                        {
                                            LoadTextureArea(room, x - 1, z, BlockFace.PositiveX_WS, texture, tempTextures, prjBlock._faces[3]);
                                        }
                                        else
                                        {
                                            LoadTextureArea(room, x - 1, z, BlockFace.PositiveX_RF, texture, tempTextures, prjBlock._faces[3]);
                                        }
                                }

                                // 4: BLOCK_TEX_N3 (North middle)
                                if (room.IsFaceDefined(x, z, BlockFace.NegativeX_Middle))
                                {
                                    LoadTextureArea(room, x, z, BlockFace.NegativeX_Middle, texture, tempTextures, prjBlock._faces[4]);
                                }
                                else
                                {
                                    if (x > 0)
                                        LoadTextureArea(room, x - 1, z, BlockFace.PositiveX_Middle, texture, tempTextures, prjBlock._faces[4]);
                                }

                                // 5: BLOCK_TEX_W4 (West QA)
                                if (room.IsFaceDefined(x, z, BlockFace.NegativeZ_QA) ||
                                    room.IsFaceDefined(x, z, BlockFace.NegativeZ_ED))
                                {
                                    if ((room.IsFaceDefined(x, z, BlockFace.NegativeZ_QA) &&
                                        room.IsFaceDefined(x, z, BlockFace.NegativeZ_ED)) ||
                                        !IsUndefinedButHasArea(room, x, z, BlockFace.NegativeZ_QA))
                                    {
                                        LoadTextureArea(room, x, z, BlockFace.NegativeZ_ED, texture, tempTextures, prjBlock._faces[12]);
                                    }
                                }
                                else
                                {
                                    if (z > 0)
                                        if ((room.IsFaceDefined(x, z - 1, BlockFace.PositiveZ_QA) &&
                                            room.IsFaceDefined(x, z - 1, BlockFace.PositiveZ_ED)) ||
                                            !IsUndefinedButHasArea(room, x, z - 1, BlockFace.PositiveZ_QA))
                                        {
                                            LoadTextureArea(room, x, z - 1, BlockFace.PositiveZ_ED, texture, tempTextures, prjBlock._faces[12]);
                                        }
                                }

                                // 6: BLOCK_TEX_W1 (West RF)
                                if (room.IsFaceDefined(x, z, BlockFace.NegativeZ_RF) ||
                                    room.IsFaceDefined(x, z, BlockFace.NegativeZ_WS))
                                {
                                    if (room.IsFaceDefined(x, z, BlockFace.NegativeZ_RF) &&
                                        !room.IsFaceDefined(x, z, BlockFace.NegativeZ_WS))
                                    {
                                        LoadTextureArea(room, x, z, BlockFace.NegativeZ_RF, texture, tempTextures, prjBlock._faces[6]);
                                    }
                                    else if (!room.IsFaceDefined(x, z, BlockFace.NegativeZ_RF) &&
                                         room.IsFaceDefined(x, z, BlockFace.NegativeZ_WS))
                                    {
                                        LoadTextureArea(room, x, z, BlockFace.NegativeZ_WS, texture, tempTextures, prjBlock._faces[6]);
                                    }
                                    else
                                    {
                                        LoadTextureArea(room, x, z, BlockFace.NegativeZ_RF, texture, tempTextures, prjBlock._faces[6]);
                                    }
                                }
                                else
                                {
                                    if (z > 0)
                                        if (room.IsFaceDefined(x, z - 1, BlockFace.PositiveZ_RF) &&
                                            !room.IsFaceDefined(x, z - 1, BlockFace.PositiveZ_WS))
                                        {
                                            LoadTextureArea(room, x, z - 1, BlockFace.PositiveZ_RF, texture, tempTextures, prjBlock._faces[6]);
                                        }
                                        else if (!room.IsFaceDefined(x, z - 1, BlockFace.PositiveZ_RF) &&
                                             room.IsFaceDefined(x, z - 1, BlockFace.PositiveZ_WS))
                                        {
                                            LoadTextureArea(room, x, z - 1, BlockFace.PositiveZ_WS, texture, tempTextures, prjBlock._faces[6]);
                                        }
                                        else
                                        {
                                            LoadTextureArea(room, x, z - 1, BlockFace.PositiveZ_RF, texture, tempTextures, prjBlock._faces[6]);
                                        }
                                }

                                // 7: BLOCK_TEX_W3 (West middle)
                                if (room.IsFaceDefined(x, z, BlockFace.NegativeZ_Middle))
                                {
                                    LoadTextureArea(room, x, z, BlockFace.NegativeZ_Middle, texture, tempTextures, prjBlock._faces[7]);
                                }
                                else
                                {
                                    if (z > 0)
                                        LoadTextureArea(room, x, z - 1, BlockFace.PositiveZ_Middle, texture, tempTextures, prjBlock._faces[7]);
                                }

                                // 8: BLOCK_TEX_F_NENW (Floor Triangle 2)
                                LoadTextureArea(room, x, z, BlockFace.FloorTriangle2, texture, tempTextures, prjBlock._faces[8]);

                                // 9: BLOCK_TEX_C_NENW (Ceiling Triangle 2)
                                LoadTextureArea(room, x, z, BlockFace.CeilingTriangle2, texture, tempTextures, prjBlock._faces[9]);

                                // 10: BLOCK_TEX_N5 (North ED)
                                if (room.IsFaceDefined(x, z, BlockFace.NegativeX_QA) ||
                                   room.IsFaceDefined(x, z, BlockFace.NegativeX_ED))
                                {
                                    if (room.IsFaceDefined(x, z, BlockFace.NegativeX_QA) &&
                                        !room.IsFaceDefined(x, z, BlockFace.NegativeX_ED))
                                    {
                                        LoadTextureArea(room, x, z, BlockFace.NegativeX_QA, texture, tempTextures, prjBlock._faces[2]);
                                    }
                                    else if (!room.IsFaceDefined(x, z, BlockFace.NegativeX_QA) &&
                                             room.IsFaceDefined(x, z, BlockFace.NegativeX_ED) &&
                                             IsUndefinedButHasArea(room, x, z, BlockFace.NegativeX_QA))
                                    {
                                        LoadTextureArea(room, x, z, BlockFace.NegativeX_ED, texture, tempTextures, prjBlock._faces[2]);
                                    }
                                    else
                                    {
                                        LoadTextureArea(room, x, z, BlockFace.NegativeX_QA, texture, tempTextures, prjBlock._faces[2]);
                                    }
                                }
                                else
                                {
                                    if (x > 0)
                                        if (room.IsFaceDefined(x - 1, z, BlockFace.PositiveX_QA) &&
                                            !room.IsFaceDefined(x - 1, z, BlockFace.PositiveX_ED))
                                        {
                                            LoadTextureArea(room, x - 1, z, BlockFace.PositiveX_QA, texture, tempTextures, prjBlock._faces[2]);
                                        }
                                        else if (!room.IsFaceDefined(x - 1, z, BlockFace.PositiveX_QA) &&
                                                 room.IsFaceDefined(x - 1, z, BlockFace.PositiveX_ED) &&
                                                 IsUndefinedButHasArea(room, x - 1, z, BlockFace.PositiveX_QA))
                                        {
                                            LoadTextureArea(room, x - 1, z, BlockFace.PositiveX_ED, texture, tempTextures, prjBlock._faces[2]);
                                        }
                                        else
                                        {
                                            LoadTextureArea(room, x - 1, z, BlockFace.PositiveX_QA, texture, tempTextures, prjBlock._faces[2]);
                                        }
                                }

                                // 11: BLOCK_TEX_N2 (North WS)
                                if (room.IsFaceDefined(x, z, BlockFace.NegativeX_RF) ||
                                    room.IsFaceDefined(x, z, BlockFace.NegativeX_WS))
                                {
                                    if (room.IsFaceDefined(x, z, BlockFace.NegativeX_RF) &&
                                        room.IsFaceDefined(x, z, BlockFace.NegativeX_WS))
                                    {
                                        LoadTextureArea(room, x, z, BlockFace.NegativeX_WS, texture, tempTextures, prjBlock._faces[11]);
                                    }
                                }
                                else
                                {
                                    if (x > 0)
                                        if (room.IsFaceDefined(x - 1, z, BlockFace.PositiveX_RF) &&
                                            room.IsFaceDefined(x - 1, z, BlockFace.PositiveX_WS))
                                        {
                                            LoadTextureArea(room, x - 1, z, BlockFace.PositiveX_WS, texture, tempTextures, prjBlock._faces[11]);
                                        }
                                }

                                // 12: BLOCK_TEX_W5
                                if (room.IsFaceDefined(x, z, BlockFace.NegativeZ_QA) ||
                                   room.IsFaceDefined(x, z, BlockFace.NegativeZ_ED))
                                {
                                    if (room.IsFaceDefined(x, z, BlockFace.NegativeZ_QA) &&
                                        !room.IsFaceDefined(x, z, BlockFace.NegativeZ_ED))
                                    {
                                        LoadTextureArea(room, x, z, BlockFace.NegativeZ_QA, texture, tempTextures, prjBlock._faces[5]);
                                    }
                                    else if (!room.IsFaceDefined(x, z, BlockFace.NegativeZ_QA) &&
                                             room.IsFaceDefined(x, z, BlockFace.NegativeZ_ED) &&
                                             IsUndefinedButHasArea(room, x, z, BlockFace.NegativeZ_QA))
                                    {
                                        LoadTextureArea(room, x, z, BlockFace.NegativeZ_ED, texture, tempTextures, prjBlock._faces[5]);
                                    }
                                    else
                                    {
                                        LoadTextureArea(room, x, z, BlockFace.NegativeZ_QA, texture, tempTextures, prjBlock._faces[5]);
                                    }
                                }
                                else
                                {
                                    if (z > 0)
                                        if (room.IsFaceDefined(x, z - 1, BlockFace.PositiveZ_QA) &&
                                            !room.IsFaceDefined(x, z - 1, BlockFace.PositiveZ_ED))
                                        {
                                            LoadTextureArea(room, x, z - 1, BlockFace.PositiveZ_QA, texture, tempTextures, prjBlock._faces[5]);
                                        }
                                        else if (!room.IsFaceDefined(x, z - 1, BlockFace.PositiveZ_QA) &&
                                                 room.IsFaceDefined(x, z - 1, BlockFace.PositiveZ_ED) &&
                                                 IsUndefinedButHasArea(room, x, z - 1, BlockFace.PositiveZ_QA))
                                        {
                                            LoadTextureArea(room, x, z - 1, BlockFace.PositiveZ_ED, texture, tempTextures, prjBlock._faces[5]);
                                        }
                                        else
                                        {
                                            LoadTextureArea(room, x, z - 1, BlockFace.PositiveZ_QA, texture, tempTextures, prjBlock._faces[5]);
                                        }
                                }

                                // 13: BLOCK_TEX_W2 (West WS)
                                if (room.IsFaceDefined(x, z, BlockFace.NegativeZ_RF) ||
                                    room.IsFaceDefined(x, z, BlockFace.NegativeZ_WS))
                                {
                                    if (room.IsFaceDefined(x, z, BlockFace.NegativeZ_RF) &&
                                        room.IsFaceDefined(x, z, BlockFace.NegativeZ_WS))
                                    {
                                        LoadTextureArea(room, x, z, BlockFace.NegativeZ_WS, texture, tempTextures, prjBlock._faces[13]);
                                    }
                                }
                                else
                                {
                                    if (z > 0)
                                        if (room.IsFaceDefined(x, z - 1, BlockFace.PositiveZ_RF) &&
                                            room.IsFaceDefined(x, z - 1, BlockFace.PositiveZ_WS))
                                        {
                                            LoadTextureArea(room, x, z - 1, BlockFace.PositiveZ_WS, texture, tempTextures, prjBlock._faces[13]);
                                        }
                                }
                            }
                    }
                }

                // Update level geometry
                progressReporter.ReportProgress(95, "Building rooms");
                Parallel.ForEach(level.Rooms.Where(r => r != null), room => room.UpdateOnlyGeometry());
                foreach (var room in level.Rooms)
                    if (room != null)
                        room.UpdateBuffers();

                progressReporter.ReportProgress(100, "Level loaded correctly!");

                return level;
            }
            catch
            {
                level.Dispose(); // We log in the level above
                throw;
            }
        }

        private static bool IsUndefinedButHasArea(Room room, int x, int z, BlockFace face)
        {
            var b = room.Blocks[x, z];

            switch (face)
            {
                case BlockFace.PositiveZ_QA:
                    return (!room.IsFaceDefined(x, z, face) &&
                            (b.QAFaces[0] >= b.EDFaces[0] && b.QAFaces[1] >= b.EDFaces[1]) &&
                            !(b.QAFaces[0] == b.EDFaces[0] && b.QAFaces[1] == b.EDFaces[1]));

                case BlockFace.NegativeZ_QA:
                    return (!room.IsFaceDefined(x, z, face) &&
                            (b.QAFaces[3] >= b.EDFaces[3] && b.QAFaces[2] >= b.EDFaces[2]) &&
                            !(b.QAFaces[3] == b.EDFaces[3] && b.QAFaces[2] == b.EDFaces[2]));

                case BlockFace.NegativeX_QA:
                    return (!room.IsFaceDefined(x, z, face) &&
                            (b.QAFaces[3] >= b.EDFaces[3] && b.QAFaces[0] >= b.EDFaces[0]) &&
                            !(b.QAFaces[3] == b.EDFaces[3] && b.QAFaces[0] == b.EDFaces[0]));

                case BlockFace.PositiveX_QA:
                    return (!room.IsFaceDefined(x, z, face) &&
                               (b.QAFaces[1] >= b.EDFaces[1] && b.QAFaces[2] >= b.EDFaces[2]) &&
                               !(b.QAFaces[1] == b.EDFaces[1] && b.QAFaces[2] == b.EDFaces[2]));
            }

            return false;
        }

#pragma warning disable 0675 // Disable warning about bitwise or
        private static void LoadTextureArea(Room room, int x, int z, BlockFace face, LevelTexture levelTexture, List<PrjTexInfo> tempTextures, PrjFace prjFace)
        {
            Block block = room.Blocks[x, z];

            switch (levelTexture == null ? 0 : prjFace._txtType)
            {
                case 0x0000: // TYPE_TEXTURE_NONE
                default:
                    block.SetFaceTexture(face, new TextureArea { });
                    return;
                case 0x0003: // TYPE_TEXTURE_COLOR
                    block.SetFaceTexture(face, new TextureArea { Texture = TextureInvisible.Instance });
                    return;
                case 0x0007: // TYPE_TEXTURE_TILE
                    int texIndex = ((prjFace._txtFlags & 0x03) << 8) | prjFace._txtIndex;
                    PrjTexInfo texInfo = tempTextures[texIndex];

                    var uv = new Vector2[]
                    {
                        new Vector2(
                            texInfo._x + 0.5f,
                            texInfo._y + 0.5f),
                        new Vector2(
                            texInfo._x + texInfo._width + 0.5f, // Must be + as well, even though it seems weird.
                            texInfo._y + 0.5f),
                        new Vector2(
                            texInfo._x + texInfo._width + 0.5f,
                            texInfo._y + texInfo._height + 0.5f),
                        new Vector2(
                            texInfo._x + 0.5f,
                            texInfo._y + texInfo._height + 0.5f)
                    };

                    TextureArea texture;
                    texture.Texture = levelTexture;
                    texture.DoubleSided = (prjFace._txtFlags & 0x04) != 0;
                    texture.BlendMode = (prjFace._txtFlags & 0x08) != 0 ? BlendMode.Additive : BlendMode.Normal;

                    // Apply flipping
                    if ((prjFace._txtFlags & 0x80) != 0)
                    {
                        var temp = uv[0];
                        uv[0] = uv[1];
                        uv[1] = temp;

                        temp = uv[2];
                        uv[2] = uv[3];
                        uv[3] = temp;
                    }

                    ushort rotation = prjFace._txtRotation;
                    if (room.GetFaceVertexRange(x, z, face).Count == 3)
                    {
                        // Get UV coordinates for polygon
                        switch (prjFace._txtTriangle)
                        {
                            case 0:
                                texture.TexCoord0 = uv[0];
                                texture.TexCoord1 = uv[1];
                                texture.TexCoord2 = uv[3];
                                break;
                            case 1:
                                texture.TexCoord0 = uv[1];
                                texture.TexCoord1 = uv[2];
                                texture.TexCoord2 = uv[0];
                                break;
                            case 2:
                                texture.TexCoord0 = uv[2];
                                texture.TexCoord1 = uv[3];
                                texture.TexCoord2 = uv[1];
                                break;
                            case 3:
                                texture.TexCoord0 = uv[3];
                                texture.TexCoord1 = uv[0];
                                texture.TexCoord2 = uv[2];
                                break;
                            default:
                                logger.Warn("Unknown texture triangle selection " + prjFace._txtTriangle);
                                block.SetFaceTexture(face, new TextureArea { });
                                return;
                        }

                        // Fix floor and ceiling texturing in our coordinate system
                        if (face == BlockFace.Floor)
                        {
                            rotation += block.FloorSplitDirectionIsXEqualsZ ? (byte)1 : (byte)2;
                        }
                        else if (face == BlockFace.Ceiling)
                        {
                            var temp = texture.TexCoord2;
                            texture.TexCoord2 = texture.TexCoord0;
                            texture.TexCoord0 = temp;

                            rotation += block.CeilingSplitDirectionIsXEqualsZ ? (byte)2 : (byte)1;
                            rotation = (ushort)(3000 - rotation); // Change of rotation direction
                        }
                        else if (face == BlockFace.CeilingTriangle2)
                        {
                            var temp = texture.TexCoord2;
                            texture.TexCoord2 = texture.TexCoord0;
                            texture.TexCoord0 = temp;

                            rotation = (ushort)(3000 - rotation); // Change of rotation direction
                        }

                        // Apply rotation
                        rotation %= 3;
                        for (int rot = 0; rot < rotation; rot++)
                        {
                            var temp = texture.TexCoord2;
                            texture.TexCoord2 = texture.TexCoord1;
                            texture.TexCoord1 = texture.TexCoord0;
                            texture.TexCoord0 = temp;
                        }

                        // Set third texture coordinate to something
                        texture.TexCoord3 = texture.TexCoord2;
                    }
                    else
                    {
                        // Fix floor and ceiling texturing in our coordinate system
                        if (face == BlockFace.Floor || face == BlockFace.FloorTriangle2)
                            rotation += 2;

                        // Apply rotation
                        rotation %= 4;
                        for (int rot = 0; rot < rotation; rot++)
                        {
                            var temp = uv[3];
                            uv[3] = uv[2];
                            uv[2] = uv[1];
                            uv[1] = uv[0];
                            uv[0] = temp;
                        }

                        // Assign texture coordinates
                        if (face == BlockFace.Ceiling || face == BlockFace.CeilingTriangle2)
                        {
                            texture.TexCoord0 = uv[1];
                            texture.TexCoord1 = uv[0];
                            texture.TexCoord2 = uv[3];
                            texture.TexCoord3 = uv[2];
                        }
                        else
                        {
                            texture.TexCoord0 = uv[0];
                            texture.TexCoord1 = uv[1];
                            texture.TexCoord2 = uv[2];
                            texture.TexCoord3 = uv[3];
                        }
                    }

                    block.SetFaceTexture(face, texture);
                    return;
            }
        }

        private static Rectangle GetArea(Room room, int roomBorder, int objPosX, int objPosZ, int objSizeX, int objSizeZ)
        {
            int startX = Math.Max(roomBorder, Math.Min(room.NumXSectors - 1 - roomBorder, objPosX));
            int startZ = Math.Max(roomBorder, Math.Min(room.NumZSectors - 1 - roomBorder, objPosZ));
            int endX = Math.Max(startX, Math.Min(room.NumXSectors - 1 - roomBorder, objPosX + objSizeX - 1));
            int endZ = Math.Max(startZ, Math.Min(room.NumZSectors - 1 - roomBorder, objPosZ + objSizeZ - 1));
            return new Rectangle(startX, startZ, endX, endZ);
        }

        private static string FindGameDirectory(string filename)
        {
            string directory = filename;
            while (!string.IsNullOrEmpty(directory))
            {
                if (File.Exists(Path.Combine(directory, "Tomb4.exe")) ||
                    File.Exists(Path.Combine(directory, "script.dat")))
                {
                    return directory;
                }
                directory = Path.GetDirectoryName(directory);
            }
            return Path.GetDirectoryName(filename);
        }
    }
}
