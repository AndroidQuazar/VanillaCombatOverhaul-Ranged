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

        #region Verb
        public static Func<Verb, int> Verb_get_ShotsPerBurst = (Func<Verb, int>)
            Delegate.CreateDelegate(typeof(Func<Verb, int>), null, AccessTools.Property(typeof(Verb), "ShotsPerBurst").GetGetMethod(true));
        #endregion

        [StaticConstructorOnStartup]
        public static class VanillaFurnitureExpandedSecurity
        {

            static VanillaFurnitureExpandedSecurity()
            {
                if (ModActive.VanillaFurnitureExpandedSecurity)
                {
                    CompThingTracker_get_Illuminated = AccessTools.Property(NonPublicTypes.VanillaFurnitureExpandedSecurity.CompThingTracker, "Illuminated");
                }
            }

            #region CompThingTracker
            public static PropertyInfo CompThingTracker_get_Illuminated;
            #endregion

        }

    }

}
