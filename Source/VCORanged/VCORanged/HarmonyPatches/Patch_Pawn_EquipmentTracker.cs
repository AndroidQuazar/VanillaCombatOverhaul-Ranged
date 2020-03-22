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

    public static class Patch_Pawn_EquipmentTracker
    {

        [HarmonyPatch(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.Notify_EquipmentAdded))]
        public static class Notify_EquipmentAdded
        {

            public static void Postfix(Pawn_EquipmentTracker __instance, ThingWithComps eq)
            {
                // Notify CompFiringModeSettable
                if (__instance.pawn.TryGetComp<CompFiringModeSettable>() is CompFiringModeSettable firingModeComp)
                    firingModeComp.Notify_WeaponChanged(eq, __instance.pawn);
            }

        }



    }

}
