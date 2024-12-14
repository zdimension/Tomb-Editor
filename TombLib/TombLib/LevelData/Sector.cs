﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using TombLib.LevelData.SectorEnums;
using TombLib.LevelData.SectorEnums.Extensions;
using TombLib.LevelData.SectorStructs;
using TombLib.Utils;

namespace TombLib.LevelData
{
    [Serializable]
    public class Sector : ICloneable
    {
        public enum SlopeCalculationMode
        {
            Legacy,
            FreeAngles
        }

        public const int MAX_EXTRA_SPLITS = 8;
        public static Sector Empty { get; } = new Sector();

        public SectorType Type { get; set; } = SectorType.Floor;
        public SectorFlags Flags { get; set; } = SectorFlags.None;
        public bool ForceFloorSolid { get; set; } // If this is set to true, portals are overwritten for this sector.
        public List<SectorSplit> ExtraFloorSplits { get; } = new();
        public List<SectorSplit> ExtraCeilingSplits { get; } = new();
        private Dictionary<FaceLayerInfo, TextureArea> _faceTextures { get; } = new();

        public SectorSurface Floor;
        public SectorSurface Ceiling;

        public List<TriggerInstance> Triggers { get; } = new List<TriggerInstance>(); // This array is not supposed to be modified here.
        public PortalInstance FloorPortal { get; internal set; } = null; // This is not supposed to be modified here.
        public PortalInstance WallPortal { get; internal set; } = null; // This is not supposed to be modified here.
        public PortalInstance CeilingPortal { get; internal set; } = null; // This is not supposed to be modified here.

        public GhostBlockInstance GhostBlock { get; internal set; } = null; // If exists, adds invisible geometry to sector.

        private Sector()
        { }

        public Sector(int floor, int ceiling)
        {
            Floor.XnZp = Floor.XpZp = Floor.XpZn = Floor.XnZn = floor;
            Ceiling.XnZp = Ceiling.XpZp = Ceiling.XpZn = Ceiling.XnZn = ceiling;
        }

        public Sector Clone()
        {
            var result = new Sector
            {
                Type = Type,
                Flags = Flags,
                ForceFloorSolid = ForceFloorSolid,
                Floor = Floor,
                Ceiling = Ceiling
            };

            foreach (KeyValuePair<FaceLayerInfo, TextureArea> entry in _faceTextures)
                result._faceTextures[entry.Key] = entry.Value;

            foreach (SectorSplit split in ExtraFloorSplits)
                result.ExtraFloorSplits.Add((SectorSplit)split.Clone());

            foreach (SectorSplit split in ExtraCeilingSplits)
                result.ExtraCeilingSplits.Add((SectorSplit)split.Clone());

            return result;
        }

        object ICloneable.Clone() => Clone();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasFlag(SectorFlags flag)
        {
            return (Flags & flag) == flag;
        }

        public bool IsFullWall => Type == SectorType.BorderWall || (Type == SectorType.Wall && Floor.DiagonalSplit == DiagonalSplit.None);
        public bool IsAnyWall => Type != SectorType.Floor;
        public bool IsAnyPortal => FloorPortal != null || CeilingPortal != null || WallPortal != null;
        public bool HasGhostBlock => GhostBlock != null;

        public bool SetFaceTexture(FaceLayerInfo face, TextureArea texture)
        {
            if (texture == TextureArea.None)
                return _faceTextures.Remove(face);

            if (texture.TextureIsDegenerate)
                texture = TextureArea.Invisible;

            if (!_faceTextures.ContainsKey(face) || _faceTextures[face] != texture)
            {
                _faceTextures[face] = texture;
                return true;
            }
            else
                return false;
        }

        public TextureArea GetFaceTexture(FaceLayerInfo face)
            => _faceTextures.GetValueOrDefault(face);

        public Dictionary<SectorFace, TextureArea> GetFaceTextures(FaceLayer layer)
            => _faceTextures.Where(kvp => kvp.Key.Layer == layer).ToDictionary(kvp => kvp.Key.Face, kvp => kvp.Value);

        public Dictionary<FaceLayerInfo, TextureArea> GetFaceTexturesAll()
            => _faceTextures;

