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
    public static class ModActive
    {

        static ModActive()
        {
            var allMods = ModsConfig.ActiveModsInLoadOrder.ToList();
            for (int i = 0; i < allMods.Count; i++)
            {
                var curMod = allMods[i];

                if (curMod.PackageId.Equals("VanillaExpanded.VFESecurity", StringComparison.CurrentCultureIgnoreCase))
                    VanillaFurnitureExpandedSecurity = true;
            }
        }

        public static bool VanillaFurnitureExpandedSecurity;

    }

}
