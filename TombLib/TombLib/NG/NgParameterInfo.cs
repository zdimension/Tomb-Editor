﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TombLib.LevelData;
using TombLib.Utils;

namespace TombLib.NG
{
    public static class NgParameterInfo
    {
        public static IEnumerable<TriggerType> GetTriggerTypeRange(LevelSettings levelSettings)
        {
            yield return TriggerType.Trigger;
            yield return TriggerType.Pad;
            yield return TriggerType.Switch;
            yield return TriggerType.Key;
            yield return TriggerType.Pickup;
            yield return TriggerType.Heavy;
            yield return TriggerType.Antipad;
            yield return TriggerType.Combat;
            yield return TriggerType.Dummy;
            yield return TriggerType.Antitrigger;

            if (levelSettings.GameVersion >= TRVersion.Game.TR4)
            {
                yield return TriggerType.HeavySwitch;
                yield return TriggerType.HeavyAntitrigger;
            }

            if (levelSettings.GameVersion == TRVersion.Game.TRNG)
                yield return TriggerType.ConditionNg;
            else
            {
                if (levelSettings.GameVersion >= TRVersion.Game.TR4)
                    yield return TriggerType.Monkey;

                if (levelSettings.GameVersion == TRVersion.Game.TR5)
                    yield return TriggerType.Skeleton;

                if (levelSettings.GameVersion.Legacy() == TRVersion.Game.TR5)
                {
                    yield return TriggerType.TightRope;
                    yield return TriggerType.Crawl;
                    yield return TriggerType.Climb;
                }
            }
        }

        public static IEnumerable<TriggerTargetType> GetTargetTypeRange(LevelSettings levelSettings, TriggerType triggerType)
        {
            if (triggerType == TriggerType.ConditionNg)
            {
                yield return TriggerTargetType.ParameterNg;
            }
            else
            {
                yield return TriggerTargetType.Object;
                yield return TriggerTargetType.Camera;
                yield return TriggerTargetType.Sink;
                yield return TriggerTargetType.FlipMap;
                yield return TriggerTargetType.FlipOn;
                yield return TriggerTargetType.FlipOff;
                yield return TriggerTargetType.Target;
                yield return TriggerTargetType.FinishLevel;
                yield return TriggerTargetType.PlayAudio;
                yield return TriggerTargetType.FlipEffect;
                yield return TriggerTargetType.Secret;

                if (levelSettings.GameVersion >= TRVersion.Game.TR4)
                    yield return TriggerTargetType.FlyByCamera;
                if (levelSettings.GameVersion == TRVersion.Game.TRNG)
                {
                    yield return TriggerTargetType.ActionNg;
                    yield return TriggerTargetType.FmvNg;
                    yield return TriggerTargetType.TimerfieldNg;
                }

                if (levelSettings.GameVersion == TRVersion.Game.TombEngine)
                {
                    yield return TriggerTargetType.VolumeEvent;
                    yield return TriggerTargetType.GlobalEvent;
                }
            }
        }

        public static NgParameterRange ToParameterRange(this IEnumerable @this)
        {
            var sortedDictionary = new SortedDictionary<ushort, TriggerParameterUshort>();
            foreach (object value in @this)
                sortedDictionary.Add((ushort)value, new TriggerParameterUshort((ushort)value, value.ToString()));
            return new NgParameterRange(sortedDictionary);
        }

