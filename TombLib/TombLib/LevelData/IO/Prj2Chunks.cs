﻿using System.Collections.Generic;
using System.Linq;
using TombLib.IO;

namespace TombLib.LevelData.IO
{
    // TODO Add documentation for binary offsets of the chunks
    internal static class Prj2Chunks
    {        
        public static HashSet<ChunkId> ChunkList => new HashSet<ChunkId>(typeof(Prj2Chunks).GetFields().Where(f => f.FieldType == typeof(ChunkId)).Select(v => v.GetValue(v)).Cast<ChunkId>());

        public static readonly byte[] MagicNumber = new byte[] { 0x50, 0x52, 0x4A, 0x32 };

        public static readonly ChunkId Settings = ChunkId.FromString("TeSettings");
        /**/public static readonly ChunkId LastRoom = ChunkId.FromString("TeLastRoom"); // UTF-8 string
        /**/public static readonly ChunkId SoundSystem = ChunkId.FromString("TeSoundSystem"); // UTF-8 string
        /**/public static readonly ChunkId ObsoleteWadFilePath = ChunkId.FromString("TeWadFilePath"); // UTF-8 string
        /**/public static readonly ChunkId FontTextureFilePath = ChunkId.FromString("TeFontTextureFilePath"); // UTF-8 string
        /**/public static readonly ChunkId SkyTextureFilePath = ChunkId.FromString("TeSkyTextureFilePath"); // UTF-8 string
        /**/public static readonly ChunkId SoundsCatalogs = ChunkId.FromString("TeSoundsCatalogs"); // UTF-8 string
        /****/public static readonly ChunkId SoundsCatalog = ChunkId.FromString("TeSoundsCatalog"); // UTF-8 string
        /******/public static readonly ChunkId SoundsCatalogPath = ChunkId.FromString("TeSoundsCatalogPath"); // UTF-8 string
        /**/public static readonly ChunkId Tr5ExtraSpritesFilePath = ChunkId.FromString("TeTr5ExtraSpritesFilePath"); // UTF-8 string
        /**/public static readonly ChunkId Tr5LaraType = ChunkId.FromString("TeTr5LaraType");
        /**/public static readonly ChunkId Tr5Weather = ChunkId.FromString("TeTr5Weather");
        /**/public static readonly ChunkId TenLuaScriptFile = ChunkId.FromString("TenLuaScriptFile");
        /**/public static readonly ChunkId OldWadSoundPaths = ChunkId.FromString("TeOldWadSoundPaths");
        /****/public static readonly ChunkId OldWadSoundUpdateTag1_0_8 = ChunkId.FromString("TeOldWadSoundUpdateTag1_0_8");
        /****/public static readonly ChunkId OldWadSoundPath = ChunkId.FromString("TeOldWadSoundPath");
        /********/public static readonly ChunkId OldWadSoundPathPath = ChunkId.FromString("TePath"); // UTF-8 string
        /**/public static readonly ChunkId ScriptDirectory = ChunkId.FromString("TeScriptDirectory"); // UTF-8 string
        /**/public static readonly ChunkId GameDirectory = ChunkId.FromString("TeGameDirectory"); // UTF-8 string
        /**/public static readonly ChunkId GameLevelFilePath = ChunkId.FromString("TeGameLevelFilePath"); // UTF-8 string
        /**/public static readonly ChunkId GameExecutableFilePath = ChunkId.FromString("TeGameExecutableFilePath"); // UTF-8 string
        /**/public static readonly ChunkId GameEnableQuickStartFeature = ChunkId.FromString("TeGameEnableQuickStartFeature"); // UTF-8 string
        /**/public static readonly ChunkId GameEnableExtraBlendingModes = ChunkId.FromString("TeGameEnableExtraBlendingModes"); // UTF-8 string
        /**/public static readonly ChunkId GameEnableExtraReverbPresets = ChunkId.FromString("TeGameEnableExtraReverbPresets"); // UTF-8 string
        /**/public static readonly ChunkId GameVersion = ChunkId.FromString("TeGameVersion");
        /**/public static readonly ChunkId DefaultAmbientLight = ChunkId.FromString("TeDefaultAmbientLight");
        /**/public static readonly ChunkId DefaultLightQuality = ChunkId.FromString("TeDefaultLightQuality");
        /**/public static readonly ChunkId OverrideLightQuality = ChunkId.FromString("TeOverrideLightQuality");
        /**/public static readonly ChunkId Dither16BitTextures = ChunkId.FromString("TeDitherTextures");
        /**/public static readonly ChunkId TexturePadding = ChunkId.FromString("TeTexturePadding");
        /**/public static readonly ChunkId TextureCompression = ChunkId.FromString("TeTextureCompression");
        /**/public static readonly ChunkId AgressiveTexturePacking = ChunkId.FromString("TeAgressiveTexturePacking");
        /**/public static readonly ChunkId AgressiveFloordataPacking = ChunkId.FromString("TeAgressiveFloordataPacking");
        /**/public static readonly ChunkId RemapAnimatedTextures = ChunkId.FromString("TeRemapAnimTextures");
        /**/public static readonly ChunkId RearrangeRooms = ChunkId.FromString("TeRearrangeRooms");
		/**/public static readonly ChunkId RemoveUnusedObjects = ChunkId.FromString("TeRemoveUnusedObjects");
		/**/public static readonly ChunkId EnableCustomSampleRate = ChunkId.FromString("TeEnableCustomSampleRate");
		/**/public static readonly ChunkId CustomSampleRate = ChunkId.FromString("TeCustomSampleRate");
        /**/public static readonly ChunkId Room32BitLighting = ChunkId.FromString("TeRoom32BitLighting");
		/**/public static readonly ChunkId Wads = ChunkId.FromString("TeWads");
        /****/public static readonly ChunkId Wad = ChunkId.FromString("TeWad");
        /**/public static readonly ChunkId SelectedSounds = ChunkId.FromString("TeSelectedSounds");
        /****/public static readonly ChunkId SelectedSound = ChunkId.FromString("TeSelSnd");
        /******/public static readonly ChunkId WadPath = ChunkId.FromString("TePath");
        /**/public static readonly ChunkId Textures = ChunkId.FromString("TeTextures");
        /******/public static readonly ChunkId InvisibleTexture = ChunkId.FromString("TeInvisibleTexture");
        /******/public static readonly ChunkId LevelTexture = ChunkId.FromString("TeLvlTexture");
        /**********/public static readonly ChunkId LevelTextureIndex = ChunkId.FromString("TeI");
        /**********/public static readonly ChunkId LevelTexturePath = ChunkId.FromString("TePath");
        /**********/public static readonly ChunkId LevelTextureConvert512PixelsToDoubleRows = ChunkId.FromString("Te512C");
        /**********/public static readonly ChunkId LevelTextureReplaceMagentaWithTransparency = ChunkId.FromString("TeMagentaR");
        /**********/public static readonly ChunkId LevelTextureFootStepSounds = ChunkId.FromString("TeTextureSounds");
        /**********/public static readonly ChunkId LevelTextureBumpmaps = ChunkId.FromString("TeTextureBumpmaps");
        /***********/public static readonly ChunkId LevelTextureCustomBumpmapPath = ChunkId.FromString("TeTextureCustomBumpmapPath");
        /**/public static readonly ChunkId ImportedGeometries = ChunkId.FromString("TeImportedGeometries");
        /******/public static readonly ChunkId ImportedGeometry = ChunkId.FromString("TeImportedGeometry");
        /********/public static readonly ChunkId ImportedGeometryIndex = ChunkId.FromString("TeI");
        /********/public static readonly ChunkId ImportedGeometryName = ChunkId.FromString("TeName");
        /********/public static readonly ChunkId ImportedGeometryPath = ChunkId.FromString("TePath");
        /********/public static readonly ChunkId ImportedGeometryScale = ChunkId.FromString("TeScale");
        /********/public static readonly ChunkId ImportedGeometryPosAxisFlags = ChunkId.FromString("TePosAxisFlags");
        /********/public static readonly ChunkId ImportedGeometryTexAxisFlags = ChunkId.FromString("TeTexAxisFlags");
        /********/public static readonly ChunkId ImportedGeometryInvertFaces = ChunkId.FromString("TeInvertFaces");
        /********/public static readonly ChunkId ImportedGeometryMappedUV = ChunkId.FromString("TeMappedUV");
        /**/public static readonly ChunkId AnimatedTextureSets = ChunkId.FromString("TeAnimatedTextureSets");
        /******/public static readonly ChunkId AnimatedTextureSet = ChunkId.FromString("TeAnimatedTextureSet");
        /**********/public static readonly ChunkId AnimatedTextureSetName = ChunkId.FromString("TeAnimatedTextureSetName");
        /**********/public static readonly ChunkId AnimatedTextureSetType = ChunkId.FromString("TeAnimatedTextureSetType");
        /**********/public static readonly ChunkId AnimatedTextureSetFps = ChunkId.FromString("TeAnimatedTextureSetFps");
        /**********/public static readonly ChunkId AnimatedTextureSetUvRotate = ChunkId.FromString("TeAnimatedTextureSetUvRotate");
        /**********/public static readonly ChunkId AnimatedTextureSetExtraInfo = ChunkId.FromString("TeAnimatedTextureSetExtra");
        /**************/public static readonly ChunkId AnimatedTextureFrames = ChunkId.FromString("TeFrames");
        /******************/public static readonly ChunkId AnimatedTextureFrame = ChunkId.FromString("TeFrame");
        /**/public static readonly ChunkId EventSets = ChunkId.FromString("TeEventSets");
        /**/public static readonly ChunkId GlobalEventSets = ChunkId.FromString("TeGlobalEventSets");
        /**/public static readonly ChunkId VolumeEventSets = ChunkId.FromString("TeVolumeEventSets");
        /******/public static readonly ChunkId EventSet = ChunkId.FromString("TeEventSet");
        /********/public static readonly ChunkId EventSetIndex = ChunkId.FromString("TeEventSetIndex");
        /********/public static readonly ChunkId EventSetName = ChunkId.FromString("TeEventSetName");
        /********/public static readonly ChunkId EventSetLastUsedEventIndex = ChunkId.FromString("TeEventSetLUEI");
        /********/public static readonly ChunkId EventSetActivators = ChunkId.FromString("TeEventSetActivators");
        /********/public static readonly ChunkId EventSetOnEnter = ChunkId.FromString("TeEventSetOnEnter");
        /********/public static readonly ChunkId EventSetOnLeave = ChunkId.FromString("TeEventSetOnLeave");
        /********/public static readonly ChunkId EventSetOnInside = ChunkId.FromString("TeEventSetOnInside");
        /********/public static readonly ChunkId Event = ChunkId.FromString("TeEvent");
        /**********/public static readonly ChunkId EventMode = ChunkId.FromString("TeEventMode");
        /**********/public static readonly ChunkId EventFunction = ChunkId.FromString("TeEventFunction");
        /**********/public static readonly ChunkId EventArgument = ChunkId.FromString("TeEventArgument");
        /**********/public static readonly ChunkId EventCallCounter = ChunkId.FromString("TeEventCallCounter");
        /**********/public static readonly ChunkId EventNodePosition = ChunkId.FromString("TeEventNodePos");
        /**********/public static readonly ChunkId EventNodeNext = ChunkId.FromString("TeEventNodeNext");
        /**********/public static readonly ChunkId EventNodeElse = ChunkId.FromString("TeEventNodeElse");
        /**********/public static readonly ChunkId EventType = ChunkId.FromString("TeEventType");
        /***********/public static readonly ChunkId NodeType = ChunkId.FromString("TeNodeType");
        /***********/public static readonly ChunkId NodeName = ChunkId.FromString("TeNodeName");
        /***********/public static readonly ChunkId NodeSize = ChunkId.FromString("TeNodeSize");
        /***********/public static readonly ChunkId NodeScreenPosition = ChunkId.FromString("TeNodeScrPos");
        /***********/public static readonly ChunkId NodeColor = ChunkId.FromString("TeNodeColor");
        /***********/public static readonly ChunkId NodeLocked = ChunkId.FromString("TeNodeLocked");
        /***********/public static readonly ChunkId NodeFunction = ChunkId.FromString("TeNodeFunc");
        /***********/public static readonly ChunkId NodeArgument = ChunkId.FromString("TeNodeArg");
        public static readonly ChunkId Rooms = ChunkId.FromString("TeRooms");
        /**/public static readonly ChunkId Room = ChunkId.FromString("TeRoom"); // Contains X, Y sectors, Name, Position directly
        /******/public static readonly ChunkId RoomIndex = ChunkId.FromString("TeI");
        /******/public static readonly ChunkId RoomName = ChunkId.FromString("TeName");
        /******/public static readonly ChunkId RoomPosition = ChunkId.FromString("TePos"); // DEPRECATED
        /******/public static readonly ChunkId RoomPosition2 = ChunkId.FromString("TePos2");
        /******/public static readonly ChunkId RoomSectors = ChunkId.FromString("TeSecs");
        /**********/public static readonly ChunkId Sector = ChunkId.FromString("TeS");
        /**************/public static readonly ChunkId SectorProperties = new ChunkId(new byte[] { 0 }); // These chunks occur very often, this minimizes their size impact
        /**************/public static readonly ChunkId SectorFloor = new ChunkId(new byte[] { 1 }); // DEPRECATED
        /**************/public static readonly ChunkId SectorCeiling = new ChunkId(new byte[] { 2 }); // DEPRECATED
        /**************/public static readonly ChunkId SectorFloorOnly = new ChunkId(new byte[] { 3 }); // DEPRECATED
        /**************/public static readonly ChunkId SectorCeilingOnly = new ChunkId(new byte[] { 4 }); // DEPRECATED
        /**************/public static readonly ChunkId SectorFloorSubdivisions = new ChunkId(new byte[] { 5 }); // DEPRECATED
        /**************/public static readonly ChunkId SectorCeilingSubdivisions = new ChunkId(new byte[] { 6 }); // DEPRECATED
        /**************/public static readonly ChunkId SectorFloorOnly2 = new ChunkId(new byte[] { 7 });
        /**************/public static readonly ChunkId SectorCeilingOnly2 = new ChunkId(new byte[] { 8 });
        /**************/public static readonly ChunkId SectorFloorSubdivisions2 = new ChunkId(new byte[] { 9 });
        /**************/public static readonly ChunkId SectorCeilingSubdivisions2 = new ChunkId(new byte[] { 10 });
        /**************/public static readonly ChunkId TextureLevelTexture = new ChunkId(new byte[] { 16 });
        /**************/public static readonly ChunkId TextureLevelTexture2 = new ChunkId(new byte[] { 18 });
        /**************/public static readonly ChunkId TextureLevelTexture3 = new ChunkId(new byte[] { 19 });
        /**************/public static readonly ChunkId TextureInvisible = new ChunkId(new byte[] { 17 });
        /**************/public static readonly ChunkId TextureInvisible2 = new ChunkId(new byte[] { 20 });
        /******/public static readonly ChunkId RoomAmbientLight = ChunkId.FromString("TeAmbient");
        /******/public static readonly ChunkId RoomAlternate = ChunkId.FromString("TeAlternate");
        /**********/public static readonly ChunkId AlternateRoom = ChunkId.FromString("TeRoom");
        /**********/public static readonly ChunkId AlternateGroup = ChunkId.FromString("TeGroup");
        /******/public static readonly ChunkId RoomFlagCold = ChunkId.FromString("TeCold");
        /******/public static readonly ChunkId RoomFlagDamage = ChunkId.FromString("TeDmg");
        /******/public static readonly ChunkId RoomFlagHorizon = ChunkId.FromString("TeHorizon");
        /******/public static readonly ChunkId RoomFlagOutside = ChunkId.FromString("TeOutside");
        /******/public static readonly ChunkId RoomFlagNoLensflare = ChunkId.FromString("TeNoLens");
        /******/public static readonly ChunkId RoomFlagExcludeFromPathFinding = ChunkId.FromString("TeNoPath");
        /******/public static readonly ChunkId RoomLightInterpolationMode = ChunkId.FromString("TeRoomLightInt");
        /******/public static readonly ChunkId RoomWaterLevel = ChunkId.FromString("TeWater");  // DEPRECATED
        /******/public static readonly ChunkId RoomRainLevel = ChunkId.FromString("TeRain");  // DEPRECATED
        /******/public static readonly ChunkId RoomSnowLevel = ChunkId.FromString("TeSnow");  // DEPRECATED
        /******/public static readonly ChunkId RoomQuickSandLevel = ChunkId.FromString("TeQuickSand");  // DEPRECATED
        /******/public static readonly ChunkId RoomMistLevel = ChunkId.FromString("TeMist");  // DEPRECATED
        /******/public static readonly ChunkId RoomReflectionLevel = ChunkId.FromString("TeReflect");  // DEPRECATED
        /******/public static readonly ChunkId RoomType = ChunkId.FromString("TeRoomType");
        /******/public static readonly ChunkId RoomTypeStrength = ChunkId.FromString("TeRoomTypeStrength");
        /******/public static readonly ChunkId RoomLightEffect = ChunkId.FromString("TeRoomLightEffect");
        /******/public static readonly ChunkId RoomLightEffectStrength = ChunkId.FromString("TeRoomLightEffectStrength");
        /******/public static readonly ChunkId RoomLightEffectStrength2 = ChunkId.FromString("TeRoomLightEffectStrength2");
        /******/public static readonly ChunkId RoomReverberation = ChunkId.FromString("TeReverb");
        /******/public static readonly ChunkId RoomLocked = ChunkId.FromString("TeLocked");
        /******/public static readonly ChunkId RoomHidden = ChunkId.FromString("TeHidden");
        /******/public static readonly ChunkId Objects = ChunkId.FromString("TeObjects");
        /******/public static readonly ChunkId RoomTags = ChunkId.FromString("TeTags");
        /**********/public static readonly ChunkId ObjectMovable = ChunkId.FromString("TeMov");
        /**********/public static readonly ChunkId ObjectMovable2 = ChunkId.FromString("TeMov2");
        /**********/public static readonly ChunkId ObjectMovable3 = ChunkId.FromString("TeMov3");
        /**********/public static readonly ChunkId ObjectMovable4 = ChunkId.FromString("TeMov4");
        /**********/public static readonly ChunkId ObjectMovableTombEngine = ChunkId.FromString("TeMovTen");
        /**********/public static readonly ChunkId ObjectMovableTombEngine2 = ChunkId.FromString("TeMovTen2");
        /**********/public static readonly ChunkId ObjectItemLuaId = ChunkId.FromString("TeItLuaId"); // DEPRECATED
        /**********/public static readonly ChunkId ObjectStatic = ChunkId.FromString("TeSta");
        /**********/public static readonly ChunkId ObjectStatic2 = ChunkId.FromString("TeSta2");
        /**********/public static readonly ChunkId ObjectStatic3 = ChunkId.FromString("TeSta3");
        /**********/public static readonly ChunkId ObjectStaticTombEngine = ChunkId.FromString("TeStaTen");
        /**********/public static readonly ChunkId ObjectStaticTombEngine2 = ChunkId.FromString("TeStaTen2");
        /**********/public static readonly ChunkId ObjectCamera = ChunkId.FromString("TeCam");
        /**********/public static readonly ChunkId ObjectCamera2 = ChunkId.FromString("TeCam2");
        /**********/public static readonly ChunkId ObjectCamera3 = ChunkId.FromString("TeCam3");
        /**********/public static readonly ChunkId ObjectCameraTombEngine = ChunkId.FromString("TeCamTen");
        /**********/public static readonly ChunkId ObjectSprite = ChunkId.FromString("TeSpr");
        /**********/public static readonly ChunkId ObjectSprite2 = ChunkId.FromString("TeSpr2");
        /**********/public static readonly ChunkId ObjectSprite3 = ChunkId.FromString("TeSpr3");
        /**********/public static readonly ChunkId ObjectFlyBy = ChunkId.FromString("TeFly");
        /**********/public static readonly ChunkId ObjectFlyBy2 = ChunkId.FromString("TeFly2");
        /**********/public static readonly ChunkId ObjectFlyBy2LuaScript = ChunkId.FromString("TeFly2Lua");
        /**********/public static readonly ChunkId ObjectMemo = ChunkId.FromString("TeMemo");
        /**********/public static readonly ChunkId ObjectMemo2 = ChunkId.FromString("TeMemo2");
        /**********/public static readonly ChunkId ObjectSink = ChunkId.FromString("TeSin");
        /**********/public static readonly ChunkId ObjectSinkTombEngine = ChunkId.FromString("TeSinTen");
        /**********/public static readonly ChunkId ObjectSoundSource1 = ChunkId.FromString("TeSou");
        /**********/public static readonly ChunkId ObjectSoundSource2 = ChunkId.FromString("TeSou2");
        /**********/public static readonly ChunkId ObjectSoundSource3 = ChunkId.FromString("TeSou3");
        /**********/public static readonly ChunkId ObjectSoundSource4 = ChunkId.FromString("TeSou4");
        /**********/public static readonly ChunkId ObjectSoundSourceFinal = ChunkId.FromString("TeSndSrc");
        /**********/public static readonly ChunkId ObjectSoundSourceReallyFinal = ChunkId.FromString("TeSoundRealFinal");
        /**********/public static readonly ChunkId ObjectSoundSourceTombEngine = ChunkId.FromString("TeSoundTen");
        /**********/public static readonly ChunkId ObjectSoundSource = ChunkId.FromString("TeSound");
        /**********/public static readonly ChunkId ObjectImportedGeometry = ChunkId.FromString("TeImp");
        /**********/public static readonly ChunkId ObjectImportedGeometry2 = ChunkId.FromString("TeImp2");
        /**********/public static readonly ChunkId ObjectImportedGeometry3 = ChunkId.FromString("TeImp3");
        /**********/public static readonly ChunkId ObjectImportedGeometry4 = ChunkId.FromString("TeImp4");
        /**********/public static readonly ChunkId ObjectImportedGeometryMeshFilter = ChunkId.FromString("TeImpMshF"); // DEPRECATED
        /**********/public static readonly ChunkId ObjectImportedGeometryLightingModel = ChunkId.FromString("TeImpLM");
        /**********/public static readonly ChunkId ObjectImportedGeometrySharpEdges = ChunkId.FromString("TeShEdg");
        /**********/public static readonly ChunkId ObjectImportedGeometryHidden = ChunkId.FromString("TeImpHidden");
        /**********/public static readonly ChunkId ObjectImportedGeometryUseAlphaTest = ChunkId.FromString("TeImpAlphaTest");
        /**********/public static readonly ChunkId ObjectLight = ChunkId.FromString("TeLig");
        /**********/public static readonly ChunkId ObjectLight2 = ChunkId.FromString("TeLig2");
        /**********/public static readonly ChunkId ObjectLight3 = ChunkId.FromString("TeLig3");
        /**********/public static readonly ChunkId ObjectLight4 = ChunkId.FromString("TeLig4");
        /**********/public static readonly ChunkId ObjectLight5 = ChunkId.FromString("TeLig5");
        /**********/public static readonly ChunkId ObjectPortal = ChunkId.FromString("TePor");
        /**********/public static readonly ChunkId ObjectTrigger = ChunkId.FromString("TeTri");
        /**********/public static readonly ChunkId ObjectTrigger2 = ChunkId.FromString("TeTri2");
        /**********/public static readonly ChunkId ObjectGhostBlock = ChunkId.FromString("TeGhost"); // DEPRECATED
        /**********/public static readonly ChunkId ObjectGhostBlock2 = ChunkId.FromString("TeGhost2");
        /**********/public static readonly ChunkId ObjectTriggerVolumeTest = ChunkId.FromString("TeVolumeTest");
        /**********/public static readonly ChunkId ObjectTriggerVolume1 = ChunkId.FromString("TeVolume1");
        /**********/public static readonly ChunkId ObjectTriggerVolume2 = ChunkId.FromString("TeVolume2");
        /**********/public static readonly ChunkId ObjectTriggerVolume3 = ChunkId.FromString("TeVolume3");
        /**********/public static readonly ChunkId ObjectTriggerVolume4 = ChunkId.FromString("TeVolume4");
        /************/public static readonly ChunkId ObjectTrigger2Type = ChunkId.FromString("TeTy");
        /************/public static readonly ChunkId ObjectTrigger2TargetType = ChunkId.FromString("TeTaTy");
        /************/public static readonly ChunkId ObjectTrigger2Target = ChunkId.FromString("TeTa");
        /************/public static readonly ChunkId ObjectTrigger2Timer = ChunkId.FromString("TeTi");
        /************/public static readonly ChunkId ObjectTrigger2Extra = ChunkId.FromString("TeEx");
        /************/public static readonly ChunkId ObjectTrigger2CodeBits = ChunkId.FromString("TeCo");
        /************/public static readonly ChunkId ObjectTrigger2OneShot = ChunkId.FromString("TeOS");
        /************/public static readonly ChunkId ObjectTrigger2LuaScript = ChunkId.FromString("TeTrLua"); // DEPRECATED
        /****/public static readonly ChunkId AutoMergeStaticMeshes = ChunkId.FromString("TeMergeStatics");
        /****/public static readonly ChunkId AutoMergeStaticMeshEntry = ChunkId.FromString("TeMergeStaticsEntry");
        /****/public static readonly ChunkId AutoMergeStaticMeshEntry2 = ChunkId.FromString("TeMergeStaticsEntry2");
        /****/public static readonly ChunkId AutoMergeStaticMeshEntry3 = ChunkId.FromString("TeMergeStaticsEntry3");
        /**/public static readonly ChunkId EmbeddedSoundInfoWad = ChunkId.FromString("TeEmbeddedSoundInfoWad"); // DEPRECATED
        /**/public static readonly ChunkId Palette = ChunkId.FromString("TePalette");
    }
}
