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

    public class VCORangedSettings : ModSettings
    {

        private static Vector2 menuScrollPos;
        private static float menuViewHeight;

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

            // Do shotgun selection
            options.Label("");

            DoShotgunSelectionWindows(wrect.width, ref menuScrollPos, ref menuViewHeight);

            options.End();

            VCORanged.settings.Write();

        }

        private void DoShotgunSelectionWindows(float rectWidth, ref Vector2 scrollPos, ref float viewHeight)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            var scribeMode = Scribe.mode;

            if (scribeMode == LoadSaveMode.Saving)
            {
                // Convert list of defs into a single semicolon-separated set of defNames
                string defNameListString = String.Empty;
                for (int i = 0; i < shotgunThingDefs.Count; i++)
                {
                    defNameListString += shotgunThingDefs[i].defName;
                    if (i < shotgunThingDefs.Count - 1)
                        defNameListString += Delimiter;
                }
                shotgunThingDefNames = defNameListString;
            }

            Scribe_Values.Look(ref shotgunThingDefNames, "shotgunThingDefNames", String.Empty);

            if (scribeMode == LoadSaveMode.PostLoadInit)
            {
                // Convert semicolon-separated list back to a list of defs
                var defNameList = shotgunThingDefNames.Split(Delimiter);
                for (int i = 0; i < defNameList.Length; i++)
                    shotgunThingDefs.Add(DefDatabase<ThingDef>.GetNamedSilentFail(defNameList[i]));
            }
        }


        private const float RowHeight = 18;
        private const float RowVerticalMargin = 12;
        private const float DefBoxWidth = 320;
        private const float DefIconSize = 64;
        private const char Delimiter = ';';


    }

}