        public static NgParameterRange GetTargetRange(LevelSettings levelSettings, TriggerType triggerType,
            TriggerTargetType targetType, ITriggerParameter timer, ITriggerParameter plugin = null)
        {
            string trgFilePath = TryGetTRGFilePath(levelSettings, plugin, out bool isTombNextGeneration);
            TRGParser parser = trgFilePath is not null ? new TRGParser(trgFilePath) : null;

            switch (triggerType)
            {
                case TriggerType.ConditionNg:
                    if (timer is not TriggerParameterUshort)
                        return new NgParameterRange(NgParameterKind.Empty);

                    if (isTombNextGeneration)
                    {
                        NgTriggerSubtype conditionSubtriggerType = NgCatalog.Instance.ConditionTrigger.MainList.TryGetOrDefault(((TriggerParameterUshort)timer).Key);
                        return conditionSubtriggerType?.Target ?? new NgParameterRange(NgParameterKind.Empty);
                    }
                    else if (parser is not null)
                        return parser.GetParameterRange(TRGBlock.ConditionNg, ((TriggerParameterUshort)timer).Key, TriggerField.Param);
                    break;

                default:
                    switch (targetType)
                    {
                        case TriggerTargetType.Object:
                            return new NgParameterRange(NgParameterKind.MoveablesInLevel);

                        case TriggerTargetType.Camera:
                            return new NgParameterRange(NgParameterKind.CamerasInLevel);

                        case TriggerTargetType.Sink:
                            return new NgParameterRange(NgParameterKind.SinksInLevel);

                        case TriggerTargetType.Target:
                            // Actually it is possible to not only target Target objects, but all movables.
                            // This is also useful: It makes sense to target egg a trap or an enemy.
                            return new NgParameterRange(NgParameterKind.MoveablesInLevel);

                        case TriggerTargetType.FlyByCamera:
                            return new NgParameterRange(NgParameterKind.FlybyCamerasInLevel);

                        case TriggerTargetType.FlipEffect:
                            if (levelSettings.GameVersion == TRVersion.Game.TRNG)
                            {
                                if (isTombNextGeneration)
                                    return new NgParameterRange(NgCatalog.Instance.FlipEffectTrigger.MainList.DicSelect(e => (TriggerParameterUshort)e.Value));
                                else if (parser is not null)
                                    return parser.GetTriggerFieldRange(TRGBlock.FlipEffect);
                            }
                            else if (levelSettings.GameVersion == TRVersion.Game.TR4)
                                return new NgParameterRange(NgCatalog.Instance.FlipEffectTrigger.MainList
                                    .DicWhere(entry => entry.Value.Name.Contains("OldFlip"))
                                    .DicSelect(e => (TriggerParameterUshort)e.Value));
                            else
                                return new NgParameterRange(NgParameterKind.AnyNumber);
                            break;

                        case TriggerTargetType.ActionNg:
                            if (timer is not TriggerParameterUshort)
                                return new NgParameterRange(NgParameterKind.Empty);

                            if (isTombNextGeneration)
                            {
                                NgTriggerSubtype actionSubtriggerType = NgCatalog.Instance.ActionTrigger.MainList.TryGetOrDefault(((TriggerParameterUshort)timer).Key);
                                return actionSubtriggerType?.Target ?? new NgParameterRange(NgParameterKind.Empty);
                            }
                            else if (parser is not null)
                                return parser.GetParameterRange(TRGBlock.ActionNG, ((TriggerParameterUshort)timer).Key, TriggerField.Param);
                            break;

                        case TriggerTargetType.TimerfieldNg:
                            return NgCatalog.Instance.TimerFieldTrigger;

                        case TriggerTargetType.VolumeEvent:
                            if (levelSettings.GameVersion == TRVersion.Game.TombEngine)
                                return new NgParameterRange(NgParameterKind.VolumeEventSets);
                            else
                                return new NgParameterRange(NgParameterKind.AnyNumber);

                        case TriggerTargetType.GlobalEvent:
                            if (levelSettings.GameVersion == TRVersion.Game.TombEngine)
                                return new NgParameterRange(NgParameterKind.GlobalEventSets);
                            else
                                return new NgParameterRange(NgParameterKind.AnyNumber);

                        default:
                            return new NgParameterRange(NgParameterKind.AnyNumber);
                    }

                    break;
            }

            return new NgParameterRange(NgParameterKind.Empty);
        }

