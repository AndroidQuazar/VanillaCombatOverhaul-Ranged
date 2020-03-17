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

    public static class Patch_Projectile
    {

        [HarmonyPatch(typeof(Projectile), nameof(Projectile.DamageAmount), MethodType.Getter)]
        public static class get_DamageAmount
        {

            public static void Postfix(Projectile __instance, ref int __result)
            {
                // Shotgun pellet
                if (__instance.EquipmentDef.IsShotgun())
                {
                    __result = GenMath.RoundRandom((float)__result / ExtendedProjectileProperties.Get(__instance.def).shotgunPelletCount);
                }
            }
        
        }

        [HarmonyPatch(typeof(Projectile), nameof(Projectile.Draw))]
        public static class Draw
        {

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                #if DEBUG
                    Log.Message("Transpiler start: Projectile.Draw (1 match)");
                #endif

                var instructionList = instructions.ToList();

                var drawMatSingleInfo = AccessTools.Property(typeof(BuildableDef), nameof(BuildableDef.DrawMatSingle)).GetGetMethod();

                var adjustedProjectileDrawMatSingle = AccessTools.Method(typeof(Draw), nameof(AdjustedProjectileDrawMatSingle));

                for (int i = 0; i < instructionList.Count; i++)
                {
                    var instruction = instructionList[i];

                    // Use shotgun pellet material if the projectile is fired from a shotgun
                    if (instruction.opcode == OpCodes.Callvirt && instruction.OperandIs(drawMatSingleInfo))
                    {
                        #if DEBUG
                            Log.Message("Projectile.Draw match 1 of 1");
                        #endif

                        yield return instruction; // this.def.DrawMatSingle
                        yield return new CodeInstruction(OpCodes.Ldarg_0); // this
                        instruction = new CodeInstruction(OpCodes.Call, adjustedProjectileDrawMatSingle); // AdjustedProjectileDrawMatSingle(this.def.DrawMatSingle, this)
                    }

                    yield return instruction;
                }
            }

            private static Material AdjustedProjectileDrawMatSingle(Material original, Projectile instance)
            {
                if (instance.EquipmentDef.IsShotgun())
                    return ExtendedProjectileProperties.Get(instance.def).PelletGraphic.MatSingle;
                return original;
            }

        }

    }

}
