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
    public static class Patch_VerbProperties
    {

        public static class GetHitChanceFactor
        {

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var instructionList = instructions.ToList();

                var lastValueLoadInstruction = instructionList.Last(i => i.opcode == OpCodes.Ldloc_0);

                for (int i = 0; i < instructionList.Count; i++)
                {
                    var instruction = instructionList[i];

                    yield return instruction;

                    // Look for the last instruction that loads 'value' local variable and remove all instructions ahead except the last one
                    if (instruction == lastValueLoadInstruction)
                    {
                        while (true)
                        {
                            int j = i + 1;
                            if (j == instructionList.Count)
                                break;
                            instructionList.RemoveAt(j);
                        }
                    }
                }
            }

        }

    }

}
