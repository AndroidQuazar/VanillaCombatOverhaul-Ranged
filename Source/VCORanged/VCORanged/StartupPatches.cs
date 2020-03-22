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

    [StaticConstructorOnStartup]
    public static class StartupPatches
    {

        static StartupPatches()
        {
            PatchThingDefs();
        }

        private static void PatchThingDefs()
        {
            var thingDefs = DefDatabase<ThingDef>.AllDefsListForReading;
            for (int i = 0; i < thingDefs.Count; i++)
            {
                var curDef = thingDefs[i];

                // Add CompFiringModeSettable to anything that can attack
                if (typeof(IAttackTargetSearcher).IsAssignableFrom(curDef.thingClass))
                {
                    if (curDef.comps == null)
                        curDef.comps = new List<CompProperties>();
                    curDef.comps.Add(new CompProperties(typeof(CompFiringModeSettable)));
                }
                
                // Increase projectile speed across the board
                if (curDef.projectile is ProjectileProperties projProps)
                {
                    projProps.speed *= 1 + (projProps.speed / 200);
                }

                else if (curDef.IsWeaponUsingProjectiles)
                {
                    // Autopatch recoil amount
                    if (curDef.statBases == null)
                        curDef.statBases = new List<StatModifier>();
                    if (!curDef.statBases.Any(s => s.stat == StatDefOf.VCOR_RecoilAmount))
                    {
                        var firstRangedVerb = curDef.Verbs.FirstOrDefault(v => typeof(Verb_LaunchProjectile).IsAssignableFrom(v.verbClass) && v.defaultProjectile != null);
                        if (firstRangedVerb != null)
                            curDef.SetStatBaseValue(StatDefOf.VCOR_RecoilAmount, firstRangedVerb.defaultProjectile.projectile.GetDamageAmount(null) * VCORangedTuning.RecoilPerDamageAmount);
                    }

                    // Basic shotgun autopatch
                    for (int j = 0; j < curDef.Verbs.Count; j++)
                    {
                        var curVerbProjectile = curDef.Verbs[j].defaultProjectile;
                        if (curVerbProjectile != null && !curVerbProjectile.HasModExtension<ExtendedProjectileProperties>())
                        {
                            if (curVerbProjectile.modExtensions == null)
                                curVerbProjectile.modExtensions = new List<DefModExtension>();

                            curVerbProjectile.modExtensions.Add(new ExtendedProjectileProperties()
                            {
                                shotgunPelletCount = 6,
                                shotgunPelletGraphicData = new GraphicData()
                                {
                                    texPath = curDef.techLevel < TechLevel.Spacer ? ExtendedProjectileProperties.PelletTexPathStandard : ExtendedProjectileProperties.PelletTexPathAdvanced,
                                    graphicClass = typeof(Graphic_Single),
                                    shaderType = ShaderTypeDefOf.Transparent
                                }
                            });
                        }
                    }
                }
            }
        }

    }

}
