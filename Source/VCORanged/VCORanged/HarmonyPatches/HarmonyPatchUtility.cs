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

    public static class HarmonyPatchUtility
    {

        public static IEnumerable<CodeInstruction> ReplaceMulWithAdd(IEnumerable<CodeInstruction> instructions)
        {
            #if DEBUG
                Log.Message("Transpiler start: HarmonyPatchUtility.ReplaceMulWithAdd (1 match)");
            #endif

            var instructionList = instructions.ToList();

            for (int i = 0; i < instructionList.Count; i++)
            {
                var instruction = instructionList[i];

                // Replace all multiplication operations with adding instead
                if (instruction.opcode == OpCodes.Mul)
                {
                    #if DEBUG
                        Log.Message("HarmonyPatchUtility.ReplaceMulWithAdd match 1 of 1");
                    #endif

                    instruction.opcode = OpCodes.Add;
                }

                yield return instruction;
            }
        }

    }

}
