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
    public static class StartupPatches
    {

        static StartupPatches()
        {
            PatchThingDefs();
        }

        private static void PatchThingDefs()
        {
            var thingDefs = DefDatabase<ThingDef>.AllDefsListForReading;
            for (int i = 0; i < thingDefs.Count; i++)
            {
                var curDef = thingDefs[i];
                
                // Increase projectile speed across the board
                if (curDef.projectile is ProjectileProperties projProps)
                {
                    projProps.speed *= 1 + (projProps.speed / 100);
                }
            }
        }

    }

}
