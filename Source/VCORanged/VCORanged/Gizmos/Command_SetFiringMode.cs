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
    public class Command_SetFiringMode : Command
    {

        static Command_SetFiringMode()
        {
            firingModeDefsSorted = DefDatabase<FiringModeDef>.AllDefsListForReading;
            firingModeDefsSorted.SortBy(m => m.displayOrder, m => m.label);

            SetFiringModeTex = ContentFinder<Texture2D>.Get("UI/Commands/SetFiringMode");
        }

        public Command_SetFiringMode()
        {
            defaultDesc = "VCO.RangedModule.CommandSetFiringModeDesc".Translate();

            FiringModeDef singleFiringMode = null;
            bool multiSelected = false;

            // Whether or not multiple dfiring mode comp parents with different modes are selected
            var selectedObjects = Find.Selector.SelectedObjectsListForReading;
            for (int i = 0; i < selectedObjects.Count; i++)
            {
                var curObj = selectedObjects[i];
                if (curObj is ThingWithComps thingWc && thingWc.GetComp<CompFiringModeSettable>() is CompFiringModeSettable firingComp)
                {
                    if (singleFiringMode != null && singleFiringMode != firingComp.firingMode)
                    {
                        multiSelected = true;
                        break;
                    }
                    singleFiringMode = firingComp.firingMode;
                }
            }

            if (multiSelected)
            {
                icon = SetFiringModeTex;
                defaultLabel = "VCO.RangedModule.CommandSetFiringModeMulti".Translate();
            }
            else
            {
                icon = singleFiringMode.uiIcon;
                defaultLabel = singleFiringMode.LabelCap;
            }
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);

            // Sort out lists of comps to change
            if (firingModeComps == null)
                firingModeComps = new List<CompFiringModeSettable>();
            if (!firingModeComps.Contains(firingModeComp))
                firingModeComps.Add(firingModeComp);

            // Sort out float menu
            var firingModeOpts = new List<FloatMenuOption>();
            for (int i = 0; i < firingModeDefsSorted.Count; i++)
            {
                var curDef = firingModeDefsSorted[i];
                if (firingModeComps.Count > 1 || curDef.Allows(firingModeComp.SearcherParent))
                    firingModeOpts.Add(new FloatMenuOption(curDef.LabelCap, () => TrySetFiringMode(firingModeComps, curDef)));
            }
            Find.WindowStack.Add(new FloatMenu(firingModeOpts));
        }

        private static void TrySetFiringMode(List<CompFiringModeSettable> comps, FiringModeDef def)
        {
            for (int i = 0; i < comps.Count; i++)
                comps[i].TrySetFiringMode(def);
        }

        public override bool InheritInteractionsFrom(Gizmo other)
        {
            if (firingModeComps == null)
                firingModeComps = new List<CompFiringModeSettable>();
            firingModeComps.Add(((Command_SetFiringMode)other).firingModeComp);
            return false;
        }

        private static List<FiringModeDef> firingModeDefsSorted;
        private static Texture2D SetFiringModeTex;

        public CompFiringModeSettable firingModeComp;
        public List<CompFiringModeSettable> firingModeComps;

    }

}