        public IEnumerable<SectorVerticalPart> GetVerticals()
        {
            yield return SectorVerticalPart.QA;
            yield return SectorVerticalPart.WS;

            for (int i = 0; i < ExtraFloorSplits.Count; i++)
                yield return SectorVerticalPartExtensions.GetExtraFloorSplit(i);

            for (int i = 0; i < ExtraCeilingSplits.Count; i++)
                yield return SectorVerticalPartExtensions.GetExtraCeilingSplit(i);
        }

        public IEnumerable<PortalInstance> Portals
        {
            get
            {
                if (WallPortal != null)
                    yield return WallPortal;
                if (CeilingPortal != null)
                    yield return CeilingPortal;
                if (FloorPortal != null)
                    yield return FloorPortal;
            }
        }

        public void ReplaceGeometry(Level level, Sector replacement)
        {
            if (Type != SectorType.BorderWall) Type = replacement.Type;

            Flags = replacement.Flags;
            ForceFloorSolid = replacement.ForceFloorSolid;

            foreach (FaceLayerInfo face in replacement.GetFaceTexturesAll().Keys.Union(_faceTextures.Keys))
            {
                var texture = replacement.GetFaceTexture(face);
                if (texture.TextureIsInvisible || level.Settings.Textures.Contains(texture.Texture))
                    SetFaceTexture(face, texture);
            }

            Floor = replacement.Floor;
            Ceiling = replacement.Ceiling;

            ExtraFloorSplits.Clear();
            ExtraFloorSplits.AddRange(replacement.ExtraFloorSplits);

            ExtraCeilingSplits.Clear();
            ExtraCeilingSplits.AddRange(replacement.ExtraCeilingSplits);
        }

        public int GetHeight(SectorVerticalPart vertical, SectorEdge edge)
        {
            switch (vertical)
            {
                case SectorVerticalPart.QA:
                    return Floor.GetHeight(edge);
                case SectorVerticalPart.WS:
                    return Ceiling.GetHeight(edge);
            }

            if (vertical.IsExtraFloorSplit())
            {
                int index = vertical.GetExtraSplitIndex();
                SectorSplit split = ExtraFloorSplits.ElementAtOrDefault(index);

                if (split is null)
                    return int.MinValue;

                return split.GetEdge(edge);
            }

            if (vertical.IsExtraCeilingSplit())
            {
                int index = vertical.GetExtraSplitIndex();
                SectorSplit split = ExtraCeilingSplits.ElementAtOrDefault(index);

                if (split is null)
                    return int.MaxValue;

                return split.GetEdge(edge);
            }

            throw new ArgumentOutOfRangeException();
        }

        public void SetHeight(SectorVerticalPart vertical, SectorEdge edge, int newValue)
        {
            if (newValue is int.MinValue or int.MaxValue)
                return;

            switch (vertical)
            {
                case SectorVerticalPart.QA:
                    Floor.SetHeight(edge, newValue);
                    return;
                case SectorVerticalPart.WS:
                    Ceiling.SetHeight(edge, newValue);
                    return;
            }

            if (vertical.IsExtraFloorSplit())
            {
                SectorSplit existingSplit = ExtraFloorSplits.ElementAtOrDefault(vertical.GetExtraSplitIndex());

                if (existingSplit is null)
                {
                    if (!IsValidNextSplit(vertical))
                        return;

                    existingSplit = ExtraFloorSplits.AddAndReturn(new SectorSplit(Floor.Min));
                }

                existingSplit.SetEdge(edge, newValue);
            }
            else if (vertical.IsExtraCeilingSplit())
            {
                SectorSplit existingSplit = ExtraCeilingSplits.ElementAtOrDefault(vertical.GetExtraSplitIndex());

                if (existingSplit is null)
                {
                    if (!IsValidNextSplit(vertical))
                        return;

                    existingSplit = ExtraCeilingSplits.AddAndReturn(new SectorSplit(Ceiling.Max));
                }

                existingSplit.SetEdge(edge, newValue);
            }
        }

        public bool IsValidNextSplit(SectorVerticalPart vertical)
        {
            if (vertical.IsExtraFloorSplit())
                return vertical.GetExtraSplitIndex() == ExtraFloorSplits.Count;
            else if (vertical.IsExtraCeilingSplit())
                return vertical.GetExtraSplitIndex() == ExtraCeilingSplits.Count;
            else
                return false;
        }

