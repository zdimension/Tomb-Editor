﻿using SharpDX;
using TombEditor.Geometry;

namespace TombEditor
{
    public class Clipboard
    {
        private static ObjectPtr? _objectPtr;
        private static Room _originalRoom;
        
        public static void Copy(Room originalRoom, ObjectPtr objectPtr)
        {
            _objectPtr = objectPtr;
            _originalRoom = originalRoom;
        }

        private static int CopyObject(int Index, Level level, Room room, Vector3 position)
        {
            PositionBasedObjectInstance result = (PositionBasedObjectInstance)level.Objects[_objectPtr.Value.Id].Clone();
            result.Position = position;
            result.Id = level.GetNewObjectId();
            result.Room = room;
            level.Objects.Add(result.Id, result);
            return result.Id;
        }

        public static bool Paste(Level level, Room room, DrawingPoint pos)
        {
            Block block = room.GetBlock(pos);
            int y = (block.QAFaces[0] + block.QAFaces[1] + block.QAFaces[2] + block.QAFaces[3]) / 4;
            Vector3 position = new Vector3(pos.X * 1024 + 512, y * 256, pos.Y * 1024 + 512);
            
            if (_objectPtr.HasValue)
                switch (_objectPtr.Value.Type)
                {
                    case ObjectInstanceType.Light:
                        {
                            var light = _originalRoom.Lights[_objectPtr.Value.Id].Clone();
                            light.Position = position;

                            room.Lights.Add(light);

                            room.BuildGeometry();
                            room.CalculateLightingForThisRoom();
                            room.UpdateBuffers();
                        }
                        break;

                    case ObjectInstanceType.Camera:
                        room.Cameras.Add(CopyObject(_objectPtr.Value.Id, level, room, position));
                        break;

                    case ObjectInstanceType.FlyByCamera:
                        room.FlyByCameras.Add(CopyObject(_objectPtr.Value.Id, level, room, position));
                        break;

                    case ObjectInstanceType.Sink:
                        room.Sinks.Add(CopyObject(_objectPtr.Value.Id, level, room, position));
                        break;

                    case ObjectInstanceType.SoundSource:
                        room.SoundSources.Add(CopyObject(_objectPtr.Value.Id, level, room, position));
                        break;

                    case ObjectInstanceType.Moveable:
                        room.Moveables.Add(CopyObject(_objectPtr.Value.Id, level, room, position));
                        break;

                    case ObjectInstanceType.Static:
                        room.Statics.Add(CopyObject(_objectPtr.Value.Id, level, room, position));
                        break;
                }

            return _objectPtr.HasValue;
        }
    }
}
