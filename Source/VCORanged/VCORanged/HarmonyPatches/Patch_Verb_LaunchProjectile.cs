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

    public static class Patch_Verb_LaunchProjectile
    {

        [HarmonyPatch(typeof(Verb_LaunchProjectile), "TryCastShot")]
        public static class TryCastShot
        {

            // Detour to take shotgun pellet count into account
            public static bool Prefix(Verb_LaunchProjectile __instance, ref bool __result, LocalTargetInfo ___currentTarget, bool ___canHitNonTargetPawnsNow)
            {
				// Determine how many times to iterate through
				int iterationCount = __instance.EquipmentSource.IsShotgun() ? ExtendedProjectileProperties.Get(__instance.verbProps.defaultProjectile).shotgunPelletCount : 1;

				for (int i = 0; i < iterationCount; i++)
				{
					if (___currentTarget.HasThing && ___currentTarget.Thing.Map != __instance.caster.Map)
					{
						return false;
					}
					ThingDef projectile = __instance.Projectile;
					if (projectile == null)
					{
						return false;
					}
					ShootLine shootLine;
					bool flag = __instance.TryFindShootLineFromTo(__instance.caster.Position, ___currentTarget, out shootLine);
					if (__instance.verbProps.stopBurstWithoutLos && !flag)
					{
						return false;
					}
					if (__instance.EquipmentSource != null)
					{
						CompChangeableProjectile comp = __instance.EquipmentSource.GetComp<CompChangeableProjectile>();
						if (comp != null)
						{
							comp.Notify_ProjectileLaunched();
						}
					}
					Thing launcher = __instance.caster;
					Thing equipment = __instance.EquipmentSource;
					CompMannable compMannable = __instance.caster.TryGetComp<CompMannable>();
					if (compMannable != null && compMannable.ManningPawn != null)
					{
						launcher = compMannable.ManningPawn;
						equipment = __instance.caster;
					}
					Vector3 drawPos = __instance.caster.DrawPos;
					Projectile projectile2 = (Projectile)GenSpawn.Spawn(projectile, shootLine.Source, __instance.caster.Map, WipeMode.Vanish);
					if (__instance.verbProps.forcedMissRadius > 0.5f)
					{
						float num = VerbUtility.CalculateAdjustedForcedMiss(__instance.verbProps.forcedMissRadius, ___currentTarget.Cell - __instance.caster.Position);
						if (num > 0.5f)
						{
							int max = GenRadial.NumCellsInRadius(num);
							int num2 = Rand.Range(0, max);
							if (num2 > 0)
							{
								IntVec3 c = ___currentTarget.Cell + GenRadial.RadialPattern[num2];
								NonPublicMethods.Verb_LaunchProjectile_ThrowDebugText_a(__instance, "ToRadius");
								NonPublicMethods.Verb_LaunchProjectile_ThrowDebugText_b(__instance, "Rad\nDest", c);
								ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.NonTargetWorld;
								if (Rand.Chance(0.5f))
								{
									projectileHitFlags = ProjectileHitFlags.All;
								}
								if (!___canHitNonTargetPawnsNow)
								{
									projectileHitFlags &= ~ProjectileHitFlags.NonTargetPawns;
								}
								projectile2.Launch(launcher, drawPos, c, ___currentTarget, projectileHitFlags, equipment, null);
								__result = true;
								continue;
							}
						}
					}
					ShotReport shotReport = ShotReport.HitReportFor(__instance.caster, __instance, ___currentTarget);
					Thing randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
					ThingDef targetCoverDef = (randomCoverToMissInto != null) ? randomCoverToMissInto.def : null;
					//if (!Rand.Chance(shotReport.AimOnTargetChance_IgnoringPosture))
					if (!Rand.Chance(shotReport.AimOnTargetChance))
					{
						shootLine.ChangeDestToMissWild(shotReport.AimOnTargetChance_StandardTarget);
						NonPublicMethods.Verb_LaunchProjectile_ThrowDebugText_a(__instance, "ToWild" + (___canHitNonTargetPawnsNow ? "\nchntp" : ""));
						NonPublicMethods.Verb_LaunchProjectile_ThrowDebugText_b(__instance, "Wild\nDest", shootLine.Dest);
						ProjectileHitFlags projectileHitFlags2 = ProjectileHitFlags.NonTargetWorld;
						if (Rand.Chance(0.5f) && ___canHitNonTargetPawnsNow)
						{
							projectileHitFlags2 |= ProjectileHitFlags.NonTargetPawns;
						}
						projectile2.Launch(launcher, drawPos, shootLine.Dest, ___currentTarget, projectileHitFlags2, equipment, targetCoverDef);
						__result = true;
						continue;
					}
					if (___currentTarget.Thing != null && ___currentTarget.Thing.def.category == ThingCategory.Pawn && !Rand.Chance(shotReport.PassCoverChance))
					{
						NonPublicMethods.Verb_LaunchProjectile_ThrowDebugText_a(__instance, "ToCover" + (___canHitNonTargetPawnsNow ? "\nchntp" : ""));
						NonPublicMethods.Verb_LaunchProjectile_ThrowDebugText_b(__instance, "Cover\nDest", randomCoverToMissInto.Position);
						ProjectileHitFlags projectileHitFlags3 = ProjectileHitFlags.NonTargetWorld;
						if (___canHitNonTargetPawnsNow)
						{
							projectileHitFlags3 |= ProjectileHitFlags.NonTargetPawns;
						}
						projectile2.Launch(launcher, drawPos, randomCoverToMissInto, ___currentTarget, projectileHitFlags3, equipment, targetCoverDef);
						__result = true;
						continue;
					}
					ProjectileHitFlags projectileHitFlags4 = ProjectileHitFlags.IntendedTarget;
					if (___canHitNonTargetPawnsNow)
					{
						projectileHitFlags4 |= ProjectileHitFlags.NonTargetPawns;
					}
					if (!___currentTarget.HasThing || ___currentTarget.Thing.def.Fillage == FillCategory.Full)
					{
						projectileHitFlags4 |= ProjectileHitFlags.NonTargetWorld;
					}
					NonPublicMethods.Verb_LaunchProjectile_ThrowDebugText_a(__instance, "ToHit" + (___canHitNonTargetPawnsNow ? "\nchntp" : ""));
					if (___currentTarget.Thing != null)
					{
						projectile2.Launch(launcher, drawPos, ___currentTarget, ___currentTarget, projectileHitFlags4, equipment, targetCoverDef);
						NonPublicMethods.Verb_LaunchProjectile_ThrowDebugText_b(__instance, "Hit\nDest", ___currentTarget.Cell);
					}
					else
					{
						projectile2.Launch(launcher, drawPos, shootLine.Dest, ___currentTarget, projectileHitFlags4, equipment, targetCoverDef);
						NonPublicMethods.Verb_LaunchProjectile_ThrowDebugText_b(__instance, "Hit\nDest", shootLine.Dest);
					}
					__result = true;
				}
				return false;
			}

        }

    }

}
