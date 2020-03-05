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

    public class VCORanged : Mod
    {

        public VCORanged(ModContentPack content) : base(content)
        {
            harmonyInstance = new Harmony("OskarPotocki.VanillaCombatOverhaul.RangedModule");
        }

        public static Harmony harmonyInstance;

    }

}
