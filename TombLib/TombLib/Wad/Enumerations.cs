﻿namespace TombLib.Wad
{
    public enum SoundSystem : long
    {
        None = 0,
        Xml = 1
    }

    public enum WadAnimCommandType : short
    {
        SetPosition = 1,
        SetJumpDistance = 2,
        EmptyHands = 3,
        KillEntity = 4,
        PlaySound = 5,
        FlipEffect = 6,
        
        // TEN specific
        DisableInterpolation = 7
    }

    public enum WadMeshLightingType : ushort
    {
        Normals,
        VertexColors
    }

    public enum WadLinkOpcode : ushort
    {
        NotUseStack = 0,
        Pop = 1,
        Push = 2,
        Read = 3
    }
}
