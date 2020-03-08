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

    public static class Patch_VerbProperties
    {

        [HarmonyPatch(typeof(VerbProperties), nameof(VerbProperties.GetHitChanceFactor))]
        public static class GetHitChanceFactor
        {

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                #if DEBUG
                    Log.Message("Transpiler start: VerbProperties.GetHitChanceFactor (2 matches)");
                #endif


                var instructionList = instructions.ToList();

                for (int i = 0; i < instructionList.Count; i++)
                {
                    var instruction = instructionList[i];

                    yield return instruction;

                    // Look for an instruction that loads local variable 'value'
                    if (instruction.opcode == OpCodes.Ldloc_0)
                    {
                        #if DEBUG
                            Log.Message("VerbProperties.GetHitChanceFactor match 1 of 2");
                        #endif
                        
                        // Check if this is the last loading of that variable (other than the one that precedes ret)
                        bool lastInstruction = true;
                        int j = 1;
                        while (i + j < instructionList.Count)
                        {
                            if (instructionList[i + j].opcode == instruction.opcode && instructionList[i + j + 1].opcode != OpCodes.Ret)
                            {
                                lastInstruction = false;
                                break;
                            }
                            j++;
                        }

                        // If so, remove all instructions ahead except the last
                        if (lastInstruction)
                        {
                            #if DEBUG
                                Log.Message("VerbProperties.GetHitChanceFactor match 2 of 2");
                            #endif
                            
                            j = i + 1;
                            while (j < instructionList.Count - 1)
                                instructionList.RemoveAt(j);
                        }
                    }
                }
            }

        }

    }

}
