﻿using System;
using System.Collections.Generic;

namespace TombLib.LevelData.Compilers.TombEngine
{
    public sealed partial class LevelCompilerTombEngine
    {
        private enum ZoneType
        {
            None,
            Skeleton,
            Basic,
            Water,
            Human,
            Flyer
        }

        private void BuildPathFindingData()
        {
            ReportProgress(48, "Building pathfinding data");

            // Fix monkey on portals
            foreach (var fixRoom in _tempRooms.Values)
            {
                for (int x = 0; x < fixRoom.NumXSectors; x++)
                {
                    for (int z = 0; z < fixRoom.NumZSectors; z++)
                    {
                        var sector = fixRoom.AuxSectors[x, z];
                        if (sector.FloorPortal != null)
                            sector.Monkey = FindMonkeyFloor(fixRoom.OriginalRoom, x, z);
                    }
                }
            }

            // Use decompiled code for generation of boxes and overlaps
            Dec_BuildBoxesAndOverlaps();

            // Convert ovelaps to TR format
            _overlaps = new List<TombEngineOverlap>();
            _overlaps.AddRange(dec_overlaps);

            // Convert boxes to TR format
            _boxes = new List<TombEngineBox>();
            _zones = new List<TombEngineZone>();
            for (var i = 0; i < dec_boxes.Count; i++)
            {
                var box = new TombEngineBox()
                {
                    Xmin = dec_boxes[i].Xmin,
                    Xmax = dec_boxes[i].Xmax,
                    Zmin = dec_boxes[i].Zmin,
                    Zmax = dec_boxes[i].Zmax,
                    TrueFloor = -(dec_boxes[i].TrueFloor * (int)Level.HeightUnit),
                    OverlapIndex = dec_boxes[i].OverlapIndex,
                    Flags = dec_boxes[i].IsolatedBox ? 0x8000 : 0
                };
                _boxes.Add(box);

                var zone = new TombEngineZone()
                {
                    GroundZone1_Normal = int.MaxValue,
                    GroundZone2_Normal = int.MaxValue,
                    GroundZone3_Normal = int.MaxValue,
                    GroundZone4_Normal = int.MaxValue,
                    FlyZone_Normal = int.MaxValue,
                    GroundZone1_Alternate = int.MaxValue,
                    GroundZone2_Alternate = int.MaxValue,
                    GroundZone3_Alternate = int.MaxValue,
                    GroundZone4_Alternate = int.MaxValue,
                    FlyZone_Alternate = int.MaxValue
                };
                _zones.Add(zone);
            }

            // Create zones
            int groundZone1 = 1;
            int groundZone2 = 1;
            int groundZone3 = 1;
            int groundZone4 = 1;
            int flyZone = 1;

            for (var i = 0; i < _zones.Count; i++)
            {
                // Skeleton like enemis: in the future implement also jump
                if (_zones[i].GroundZone1_Normal == int.MaxValue)
                {
                    _zones[i].GroundZone1_Normal = groundZone1;

                    foreach (var box in GetAllReachableBoxes(i, ZoneType.Skeleton, false))
                    {
                        if (_zones[box].GroundZone1_Normal == int.MaxValue) _zones[box].GroundZone1_Normal = groundZone1;
                    }

                    groundZone1++;
                }

                // Mummy like enemis: the simplest case
                if (_zones[i].GroundZone2_Normal == int.MaxValue)
                {
                    _zones[i].GroundZone2_Normal = groundZone2;

                    foreach (var box in GetAllReachableBoxes(i, ZoneType.Basic, false))
                    {
                        if (_zones[box].GroundZone2_Normal == int.MaxValue) _zones[box].GroundZone2_Normal = groundZone2;
                    }

                    groundZone2++;
                }

                // Crocodile like enemis: like 1 & 2 but they can go inside water and swim
                if (_zones[i].GroundZone3_Normal == int.MaxValue)
                {
                    _zones[i].GroundZone3_Normal = groundZone3;

                    foreach (var box in GetAllReachableBoxes(i, ZoneType.Water, false))
                    {
                        if (_zones[box].GroundZone3_Normal == int.MaxValue) _zones[box].GroundZone3_Normal = groundZone3;
                    }

                    groundZone3++;
                }

                // Baddy like enemis: they can jump, grab and monkey
                if (_zones[i].GroundZone4_Normal == int.MaxValue)
                {
                    _zones[i].GroundZone4_Normal = groundZone4;

                    foreach (var box in GetAllReachableBoxes(i, ZoneType.Human, false))
                    {
                        if (_zones[box].GroundZone4_Normal == int.MaxValue) _zones[box].GroundZone4_Normal = groundZone4;
                    }

                    groundZone4++;
                }

                // Bat like enemis: they can fly everywhere, except into the water
                if (_zones[i].FlyZone_Normal == int.MaxValue)
                {
                    _zones[i].FlyZone_Normal = flyZone;

                    foreach (var box in GetAllReachableBoxes(i, ZoneType.Flyer, false))
                    {
                        if (_zones[box].FlyZone_Normal == int.MaxValue) _zones[box].FlyZone_Normal = flyZone;
                    }

                    flyZone++;
                }
            }

            // Flipped rooms------------------------------------------
            int aGroundZone1 = 1;
            int aGroundZone2 = 1;
            int aGroundZone3 = 1;
            int aGroundZone4 = 1;
            int aFlyZone = 1;
            for (var i = 0; i < _zones.Count; i++)
            {
                // Skeleton like enemis: in the future implement also jump
                if (_zones[i].GroundZone1_Alternate == int.MaxValue)
                {
                    _zones[i].GroundZone1_Alternate = aGroundZone1;

                    foreach (var box in GetAllReachableBoxes(i, ZoneType.Skeleton, true))
                    {
                        if (_zones[box].GroundZone1_Alternate == int.MaxValue) _zones[box].GroundZone1_Alternate = aGroundZone1;
                    }

                    aGroundZone1++;
                }

                // Mummy like enemis: the simplest case
                if (_zones[i].GroundZone2_Alternate == int.MaxValue)
                {
                    _zones[i].GroundZone2_Alternate = aGroundZone2;

                    foreach (var box in GetAllReachableBoxes(i, ZoneType.Basic, true))
                    {
                        if (_zones[box].GroundZone2_Alternate == int.MaxValue) _zones[box].GroundZone2_Alternate = aGroundZone2;
                    }

                    aGroundZone2++;
                }

                // Crocodile like enemis: like 1 & 2 but they can go inside water and swim
                if (_zones[i].GroundZone3_Alternate == int.MaxValue)
                {
                    _zones[i].GroundZone3_Alternate = aGroundZone3;

                    foreach (var box in GetAllReachableBoxes(i, ZoneType.Water, true))
                    {
                        if (_zones[box].GroundZone3_Alternate == int.MaxValue) _zones[box].GroundZone3_Alternate = aGroundZone3;
                    }

                    aGroundZone3++;
                }

                // Baddy like enemis: they can jump, grab and monkey
                if (_zones[i].GroundZone4_Alternate == int.MaxValue)
                {
                    _zones[i].GroundZone4_Alternate = aGroundZone4;

                    foreach (var box in GetAllReachableBoxes(i, ZoneType.Human, true))
                    {
                        if (_zones[box].GroundZone4_Alternate == int.MaxValue) _zones[box].GroundZone4_Alternate = aGroundZone4;
                    }

                    aGroundZone4++;
                }

                // Bat like enemis: they can fly everywhere, except into the water
                if (_zones[i].FlyZone_Alternate == int.MaxValue)
                {
                    _zones[i].FlyZone_Alternate = aFlyZone;

                    foreach (var box in GetAllReachableBoxes(i, ZoneType.Flyer, true))
                    {
                        if (_zones[box].FlyZone_Alternate == int.MaxValue) _zones[box].FlyZone_Alternate = aFlyZone;
                    }

                    aFlyZone++;
                }
            }

            ReportProgress(52, "    Number of boxes/zones: " + _boxes.Count);
            ReportProgress(52, "    Number of overlaps: " + _overlaps.Count);
        }