        public bool SplitExists(SectorVerticalPart vertical)
        {
            if (vertical.IsExtraFloorSplit())
                return ExtraFloorSplits.ElementAtOrDefault(vertical.GetExtraSplitIndex()) is not null;
            else if (vertical.IsExtraCeilingSplit())
                return ExtraCeilingSplits.ElementAtOrDefault(vertical.GetExtraSplitIndex()) is not null;
            else
                return false;
        }

        private static DiagonalSplit TransformDiagonalSplit(DiagonalSplit split, RectTransformation transformation)
        {
            if (transformation.MirrorX)
                switch (split)
                {
                    case DiagonalSplit.XnZn:
                        split = DiagonalSplit.XpZn;
                        break;
                    case DiagonalSplit.XpZn:
                        split = DiagonalSplit.XnZn;
                        break;
                    case DiagonalSplit.XnZp:
                        split = DiagonalSplit.XpZp;
                        break;
                    case DiagonalSplit.XpZp:
                        split = DiagonalSplit.XnZp;
                        break;
                }

            for (int i = 0; i < transformation.QuadrantRotation; ++i)
                switch (split)
                {
                    case DiagonalSplit.XpZp:
                        split = DiagonalSplit.XnZp;
                        break;
                    case DiagonalSplit.XnZp:
                        split = DiagonalSplit.XnZn;
                        break;
                    case DiagonalSplit.XnZn:
                        split = DiagonalSplit.XpZn;
                        break;
                    case DiagonalSplit.XpZn:
                        split = DiagonalSplit.XpZp;
                        break;
                }

            return split;
        }

        private void MirrorWallTexture(FaceLayerInfo oldFace, Func<FaceLayerInfo, FaceShape> oldFaceIsTriangle)
        {
            if (!_faceTextures.TryGetValue(oldFace, out TextureArea area))
                return;

            switch (oldFaceIsTriangle(oldFace))
            {
                case FaceShape.Triangle:
                    Swap.Do(ref area.TexCoord0, ref area.TexCoord1);
                    break;
                case FaceShape.Quad:
                    Swap.Do(ref area.TexCoord0, ref area.TexCoord3);
                    Swap.Do(ref area.TexCoord1, ref area.TexCoord2);
                    break;
            }

            _faceTextures[oldFace] = area;
        }

