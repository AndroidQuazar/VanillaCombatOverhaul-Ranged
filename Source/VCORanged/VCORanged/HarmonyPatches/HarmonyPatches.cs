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
    public static class HarmonyPatches
    {

        static HarmonyPatches()
        {
            #if DEBUG
                Harmony.DEBUG = true;
            #endif

            VCORanged.harmonyInstance.PatchAll();

            #region RunAndGun
            if (ModActive.RunAndGun)
            {
                var runAndGunWeaponAccuracyPatchType = GenTypes.GetTypeInAnyAssembly("RunAndGun.Harmony.VerbProperties_AdjustedAccuracy", "RunAndGun.Harmony");
                if (runAndGunWeaponAccuracyPatchType != null)
                    VCORanged.harmonyInstance.Patch(AccessTools.Method(runAndGunWeaponAccuracyPatchType, "Postfix"),
                        transpiler: new HarmonyMethod(typeof(Patch_RunAndGun_Harmony.manual_VerbProperties_AdjustedAccuracy_Postfix), "Transpiler"));
                else
                    Log.Error("Could not find type RunAndGun.Harmony.VerbProperties_AdjustedAccuracy in RunAndGun");
            }
            #endregion
        }

    }

}
