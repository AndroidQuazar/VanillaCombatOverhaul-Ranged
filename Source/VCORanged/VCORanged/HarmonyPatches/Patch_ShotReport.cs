using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;
using RimWorld;
using HarmonyLib;

namespace VCORanged
{

    public static class Patch_ShotReport
    {

        [HarmonyPatch(typeof(ShotReport), nameof(ShotReport.AimOnTargetChance_StandardTarget), MethodType.Getter)]
        public static class get_AimOnTargetChance_StandardTarget
        {

            public static bool Prefix(ShotReport __instance, ref float __result)
            {
                // Use score system
                __result = VCORangedUtility.AccuracyScoreToPercentageCurve.Evaluate(__instance.AimOnTargetScore_StandardTarget());
                return false;
            }

        }

        [HarmonyPatch(typeof(ShotReport), nameof(ShotReport.AimOnTargetChance_IgnoringPosture), MethodType.Getter)]
        public static class get_AimOnTargetChance_IgnoringPosture
        {

            public static bool Prefix(ShotReport __instance, ref float __result)
            {
                // Use score system
                __result = VCORangedUtility.AccuracyScoreToPercentageCurve.Evaluate(__instance.AimOnTargetScore_IgnoringPosture());
                return false;
            }

        }

        [HarmonyPatch(typeof(ShotReport), nameof(ShotReport.AimOnTargetChance), MethodType.Getter)]
        public static class get_AimOnTargetChance
        {

            public static bool Prefix(ShotReport __instance, ref float __result)
            {
                // Use score system
                __result = VCORangedUtility.AccuracyScoreToPercentageCurve.Evaluate(__instance.AimOnTargetScore());
                return false;
            }

        }

        [HarmonyPatch(typeof(ShotReport), nameof(ShotReport.TotalEstimatedHitChance), MethodType.Getter)]
        public static class get_TotalEstimatedHitChance
        {

            public static bool Prefix(ShotReport __instance, ref float __result)
            {
                // Use score system
                __result = VCORangedUtility.AccuracyScoreToPercentageCurve.Evaluate(__instance.TotalHitScore());
                return false;
            }

        }

        [HarmonyPatch(typeof(ShotReport), nameof(ShotReport.HitFactorFromShooter), new Type[] { typeof(float), typeof(float) })]
        public static class HitFactorFromShooter
        {

            public static bool Prefix(float accRating, ref float __result)
            {
                // Don't do power by distance
                __result = accRating;
                return false;
            }

        }

        [HarmonyPatch(typeof(ShotReport), "FactorFromPosture", MethodType.Getter)]
        public static class get_FactorFromPosture
        {

            public static bool Prefix(ShotReport __instance, ref float __result)
            {
                // Convert to score
                var target = (TargetInfo)NonPublicFields.ShotReport_target.GetValue(__instance);
                if (target.Thing is Pawn pawn)
                {
                    float distance = (float)NonPublicFields.ShotReport_distance.GetValue(__instance);
                    if (distance >= ShootTuning.LayingDownHitChanceFactorMinDistance && pawn.GetPosture() != PawnPosture.Standing)
                        __result = VCORangedTuning.AccuracyScoreProne;
                }

                return false;
            }

        }

        [HarmonyPatch(typeof(ShotReport), "FactorFromExecution", MethodType.Getter)]
        public static class get_FactorFromExecution
        {

            public static bool Prefix(ShotReport __instance, ref float __result)
            {
                // Convert to score
                var target = (TargetInfo)NonPublicFields.ShotReport_target.GetValue(__instance);
                if (target.Thing is Pawn pawn)
                {
                    float distance = (float)NonPublicFields.ShotReport_distance.GetValue(__instance);
                    if (distance <= ShootTuning.ExecutionMaxDistance && pawn.GetPosture() != PawnPosture.Standing)
                        __result = VCORangedTuning.AccuracyScoreExecution;
                }

                return false;
            }

        }