        public static NgParameterRange GetTimerRange(LevelSettings levelSettings, TriggerType triggerType,
            TriggerTargetType targetType, ITriggerParameter target, ITriggerParameter plugin = null)
        {
            string trgFilePath = TryGetTRGFilePath(levelSettings, plugin, out bool isTombNextGeneration);
            TRGParser parser = trgFilePath is not null ? new TRGParser(trgFilePath) : null;

            switch (triggerType)
            {
                case TriggerType.ConditionNg:
                    if (isTombNextGeneration)
                        return new NgParameterRange(NgCatalog.Instance.ConditionTrigger.MainList.DicSelect(e => (TriggerParameterUshort)e.Value));
                    else if (parser is not null)
                        return parser.GetTriggerFieldRange(TRGBlock.ConditionNg);
                    break;

                default:
                    switch (targetType)
                    {
                        case TriggerTargetType.FlipEffect:
                            if (target is not TriggerParameterUshort)
                                return new NgParameterRange(NgParameterKind.Empty);

                            if (levelSettings.GameVersion.Native() != TRVersion.Game.TR4)
                                return new NgParameterRange(NgParameterKind.AnyNumber);

                            if (isTombNextGeneration)
                            {
                                NgTriggerSubtype flipEffectSubtriggerType = NgCatalog.Instance.FlipEffectTrigger.MainList.TryGetOrDefault(((TriggerParameterUshort)target).Key);
                                return flipEffectSubtriggerType?.Timer ?? new NgParameterRange(NgParameterKind.Empty);
                            }
                            else if (parser is not null)
                                return parser.GetParameterRange(TRGBlock.FlipEffect, ((TriggerParameterUshort)target).Key, TriggerField.Timer);
                            break;

                        case TriggerTargetType.ActionNg:
                            if (isTombNextGeneration)
                                return new NgParameterRange(NgCatalog.Instance.ActionTrigger.MainList.DicSelect(e => (TriggerParameterUshort)e.Value));
                            else if (parser is not null)
                                return parser.GetTriggerFieldRange(TRGBlock.ActionNG);
                            break;

                        case TriggerTargetType.TimerfieldNg:
                            return new NgParameterRange(NgParameterKind.Empty);

                        case TriggerTargetType.VolumeEvent:
                            return new NgParameterRange(NgParameterKind.VolumeEventTypes);

                        case TriggerTargetType.GlobalEvent:
                            return new NgParameterRange(NgParameterKind.GlobalEventTypes);

                        default:
                            return new NgParameterRange(NgParameterKind.AnyNumber);
                    }

                    break;
            }

            return new NgParameterRange(NgParameterKind.Empty);
        }

        public static NgParameterRange GetExtraRange(LevelSettings levelSettings, TriggerType triggerType,
            TriggerTargetType targetType, ITriggerParameter target, ITriggerParameter timer, ITriggerParameter plugin = null)
        {
            if (levelSettings.GameVersion != TRVersion.Game.TRNG)
                return new NgParameterRange(NgParameterKind.Empty);

            string trgFilePath = TryGetTRGFilePath(levelSettings, plugin, out bool isTombNextGeneration);
            TRGParser parser = trgFilePath is not null ? new TRGParser(trgFilePath) : null;

            switch (triggerType)
            {
                case TriggerType.ConditionNg:
                    if (timer is not TriggerParameterUshort)
                        return new NgParameterRange(NgParameterKind.Empty);

                    if (isTombNextGeneration)
                    {
                        NgTriggerSubtype conditionSubtriggerType = NgCatalog.Instance.ConditionTrigger.MainList.TryGetOrDefault(((TriggerParameterUshort)timer).Key);
                        return conditionSubtriggerType?.Extra ?? new NgParameterRange(NgParameterKind.Empty);
                    }
                    else if (parser is not null)
                        return parser.GetParameterRange(TRGBlock.ConditionNg, ((TriggerParameterUshort)timer).Key, TriggerField.Extra);
                    break;

                default:
                    switch (targetType)
                    {
                        case TriggerTargetType.FlipEffect:
                            if (target is not TriggerParameterUshort)
                                return new NgParameterRange(NgParameterKind.Empty);

                            if (isTombNextGeneration)
                            {
                                NgTriggerSubtype flipEffectSubtriggerType = NgCatalog.Instance.FlipEffectTrigger.MainList.TryGetOrDefault(((TriggerParameterUshort)target).Key);
                                return flipEffectSubtriggerType?.Extra ?? new NgParameterRange(NgParameterKind.Empty);
                            }
                            else if (parser is not null)
                                return parser.GetParameterRange(TRGBlock.FlipEffect, ((TriggerParameterUshort)target).Key, TriggerField.Extra);
                            break;

                        case TriggerTargetType.ActionNg:
                            if (timer is not TriggerParameterUshort)
                                return new NgParameterRange(NgParameterKind.Empty);

                            if (isTombNextGeneration)
                            {
                                NgTriggerSubtype actionSubtriggerType = NgCatalog.Instance.ActionTrigger.MainList.TryGetOrDefault(((TriggerParameterUshort)timer).Key);
                                return actionSubtriggerType?.Extra ?? new NgParameterRange(NgParameterKind.Empty);
                            }
                            else if (parser is not null)
                                return parser.GetParameterRange(TRGBlock.ActionNG, ((TriggerParameterUshort)timer).Key, TriggerField.Extra);
                            break;

                        default:
                            return new NgParameterRange(NgParameterKind.Empty);
                    }

                    break;
            }

            return new NgParameterRange(NgParameterKind.Empty);
        }

