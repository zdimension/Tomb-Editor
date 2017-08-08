﻿using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using TombEditor.Geometry;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace TombEditor.Compilers
{
    public enum Dec_BoxFlag
    {
        None = 0,
        Water = 1,
        Monkey = 2,
        Jump = 4,
        FlipMap = 8,
        IsolatedBox = 16,
        NotWalkableBox = 32
    }

    public struct dec_tr_box_aux
    {
        public int Zmin;
        public int Zmax;
        public int Xmin;
        public int Xmax;
        public short TrueFloor;
        public short Clicks;
        public short OverlapIndex;
        public byte NumXSectors;
        public byte NumZSectors;
        public bool IsolatedBox;
        public bool NotWalkableBox;
        public bool Monkey;
        public bool Jump;
        public short Room;
        public bool Water;
        public int IsBaseRoom;
        public int IsAlternateRoom;
    }

    public sealed partial class LevelCompilerTr4
    {
        private bool dec_graybox = false;
        private bool dec_water = true;
        private bool dec_monkey = false;
        private bool dec_flipped = false;
        private bool dec_jump = false;
        private int dec_roomIndex = -1;
        private short dec_q0 = -1;
        private short dec_q1 = -1;
        private short dec_q2 = -1;
        private short dec_q3 = -1;
        private dec_tr_box_aux[] dec_boxes;
        private ushort[] dec_overlaps;
        private int dec_numBoxes = 0;
        private int dec_numOverlaps = 0;
        private bool dec_boxExtendsInAnotherRoom = false;
        private Room[] dec_rooms;
        private Room[] dec_baseRooms;
        private Room[] dec_alternateRooms;

        private void Dec_BuildBoxesAndOverlaps()
        {
            dec_roomIndex = 0;
            dec_boxes = new dec_tr_box_aux[2040];

            int boxIndex = 0x7ff;

            Stopwatch watch = new Stopwatch();
            watch.Start();

            // TODO: for now I replicate the flipping method of winroomedit
            dec_rooms = new Room[_level.Rooms.Length];
            dec_baseRooms = new Room[_level.Rooms.Length];
            dec_alternateRooms = new Room[_level.Rooms.Length];

            /* for (int n = 0; n < _level.Rooms.Length; n++)
             {
                 dec_rooms[n] = null;
                 dec_baseRooms[n] = null;
                 dec_alternateRooms[n] = null;
             }

             for (int n = 0; n < _level.Rooms.Length; n++)
             {
                 Room room = _level.Rooms[n];
                 if (room != null && room.BaseRoom==null)
                 {
                     dec_rooms[n] = room;
                     dec_baseRooms[n] = room;
                 }
             }

             for (int n=0;n<_level.Rooms.Length;n++)
             {
                 Room room = _level.Rooms[n];
                 if (room != null && room.BaseRoom != null)
                 {
                     dec_alternateRooms[dec_rooms.ReferenceIndexOf(room.BaseRoom)] = room;
                 } 
             }
             */

            dec_rooms = _level.Rooms;

            for (int flipped = 0; flipped < 2; flipped++)
            {
                for (int i = 0; i < _level.Rooms.Length; i++)
                {
                    Room room = dec_rooms[i];

                    // Room must be defined and also must be base room or the flipped version
                    if (room != null && (flipped == 0 && room.BaseRoom == null || flipped == 1 && room.BaseRoom != null))
                    {
                        for (int z = 0; z < room.NumZSectors; z++)
                        {
                            for (int x = 0; x < room.NumXSectors; x++)
                            {
                             //   Console.WriteLine("Room: " + i + ", X: " + x + ", Z: " + z);
                                dec_tr_box_aux box = new dec_tr_box_aux();

                                // First create the box...
                                if (x != 0 &&
                                    z != 0 &&
                                    x != room.NumXSectors - 1 &&
                                    z != room.NumZSectors - 1 &&
                                    Dec_CreateNewBox(ref box, x, z, i))
                                {
                                    // ...then try to add it to the box array
                                    boxIndex = Dec_AddBox(ref box);
                                    if (boxIndex < 0) return;
                                }
                                else
                                {
                                    boxIndex = 0x7ff;
                                }

                                // Assign the box index to the sector
                                room._compiled.Sectors[room._compiled.NumZSectors * x + z].BoxIndex = (short)(boxIndex << 4);
                            }
                        }
                    }
                }

                dec_flipped = true;
            }

            dec_flipped = false;

            watch.Stop();
            Console.WriteLine("Dec_BuildBoxesAndOverlaps(): " + watch.ElapsedMilliseconds + " ms");

            // Build the overlaps
            //Dec_BuildOverlaps();

             /*// Now put the boxes in the final array
            if (dec_numBoxes > 0)
            {
                int currentBoxIndex = dec_numBoxes - 1;

                do
                {
                    if (dec_boxes[currentBoxIndex].OverlapIndex != 0x7ff)
                    {
                        if (dec_boxes[currentBoxIndex].IsolatedBox)
                            dec_boxes[currentBoxIndex].OverlapIndex = (short)(dec_boxes[currentBoxIndex].OverlapIndex | 0x8000);
                    }

                    /* dec_boxes[currentBoxIndex].Xmin <<= 10;
                     dec_boxes[currentBoxIndex].Zmin <<= 10;
                     dec_boxes[currentBoxIndex].Xmax = (byte)(dec_boxes[currentBoxIndex].Xmax << 10) - 1;
                    dec_boxes[currentBoxIndex].TrueFloor *= -256;

                    currentBoxIndex--;
                }
                while (currentBoxIndex > 0);
            }*/
        }

       /* private void Dec_FlipAllRooms()
        {
            for (int i=0;i<dec_rooms.Length;i++)
            {
                if ()
            }
        }*/

        private bool Dec_BuildOverlaps()
        {
           /* int numBoxes = dec_numBoxes;
            int numOverlaps = 0;
            dec_numOverlaps = 0;

            int i = 0;
            int j = 0;

            if (dec_numBoxes > 0)
            {
                i = 0;

                do
                {
                    dec_tr_box_aux box1 = dec_boxes[i];
                    dec_boxes[i].OverlapIndex = 0x7ff;

                    if (!box1.FlipMap)
                    {
                        if (dec_flipped)
                        {
                            dec_flipped = false;
                            numOverlaps = dec_numOverlaps;
                        }

                        numBoxes = dec_numBoxes;

                        if (dec_numBoxes > 0)
                        {
                            j = 0;
                            do
                            {
                                if (i != j)
                                {
                                    dec_tr_box_aux box2 = dec_boxes[j];

                                    if (!box2.FlipMap)
                                    {
                                        bool overlap = Dec_BoxesOverlap(ref box1, ref box2);
                                        numOverlaps = dec_numOverlaps;

                                        if (overlap)
                                        {
                                            if (dec_numOverlaps == 16384) return false;
                                            if (box1.OverlapIndex == 0x7ff) dec_boxes[i].OverlapIndex = (ushort)dec_numOverlaps;

                                            dec_overlaps[numOverlaps++] = (ushort)j;
                                            dec_numOverlaps = numOverlaps;

                                            if (dec_jump) dec_overlaps[numOverlaps] |= 0x800;
                                            if (dec_monkey) dec_overlaps[numOverlaps] |= 0x2000;
                                        }
                                    }
                                }

                                numBoxes = dec_numBoxes;
                                j++;
                            }
                            while (j < dec_numBoxes);
                        }
                    }

                    if (box1.FlipMap)
                    {
                        if (!dec_flipped)
                        {
                            dec_flipped = true;
                            numOverlaps = dec_numOverlaps;
                        }

                        numBoxes = dec_numBoxes;

                        if (dec_numBoxes > 0)
                        {
                            j = 0;
                            do
                            {
                                if (i != j)
                                {
                                    dec_tr_box_aux box2 = dec_boxes[j];

                                    if (!box2.FlipMap)
                                    {
                                        bool overlap = Dec_BoxesOverlap(ref box1, ref box2);
                                        numOverlaps = dec_numOverlaps;

                                        if (overlap)
                                        {
                                            if (dec_numOverlaps == 16384) return false;
                                            if (box1.OverlapIndex == 0x7ff) dec_boxes[i].OverlapIndex = (ushort)dec_numOverlaps;

                                            dec_overlaps[numOverlaps++] = (ushort)j;
                                            dec_numOverlaps = numOverlaps;

                                            if (dec_jump) dec_overlaps[numOverlaps] |= 0x800;
                                            if (dec_monkey) dec_overlaps[numOverlaps] |= 0x2000;
                                        }
                                    }
                                }

                                numBoxes = dec_numBoxes;
                                j++;
                            }
                            while (j < dec_numBoxes);
                        }
                    }

                    i++;
                }
                while (i < dec_numBoxes);

            }*/

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Dec_AddBox(ref dec_tr_box_aux box)
        {
            if (dec_numBoxes == 2040) return -1;

            int boxIndex = -1;

            for (int i = 0; i < dec_numBoxes; i++)
            {
                if (dec_boxes[i].Xmin == box.Xmin &&
                    dec_boxes[i].Xmax == box.Xmax &&
                    dec_boxes[i].Zmin == box.Zmin &&
                    dec_boxes[i].Zmax == box.Zmax &&
                    dec_boxes[i].TrueFloor == box.TrueFloor &&
                    dec_boxes[i].IsBaseRoom == box.IsBaseRoom &&
                    dec_boxes[i].IsAlternateRoom == box.IsAlternateRoom &&
                    dec_boxes[i].IsolatedBox == box.IsolatedBox &&
                    dec_boxes[i].Jump == box.Jump &&
                    dec_boxes[i].Monkey == box.Monkey &&
                    dec_boxes[i].NotWalkableBox == box.NotWalkableBox &&
                    dec_boxes[i].Water == box.Water)
                {
                    boxIndex = i;
                    break;
                }
            }

            if (boxIndex == -1)
            {
                boxIndex = dec_numBoxes;
                box.OverlapIndex = 0x7ff;
                dec_boxes[dec_numBoxes] = box;
                dec_numBoxes++;
            }

            return boxIndex;
        }

        private bool Dec_CreateNewBox(ref dec_tr_box_aux box, int x, int z, int roomIndex)
        {
            bool monkey = false;

            Room room = dec_rooms[roomIndex];
            Block block = room.Blocks[x, z];

            if (block.Type == BlockType.Wall ||
                block.Type == BlockType.BorderWall ||
                block.FloorOpacity == PortalOpacity.Opacity1 ||
                block.CeilingOpacity == PortalOpacity.Opacity1)
            {
                return false;
            }

            dec_q0 = block.QAFaces[0];
            dec_q1 = block.QAFaces[1];
            dec_q2 = block.QAFaces[2];
            dec_q3 = block.QAFaces[3];

            int currentX = (int)room.Position.X + x;
            int currentZ = (int)room.Position.Z + z;

            dec_roomIndex = roomIndex;

            Dec_CanSectorBeReachedAndIsSolid(currentX, currentZ);

            dec_graybox = false;
            dec_water = true;
            dec_monkey = false;

            short floor = (short)Dec_GetBoxFloorHeight(currentX, currentZ);
            box.TrueFloor = floor;

            if (floor == 0x7fff) return false;

            box.Room = (short)dec_roomIndex;
            box.Water = room.FlagWater;

            if (dec_flipped)
            {
              //  box.IsAlternateRoom = true;
            }
            else
            {
             //   box.FlipMap = false;
            }

            if (dec_monkey)
            {
                box.Monkey = true;
                monkey = true;
            }

            if (!dec_water)
            {
                box.Water = false;
            }

            if (dec_graybox)
            {
                box.Xmin = currentX;
                box.Zmin = currentZ;
                box.Xmax = currentX + 1;
                box.Zmax = currentZ + 1;
                box.IsolatedBox = true;

                return true;
            }
            else
            {
                dec_graybox = true;

                int direction = 0x0f;
                int directionBase = 0x0f;

                int xMin = currentX;
                int xMax = currentX;
                int zMin = currentZ;
                int zMax = currentZ;

               /* int v34 = roomIndex;
                int v22 = roomIndex;
                int v35 = roomIndex;
                int v25 = roomIndex;
                int v36 = roomIndex;*
                int v28 = roomIndex;*/

                int currentRoomIndex = roomIndex;
                int currentRoomIndex1 = roomIndex;
                int currentRoomIndex2 = roomIndex;
                int currentRoomIndex3 = roomIndex;
                int currentRoomIndex4 = roomIndex;

                int searchX = xMin;
                int searchZ = zMin;

                while (true)
                {
                    if ((directionBase & 0x04) == 0x04)
                    {
                        dec_boxExtendsInAnotherRoom = false;
                        dec_roomIndex = currentRoomIndex1;
                        currentRoomIndex = currentRoomIndex1;

                        searchX = xMin;

                        if (xMin <= xMax)
                        {
                            bool finishedDirection = true;

                            while (floor == Dec_GetBoxFloorHeight(searchX, zMin) &&
                                   floor == Dec_GetBoxFloorHeight(searchX, zMin - 1) &&
                                   dec_monkey == monkey)
                            {
                                if (searchX == xMin) currentRoomIndex1 = dec_roomIndex;

                                if (dec_boxExtendsInAnotherRoom)
                                {
                                    // If the box goes in another room and one of current rooms has a flipped room that stop now
                                    if (dec_roomIndex != currentRoomIndex &&
                                        (dec_rooms[dec_roomIndex].AlternateRoom != null ||
                                         dec_rooms[currentRoomIndex].AlternateRoom != null))
                                    {
                                        break;
                                    }

                                    // Reset current room index to start room index
                                    dec_roomIndex = currentRoomIndex;

                                    // If floor of starting block is != floor of block (X, Z - 1) exit loop
                                    if (floor != Dec_GetBoxFloorHeight(searchX, zMin - 1)) break;

                                    // Reset flag of box extended in another room
                                    dec_boxExtendsInAnotherRoom = false;
                                }

                                searchX++;

                                if (searchX > xMax)
                                {
                                    finishedDirection = false;
                                    break;
                                }
                            }

                            if (finishedDirection) direction -= 0x04;
                        }

                        directionBase = direction;
                        if ((directionBase & 0x04) == 0x04) zMin--;
                    }

                    if ((directionBase & 0x02) == 0x02)
                    {
                        dec_boxExtendsInAnotherRoom = false;
                        dec_roomIndex = currentRoomIndex2;
                        currentRoomIndex = currentRoomIndex2;

                        searchZ = zMin;

                        if (zMin <= zMax)
                        {
                            bool finishedDirection = true;

                            while (floor == Dec_GetBoxFloorHeight(xMax, searchZ) &&
                                   floor == Dec_GetBoxFloorHeight(xMax + 1, searchZ) &&
                                   dec_monkey == monkey)
                            {
                                if (searchZ == zMin) currentRoomIndex2 = dec_roomIndex;

                                if (dec_boxExtendsInAnotherRoom)
                                {
                                    // If the box goes in another room and one of current rooms has a flipped room that stop now
                                    if (dec_roomIndex != currentRoomIndex &&
                                        (dec_rooms[dec_roomIndex].AlternateRoom != null ||
                                         dec_rooms[currentRoomIndex].AlternateRoom != null))
                                    {
                                        break;
                                    }

                                    // Reset current room index to start room index
                                    dec_roomIndex = currentRoomIndex;

                                    // If floor of starting block is != floor of block (X, Z - 1) exit loop
                                    if (floor != Dec_GetBoxFloorHeight(xMax + 1, searchZ)) break;

                                    // Reset flag of box extended in another room
                                    dec_boxExtendsInAnotherRoom = false;
                                }

                                searchZ++;

                                if (searchZ > zMax)
                                {
                                    finishedDirection = false;
                                    break;
                                }
                            }

                            if (finishedDirection) direction -= 0x02;
                        }

                        directionBase = direction;
                        if ((directionBase & 0x02) == 0x02) xMax++;
                    }

                    if ((directionBase & 0x08) == 0x08)
                    {
                        dec_boxExtendsInAnotherRoom = false;
                        dec_roomIndex = currentRoomIndex3;
                        currentRoomIndex = currentRoomIndex3;

                        searchX = xMax;

                        if (xMax >= xMin)
                        {
                            bool finishedDirection = true;

                            while (floor == Dec_GetBoxFloorHeight(searchX, zMax) &&
                                   floor == Dec_GetBoxFloorHeight(searchX, zMax + 1) &&
                                   dec_monkey == monkey)
                            {
                                if (searchX == xMax) currentRoomIndex3 = dec_roomIndex;

                                if (dec_boxExtendsInAnotherRoom)
                                {
                                    // If the box goes in another room and one of current rooms has a flipped room that stop now
                                    if (dec_roomIndex != currentRoomIndex &&
                                        (dec_rooms[dec_roomIndex].AlternateRoom != null ||
                                         dec_rooms[currentRoomIndex].AlternateRoom != null))
                                    {
                                        break;
                                    }

                                    // Reset current room index to start room index
                                    dec_roomIndex = currentRoomIndex;

                                    // If floor of starting block is != floor of block (X, Z + 1) exit loop
                                    if (floor != Dec_GetBoxFloorHeight(searchX, zMax + 1)) break;

                                    // Reset flag of box extended in another room
                                    dec_boxExtendsInAnotherRoom = false;
                                }

                                searchX--;

                                if (searchX < xMin)
                                {
                                    finishedDirection = false;
                                    break;
                                }
                            }

                            if (finishedDirection) direction -= 0x08;
                        }

                        directionBase = direction;
                        if ((directionBase & 0x08) == 0x08) zMax++;
                    }

                    if ((directionBase & 0x01) == 0x01)
                    {
                        dec_boxExtendsInAnotherRoom = false;
                        dec_roomIndex = currentRoomIndex4;
                        currentRoomIndex = currentRoomIndex4;

                        searchZ = zMax;

                        if (zMax >= zMin)
                        {
                            bool finishedDirection = true;

                            while (floor == Dec_GetBoxFloorHeight(xMin, searchZ) &&
                                   floor == Dec_GetBoxFloorHeight(xMin - 1, searchZ) &&
                                   dec_monkey == monkey)
                            {
                                if (searchZ == zMax) currentRoomIndex4 = dec_roomIndex;

                                if (dec_boxExtendsInAnotherRoom)
                                {
                                    // If the box goes in another room and one of current rooms has a flipped room that stop now
                                    if (dec_roomIndex != currentRoomIndex &&
                                        (dec_rooms[dec_roomIndex].AlternateRoom != null ||
                                         dec_rooms[currentRoomIndex].AlternateRoom != null))
                                    {
                                        break;
                                    }

                                    // Reset current room index to start room index
                                    dec_roomIndex = currentRoomIndex;

                                    // If floor of starting block is != floor of block (X, Z - 1) exit loop
                                    if (floor != Dec_GetBoxFloorHeight(xMin - 1, searchZ)) break;

                                    // Reset flag of box extended in another room
                                    dec_boxExtendsInAnotherRoom = false;
                                }

                                searchZ--;

                                if (searchZ < zMin)
                                {
                                    finishedDirection = false;
                                    break;
                                }
                            }

                            if (finishedDirection) direction -= 0x01;
                        }

                        directionBase = direction;
                        if ((directionBase & 0x01) == 0x01) xMin--;
                    }

                    if (directionBase == 0x00) break;

                    currentX = xMin;
                }

                box.Xmin = xMin;
                box.Zmin = zMin;
                box.Xmax = xMax + 1;
                box.Zmax = zMax + 1;

                return true;
            }
        }

        private bool Dec_CanSectorBeReachedAndIsSolid(int x, int z)
        {
            bool borderOrOutside = Dec_IsOutsideOrdBorderRoom(x, z);

            int roomIndex = dec_roomIndex;

            int xInRoom = 0;
            int zInRoom = 0;

            Room room;
            Block block;

            if (borderOrOutside)
            {
                while (true)
                {
                    room = dec_rooms[roomIndex];

                    xInRoom = x - (int)room.Position.X;
                    zInRoom = z - (int)room.Position.Z;

                    if (xInRoom >= 0)
                    {
                        if (xInRoom >= room.NumXSectors)
                            xInRoom = room.NumXSectors - 1;
                    }
                    else
                    {
                        xInRoom = 0;
                    }

                    if (zInRoom >= 0)
                    {
                        if (zInRoom >= room.NumZSectors)
                            zInRoom = room.NumZSectors - 1;
                    }
                    else
                    {
                        zInRoom = 0;
                    }

                    block = room.Blocks[xInRoom, zInRoom];

                    bool isWallPortal = block.WallPortal != -1;
                    if (!isWallPortal) break;

                    Portal portal = _level.Portals[block.WallPortal];

                    dec_roomIndex = dec_rooms.ReferenceIndexOf(portal.AdjoiningRoom);

                    if (block.WallOpacity == PortalOpacity.Opacity1) return false;

                    if (!Dec_IsOutsideOrdBorderRoom(x, z)) break;
                }

                room = dec_rooms[dec_roomIndex];

                xInRoom = x - (int)room.Position.X;
                zInRoom = z - (int)room.Position.Z;

                block = room.Blocks[xInRoom, zInRoom];

                // After having probed that we can reach X, Z from the original room, do the following
                while (block.FloorPortal != -1 && !block.IsFloorSolid)
                {
                    Portal portal = _level.Portals[block.FloorPortal];

                    if (block.FloorOpacity == PortalOpacity.Opacity1 &&
                        !(room.FlagWater ^ portal.AdjoiningRoom.FlagWater))
                    {
                        break;
                    }

                    dec_roomIndex = dec_rooms.ReferenceIndexOf(portal.AdjoiningRoom);

                    room = dec_rooms[dec_roomIndex];

                    xInRoom = x - (int)room.Position.X;
                    zInRoom = z - (int)room.Position.Z;

                    block = room.Blocks[xInRoom, zInRoom];
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Dec_IsOutsideOrdBorderRoom(int x, int z)
        {
            Room room = dec_rooms[dec_roomIndex];
            return (x < 0 || z < 0 || x > room.NumXSectors - 1 || z > room.NumZSectors - 1);
        }

        private int Dec_GetBoxFloorHeight(int x, int z)
        {
            int adjoiningRoom = dec_roomIndex;
            Room room = dec_rooms[dec_roomIndex];

            int posXblocks = (int)room.Position.X;
            int posZblocks = (int)room.Position.Z;

            int xInRoom = x - posXblocks;
            int zInRoom = z - posZblocks;

            if (xInRoom < 0 ||
                xInRoom > room.NumXSectors - 1 ||
                zInRoom < 0 ||
                zInRoom > room.NumZSectors - 1)
            {
                return 0x7fff;
            }

            Block block = room.Blocks[xInRoom, zInRoom];

            // If block is a wall or is a vertical toggle opacity 1
            // Note that is & 8 because wall and border wall are the only blocks with bit 4 (0x08) set
            if (((block.Type == BlockType.Wall ||
                block.Type == BlockType.BorderWall) && block.WallPortal == -1) ||
                block.WallOpacity == PortalOpacity.Opacity1)
            {
                dec_q0 = -1;
                dec_q1 = -1;
                dec_q2 = -1;
                dec_q3 = -1;

                return 0x7fff;
            }

            // If it's not a wall portal or is vertical toggle opacity 1
            if ((block.WallPortal == -1 || block.WallOpacity == PortalOpacity.Opacity1))
            {

            }
            else
            {
                if (block.WallPortal == -1) return 0x7fff;

                Portal portal = _level.Portals[block.WallPortal];
                adjoiningRoom = dec_rooms.ReferenceIndexOf(portal.AdjoiningRoom);

                dec_roomIndex = adjoiningRoom;
                dec_boxExtendsInAnotherRoom = true;

                room = dec_rooms[dec_roomIndex];

                posXblocks = (int)room.Position.X;
                posZblocks = (int)room.Position.Z;

                xInRoom = x - posXblocks;
                zInRoom = z - posZblocks;

                block = room.Blocks[xInRoom, zInRoom];
            }

            int oldRoom = adjoiningRoom;

            while (block.FloorPortal != -1 && !block.IsFloorSolid)
            {
                Portal portal = _level.Portals[block.FloorPortal];

                int adjoiningRoom2 = dec_rooms.ReferenceIndexOf(portal.AdjoiningRoom);

                if (block.FloorOpacity == PortalOpacity.Opacity1)
                {
                    if (!(room.FlagWater ^ dec_rooms[adjoiningRoom].FlagWater))
                    {
                        break;
                    }
                }

                dec_roomIndex = adjoiningRoom2;
                room = dec_rooms[dec_roomIndex];

                posXblocks = (int)room.Position.X;
                posZblocks = (int)room.Position.Z;

                xInRoom = x - posXblocks;
                zInRoom = z - posZblocks;

                block = room.Blocks[xInRoom, zInRoom];
            }

            int sumHeights = block.QAFaces[0] + block.QAFaces[1] + block.QAFaces[2] + block.QAFaces[3];
            int meanFloorCornerHeight = sumHeights >> 2;

            dec_q0 = block.QAFaces[0];
            dec_q1 = block.QAFaces[1];
            dec_q2 = block.QAFaces[2];
            dec_q3 = block.QAFaces[3];

            int slope1 = (Math.Abs(dec_q0 - dec_q1) >= 3 ? 1 : 0);
            int slope2 = (Math.Abs(dec_q1 - dec_q2) >= 3 ? 1 : 0);
            int slope3 = (Math.Abs(dec_q2 - dec_q3) >= 3 ? 1 : 0);
            int slope4 = (Math.Abs(dec_q3 - dec_q0) >= 3 ? 1 : 0);

            bool someFlag = false;

            if (block.QAFaces[0] == block.QAFaces[2])
            {
                someFlag = false;
            }
            else
            {
                if (block.QAFaces[1] != block.QAFaces[3])
                {
                    if (block.QAFaces[0] < block.QAFaces[1] && block.QAFaces[0] < block.QAFaces[3] ||
                        block.QAFaces[2] < block.QAFaces[1] && block.QAFaces[2] < block.QAFaces[3] ||
                        block.QAFaces[0] < block.QAFaces[1] && block.QAFaces[0] < block.QAFaces[3] ||
                        block.QAFaces[2] < block.QAFaces[1] && block.QAFaces[2] < block.QAFaces[3])
                    {
                        someFlag = true;
                    }
                    else
                    {
                        someFlag = false;
                    }
                }
                else
                {
                    someFlag = true;
                }
            }

            int floorHeight = meanFloorCornerHeight + (int)room.Position.Y;
            int ceiling = room.GetHighestCeilingCorner(xInRoom, zInRoom) + (int)room.Position.Y;

            if (dec_water && room.FlagWater && (ceiling - meanFloorCornerHeight) <= 1 && block.CeilingPortal != -1 && !block.IsCeilingSolid)
            {
                Portal portal = _level.Portals[block.CeilingPortal];
                if (!portal.AdjoiningRoom.FlagWater)
                {
                    dec_water = false;
                }
            }

            dec_roomIndex = oldRoom;

            if (slope1 + slope2 + slope4 + slope3 >= 3 || slope1 + slope3 == 2 || slope2 + slope4 == 2)
            {
                if (dec_water && !room.FlagWater) return 0x7fff;
            }
            else
            {
                if (someFlag)
                {
                    if ((slope1 == 0 || slope2 == 0) && (slope3 == 0 || slope4 == 0))
                    {

                    }
                    else
                    {
                        if (dec_water && !room.FlagWater) return 0x7fff;
                    }
                }
                else
                {
                    if (slope1 + slope4 == 2 || slope2 + slope3 == 2)
                    {
                        if (dec_water && !room.FlagWater) return 0x7fff;
                    }
                }
            }

            if ((block.Flags & BlockFlags.Box) == 0)
            {
                dec_monkey = (block.Flags & BlockFlags.Monkey) != 0;
                return floorHeight;
            }
            else
            {
                if (!dec_graybox)
                {
                    dec_graybox = true;
                    dec_monkey = (block.Flags & BlockFlags.Monkey) != 0;
                    return floorHeight;
                }
                else
                {
                    return 0x7fff;
                }
            }
        }

        private bool Dec_CheckIfCanJumpX(ref dec_tr_box_aux a, ref dec_tr_box_aux b)
        {
            // Boxes must have the same height for jump
            if (a.TrueFloor != b.TrueFloor) return false;

            int xMin = a.Xmin;
            if (a.Xmin <= b.Xmin)
                xMin = b.Xmin;

            int xMax = a.Xmax;
            if (a.Xmax <= b.Xmax)
                xMax = b.Xmax;

            int zMin = a.Zmin;
            int zMax = b.Zmax;

            int currentX = (xMin + xMax) >> 1;

            int floor = 0;

            if (zMax == zMin - 1)
            {
                dec_roomIndex = b.Room;

                if (!Dec_CanSectorBeReachedAndIsSolid(currentX, zMax - 1)) return false;

                if (Dec_CanSectorBeReachedAndIsSolid(currentX, zMax))
                {
                    floor = Dec_GetBoxFloorHeight(currentX, zMax);
                    if (floor <= b.TrueFloor - 2 && floor != 0x7fff) return true;

                    return false;
                }

                return false;
            }

            if (zMax == zMin - 2)
            {
                dec_roomIndex = b.Room;

                if (Dec_CanSectorBeReachedAndIsSolid(currentX, zMax - 1))
                {
                    if (Dec_CanSectorBeReachedAndIsSolid(currentX, zMax))
                    {
                        floor = Dec_GetBoxFloorHeight(currentX, zMax);
                        if (floor <= b.TrueFloor - 2 && floor != 0x7fff)
                        {
                            if (Dec_CanSectorBeReachedAndIsSolid(currentX, zMax + 1))
                            {
                                floor = Dec_GetBoxFloorHeight(currentX, zMax + 1);
                                if (floor <= b.TrueFloor - 2 && floor != 0x7fff) return true;
                            }
                        }
                    }
                }

                return false;
            }

            zMin = b.Zmin;
            zMax = a.Zmax;

            if (zMin != zMax + 1)
            {
                if (zMin != zMax + 2) return false;

                dec_roomIndex = a.Room;

                if (!Dec_CanSectorBeReachedAndIsSolid(currentX, zMax - 1)) return false;

                if (!Dec_CanSectorBeReachedAndIsSolid(currentX, zMax)) return false;

                floor = Dec_GetBoxFloorHeight(currentX, zMax);
                if (floor > b.TrueFloor || floor == 0x7fff) return false;

                if (Dec_CanSectorBeReachedAndIsSolid(currentX, zMax + 1))
                {
                    floor = Dec_GetBoxFloorHeight(currentX, zMax + 1);
                    if (floor <= b.TrueFloor - 2 && floor != 0x7fff) return true;

                    return false;
                }

                return false;
            }

            dec_roomIndex = a.Room;

            if (Dec_CanSectorBeReachedAndIsSolid(currentX, zMax - 1) && Dec_CanSectorBeReachedAndIsSolid(currentX, zMax))
            {
                floor = Dec_GetBoxFloorHeight(currentX, zMax);
                if (floor <= b.TrueFloor - 2 && floor != 0x7fff) return true;

                return false;
            }

            return false;
        }

        private bool Dec_CheckIfCanJumpZ(ref dec_tr_box_aux a, ref dec_tr_box_aux b)
        {
            // Boxes must have the same height for jump
            if (a.TrueFloor != b.TrueFloor) return false;

            int zMin = a.Zmin;
            if (a.Zmin <= b.Zmin)
                zMin = b.Zmin;

            int zMax = a.Zmax;
            if (a.Zmax <= b.Zmax)
                zMax = b.Zmax;

            int xMin = a.Xmin;
            int xMax = b.Xmax;

            int currentZ = (zMin + zMax) >> 1;

            int floor = 0;

            if (xMax == xMin - 1)
            {
                dec_roomIndex = b.Room;

                if (!Dec_CanSectorBeReachedAndIsSolid(xMax - 1, currentZ)) return false;

                if (Dec_CanSectorBeReachedAndIsSolid(xMax, currentZ))
                {
                    floor = Dec_GetBoxFloorHeight(xMax, currentZ);
                    if (floor <= b.TrueFloor - 2 && floor != 0x7fff) return true;

                    return false;
                }

                return false;
            }

            if (xMax == xMin - 2)
            {
                dec_roomIndex = b.Room;

                if (Dec_CanSectorBeReachedAndIsSolid(xMax - 1, currentZ))
                {
                    if (Dec_CanSectorBeReachedAndIsSolid(xMax, currentZ))
                    {
                        floor = Dec_GetBoxFloorHeight(xMax, currentZ);
                        if (floor <= b.TrueFloor - 2 && floor != 0x7fff)
                        {
                            if (Dec_CanSectorBeReachedAndIsSolid(xMax + 1, currentZ))
                            {
                                floor = Dec_GetBoxFloorHeight(xMax + 1, currentZ);
                                if (floor <= b.TrueFloor - 2 && floor != 0x7fff) return true;
                            }
                        }
                    }
                }

                return false;
            }

            xMin = b.Xmin;
            xMax = a.Xmax;

            if (xMin != xMax + 1)
            {
                if (xMin != xMax + 2) return false;

                dec_roomIndex = a.Room;

                if (!Dec_CanSectorBeReachedAndIsSolid(xMax - 1, currentZ)) return false;

                if (!Dec_CanSectorBeReachedAndIsSolid(xMax, currentZ)) return false;

                floor = Dec_GetBoxFloorHeight(xMax, currentZ);
                if (floor > b.TrueFloor || floor == 0x7fff) return false;

                if (Dec_CanSectorBeReachedAndIsSolid(xMax + 1, currentZ))
                {
                    floor = Dec_GetBoxFloorHeight(xMax + 1, currentZ);
                    if (floor <= b.TrueFloor - 2 && floor != 0x7fff) return true;

                    return false;
                }

                return false;
            }

            dec_roomIndex = a.Room;

            if (Dec_CanSectorBeReachedAndIsSolid(xMax - 1, currentZ) && Dec_CanSectorBeReachedAndIsSolid(xMax, currentZ))
            {
                floor = Dec_GetBoxFloorHeight(xMax, currentZ);
                if (floor <= b.TrueFloor - 2 && floor != 0x7fff) return true;

                return false;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Dec_OverlapXmax(ref dec_tr_box_aux a, ref dec_tr_box_aux b)
        {
            int startZ = b.Zmin;
            if (a.Zmin > startZ)
                startZ = a.Zmin;

            int endZ = a.Zmax;
            if (a.Zmax >= endZ)
                endZ = b.Zmax;

            if (startZ >= endZ)
            {
                return true;
            }
            else
            {
                while (true)
                {
                    dec_roomIndex = a.Room;

                    if (!Dec_CanSectorBeReachedAndIsSolid(a.Xmax - 1, startZ)) break;

                    dec_graybox = false;

                    if (b.TrueFloor != Dec_GetBoxFloorHeight(a.Xmax, startZ)) break;

                    startZ++;

                    if (startZ >= endZ) return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Dec_OverlapXmin(ref dec_tr_box_aux a, ref dec_tr_box_aux b)
        {
            int startZ = b.Zmin;
            if (a.Zmin > startZ)
                startZ = a.Zmin;

            int endZ = a.Zmax;
            if (a.Zmax >= endZ)
                endZ = b.Zmax;

            if (startZ >= endZ)
            {
                return true;
            }
            else
            {
                while (true)
                {
                    dec_roomIndex = a.Room;

                    if (!Dec_CanSectorBeReachedAndIsSolid(a.Xmin, startZ)) break;

                    dec_graybox = false;

                    if (b.TrueFloor != Dec_GetBoxFloorHeight(a.Xmin - 1, startZ)) break;

                    startZ++;

                    if (startZ >= endZ) return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Dec_OverlapZmax(ref dec_tr_box_aux a, ref dec_tr_box_aux b)
        {
            int startX = b.Xmin;
            if (a.Xmin > startX)
                startX = a.Xmin;

            int endX = a.Xmax;
            if (a.Xmax >= endX)
                endX = b.Xmax;

            if (startX >= endX)
            {
                return true;
            }
            else
            {
                while (true)
                {
                    dec_roomIndex = a.Room;

                    if (!Dec_CanSectorBeReachedAndIsSolid(startX, a.Zmax - 1)) break;

                    dec_graybox = false;

                    if (b.TrueFloor != Dec_GetBoxFloorHeight(startX, a.Zmax)) break;

                    startX++;

                    if (startX >= endX) return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Dec_OverlapZmin(ref dec_tr_box_aux a, ref dec_tr_box_aux b)
        {
            int startX = b.Xmin;
            if (a.Xmin > startX)
                startX = a.Xmin;

            int endX = a.Xmax;
            if (a.Xmax >= endX)
                endX = b.Xmax;

            if (startX >= endX)
            {
                return true;
            }
            else
            {
                while (true)
                {
                    dec_roomIndex = a.Room;

                    if (!Dec_CanSectorBeReachedAndIsSolid(startX, a.Zmin)) break;

                    dec_graybox = false;

                    if (b.TrueFloor != Dec_GetBoxFloorHeight(startX, a.Zmin - 1)) break;

                    startX++;

                    if (startX >= endX) return true;
                }
            }

            return false;
        }

        private bool Dec_BoxesOverlap(ref dec_tr_box_aux a, ref dec_tr_box_aux b)
        {
            dec_jump = false;
            dec_monkey = false;

            dec_tr_box_aux box1 = a;
            dec_tr_box_aux box2 = b;

            if (b.TrueFloor > a.TrueFloor)
            {
                box1 = b;
                box2 = a;
            }

            if (box1.Xmax <= box2.Xmin || box1.Xmin >= box2.Xmax)
            {
                if (box1.Zmax > box2.Zmin && box1.Zmin < box2.Zmax && Dec_CheckIfCanJumpZ(ref box1, ref box2))
                {
                    dec_jump = true;
                    return true;
                }

                if (box1.Xmax < box2.Xmin ||
                    box1.Xmin > box2.Xmax ||
                    box1.Zmax <= box2.Zmin ||
                    box1.Zmin >= box2.Zmax ||
                    box1.Xmax == box2.Xmin && !Dec_OverlapXmax(ref box1, ref box2) ||
                    box1.Xmin == box2.Xmax && !Dec_OverlapXmin(ref box1, ref box2))
                {
                    return false;
                }

                if (box1.Monkey && box2.Monkey) dec_monkey = true;
                return true;
            }

            if (box1.Zmax > box2.Zmin && box1.Zmin < box2.Zmax)
            {
                if (box1.TrueFloor != box2.TrueFloor) return false;

                if (box1.Monkey && box2.Monkey) dec_monkey = true;
                return true;
            }

            if (Dec_CheckIfCanJumpX(ref box2, ref box1))
            {
                dec_jump = true;
                return true;
            }

            if (box1.Zmax < box2.Zmin ||
                box1.Zmin > box2.Zmax ||
                box1.Zmax == box2.Zmin && !Dec_OverlapZmax(ref box1, ref box2))
            {
                return false;
            }

            if (box1.Zmin != box2.Zmax)
            {
                if (box1.Monkey && box2.Monkey) dec_monkey = true;
                return true;
            }

            if (Dec_OverlapZmin(ref box1, ref box2))
            {
                if (box1.Monkey && box2.Monkey) dec_monkey = true;
                return true;
            }

            return false;
        }
    }
}