        [HarmonyPatch(typeof(ShotReport), "FactorFromCoveringGas", MethodType.Getter)]
        public static class get_FactorFromCoveringGas
        {

            public static bool Prefix(ShotReport __instance, ref float __result)
            {
                // Convert to score
                var coveringGas = NonPublicFields.ShotReport_coveringGas.GetValue(__instance) as ThingDef;
                if (coveringGas != null)
                {
                    __result = coveringGas.gas.accuracyPenalty * VCORangedTuning.AccuracyScoreCoveringGas;
                }

                return false;
            }

        }



        [HarmonyPatch(typeof(ShotReport), nameof(ShotReport.HitReportFor))]
        public static class HitReportFor
        {

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                #if DEBUG
                    Log.Message("Transpiler start: ShotReport.HitReportFor (6 matches)");
                #endif

                var instructionList = instructions.ToList();

                var offsetFromTargetSizeInfo = AccessTools.Method(typeof(VCORangedUtility), nameof(VCORangedUtility.OffsetFromTargetSize));
                var offsetFromWeatherInfo = AccessTools.Method(typeof(VCORangedUtility), nameof(VCORangedUtility.OffsetFromWeather));

                bool factorFromTargetSizeBlockDone = false;

                for (int i = 0; i < instructionList.Count; i++)
                {
                    var instruction = instructionList[i];

                    // Nop out the entirety of the factorFromTargetSize block; we're doing this our own way, and precious floating point ops are best spent elsewhere
                    if (!factorFromTargetSizeBlockDone && instruction.opcode == OpCodes.Ldarga_S && instruction.OperandIs(2))
                    {
                        #if DEBUG
                            Log.Message("ShotReport.HitReportFor match 1 of 7");
                        #endif

                        bool factorFromTargetSizeBlockFound = false;
                        
                        for (int j = 1; i + j < instructionList.Count; j++)
                        {
                            var checkingInstruction = instructionList[i + j];
                            if (checkingInstruction.opcode == OpCodes.Stfld)
                            {
                                if (checkingInstruction.OperandIs(NonPublicFields.ShotReport_factorFromTargetSize))
                                    factorFromTargetSizeBlockFound = true;
                                break;
                            }
                            j++;
                        }

                        if (factorFromTargetSizeBlockFound)
                        {
                            #if DEBUG
                                Log.Message("ShotReport.HitReportFor match 2 of 7");
                            #endif

                            for (int j = 0; ; j++)
                            {
                                var targetInstruction = instructionList[i + j];
                                if (targetInstruction.opcode.FlowControl == FlowControl.Cond_Branch)
                                    targetInstruction.opcode = OpCodes.Br;
                                else
                                {
                                    // Found another instruction that loads shotReport; check what field the method wants to change. If not factorFromTargetSize, the block is fully Nopped
                                    if (targetInstruction.opcode == OpCodes.Ldloca_S && ((LocalBuilder)targetInstruction.operand).LocalIndex == 0)
                                    {
                                        #if DEBUG
                                            Log.Message("ShotReport.HitReportFor match 3 of 7");
                                        #endif

                                        for (int k = 1; i + j + k < instructionList.Count; k++)
                                        {
                                            var checkingInstruction = instructionList[i + j + k];
                                            if (checkingInstruction.opcode == OpCodes.Stfld)
                                            {
                                                #if DEBUG
                                                    Log.Message("ShotReport.HitReportFor match 4 of 7");
                                                #endif

                                                if (!checkingInstruction.OperandIs(NonPublicFields.ShotReport_factorFromTargetSize))
                                                {
                                                    #if DEBUG
                                                        Log.Message("ShotReport.HitReportFor match 5 of 7");
                                                    #endif

                                                    factorFromTargetSizeBlockDone = true;
                                                }
                                                break;
                                            }
                                        }
                                    }



                                    if (!factorFromTargetSizeBlockDone)
                                        targetInstruction.opcode = OpCodes.Nop;
                                    else
                                        break;
                                }
                            }
                        }
                    }

                    // Look for all cases where the 'shotReport' local variable is about to be returned - make our final adjustments first
                    else if (instruction.opcode == OpCodes.Ldloc_0)
                    {
                        #if DEBUG
                            Log.Message("ShotReport.HitReportFor match 6 of 7");
                        #endif

                        var nextInstruction = instructionList[i + 1];
                        if (nextInstruction.opcode == OpCodes.Ret)
                        {
                            #if DEBUG
                                Log.Message("ShotReport.HitReportFor match 7 of 7");
                            #endif

                            yield return new CodeInstruction(OpCodes.Ldloca_S, 0); // shotReport
                            yield return new CodeInstruction(OpCodes.Ldarg_0); // caster
                            yield return new CodeInstruction(OpCodes.Ldarg_2); // target
                            yield return new CodeInstruction(OpCodes.Call, offsetFromTargetSizeInfo); // VCOUtility.OffsetFromTargetSize(caster, target)
                            yield return new CodeInstruction(OpCodes.Stfld, NonPublicFields.ShotReport_factorFromTargetSize); // shotReport.factorFromTargetSize = VCOUtility.OffsetFromTargetSize(caster, target)

                            yield return new CodeInstruction(OpCodes.Ldloca_S, 0); // shotReport
                            yield return new CodeInstruction(OpCodes.Ldloc_0); // shotReport
                            yield return new CodeInstruction(OpCodes.Ldfld, NonPublicFields.ShotReport_factorFromWeather); // shotReport.factorFromWeather
                            yield return new CodeInstruction(OpCodes.Call, offsetFromWeatherInfo); // VCOUtility.OffsetFromTargetSize(shotReport.factorFromWeather)
                            yield return new CodeInstruction(OpCodes.Stfld, NonPublicFields.ShotReport_factorFromWeather); // shotReport.factorFromWeather = VCOUtility.OffsetFromWeathe(shotReport.factorFromWeather)
                        }
                    }

                    yield return instruction;
                }
            }

        }

