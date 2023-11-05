LevelFuncs.Engine.Node = {}

-- Helper function for value comparisons. Any function which uses
-- CompareOperator arguments should use this helper function for comparison.
LevelFuncs.Engine.Node.CompareValue = function(operand, reference, operator)
	local result = false

	-- Fix Lua-specific treatment of bools as non-numerical values
	if (operand == false) then operand = 0 end;
	if (operand == true) then operand = 1 end;
	if (reference == false) then reference = 0 end;
	if (reference == true) then reference = 1 end;

	if (operator == 0 and operand == reference) then result = true end
	if (operator == 1 and operand ~= reference) then result = true end
	if (operator == 2 and operand < reference) then result = true end
	if (operator == 3 and operand <= reference) then result = true end
	if (operator == 4 and operand > reference) then result = true end
	if (operator == 5 and operand >= reference) then result = true end
	return result
end

-- Helper function for value modification.
LevelFuncs.Engine.Node.ModifyValue = function(operand, reference, operator)
	local result = reference
	if (operator == 0) then result = reference + operand end
	if (operator == 1) then result = reference - operand end
	if (operator == 2) then result = reference * operand end
	if (operator == 3) then result = reference / operand end
	if (operator == 4) then result = operand end
	return result
end

-- Helper function for easy generation of a display string with all parameters set.
LevelFuncs.Engine.Node.GenerateString = function(text, x, y, scale, alignment, shadow, color)
	local options = {}
	if (shadow == true) then table.insert(options, TEN.Strings.DisplayStringOption.SHADOW) end
	if (alignment == 1) then table.insert(options, TEN.Strings.DisplayStringOption.CENTER) end
	if (alignment == 2) then table.insert(options, TEN.Strings.DisplayStringOption.RIGHT)  end
	local rX, rY = TEN.Util.PercentToScreen(x, y)
	return TEN.Strings.DisplayString(text, TEN.Vec2(rX, rY), scale, color, false, options)
end

-- Helper function to split string using specified delimiter.
LevelFuncs.Engine.Node.SplitString = function(inputStr, delimiter)
	if inputStr == nil then
		inputStr = "%s"
	end

	local t = {}
	for str in string.gmatch(inputStr, "([^" .. delimiter .. "]+)") do
		table.insert(t, str)
	end

	return t
end

-- Wrap angle value around 360
LevelFuncs.Engine.Node.WrapRotation = function(source, value)
	if (value == 0) then
		return source
	end

	local rot = source + value
	if (rot > 360) then
		rot = rot - 360
	elseif (rot < 0) then
		rot = 360 + rot
	end
	return rot
end

-- Convert UI enum to room flag ID enum
LevelFuncs.Engine.Node.GetRoomFlag = function(value)
	local RoomFlagID =
	{
		[0] = Objects.RoomFlagID.WATER,
		[1] = Objects.RoomFlagID.QUICKSAND,
		[2] = Objects.RoomFlagID.SKYBOX,
		[3] = Objects.RoomFlagID.WIND,
		[4] = Objects.RoomFlagID.COLD,
		[5] = Objects.RoomFlagID.DAMAGE,
		[6] = Objects.RoomFlagID.NOLENSFLARE,
	}
	return RoomFlagID[value]
end

LevelFuncs.Engine.Node.GetSoundTrackType = function(value)
	local SoundTrackType =
	{
		[0] = Sound.SoundTrackType.ONESHOT,
		[1] = Sound.SoundTrackType.LOOPED,
		[2] = Sound.SoundTrackType.VOICE,
	}
	return SoundTrackType[value]
end

LevelFuncs.Engine.Node.GetBlendMode = function(index)
	local blendID =
	{
		[0] = TEN.Effects.BlendID.OPAQUE,
		[1] = TEN.Effects.BlendID.ALPHATEST,
		[2] = TEN.Effects.BlendID.ADDITIVE,
		[3] = TEN.Effects.BlendID.NOZTEST,
		[4] = TEN.Effects.BlendID.SUBTRACTIVE,
		[5] = TEN.Effects.BlendID.WIREFRAME,
		[6] = TEN.Effects.BlendID.EXCLUDE,
		[7] = TEN.Effects.BlendID.SCREEN,
		[8] = TEN.Effects.BlendID.LIGHTEN,
		[9] = TEN.Effects.BlendID.ALPHABLEND
	}
	return blendID[index]