        private static string TryGetTRGFilePath(LevelSettings levelSettings, ITriggerParameter plugin, out bool isTombNextGeneration)
        {
            isTombNextGeneration = false;

            if (plugin is not TriggerParameterUshort pluginParam || pluginParam.Key == 0) // Is null or Tomb_NextGeneration
            {
                isTombNextGeneration = true;
                return null;
            }

            string[] trgFiles = levelSettings.TryGetTRGFiles();

            return trgFiles.FirstOrDefault(file
                => Path.GetFileNameWithoutExtension(file).Equals(pluginParam.Name, StringComparison.OrdinalIgnoreCase));
        }

        public static NgParameterRange GetPluginRange(LevelSettings levelSettings)
        {
            var result = new Dictionary<ushort, TriggerParameterUshort>
            {
                { 0, new TriggerParameterUshort(0, "Tomb_NextGeneration") }
            };

            string scriptDirectory = levelSettings.MakeAbsolute(levelSettings.ScriptDirectory);
            string scriptFilePath = Path.Combine(scriptDirectory, "Script.txt");

            if (!File.Exists(scriptFilePath))
                return new NgParameterRange(result);

            string[] trgFiles = levelSettings.TryGetTRGFiles();

            if (trgFiles.Length == 0)
                return new NgParameterRange(result);

            string[] scriptFileLines = File.ReadAllLines(scriptFilePath);
            string[] pluginCommands = scriptFileLines
                .Where(line => Regex.IsMatch(line, @"^Plugin\s*=", RegexOptions.IgnoreCase))
                .ToArray();

            if (pluginCommands.Length == 0)
                return new NgParameterRange(result);

            foreach (string pluginCommand in pluginCommands)
            {
                string[] commandParts = pluginCommand.Split('=');

                if (commandParts.Length != 2)
                    continue;

                commandParts = commandParts[1].Split(',');

                if (commandParts.Length is not 2 and not 3)
                    continue;

                if (!ushort.TryParse(commandParts[0], out ushort id) || id < 1) // ID 0 is reserved for Tomb_NextGeneration
                    continue;

                string pluginName = commandParts[1].Trim();

                if (string.IsNullOrEmpty(pluginName))
                    continue;

                if (trgFiles.Any(file => Path.GetFileNameWithoutExtension(file).Equals(pluginName, StringComparison.OrdinalIgnoreCase)))
                    result[id] = new TriggerParameterUshort(id, pluginName);
            }

            return new NgParameterRange(result);
        }

        public static bool TriggerIsValid(LevelSettings levelSettings, TriggerInstance trigger)
        {
            if (!GetTriggerTypeRange(levelSettings).Contains(trigger.TriggerType))
                return false;
            if (!GetTargetTypeRange(levelSettings, trigger.TriggerType).Contains(trigger.TargetType))
                return false;
            if (!GetTargetRange(levelSettings, trigger.TriggerType, trigger.TargetType, trigger.Timer).ParameterMatches(trigger.Target, false))
                return false;
            if (!GetTimerRange(levelSettings, trigger.TriggerType, trigger.TargetType, trigger.Target).ParameterMatches(trigger.Timer, false))
                return false;
            if (!GetExtraRange(levelSettings, trigger.TriggerType, trigger.TargetType, trigger.Target, trigger.Timer).ParameterMatches(trigger.Extra, false))
                return false;
            return true;
        }

        public delegate ushort BoundedValueCallback(ushort upperBound);

        public static ushort EncodeNGRealTimer(TriggerTargetType targetType, TriggerType triggerType, ushort target, ushort upperBound, BoundedValueCallback timer, BoundedValueCallback extra)
        {
            ushort timerUpperBound = (ushort)(upperBound & 255);
            ushort extraUpperBound = (ushort)(upperBound >> 8);
            switch (triggerType)
            {
                case TriggerType.ConditionNg:
                    // Bit 8 is one shot in trigger setup so we must shift by 9
                    return (ushort)(timer(timerUpperBound) | (extra(extraUpperBound) << 9));

                default:
                    switch (targetType)
                    {
                        case TriggerTargetType.ActionNg:
                            return (ushort)(timer(timerUpperBound) | (extra(extraUpperBound) << 8));

                        case TriggerTargetType.TimerfieldNg:
                            return timer(upperBound);

                        case TriggerTargetType.FlipEffect:
                            NgTriggerSubtype flipEffectSubtriggerType = NgCatalog.Instance.FlipEffectTrigger.MainList.TryGetOrDefault(target);
                            if (flipEffectSubtriggerType != null && flipEffectSubtriggerType.Extra.IsEmpty)
                                return timer(upperBound);
                            else
                                return (ushort)(timer(timerUpperBound) | (extra(extraUpperBound) << 8));

                        default:
                            return timer(upperBound);
                    }
            }
        }