        private IEnumerable<int> GetAllReachableBoxes(int box, ZoneType zoneType, bool flipped)
        {
            var boxes = new List<int>();

            // HACK: boxes with no overlaps have overlapIndex = -1
            if (_boxes[box].OverlapIndex < 0)
                return boxes;

            // This is a non-recursive version of the algorithm for finding all reachable boxes.
            // Avoid recursion all the times you can!
            var stack = new Stack<int>();
            stack.Push(box);

            // All reachable boxes must have the same water flag and same flipped flag
            var isWater = (_tempRooms[dec_boxes[box].Room].Flags & 0x01) != 0;

            while (stack.Count > 0)
            {
                var next = stack.Pop();
                var last = false;

                for (int i = _boxes[next].OverlapIndex; i < _overlaps.Count && !last; i++)
                {
                    int overlapIndex = i;
                    if (overlapIndex < 0)
                        return boxes;

                    last = (_overlaps[overlapIndex].Flags & 0x8000) != 0;

                    bool canJump = (_overlaps[overlapIndex].Flags & 0x0800) != 0;
                    bool canMonkey = (_overlaps[overlapIndex].Flags & 0x2000) != 0;

                    var boxIndex = _overlaps[overlapIndex].Box;

                    // Don't add a box if it doesn't belong to a same flip state.
                    bool sameFlip = (!flipped && dec_boxes[boxIndex].Flag0x04 || flipped && dec_boxes[boxIndex].Flag0x02);
                    if (!sameFlip)
                        continue;

                    bool water = (_tempRooms[dec_boxes[boxIndex].Room].Flags & 0x01) != 0;
                    int  step = Math.Abs(_boxes[next].TrueFloor - _boxes[boxIndex].TrueFloor);

                    // Don't add a box if it is underwater (for fly zone) or a slope (for all other zones).
                    if ((zoneType == ZoneType.Flyer && water) || (zoneType != ZoneType.Flyer && dec_boxes[boxIndex].Slope))
                        continue;

                    // Don't add a box which doesn't match water state.
                    if (water != isWater)
                        continue;

                    bool add = false;

                    switch (zoneType)
                    {
                        case ZoneType.Skeleton:
                            // Enemies like skeletons. They can go only on land, and climb 1 click step. They can also jump 2 blocks.
                            add = (step <= (int)Level.HeightUnit || canJump);
                            break;

                        case ZoneType.Basic:
                            // Enemies like scorpions, mummies, dogs, wild boars. They can go only on land, and climb 1 click step
                            add = (step <= (int)Level.HeightUnit);
                            break;

                        case ZoneType.Water:
                            // Enemies like crocodiles. They can go on land and inside water, and climb 1 click step.
                            // In water they act like flying enemies. Guide seems to belong to this zone.
                            add = (step <= (int)Level.HeightUnit || water);
                            break;

                        case ZoneType.Human:
                            // Enemies like baddy 1 & 2. They can go only on land, and climb 4 clicks step. They can also jump 2 blocks and monkey.
                            add = (step <= (int)Level.BlockSizeUnit || canJump || canMonkey);
                            break;

                        case ZoneType.Flyer:
                            // Flying enemies. Always added, if not a water room (checked in the condition above).
                            add = true;
                            break;

                        default:
                            logger.Error("Unknown zone specified for box " + box);
                            break;
                    }

                    if (stack.Contains(boxIndex) || !add)
                        continue;

                    if (!boxes.Contains(boxIndex))
                        stack.Push(boxIndex);

                    boxes.Add(boxIndex);
                }
            }

            return boxes;
        }
    }
}