end

LevelFuncs.Engine.Node.GetDisplaySpriteAlignMode = function(index)
	local displaySpriteAlignMode =
	{
		[0] = TEN.DisplaySpriteEnum.AlignMode.CENTER,
		[1] = TEN.DisplaySpriteEnum.AlignMode.CENTER_TOP,
		[2] = TEN.DisplaySpriteEnum.AlignMode.CENTER_BOTTOM,
		[3] = TEN.DisplaySpriteEnum.AlignMode.CENTER_LEFT,
		[4] = TEN.DisplaySpriteEnum.AlignMode.CENTER_RIGHT,
		[5] = TEN.DisplaySpriteEnum.AlignMode.TOP_LEFT,
		[6] = TEN.DisplaySpriteEnum.AlignMode.TOP_RIGHT,
		[7] = TEN.DisplaySpriteEnum.AlignMode.BOTTOM_LEFT,
		[8] = TEN.DisplaySpriteEnum.AlignMode.BOTTOM_RIGHT
	}
	return displaySpriteAlignMode
end

LevelFuncs.Engine.Node.GetDisplaySpriteScaleMode = function(index)
	local displaySpriteScaleMode =
	{
		[0] = TEN.DisplaySpriteEnum.ScaleMode.FIT,
		[1] = TEN.DisplaySpriteEnum.ScaleMode.FILL,
		[2] = TEN.DisplaySpriteEnum.ScaleMode.STRETCH
	}
	return displaySpriteScaleMode
end

-- Helper function for choosing sprite slots
LevelFuncs.Engine.Node.GetSpriteSlot = function(index)
	local spriteSlot =
	{
		[0] = TEN.Objects.ObjID.SKY_GRAPHICS,
		[1] = TEN.Objects.ObjID.DEFAULT_SPRITES,
		[2] = TEN.Objects.ObjID.MISC_SPRITES,
		[3] = TEN.Objects.ObjID.CUSTOM_SPRITES,
		[4] = TEN.Objects.ObjID.FIRE_SPRITES,
		[5] = TEN.Objects.ObjID.SMOKE_SPRITES,
		[6] = TEN.Objects.ObjID.SPARK_SPRITE,
		[7] = TEN.Objects.ObjID.DRIP_SPRITE,
		[8] = TEN.Objects.ObjID.EXPLOSION_SPRITES,
		[9] = TEN.Objects.ObjID.MOTORBOAT_FOAM_SPRITES,
		[10] = TEN.Objects.ObjID.RUBBER_BOAT_WAVE_SPRITES,
		[11] = TEN.Objects.ObjID.SKIDOO_SNOW_TRAIL_SPRITES,
		[12] = TEN.Objects.ObjID.KAYAK_PADDLE_TRAIL_SPRITE,
		[13] = TEN.Objects.ObjID.KAYAK_WAKE_SPRTIES,
		[14] = TEN.Objects.ObjID.BINOCULAR_GRAPHIC,
		[15] = TEN.Objects.ObjID.LASER_SIGHT_GRAPHIC,
		[16] = TEN.Objects.ObjID.CAUSTICS_TEXTURES,
		[17] = TEN.Objects.ObjID.BAR_BORDER_GRAPHIC,
		[18] = TEN.Objects.ObjID.HEALTH_BAR_TEXTURE,
		[19] = TEN.Objects.ObjID.AIR_BAR_TEXTURE,
		[20] = TEN.Objects.ObjID.DASH_BAR_TEXTURE,
		[21] = TEN.Objects.ObjID.SFX_BAR_TEXTURE,
		[22] = TEN.Objects.ObjID.CROSSHAIR,
	}
	return spriteSlot[index]
end
