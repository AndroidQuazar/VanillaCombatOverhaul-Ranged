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

    // [StaticConstructorOnStartup]
    public static class VCORangedSettingsUtility
    {

        static VCORangedSettingsUtility()
        {
            // Sort out a list of specifically weapon defs in alphabetical order
            var thingDefs = DefDatabase<ThingDef>.AllDefsListForReading;
            thingDefs.SortBy(d => d.LabelCap.RawText);
            for (int i = 0; i < thingDefs.Count; i++)
            {
                var curDef = thingDefs[i];
                if (curDef.IsWeaponUsingProjectiles)
                    weaponDefsAlphabetical.Add(curDef);
            }

            // Resolve initial NonShotgunDefs
            for (int i = 0; i < weaponDefsAlphabetical.Count; i++)
            {
                var curWeapon = weaponDefsAlphabetical[i];
                if (!VCORangedSettings.shotgunThingDefs.Contains(curWeapon))
                    VCORangedSettings.nonShotgunThingDefs.Add(curWeapon);
            }
        }

        public static void AutoDetectShotguns()
        {
            VCORangedSettings.shotgunThingDefs.Clear();
            VCORangedSettings.nonShotgunThingDefs.Clear();
        }

        public static List<ThingDef> weaponDefsAlphabetical;


    }

}