        // Rotates and mirrors a sector according to a given transformation. The transformation can be combination of rotations and mirrors
        // but all possible transformation essentially are boiled down to a mirror on the x axis and a rotation afterwards.
        // Thus all that needs to be handled is mirroring on x and a single counterclockwise rotation (perhaps multiple times in a row).
        // When mirroring is done, a oldFaceIsTriangle must be provided that can return the previous shape of texture faces.
        // Set "onlyFloor" to true, to only modify the floor.
        // Set "onlyFloor" to false, to only modify the ceiling.
        public void Transform(RectTransformation transformation, bool? onlyFloor = null, Func<FaceLayerInfo, FaceShape> oldFaceIsTriangle = null)
        {
            // Rotate sector flags
            if (transformation.MirrorX)
                Flags =
                    (Flags & ~(SectorFlags.ClimbPositiveX | SectorFlags.ClimbNegativeX)) |
                    ((Flags & SectorFlags.ClimbPositiveX) != SectorFlags.None ? SectorFlags.ClimbNegativeX : SectorFlags.None) |
                    ((Flags & SectorFlags.ClimbNegativeX) != SectorFlags.None ? SectorFlags.ClimbPositiveX : SectorFlags.None);

            for (int i = 0; i < transformation.QuadrantRotation; ++i)
                Flags =
                    (Flags & ~(SectorFlags.ClimbPositiveX | SectorFlags.ClimbPositiveZ | SectorFlags.ClimbNegativeX | SectorFlags.ClimbNegativeZ)) |
                    ((Flags & SectorFlags.ClimbPositiveX) != SectorFlags.None ? SectorFlags.ClimbPositiveZ : SectorFlags.None) |
                    ((Flags & SectorFlags.ClimbPositiveZ) != SectorFlags.None ? SectorFlags.ClimbNegativeX : SectorFlags.None) |
                    ((Flags & SectorFlags.ClimbNegativeX) != SectorFlags.None ? SectorFlags.ClimbNegativeZ : SectorFlags.None) |
                    ((Flags & SectorFlags.ClimbNegativeZ) != SectorFlags.None ? SectorFlags.ClimbPositiveX : SectorFlags.None);

            // Rotate sector geometry
            bool diagonalChange = transformation.MirrorX != (transformation.QuadrantRotation % 2 != 0);
            bool oldFloorSplitDirectionIsXEqualsZReal = Floor.SplitDirectionIsXEqualsZWithDiagonalSplit;
            if (onlyFloor != false)
            {
                bool requiredFloorSplitDirectionIsXEqualsZ = Floor.SplitDirectionIsXEqualsZ != diagonalChange;
                Floor.DiagonalSplit = TransformDiagonalSplit(Floor.DiagonalSplit, transformation);
                transformation.TransformValueDiagonalQuad(ref Floor.XpZp, ref Floor.XnZp, ref Floor.XnZn, ref Floor.XpZn);

                for (int i = 0; i < ExtraFloorSplits.Count; i++)
                {
                    SectorSplit split = ExtraFloorSplits[i];
                    transformation.TransformValueDiagonalQuad(ref split.XpZp, ref split.XnZp, ref split.XnZn, ref split.XpZn);
                }

                if (requiredFloorSplitDirectionIsXEqualsZ != Floor.SplitDirectionIsXEqualsZ)
                    Floor.SplitDirectionToggled = !Floor.SplitDirectionToggled;

                if (HasGhostBlock)
                    transformation.TransformValueDiagonalQuad(ref GhostBlock.Floor.XpZp, ref GhostBlock.Floor.XnZp, ref GhostBlock.Floor.XnZn, ref GhostBlock.Floor.XpZn);

            }

            bool oldCeilingSplitDirectionIsXEqualsZReal = Ceiling.SplitDirectionIsXEqualsZWithDiagonalSplit;
            if (onlyFloor != true)
            {
                bool requiredCeilingSplitDirectionIsXEqualsZ = Ceiling.SplitDirectionIsXEqualsZ != diagonalChange;
                Ceiling.DiagonalSplit = TransformDiagonalSplit(Ceiling.DiagonalSplit, transformation);
                transformation.TransformValueDiagonalQuad(ref Ceiling.XpZp, ref Ceiling.XnZp, ref Ceiling.XnZn, ref Ceiling.XpZn);

                for (int i = 0; i < ExtraCeilingSplits.Count; i++)
                {
                    SectorSplit split = ExtraCeilingSplits[i];
                    transformation.TransformValueDiagonalQuad(ref split.XpZp, ref split.XnZp, ref split.XnZn, ref split.XpZn);
                }

                if (requiredCeilingSplitDirectionIsXEqualsZ != Ceiling.SplitDirectionIsXEqualsZ)
                    Ceiling.SplitDirectionToggled = !Ceiling.SplitDirectionToggled;

                if (HasGhostBlock)
                    transformation.TransformValueDiagonalQuad(ref GhostBlock.Ceiling.XpZp, ref GhostBlock.Ceiling.XnZp, ref GhostBlock.Ceiling.XnZn, ref GhostBlock.Ceiling.XpZn);
            }

            for (FaceLayer layer = 0; layer < FaceLayer.Count; layer++)
            {
            // Rotate applied textures
            if (onlyFloor != false)
            {
                // Fix lower wall textures
                if (transformation.MirrorX)
                {
                    var faces = _faceTextures.Where(pair => pair.Key.Face.IsFloorWall()).Select(pair => pair.Key).ToList();

                    for (int i = 0; i < faces.Count; i++)
                        MirrorWallTexture(faces[i], oldFaceIsTriangle);
                }

                transformation.TransformValueQuad(_faceTextures, SectorFace.Wall_PositiveX_QA, SectorFace.Wall_PositiveZ_QA, SectorFace.Wall_NegativeX_QA, SectorFace.Wall_NegativeZ_QA);

                for (int i = 0; i < ExtraFloorSplits.Count; i++)
                {
                    transformation.TransformValueQuad(_faceTextures,
                        SectorFaceExtensions.GetExtraFloorSplitFace(Direction.PositiveX, i),
                        SectorFaceExtensions.GetExtraFloorSplitFace(Direction.PositiveZ, i),
                        SectorFaceExtensions.GetExtraFloorSplitFace(Direction.NegativeX, i),
                        SectorFaceExtensions.GetExtraFloorSplitFace(Direction.NegativeZ, i));
                }

                FaceLayerInfo
                    floorKey = new(SectorFace.Floor, layer),
                    floorTriangle2Key = new(SectorFace.Floor_Triangle2, layer);

                // Fix floor textures
                if (Floor.IsQuad)
                {
                    if (_faceTextures.ContainsKey(floorKey))
                        _faceTextures[floorKey] = _faceTextures[floorKey].Transform(transformation * new RectTransformation { QuadrantRotation = 2 });
                }
                else
                {
                    // Mirror
                    if (transformation.MirrorX)
                    {
                        _faceTextures.TrySwap(floorKey, floorTriangle2Key);

                        if (_faceTextures.ContainsKey(floorKey))
                        {
                            TextureArea floor = _faceTextures[floorKey];
                            Swap.Do(ref floor.TexCoord0, ref floor.TexCoord2);
                            _faceTextures[floorKey] = floor;
                        }

                        if (_faceTextures.ContainsKey(floorTriangle2Key))
                        {
                            TextureArea floorTriangle2 = _faceTextures[floorTriangle2Key];
                            Swap.Do(ref floorTriangle2.TexCoord0, ref floorTriangle2.TexCoord2);
                            _faceTextures[floorTriangle2Key] = floorTriangle2;
                        }

                        if (Floor.DiagonalSplit != DiagonalSplit.None) // REMOVE this when we have better diaognal steps.
                            _faceTextures.TrySwap(floorKey, floorTriangle2Key);
                    }

                    // Rotation
                    for (int i = 0; i < transformation.QuadrantRotation; ++i)
                    {
                        if (!oldFloorSplitDirectionIsXEqualsZReal)
                            _faceTextures.TrySwap(floorKey, floorTriangle2Key);
                        if (Floor.DiagonalSplit != DiagonalSplit.None) // REMOVE this when we have better diaognal steps.
                            _faceTextures.TrySwap(floorKey, floorTriangle2Key);

                        oldFloorSplitDirectionIsXEqualsZReal = !oldFloorSplitDirectionIsXEqualsZReal;
                    }
                }
            }
            if (onlyFloor != true)
            {
                // Fix upper wall textures
                if (transformation.MirrorX)
                {
                    var faces = _faceTextures.Where(pair => pair.Key.Face.IsCeilingWall()).Select(pair => pair.Key).ToList();

                    for (int i = 0; i < faces.Count; i++)
                        MirrorWallTexture(faces[i], oldFaceIsTriangle);
                }

                transformation.TransformValueQuad(_faceTextures, SectorFace.Wall_PositiveX_WS, SectorFace.Wall_PositiveZ_WS, SectorFace.Wall_NegativeX_WS, SectorFace.Wall_NegativeZ_WS);

                for (int i = 0; i < ExtraCeilingSplits.Count; i++)
                {
                    transformation.TransformValueQuad(_faceTextures,
                        SectorFaceExtensions.GetExtraCeilingSplitFace(Direction.PositiveX, i),
                        SectorFaceExtensions.GetExtraCeilingSplitFace(Direction.PositiveZ, i),
                        SectorFaceExtensions.GetExtraCeilingSplitFace(Direction.NegativeX, i),
                        SectorFaceExtensions.GetExtraCeilingSplitFace(Direction.NegativeZ, i));
                }

                FaceLayerInfo
                    ceilingKey = new(SectorFace.Ceiling, layer),
                    ceilingTriangle2Key = new(SectorFace.Ceiling_Triangle2, layer);

                // Fix ceiling textures
                if (Ceiling.IsQuad)
                {
                    if (_faceTextures.ContainsKey(ceilingKey))
                        _faceTextures[ceilingKey] = _faceTextures[ceilingKey].Transform(transformation * new RectTransformation { QuadrantRotation = 2 });
                }
                else
                {
                    // Mirror
                    if (transformation.MirrorX)
                    {
                        _faceTextures.TrySwap(ceilingKey, ceilingTriangle2Key);

                        if (_faceTextures.ContainsKey(ceilingKey))
                        {
                            TextureArea ceiling = _faceTextures[ceilingKey];
                            Swap.Do(ref ceiling.TexCoord0, ref ceiling.TexCoord2);
                            _faceTextures[ceilingKey] = ceiling;
                        }

                        if (_faceTextures.ContainsKey(ceilingTriangle2Key))
                        {
                            TextureArea ceilingTriangle2 = _faceTextures[ceilingTriangle2Key];
                            Swap.Do(ref ceilingTriangle2.TexCoord0, ref ceilingTriangle2.TexCoord2);
                            _faceTextures[ceilingTriangle2Key] = ceilingTriangle2;
                        }

                        if (Ceiling.DiagonalSplit != DiagonalSplit.None) // REMOVE this when we have better diaognal steps.
                            _faceTextures.TrySwap(ceilingKey, ceilingTriangle2Key);
                    }

                    // Rotation
                    for (int i = 0; i < transformation.QuadrantRotation; ++i)
                    {
                        if (!oldCeilingSplitDirectionIsXEqualsZReal)
                            _faceTextures.TrySwap(ceilingKey, ceilingTriangle2Key);
                        if (Ceiling.DiagonalSplit != DiagonalSplit.None) // REMOVE this when we have better diaognal steps.
                            _faceTextures.TrySwap(ceilingKey, ceilingTriangle2Key);

                        oldCeilingSplitDirectionIsXEqualsZReal = !oldCeilingSplitDirectionIsXEqualsZReal;
                    }
                }
            }
            if (onlyFloor == null)
            {
                if (transformation.MirrorX)
                {
                    MirrorWallTexture(new(SectorFace.Wall_PositiveX_Middle, layer), oldFaceIsTriangle);
                    MirrorWallTexture(new(SectorFace.Wall_PositiveZ_Middle, layer), oldFaceIsTriangle);
                    MirrorWallTexture(new(SectorFace.Wall_NegativeX_Middle, layer), oldFaceIsTriangle);
                    MirrorWallTexture(new(SectorFace.Wall_NegativeZ_Middle, layer), oldFaceIsTriangle);
                    MirrorWallTexture(new(SectorFace.Wall_Diagonal_Middle, layer), oldFaceIsTriangle);
                }

                transformation.TransformValueQuad(_faceTextures, SectorFace.Wall_PositiveX_Middle, SectorFace.Wall_PositiveZ_Middle, SectorFace.Wall_NegativeX_Middle, SectorFace.Wall_NegativeZ_Middle);
            }
            }
        }