        public static void DecodeNGRealTimer(TriggerTargetType targetType, TriggerType triggerType, ushort target, ushort realTimer, short triggerFlags, out ushort? timer, out ushort? extra)
        {
            switch (triggerType)
            {
                case TriggerType.ConditionNg:
                    timer = (ushort)(realTimer & 255);
                    var conditionTrigger = NgCatalog.Instance.ConditionTrigger.MainList.TryGetOrDefault(timer.Value);
                    if (conditionTrigger != null && conditionTrigger.Extra.IsEmpty)
                        extra = null;
                    else
                        extra = (ushort)(~(triggerFlags >> 1) & 0x1f); //   (realTimer >> 8);
                    return;

                default:
                    switch (targetType)
                    {
                        case TriggerTargetType.ActionNg:
                            timer = (ushort)(realTimer & 255);
                            var actionTrigger = NgCatalog.Instance.ActionTrigger.MainList.TryGetOrDefault(timer.Value);
                            if (actionTrigger != null && actionTrigger.Extra.IsEmpty)
                                extra = null;
                            else
                                extra = (ushort)(realTimer >> 8);
                            return;

                        case TriggerTargetType.TimerfieldNg:
                            timer = realTimer;
                            extra = null;
                            return;

                        case TriggerTargetType.FlipEffect:
                            var flipEffectTrigger = NgCatalog.Instance.FlipEffectTrigger.MainList.TryGetOrDefault(target);
                            if (flipEffectTrigger != null && flipEffectTrigger.Extra.IsEmpty)
                            {
                                timer = realTimer;
                                extra = null;
                            }
                            else
                            {
                                timer = (ushort)(realTimer & 255);
                                extra = (ushort)(realTimer >> 8);
                            }
                            return;

                        default:
                            timer = realTimer;
                            extra = null;
                            return;
                    }
            }
        }

        public class ExceptionScriptIdMissing : Exception
        {
            public ExceptionScriptIdMissing()
                : base("ScriptID is missing")
            { }
        }

        public class ExceptionScriptNotSupported : NotSupportedException
        {
            public ExceptionScriptNotSupported()
                : base("Script not supported")
            { }
        }

        private static ushort GetValue(Level level, ITriggerParameter parameter)
        {
            if (parameter == null)
                return 0;
            else if (parameter is TriggerParameterUshort)
                return ((TriggerParameterUshort)parameter).Key;
            else if (parameter is IHasScriptID)
            {
                uint? Id = ((IHasScriptID)parameter).ScriptId;
                if (Id == null)
                    throw new ExceptionScriptIdMissing();
                return checked((ushort)Id);
            }
            else if (parameter is Room)
                return (ushort)level.GetRearrangedRooms().ReferenceIndexOf(parameter);
            else
                throw new Exception("Trigger parameter of invalid type!");
        }

        public static ITriggerParameter FixTriggerParameter(Level level, TriggerInstance instance, ITriggerParameter parameter, NgParameterRange range, Dictionary<uint, PositionBasedObjectInstance> objectLookup, IProgressReporter progressReporter = null)
        {
            List<FlybyCameraInstance> flyByLookup = null;

            if (!(parameter is TriggerParameterUshort))
                return parameter;
            ushort index = ((TriggerParameterUshort)parameter).Key;
            if (range.IsObject)
            {
                // Special lookup for flybys.
                // Triggers marked as flyby can point to a sequence directly instead an object ID
                if (instance.TargetType == TriggerTargetType.FlyByCamera)
                {
                    if (flyByLookup == null)
                        flyByLookup = objectLookup.Values
                            .OfType<FlybyCameraInstance>()
                            .GroupBy(flyBy => flyBy.Sequence)
                            .Select(flyByGroup => flyByGroup.OrderBy(flyBy => flyBy.Number).First())
                            .ToList();
                    FlybyCameraInstance foundFlyBy = flyByLookup.FirstOrDefault(flyBy => flyBy.Sequence == index);
                    if (foundFlyBy != null)
                        return foundFlyBy;
                }

                // Undo object indexing
                PositionBasedObjectInstance @object;
                if (!objectLookup.TryGetValue(index, out @object))
                {
                    progressReporter?.ReportWarn("Trigger '" + instance + "' in '" + instance.Room + "' refers to an object with ID " + index + " that is unavailable.");
                    return null;
                }
                return @object;
            }
            else if (range.IsRoom)
            {
                // Undo room indexing
                Room room3 = level.ExistingRooms.ElementAtOrDefault(index);
                if (room3 == null)
                {
                    progressReporter?.ReportWarn("Trigger '" + instance + "' in '" + instance.Room + "' refers to a room with ID " + index + " that is unavailable.");
                    return parameter;
                }
                return room3;
            }
            else
                return parameter;
        }

