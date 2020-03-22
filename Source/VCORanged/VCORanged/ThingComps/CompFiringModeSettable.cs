using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;
using HarmonyLib;

namespace VCORanged
{

    public class CompFiringModeSettable : ThingComp
    {

        public IAttackTargetSearcher SearcherParent => (IAttackTargetSearcher)parent;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (SearcherParent.CurrentEffectiveVerb is Verb_LaunchProjectile)
            {
                yield return new Command_SetFiringMode
                {
                    firingModeComp = this
                };
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            // Set up initial firing modes
            if (SearcherParent is Pawn pawn && pawn.equipment != null)
                Notify_WeaponChanged(pawn.equipment.Primary, pawn);
            else if (SearcherParent is Building_TurretGun turret)
                Notify_WeaponChanged(turret.gun, turret);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref firingMode, "firingMode");

            // Save compatibility
            //if (Scribe.mode == LoadSaveMode.PostLoadInit && firingMode == null)
            //    firingMode = FiringModeDefOf.VCOR_FullAuto;
        }

        public void TrySetFiringMode(FiringModeDef newFiringMode)
        {
            if (newFiringMode.Allows(SearcherParent))
                firingMode = newFiringMode;
        }

        public void Notify_WeaponChanged(Thing eq, IAttackTargetSearcher searcher)
        {
            if (eq != null && eq.def.IsRangedWeapon)
                firingMode = FiringModeDefOf.VCOR_FullAuto.Allows(searcher) ? FiringModeDefOf.VCOR_FullAuto : FiringModeDefOf.VCOR_SingleShot;
        }

        public FiringModeDef firingMode = FiringModeDefOf.VCOR_FullAuto;

    }

}
