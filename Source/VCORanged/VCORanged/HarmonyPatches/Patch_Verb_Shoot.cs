using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;
using HarmonyLib;

namespace VCORanged
{

    public static class Patch_Verb_Shoot
    {

        [HarmonyPatch(typeof(Verb_Shoot), "ShotsPerBurst", MethodType.Getter)]
        public static class get_ShotsPerBurst
        {

            public static void Postfix(Verb_Shoot __instance, ref int __result)
            {
                // Adjust burst shot count
                if (__instance.Caster != null && __instance.Caster.TryGetComp<CompFiringModeSettable>() is CompFiringModeSettable firingModeComp)
                {
                    __result = firingModeComp.firingMode.AdjustedBurstShotCount(__result);
                }
            }

        }

    }

}
