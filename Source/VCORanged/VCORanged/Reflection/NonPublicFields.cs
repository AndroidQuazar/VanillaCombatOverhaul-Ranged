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
    public static class NonPublicFields
    {

        #region ShotReport
        public static FieldInfo ShotReport_target = AccessTools.Field(typeof(ShotReport), "target");
        public static FieldInfo ShotReport_distance = AccessTools.Field(typeof(ShotReport), "distance");
        public static FieldInfo ShotReport_factorFromShooterAndDist = AccessTools.Field(typeof(ShotReport), "factorFromShooterAndDist");
        public static FieldInfo ShotReport_factorFromEquipment = AccessTools.Field(typeof(ShotReport), "factorFromEquipment");
        public static FieldInfo ShotReport_factorFromWeather = AccessTools.Field(typeof(ShotReport), "factorFromWeather");
        public static FieldInfo ShotReport_factorFromTargetSize = AccessTools.Field(typeof(ShotReport), "factorFromTargetSize");
        public static FieldInfo ShotReport_forcedMissRadius = AccessTools.Field(typeof(ShotReport), "forcedMissRadius");
        public static FieldInfo ShotReport_coveringGas = AccessTools.Field(typeof(ShotReport), "coveringGas");
        public static FieldInfo ShotReport_covers = AccessTools.Field(typeof(ShotReport), "covers");
        #endregion

    }

}