        [HarmonyPatch(typeof(ShotReport), nameof(ShotReport.GetTextReadout))]
        public static class GetTextReadout
        {

            public static bool Prefix(ShotReport __instance, ref string __result)
            {
                var reportBuilder = new StringBuilder();

                float forcedMissRadius = (float)NonPublicFields.ShotReport_forcedMissRadius.GetValue(__instance);
                if (forcedMissRadius > 0.5f)
                {
                    reportBuilder.AppendLine();
                    reportBuilder.AppendLine("WeaponMissRadius".Translate() + "   " + forcedMissRadius.ToString("F1"));
                    reportBuilder.AppendLine("DirectHitChance".Translate() + "   " + (1f / (float)GenRadial.NumCellsInRadius(forcedMissRadius)).ToStringPercent());
                }

                else
                {
                    // Total hit chance
                    float hitChance = __instance.TotalEstimatedHitChance;
                    reportBuilder.AppendLine(" " + hitChance.ToStringPercent() + $" ({VCORangedUtility.TotalHitScore(__instance).ToStringByStyle(ToStringStyle.FloatOne)} => {hitChance.ToStringPercent()})");
                    
                    // Shooter
                    float offsetFromShooter = (float)NonPublicFields.ShotReport_factorFromShooterAndDist.GetValue(__instance);
                    reportBuilder.AppendLine("   " + "ShootReportShooterAbility".Translate() + "  " + offsetFromShooter.ToStringByStyle(ToStringStyle.FloatOne, ToStringNumberSense.Offset));

                    // Distance
                    float offsetFromDist = VCORangedUtility.OffsetFromDistance(__instance);
                    reportBuilder.AppendLine("   " + "distance".Translate().CapitalizeFirst() + "  " + offsetFromDist.ToStringByStyle(ToStringStyle.FloatOne, ToStringNumberSense.Offset));

                    // Weapon
                    float offsetFromWeapon = (float)NonPublicFields.ShotReport_factorFromEquipment.GetValue(__instance);
                    reportBuilder.AppendLine("   " + "ShootReportWeapon".Translate() + "        " + offsetFromWeapon.ToStringByStyle(ToStringStyle.FloatOne, ToStringNumberSense.Offset));

                    // Target
                    var target = (TargetInfo)NonPublicFields.ShotReport_target.GetValue(__instance);
                    float offsetFromTargetSize = (float)NonPublicFields.ShotReport_factorFromTargetSize.GetValue(__instance);
                    if (target.HasThing && offsetFromTargetSize != 0)
                    {
                        reportBuilder.AppendLine("   " + "TargetSize".Translate() + "       " + offsetFromTargetSize.ToStringByStyle(ToStringStyle.FloatOne, ToStringNumberSense.Offset));
                    }

                    // Weather
                    float offsetFromDarkness = VCORangedUtility.OffsetFromGlow(__instance);
                    if (offsetFromDarkness != 0)
                    {
                        reportBuilder.AppendLine("   " + "Darkness".Translate() + "         " + offsetFromDarkness.ToStringByStyle(ToStringStyle.FloatOne, ToStringNumberSense.Offset));
                    }

                    // Weather
                    float offsetFromWeather = (float)NonPublicFields.ShotReport_factorFromWeather.GetValue(__instance);
                    if (offsetFromWeather != 0)
                    {
                        reportBuilder.AppendLine("   " + "Weather".Translate() + "         " + offsetFromWeather.ToStringByStyle(ToStringStyle.FloatOne, ToStringNumberSense.Offset));
                    }

                    // Gas
                    float offsetFromCoveringGas = (float)NonPublicProperties.ShotReport_get_FactorFromCoveringGas.GetValue(__instance);
                    if (offsetFromCoveringGas != 0)
                    {
                        var coveringGas = (ThingDef)NonPublicFields.ShotReport_coveringGas.GetValue(__instance);
                        reportBuilder.AppendLine("   " + coveringGas.LabelCap + "         " + offsetFromCoveringGas.ToStringByStyle(ToStringStyle.FloatOne, ToStringNumberSense.Offset));
                    }

                    // Posture
                    float offsetFromPosture = (float)NonPublicProperties.ShotReport_get_FactorFromPosture.GetValue(__instance);
                    if (offsetFromPosture != 0)
                    {
                        reportBuilder.AppendLine("   " + "TargetProne".Translate() + "  " + offsetFromPosture.ToStringByStyle(ToStringStyle.FloatOne, ToStringNumberSense.Offset));
                    }

                    // Execution
                    float offsetFromExecution = (float)NonPublicProperties.ShotReport_get_FactorFromExecution.GetValue(__instance);
                    if (offsetFromExecution != 0)
                    {
                        reportBuilder.AppendLine("   " + "Execution".Translate() + "   " + offsetFromExecution.ToStringByStyle(ToStringStyle.FloatOne, ToStringNumberSense.Offset));
                    }

                    // Cover
                    var coverList = (List<CoverInfo>)NonPublicFields.ShotReport_covers.GetValue(__instance);
                    if (__instance.PassCoverChance < 1f)
                    {
                        reportBuilder.AppendLine("   " + "ShootingCover".Translate() + "        " + __instance.PassCoverChance.ToStringByStyle(ToStringStyle.FloatOne, ToStringNumberSense.Offset));
                        for (int i = 0; i < coverList.Count; i++)
                        {
                            CoverInfo coverInfo = coverList[i];
                            if (coverInfo.BlockChance > 0f)
                            {
                                reportBuilder.AppendLine("     " + "CoverThingBlocksPercentOfShots".Translate(coverInfo.Thing.LabelCap, coverInfo.BlockChance.ToStringByStyle(ToStringStyle.FloatOne, ToStringNumberSense.Offset), new NamedArgument(coverInfo.Thing.def, "COVER")).CapitalizeFirst());
                            }
                        }
                    }
                    else
                    {
                        reportBuilder.AppendLine("   (" + "NoCoverLower".Translate() + ")");
                    }
                }

                __result = reportBuilder.ToString();
                return false;
            }

        }


    }

}
