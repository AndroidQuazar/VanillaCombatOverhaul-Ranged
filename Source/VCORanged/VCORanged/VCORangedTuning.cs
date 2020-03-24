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

    public static class VCORangedTuning
    {

        public const float AccuracyScorePerDistance = -0.6f;
        public const float AccuracyScoreWeather = -12;
        public const float AccuracyScoreCoveringGas = -12;
        public const float AccuracyScoreCover = 12;
        public const float AccuracyScoreProne = -6;
        public const float AccuracyScoreDarkness = -6;
        public const float AccuracyScoreCloseRange = 7.5f;
        public const float AccuracyScorePerTargetSize = 6;
        public const float AccuracyScoreExecution = 30;

        public const float MaxDistForAccuracyBonus = 5;

        public const float RecoilPerDamageAmount = 0.15f;

        public static readonly SimpleCurve AccuracyScoreToPercentageCurve = new SimpleCurve()
        {
            new CurvePoint(-60, 0.01f),
            new CurvePoint(-40, 0.02f),
            new CurvePoint(-30, 0.04f),
            new CurvePoint(-20, 0.10f),
            new CurvePoint(-10, 0.25f),
            new CurvePoint(0, 0.70f),
            new CurvePoint(10, 0.90f),
            new CurvePoint(20, 0.99f),
            new CurvePoint(40, 1.00f),
        };

    }

}
