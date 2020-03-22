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
    public class VCORangedSettings : ModSettings
    {

        private static Vector2 menuScrollPos;
        private static float menuViewHeight;

        public static bool weaponRecoil = true;
        public static bool shotgunRevamp = true;
        public static ShotgunDamageRoundMode shotgunDamageRounding = ShotgunDamageRoundMode.Random;
        public static string shotgunThingDefNames = string.Empty;

        [Unsaved]
        public static List<ThingDef> shotgunThingDefs = new List<ThingDef>();

        [Unsaved]
        public static List<ThingDef> nonShotgunThingDefs = new List<ThingDef>();


        public void DoWindowContents(Rect wrect)
        {
            var options = new Listing_Standard();
            var defaultColor = GUI.color;
            options.Begin(wrect);

            GUI.color = defaultColor;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            options.Gap();

            // Weapon recoil
            options.CheckboxLabeled("VCO.RangedModule.WeaponRecoil".Translate(), ref weaponRecoil, "VCO.RangedModule.WeaponRecoil_Desc".Translate());
            options.Gap();

            #region Shotguns
            // Shotgun revamp toggle
            options.CheckboxLabeled("VCO.RangedModule.ShotgunRevamp".Translate(), ref shotgunRevamp, "VCO.RangedModule.ShotgunRevamp_Desc".Translate());
            options.Gap();

            if (!shotgunRevamp)
                GUI.color = Color.grey;

            // Shotgun damage rounding mode
            options.Label("VCO.RangedModule.ShotgunDamageRounding".Translate());
            var shotgunDamageRoundingOpts = Enum.GetValues(typeof(ShotgunDamageRoundMode)).Cast<ShotgunDamageRoundMode>().ToList();
            for (int i = 0; i < shotgunDamageRoundingOpts.Count; i++)
            {
                var curOpt = shotgunDamageRoundingOpts[i];
                if (options.RadioButton($"VCO.RangedModule.ShotgunDamageRounding_{curOpt}".Translate(), shotgunDamageRounding == curOpt, 12,
                    $"VCO.RangedModule.ShotgunDamageRounding_{curOpt}_Desc".Translate()) && shotgunRevamp)
                    shotgunDamageRounding = curOpt;
            }

            GUI.color = defaultColor;
            #endregion

            options.End();

            VCORanged.settings.Write();

        }

        private void DoShotgunSelectionWindows(Listing_Standard listing, ref Vector2 scrollPos, ref float viewHeight)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            var scribeMode = Scribe.mode;

            //if (scribeMode == LoadSaveMode.Saving)
            //{
            //    // Convert list of defs into a single semicolon-separated set of defNames
            //    string defNameListString = String.Empty;
            //    for (int i = 0; i < shotgunThingDefs.Count; i++)
            //    {
            //        defNameListString += shotgunThingDefs[i].defName;
            //        if (i < shotgunThingDefs.Count - 1)
            //            defNameListString += Delimiter;
            //    }
            //    shotgunThingDefNames = defNameListString;
            //}

            Scribe_Values.Look(ref weaponRecoil, "weaponRecoil", true);
            Scribe_Values.Look(ref shotgunRevamp, "shotgunRevamp", true);
            Scribe_Values.Look(ref shotgunDamageRounding, "shotgunDamageRounding", ShotgunDamageRoundMode.Random);
            Scribe_Values.Look(ref shotgunThingDefNames, "shotgunThingDefNames", String.Empty);

            //if (scribeMode == LoadSaveMode.PostLoadInit)
            //{
            //    // Convert semicolon-separated list back to a list of defs
            //    var defNameList = shotgunThingDefNames.Split(Delimiter);
            //    for (int i = 0; i < defNameList.Length; i++)
            //        shotgunThingDefs.Add(DefDatabase<ThingDef>.GetNamedSilentFail(defNameList[i]));
            //}
        }


        private const float RowHeight = 18;
        private const float RowVerticalMargin = 12;
        private const float DefBoxWidth = 320;
        private const float DefIconSize = 64;
        private const char Delimiter = ';';


    }

}
