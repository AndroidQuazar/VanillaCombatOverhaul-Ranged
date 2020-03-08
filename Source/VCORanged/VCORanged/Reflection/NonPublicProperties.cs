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

    [StaticConstructorOnStartup]
    public static class NonPublicProperties
    {

        #region ShotReport
        // Had no luck with delegates so this is the best alternative I could come up with

        public static PropertyInfo ShotReport_get_FactorFromPosture = AccessTools.Property(typeof(ShotReport), "FactorFromPosture");
        public static PropertyInfo ShotReport_get_FactorFromExecution = AccessTools.Property(typeof(ShotReport), "FactorFromExecution");
        public static PropertyInfo ShotReport_get_FactorFromCoveringGas = AccessTools.Property(typeof(ShotReport), "FactorFromCoveringGas");
        #endregion

    }

}
