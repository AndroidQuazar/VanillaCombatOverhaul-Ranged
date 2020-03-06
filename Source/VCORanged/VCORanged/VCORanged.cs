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
            #if DEBUG
                Log.Error("Somebody left debugging enabled in Vanilla Combat Overhaul Ranged - please let him know!");
            #endif

            settings = GetSettings<VCORangedSettings>();
            harmonyInstance = new Harmony("OskarPotocki.VanillaCombatOverhaul.RangedModule");
        }

        public override string SettingsCategory() => "VCO.RangedModule.SettingsCategory".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DoWindowContents(inRect);
        }

        public static Harmony harmonyInstance;
        public static VCORangedSettings settings;

    }

}
