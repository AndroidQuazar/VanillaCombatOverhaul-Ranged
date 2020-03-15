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

    public static class Patch_Verb_LaunchProjectile
    {

        [HarmonyPatch(typeof(Verb_LaunchProjectile), "TryCastShot")]
        public static class TryCastShot
        {

            // __state[0] is int representing how many times the method can be called
            // __state[1] is int capturing __result

            public static bool Prefix(Verb_LaunchProjectile __instance, ref int[] __state)
            {
                if (__state == null)
                    __state = new int[2];



                NonPublicMethods.Verb_LaunchProjectile_TryCastShot(__instance);
                return false;
            }

            public static void Postfix(Verb_LaunchProjectile __instance, ref int[] __state, ref bool __result)
            {
                // Check whether or not a shot was successfully cast
                if (__result)
                    __state[1] = 1;

                // Keep casting shots if applicable
                Prefix(__instance, ref __state);
            }

        }

    }

}