        public void FixHeights(SectorVerticalPart? vertical = null)
        {
            for (SectorEdge i = 0; i < SectorEdge.Count; i++)
            {
                for (int j = 0; j < ExtraFloorSplits.Count; j++)
                {
                    SectorVerticalPart splitVertical = SectorVerticalPartExtensions.GetExtraFloorSplit(j);
                    SectorVerticalPart lastSectorVertical = j == 0 ? SectorVerticalPart.QA : SectorVerticalPartExtensions.GetExtraFloorSplit(j - 1);
                    SetHeight(splitVertical, i, Math.Min(GetHeight(splitVertical, i), GetHeight(lastSectorVertical, i)));
                }

                for (int j = 0; j < ExtraCeilingSplits.Count; j++)
                {
                    SectorVerticalPart splitVertical = SectorVerticalPartExtensions.GetExtraCeilingSplit(j);
                    SectorVerticalPart lastSectorVertical = j == 0 ? SectorVerticalPart.WS : SectorVerticalPartExtensions.GetExtraCeilingSplit(j - 1);
                    SetHeight(splitVertical, i, Math.Max(GetHeight(splitVertical, i), GetHeight(lastSectorVertical, i)));
                }

                if (vertical == null || vertical.Value.IsOnFloor())
                    if (Floor.DiagonalSplit != DiagonalSplit.None)
                        Floor.SetHeight(i, Math.Min(Floor.GetHeight(i), Ceiling.Min));
                    else
                        Floor.SetHeight(i, Math.Min(Floor.GetHeight(i), Ceiling.GetHeight(i)));

                if (vertical == null || vertical.Value.IsOnCeiling())
                    if (Ceiling.DiagonalSplit != DiagonalSplit.None)
                        Ceiling.SetHeight(i, Math.Max(Ceiling.GetHeight(i), Floor.Max));
                    else
                        Ceiling.SetHeight(i, Math.Max(Ceiling.GetHeight(i), Floor.GetHeight(i)));
            }
        }

