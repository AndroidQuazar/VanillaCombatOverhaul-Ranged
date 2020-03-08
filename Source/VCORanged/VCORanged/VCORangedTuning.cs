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

        public const float AccuracyScorePerDistance = -0.5f;
        public const float AccuracyScoreWeather = -18;
        public const float AccuracyScoreCoveringGas = -18;
        public const float AccuracyScoreCover = 18;
        public const float AccuracyScoreProne = -8;
        public const float AccuracyScoreDarkness = -8;
        public const float AccuracyScoreCloseRange = 8;
        public const float AccuracyScorePerTargetSize = 6;
        public const float AccuracyScoreExecution = 40;

    }

}
