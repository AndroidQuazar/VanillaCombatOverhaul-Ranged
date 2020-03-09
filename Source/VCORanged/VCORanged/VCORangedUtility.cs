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

    public static class VCORangedUtility
    {

        public static float OffsetFromTargetSize(Thing caster, TargetInfo target)
        {
            // Targeting a thing
            if (target.HasThing)
            {
                // Pawn - base on body size as usual
                if (target.Thing is Pawn pawn)
                    return (pawn.BodySize - 1) * VCORangedTuning.AccuracyScorePerTargetSize;

                // Not a pawn
                else
                {
                    var targetThing = target.Thing;
                    var targetSize = targetThing.RotatedSize;
                    float angleFlat = (caster.TrueCenter() - targetThing.TrueCenter()).AngleFlat();
                    float effectiveTargetWidth;
                    if (angleFlat < 90 || (angleFlat > 180 && angleFlat <= 270))
                        effectiveTargetWidth = Mathf.Lerp(targetSize.x, targetSize.z, Mathf.Tan((angleFlat % 90) / 90));
                    else
                        effectiveTargetWidth = Mathf.Lerp(targetSize.z, targetSize.x, Mathf.Tan((angleFlat % 90) / 90));

                    return (targetThing.def.fillPercent * effectiveTargetWidth - 1) * VCORangedTuning.AccuracyScorePerTargetSize;
                }
            }

            // Targeting nothing
            return 0;
        }

        public static float OffsetFromWeather(float originalFactor) => (1 - originalFactor) * VCORangedTuning.AccuracyScoreWeather;

        public static float OffsetFromDistance(this ShotReport report) => DistanceToAccuracyScoreCurve.Evaluate((float)NonPublicFields.ShotReport_distance.GetValue(report));

        public static float OffsetFromGlow(this ShotReport report)
        {
            var reportTarget = (TargetInfo)NonPublicFields.ShotReport_target.GetValue(report);

            // VFE Security - return 0 if target is illuminated by a searchlight
            if (ModCompatibilityCheck.VanillaFurnitureExpandedSecurity && reportTarget.Thing is ThingWithComps thingWComps)
            {
                var thingTracker = thingWComps.AllComps.FirstOrDefault(c => c.GetType() == NonPublicTypes.VanillaFurnitureExpandedSecurity.CompThingTracker);
                if (thingTracker != null && (bool)NonPublicProperties.VanillaFurnitureExpandedSecurity.CompThingTracker_get_Illuminated.GetValue(thingTracker))
                    return 0;
            }

            return GlowToAccuracyScoreCurve.Evaluate(reportTarget.Map.glowGrid.GameGlowAt(reportTarget.Cell));
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


        public static bool IsShotgun(this Thing thing) => thing.def.IsShotgun();

        public static bool IsShotgun(this ThingDef def) => VCORangedSettings.shotgunThingDefs.Contains(def);

        public static readonly SimpleCurve AccuracyScoreToPercentageCurve = new SimpleCurve()
        {
            new CurvePoint(-60, 0.01f),
            new CurvePoint(-40, 0.02f),
            new CurvePoint(-30, 0.04f),
            new CurvePoint(-20, 0.10f),
            new CurvePoint(-10, 0.25f),
            new CurvePoint(0, 0.7f),
            new CurvePoint(10, 0.9f),
            new CurvePoint(20, 0.99f),
            new CurvePoint(40, 1.00f),
        };

        public static readonly SimpleCurve DistanceToAccuracyScoreCurve = new SimpleCurve()
        {
            new CurvePoint(1, VCORangedTuning.AccuracyScoreCloseRange),
            new CurvePoint(ShootTuning.DistTouch, 0),
            new CurvePoint(10000 + ShootTuning.DistTouch, 10000 * VCORangedTuning.AccuracyScorePerDistance)
        };

        public static readonly SimpleCurve GlowToAccuracyScoreCurve = new SimpleCurve()
        {
            new CurvePoint(0, VCORangedTuning.AccuracyScoreDarkness),
            new CurvePoint(0.3f, 0)
        };

    }

}