        public Vector3[] GetFloorTriangleNormals()
        {
            Plane[] tri = new Plane[2];

            var p0 = new Vector3(0, Floor.XnZp, 0);
            var p1 = new Vector3(1024, Floor.XpZp, 0);
            var p2 = new Vector3(1024, Floor.XpZn, -1024);
            var p3 = new Vector3(0, Floor.XnZn, -1024);

            // Create planes based on floor split direction

            if (Floor.SplitDirectionIsXEqualsZ)
            {
                tri[0] = Plane.CreateFromVertices(p1, p2, p3);
                tri[1] = Plane.CreateFromVertices(p1, p3, p0);
            }
            else
            {
                tri[0] = Plane.CreateFromVertices(p0, p1, p2);
                tri[1] = Plane.CreateFromVertices(p0, p2, p3);
            }

            return new Vector3[2] { tri[0].Normal, tri[1].Normal };
        }

        public int GetTriangleMinimumFloorPoint(int triangle)
        {
            if (triangle != 0 && triangle != 1)
                return 0;

            if (Floor.SplitDirectionIsXEqualsZ)
            {
                if (triangle == 0)
                    return Math.Min(Math.Min(Floor.XpZp, Floor.XpZn), Floor.XnZn);
                else
                    return Math.Min(Math.Min(Floor.XpZp, Floor.XnZn), Floor.XnZp);
            }
            else
            {
                if (triangle == 0)
                    return Math.Min(Math.Min(Floor.XnZp, Floor.XpZp), Floor.XpZn);
                else
                    return Math.Min(Math.Min(Floor.XnZp, Floor.XpZn), Floor.XnZn);
            }
        }

