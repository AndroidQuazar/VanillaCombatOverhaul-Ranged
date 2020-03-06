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

        public static string shotgunThingDefNames = string.Empty;

        [Unsaved]
        public static List<ThingDef> shotgunThingDefs = new List<ThingDef>();

        public void DoWindowContents(Rect wrect)
        {
            var options = new Listing_Standard();
            var defaultColor = GUI.color;
            options.Begin(wrect);

            GUI.color = defaultColor;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            options.Gap();



            options.End();

            VCORanged.settings.Write();

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

        private const char Delimiter = ';';


    }

}