        public static TriggerInstance ImportFromScriptTrigger(Level level, string script)
        {
            // Build object lookup table
            var objectLookup = level.ExistingRooms
                                          .SelectMany(room => room.Objects)
                                          .Where(instance => instance is IHasScriptID && ((IHasScriptID)instance).ScriptId.HasValue)
                                          .ToDictionary(instance => ((IHasScriptID)instance).ScriptId.Value);
            // Make dummy trigger
            var result = new TriggerInstance(RectangleInt2.Zero) { TriggerType = TriggerType.Trigger };

            // Split script string into tokens and clear spaces
            var tokens = script.Split(',').Select(s => s.Trim(' ')).ToList();

            if (tokens.Count != 3)
                return null; // Incorrect amount of operands!

            ushort[] operands = new ushort[tokens.Count];

            // Parse tokens
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                if (token.StartsWith("$") && ushort.TryParse(token.Trim('$'), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out operands[i]))
                    continue;
                if (ushort.TryParse(token, out operands[i]))
                    continue;
                return null;  // Incorrect operand!
            }

            // Should we splice timer into extra?
            bool decode = false;

            try
            {
                switch (operands[0])
                {
                    case 0x8000:
                    case 0x9000:
                        result.TriggerType = TriggerType.ConditionNg;
                        result.TargetType = TriggerTargetType.ParameterNg;
                        decode = !NgCatalog.Instance.ConditionTrigger.MainList[(ushort)(operands[2] & 0xFF)].Extra.IsEmpty;
                        break;

                    case 0x2000:
                        result.TargetType = TriggerTargetType.FlipEffect;
                        decode = !NgCatalog.Instance.FlipEffectTrigger.MainList[operands[1]].Extra.IsEmpty;
                        break;

                    case 0x4000:
                    case 0x5000:
                        result.TargetType = TriggerTargetType.ActionNg;
                        decode = !NgCatalog.Instance.ActionTrigger.MainList[(ushort)(operands[2] & 0xFF)].Extra.IsEmpty;
                        break;

                    default:
                        return null; // Incorrect first operand!
                }
            }
            catch
            {
                return null;
            }

            // Target is always raw
            result.Target = new TriggerParameterUshort(operands[1]);

            // Splice timer into extra if needed
            if (decode)
            {
                result.Timer = new TriggerParameterUshort((ushort)(operands[2] & 0xFF));
                result.Extra = new TriggerParameterUshort((ushort)(operands[2] >> 8));
            }
            else
                result.Timer = new TriggerParameterUshort(operands[2]);

            // Link actual objects
            result.Target = FixTriggerParameter(level, result, result.Target,
                GetTargetRange(level.Settings, result.TriggerType, result.TargetType, result.Timer), objectLookup);
            result.Timer = FixTriggerParameter(level, result, result.Timer,
                GetTimerRange(level.Settings, result.TriggerType, result.TargetType, result.Target), objectLookup);
            result.Extra = FixTriggerParameter(level, result, result.Extra,
                GetExtraRange(level.Settings, result.TriggerType, result.TargetType, result.Target, result.Timer), objectLookup);

            return result;
        }