        public Direction[] GetFloorTriangleSlopeDirections(SlopeCalculationMode mode)
        {
            var normals = GetFloorTriangleNormals();

            // Initialize slope directions as unslidable by default (EntireFace means unslidable in our case).

            var slopeDirections = new Direction[2] { Direction.None, Direction.None };

            if (!Floor.HasSlope())
                return slopeDirections;

            for (int i = 0; i < (Floor.IsQuad ? 1 : 2); i++) // If floor is quad, we don't solve second triangle
            {
                if (mode == SlopeCalculationMode.Legacy)
                {
                    // Use legacy tilt mode calculation from TEN, as it's more consistent.

                    float scaleFactor = 1.0f / normals[i].Y;
                    var scaledNormal = normals[i] * -scaleFactor;

                    var tilt = new Vector2(
                        (float)(Math.Round(scaledNormal.X, MidpointRounding.AwayFromZero) * 4.0f),
                        (float)(Math.Round(scaledNormal.Z, MidpointRounding.AwayFromZero) * 4.0f));

                    if (tilt.X > 2)
                        slopeDirections[i] = Direction.NegativeX;
                    else if (tilt.X < -2)
                        slopeDirections[i] = Direction.PositiveX;

                    if (tilt.Y > 2 && tilt.Y > Math.Abs(tilt.X))
                        slopeDirections[i] = Direction.NegativeZ;
                    else if (tilt.Y < -2 && -tilt.Y > Math.Abs(tilt.X))
                        slopeDirections[i] = Direction.PositiveZ;
                }
            }

            // We swap triangle directions for XpZn and XnZp cases, because in these cases
            // triangle indices are inverted.
            // For other cases, we move slide direction triangle to proper one accordingly to
            // step slant value encoded in corner heights.

            if (Floor.DiagonalSplit != DiagonalSplit.None)
            {
                switch (Floor.DiagonalSplit)
                {
                    case DiagonalSplit.XpZn:
                        slopeDirections[1] = slopeDirections[0];
                        slopeDirections[0] = Direction.None;
                        break;

                    case DiagonalSplit.XnZp:
                        if (!Floor.IsQuad)
                        {
                            slopeDirections[0] = slopeDirections[1];
                            slopeDirections[1] = Direction.None;
                        }
                        break;

                    case DiagonalSplit.XnZn:
                        if (Floor.IsQuad)
                        {
                            slopeDirections[1] = slopeDirections[0];
                            slopeDirections[0] = Direction.None;
                        }
                        else
                            slopeDirections[0] = Direction.None;
                        break;

                    case DiagonalSplit.XpZp:
                        if (!Floor.IsQuad)
                            slopeDirections[1] = Direction.None;
                        break;
                }
            }

            return slopeDirections;
        }
    }
}
