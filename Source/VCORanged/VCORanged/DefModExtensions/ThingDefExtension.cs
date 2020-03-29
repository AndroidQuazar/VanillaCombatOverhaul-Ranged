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

    public class ThingDefExtension : DefModExtension
    {
        private static readonly ThingDefExtension defaultValues = new ThingDefExtension();

        public static ThingDefExtension Get(Def def) => def.GetModExtension<ThingDefExtension>() ?? defaultValues;

        public bool? isShotgun;

    }

}