        public static string ExportToScriptTrigger(Level level, TriggerInstance trigger, int? animCommandNumber, bool withComment = false)
        {
            checked
            {
                string result = null;
                ushort mask = 0;
                ushort firstValue = 0;
                ushort secondValue = 0;

                if (animCommandNumber.HasValue)
                {
                    mask |= 0x8000;

                    if (animCommandNumber.Value < 0)
                        mask |= 0xFF;
                    else
                        mask |= (byte)Math.Min(animCommandNumber.Value, byte.MaxValue - 1);
                }
                else
                    mask = 0;

                switch (trigger.TriggerType)
                {
                    case TriggerType.ConditionNg:
                        {
                            // Condition triggers can't be exported as anim command because they use same
                            // bit flag as animcommand exporter flag

                            if (!TriggerIsValid(level.Settings, trigger) || animCommandNumber.HasValue)
                                throw new Exception("Trigger is invalid.");

                            ushort conditionId = GetValue(level, trigger.Timer);
                            NgTriggerSubtype conditionTrigger = NgCatalog.Instance.ConditionTrigger.MainList[conditionId];

                            mask |= (ushort)(trigger.Target is ObjectInstance ? 0x9000 : 0x8000);
                            firstValue = GetValue(level, trigger.Target);
                            secondValue = conditionId;

                            if (!conditionTrigger.Extra.IsEmpty)
                                secondValue |= (ushort)(GetValue(level, trigger.Extra) << 8);
                            break;
                        }
                    default:
                        switch (trigger.TargetType)
                        {
                            case TriggerTargetType.FlipEffect:
                                {
                                    if (!TriggerIsValid(level.Settings, trigger))
                                        throw new Exception("Trigger is invalid.");

                                    ushort flipeffectId = GetValue(level, trigger.Target);
                                    NgTriggerSubtype flipeffectTrigger = NgCatalog.Instance.FlipEffectTrigger.MainList[flipeffectId];

                                    mask |= 0x2000;
                                    firstValue = flipeffectId;
                                    secondValue = GetValue(level, trigger.Timer);

                                    if (!flipeffectTrigger.Extra.IsEmpty)
                                        secondValue |= (ushort)(GetValue(level, trigger.Extra) << 8);
                                    break;
                                }

                            case TriggerTargetType.ActionNg:
                                {
                                    if (!TriggerIsValid(level.Settings, trigger))
                                        throw new Exception("Trigger is invalid.");

                                    ushort actionId = GetValue(level, trigger.Timer);
                                    NgTriggerSubtype actionTrigger = NgCatalog.Instance.ActionTrigger.MainList[actionId];

                                    mask |= (ushort)(trigger.Target is StaticInstance ? 0x4000 : 0x5000);
                                    firstValue = GetValue(level, trigger.Target);
                                    secondValue = actionId;

                                    if (!actionTrigger.Extra.IsEmpty)
                                        secondValue |= (ushort)(GetValue(level, trigger.Extra) << 8);
                                    break;
                                }
                        }
                        break;
                }

                if (mask != 0)
                    result = animCommandNumber.HasValue ? unchecked((short)mask) + " ," + firstValue + " ," + secondValue :
                        "$" + mask.ToString("X4") + "," + firstValue + ",$" + secondValue.ToString("X4");

                if (!string.IsNullOrEmpty(result))
                    return (withComment ?
                            "; " + trigger.TriggerType + " for " + trigger.TargetType +
                            "\n; <#> " + trigger.Target +
                            "\n; <&> " + trigger.Timer +
                            (trigger.Extra == null ? "" : "\n; <E> " + trigger.Extra) +
                            "\n; Copy following values to your script:" +
                            "\n; "
                        : "")
                        + result;
            }
            throw new ExceptionScriptNotSupported();
        }
    }

    public enum TRGBlock
    {
        ConditionNg,
        FlipEffect,
        ActionNG,
    }

    public enum TriggerField
    {
        Unknown,
        Param,
        Timer,
        Extra
    }

    public class TRGParser
    {
        public string[] Lines { get; }

        public TRGParser(string trgFilePath)
            => Lines = File.ReadAllLines(trgFilePath);

        public NgParameterRange GetTriggerFieldRange(TRGBlock block)
        {
            var results = new Dictionary<ushort, TriggerParameterUshort>();

            int sectionStartLineIndex = block switch
            {
                TRGBlock.ConditionNg => Array.FindIndex(Lines, line => line.StartsWith("<START_TRIGGERTYPE_12_T", StringComparison.OrdinalIgnoreCase)),
                TRGBlock.ActionNG => Array.FindIndex(Lines, line => line.StartsWith("<START_TRIGGERWHAT_11_T", StringComparison.OrdinalIgnoreCase)),
                _ => Array.FindIndex(Lines, line => line.StartsWith("<START_TRIGGERWHAT_9_O", StringComparison.OrdinalIgnoreCase)), // Flip Effect
            };

            if (sectionStartLineIndex == -1)
                return new NgParameterRange(results);

            int sectionEndLineIndex = Array.FindIndex(Lines, sectionStartLineIndex, line => line.StartsWith("<END", StringComparison.OrdinalIgnoreCase));

            if (sectionEndLineIndex == -1)
                sectionEndLineIndex = Lines.Length;

            var keyValueRegex = new Regex(@"^(\d+):(.*)");

            for (int i = sectionStartLineIndex + 1; i < sectionEndLineIndex; i++)
            {
                string line = Lines[i];
                Match match = keyValueRegex.Match(line);

                if (!match.Success)
                    continue;

                if (!ushort.TryParse(match.Groups[1].Value, out ushort id))
                    continue;

                string name = match.Groups[2].Value
                    .Split("#END_DOC#")[0]
                    .Split("#START_DOC#")[0]
                    .Split("#REMARK#")[0]
                    .Trim();

                if (string.IsNullOrEmpty(name))
                    continue;

                results[id] = new TriggerParameterUshort(id, name);
            }

            return new NgParameterRange(results);
        }

        public NgParameterRange GetParameterRange(TRGBlock block, int id, TriggerField field)
            => new(GetRangeKeyValues(block, id, field));

        private IDictionary<ushort, TriggerParameterUshort> GetRangeKeyValues(TRGBlock block, int id, TriggerField field)
        {
            string triggerField = block switch
            {
                TRGBlock.ConditionNg => "CONDITION",
                TRGBlock.ActionNG => "ACTION",
                _ => "EFFECT"
            };

            string startsWithString = field switch
            {
                TriggerField.Param => $"<START_{triggerField}_{id}_O",
                TriggerField.Timer => $"<START_{triggerField}_{id}_T",
                _ => $"<START_{triggerField}_{id}_E"
            };

            int sectionStartLineIndex = Array.FindIndex(Lines, line => line.StartsWith(startsWithString, StringComparison.OrdinalIgnoreCase));

            if (sectionStartLineIndex == -1)
                return new Dictionary<ushort, TriggerParameterUshort>();

            int sectionEndLineIndex = Array.FindIndex(Lines, sectionStartLineIndex, line => line.StartsWith("<END", StringComparison.OrdinalIgnoreCase));

            if (sectionEndLineIndex == -1)
                sectionEndLineIndex = Lines.Length;

            return GetKeyValues(Lines[sectionStartLineIndex..sectionEndLineIndex], true);
        }

        private IDictionary<ushort, TriggerParameterUshort> GetKeyValues(string[] lines, bool allowLists)
        {
            var results = new Dictionary<ushort, TriggerParameterUshort>();
            var keyValueRegex = new Regex(@"^(\d+):(.*)");

            foreach (string line in lines)
            {
                Match match = keyValueRegex.Match(line);

                if (!match.Success)
                {
                    if (line.StartsWith('#') && allowLists)
                    {
                        foreach (KeyValuePair<ushort, TriggerParameterUshort> item in GetList(line))
                            results.Add(item.Key, item.Value);
                    }

                    continue;
                }

                if (!ushort.TryParse(match.Groups[1].Value, out ushort id))
                    continue;

                string name = match.Groups[2].Value.Trim();

                if (string.IsNullOrEmpty(name))
                    continue;

                results[id] = new TriggerParameterUshort(id, name);
            }

            return results;
        }

        private IDictionary<ushort, TriggerParameterUshort> GetList(string line)
        {
            var results = new Dictionary<ushort, TriggerParameterUshort>();

            // Repeated strings
            if (line.StartsWith("#REPEAT#"))
            {
                string[] tokens = line["#REPEAT#".Length..].Split('#');

                if (tokens.Length != 3) // Invalid format
                    return results;

                string radix = tokens[0].Replace("\"", "");

                if (!int.TryParse(tokens[1], out int start) ||
                    !int.TryParse(tokens[2], out int end))
                    return results;

                for (int i = start; i < end; i++)
                    results.Add(unchecked((ushort)checked((short)i)), new TriggerParameterUshort((ushort)i, radix + i));

                return results;
            }
            else if (line.StartsWith("#SAME_OF#="))
            {
                string value = line["#SAME_OF#=".Length..].Trim();
                string[] tokens = value.Split('_');

                if (tokens.Length != 3) // Invalid format
                    return results;

                if (!int.TryParse(tokens[1], out int id))
                    return results;

                TRGBlock block = tokens[0] switch
                {
                    "CONDITION" => TRGBlock.ConditionNg,
                    "ACTION" => TRGBlock.ActionNG,
                    _ => TRGBlock.FlipEffect
                };

                TriggerField field = tokens[2] switch
                {
                    "O" => TriggerField.Param,
                    "T" => TriggerField.Timer,
                    _ => TriggerField.Extra
                };

                return GetRangeKeyValues(block, id, field);
            }

            string listName = line.Replace("#", "");
            string executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filePath = Path.Combine(executablePath, "Catalogs\\NG\\" + listName + ".txt");

            if (!File.Exists(filePath))
                return results;

            string[] lines = File.ReadAllLines(filePath);
            return GetKeyValues(lines, false);
        }
    }
}
