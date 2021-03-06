﻿using System;
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

    public static class VCORangedUtility
    {

        public static float OffsetFromTargetSize(Thing caster, TargetInfo target)
        {
            // Targeting a thing
            if (target.HasThing)
            {
                // Pawn - base on body size as usual
                if (target.Thing is Pawn pawn) 
                {
                    float bodySizeDeviation = pawn.BodySize - 1;
                    if (bodySizeDeviation < 0)
                        return bodySizeDeviation * VCORangedTuning.AccuracyScorePerTargetSize * 2;
                    return bodySizeDeviation * VCORangedTuning.AccuracyScorePerTargetSize;
                }

                // Not a pawn
                else
                {
                    var targetThing = target.Thing;
                    var targetSize = targetThing.RotatedSize;
                    float angleFlat = (caster.TrueCenter() - targetThing.TrueCenter()).AngleFlat();
                    float effectiveTargetWidth;
                    if (angleFlat < 90 || (angleFlat > 180 && angleFlat <= 270))
                        effectiveTargetWidth = Mathf.Lerp(targetSize.x, targetSize.z, (angleFlat % 90) / 90);
                    else
                        effectiveTargetWidth = Mathf.Lerp(targetSize.z, targetSize.x, (angleFlat % 90) / 90);

                    return (targetThing.def.fillPercent * effectiveTargetWidth - 1) * VCORangedTuning.AccuracyScorePerTargetSize;
                }
            }

            // Targeting nothing
            return 0;
        }

        public static float OffsetFromWeather(float originalFactor, float distance)
        {
            return Mathf.Lerp(0, (1 - originalFactor) * VCORangedTuning.AccuracyScoreWeather, distance / ShootTuning.DistShort);
        }

        public static float OffsetFromDistance(this ShotReport report)
        {
            float adjustedDistance = Mathf.Max((float)NonPublicFields.ShotReport_distance.GetValue(report) - 1, 0);

            // Close up - accuracy bonus
            if (adjustedDistance < VCORangedTuning.MaxDistForAccuracyBonus)
                return (VCORangedTuning.MaxDistForAccuracyBonus - adjustedDistance) / VCORangedTuning.MaxDistForAccuracyBonus * VCORangedTuning.AccuracyScoreCloseRange;

            // Normal
            return (adjustedDistance - VCORangedTuning.MaxDistForAccuracyBonus) * VCORangedTuning.AccuracyScorePerDistance;
        }

        public static float OffsetFromGlow(this ShotReport report)
        {
            var reportTarget = (TargetInfo)NonPublicFields.ShotReport_target.GetValue(report);

            // VFE Security - return 0 if target is illuminated by a searchlight
            if (ModActive.VanillaFurnitureExpandedSecurity && reportTarget.Thing is ThingWithComps thingWComps)
            {
                var thingTracker = thingWComps.AllComps.FirstOrDefault(c => c.GetType() == NonPublicTypes.VanillaFurnitureExpandedSecurity.CompThingTracker);
                if (thingTracker != null && (bool)NonPublicProperties.VanillaFurnitureExpandedSecurity.CompThingTracker_get_Illuminated.GetValue(thingTracker))
                    return 0;
            }

            // Low light level
            float cellLight = reportTarget.Map.glowGrid.GameGlowAt(reportTarget.Cell);
            if (cellLight < 0.3f)
                return (1 - cellLight / 0.3f) * VCORangedTuning.AccuracyScoreDarkness;

            return 0;
        }

        public static float AimOnTargetScore_StandardTarget(this ShotReport report)
        {
            return
                (float)NonPublicFields.ShotReport_factorFromShooterAndDist.GetValue(report) + OffsetFromDistance(report) +
                (float)NonPublicFields.ShotReport_factorFromEquipment.GetValue(report) + OffsetFromGlow(report) + 
                (float)NonPublicFields.ShotReport_factorFromWeather.GetValue(report) + (float)NonPublicProperties.ShotReport_get_FactorFromCoveringGas.GetValue(report) + 
                (float)NonPublicProperties.ShotReport_get_FactorFromExecution.GetValue(report);
        }

        public static float AimOnTargetScore_IgnoringPosture(this ShotReport report)
        {
            return AimOnTargetScore_StandardTarget(report) + (float)NonPublicFields.ShotReport_factorFromTargetSize.GetValue(report);
        }

        public static float AimOnTargetScore(this ShotReport report)
        {
            return AimOnTargetScore_IgnoringPosture(report) + (float)NonPublicProperties.ShotReport_get_FactorFromPosture.GetValue(report);
        }

        public static float TotalHitScore(this ShotReport report)
        {
            return AimOnTargetScore(report) - (float)NonPublicFields.ShotReport_coversOverallBlockChance.GetValue(report);
        }


        public static bool IsShotgun(this Thing thing) => thing != null && thing.def.IsShotgun();

        public static bool IsShotgun(this ThingDef def)
        {
            if (VCORangedSettings.shotgunRevamp && def != null)
            {
                var thingDefExtension = ThingDefExtension.Get(def);
                return (!thingDefExtension.isShotgun.HasValue && def.defName.Contains("Shotgun")) || (thingDefExtension.isShotgun.HasValue && thingDefExtension.isShotgun.Value);
                // return VCORangedSettings.shotgunThingDefs.Contains(def);
            }
            return false;
        }

    }

}
