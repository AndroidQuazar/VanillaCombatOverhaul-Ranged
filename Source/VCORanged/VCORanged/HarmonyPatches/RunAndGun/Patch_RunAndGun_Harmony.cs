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

    public static class Patch_RunAndGun_Harmony
    {

        public static class manual_VerbProperties_AdjustedAccuracy_Postfix
        {

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase method)
            {
                var instructionList = instructions.ToList();

                int accMultLocalIndex = method.GetMethodBody().LocalVariables.FirstIndexOf(l => l.LocalType == typeof(float));
                bool accMultDeclarationDone = false;

                var movementAccuracyOffsetInfo = AccessTools.Method(typeof(manual_VerbProperties_AdjustedAccuracy_Postfix), nameof(MovementAccuracyOffset));

                for (int i = 0; i < instructionList.Count; i++)
                {
                    var instruction = instructionList[i];

                    // Look for first storage of 'num' - the multiplier on weapon accuracy
                    if (!accMultDeclarationDone && instruction.opcode == OpCodes.Stloc_S && instruction.operand is LocalBuilder lb && lb.LocalIndex == accMultLocalIndex)
                    {
                        yield return instruction; // float num = (float)(100 - value2) / 100f;
                        yield return new CodeInstruction(OpCodes.Ldloc_S, accMultLocalIndex); // num
                        yield return new CodeInstruction(OpCodes.Call, movementAccuracyOffsetInfo); // MovementAccuracyOffset(num)
                        instruction = new CodeInstruction(OpCodes.Stloc_S, accMultLocalIndex); // num = MovementAccuracyOffset(num)

                        accMultDeclarationDone = true;
                    }

                    // Replace multiplications with additions
                    else if (instruction.opcode == OpCodes.Mul)
                        instruction.opcode = OpCodes.Add;

                    yield return instruction;
                }
            }

            private static float MovementAccuracyOffset(float original)
            {
                if (RimWorld.StatDefOf.AccuracyTouch.postProcessCurve != null)
                    return RimWorld.StatDefOf.AccuracyTouch.postProcessCurve.Evaluate(original);
                return original;
            }
        
        }


    }

}
