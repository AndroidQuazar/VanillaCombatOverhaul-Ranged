﻿using System;
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

    public static class Patch_CoverUtility
    {

        [HarmonyPatch(typeof(CoverUtility), nameof(CoverUtility.CalculateOverallBlockChance))]
        public static class CalculateOverallBlockChance
        {

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase method)
            {
                #if DEBUG
                    Log.Message("Transpiler start: CoverUtility.CalculateOverallBlockChance (2 matches)");
                #endif

                var instructionList = instructions.ToList();

                int coverInfoLocalIndex = method.GetMethodBody().LocalVariables.First(l => l.LocalType == typeof(CoverInfo)).LocalIndex;

                var coverBlockChanceInfo = AccessTools.Method(typeof(CalculateOverallBlockChance), nameof(CalculateOverallBlockChance.CoverBlockChance));
                //var getBlockChanceInfo = AccessTools.Property(typeof(CoverInfo), nameof(CoverInfo.BlockChance)).GetGetMethod();

                for (int i = 0; i < instructionList.Count; i++)
                {
                    var instruction = instructionList[i];

                    // Look for the section that adds to BlockChance
                    if (instruction.opcode == OpCodes.Ldloc_1)
                    {
                        #if DEBUG
                            Log.Message("CoverUtility.CalculateOverallBlockChance match 1 of 2");
                        #endif

                        bool instructionMatch = false;
                        int j = 1;
                        while (i + j < instructionList.Count)
                        {
                            var futureInstruction = instructionList[i + j];
                            if (futureInstruction.opcode == OpCodes.Stloc_1)
                            {
                                var previousFutureInstruction = instructionList[i + j - 1];
                                if (previousFutureInstruction.opcode == OpCodes.Add)
                                    instructionMatch = true;
                                break;
                            }
                            j++;
                        }

                        // Replace old multiplicative block with new additive block
                        if (instructionMatch)
                        {
                            #if DEBUG
                                Log.Message("CoverUtility.CalculateOverallBlockChance match 2 of 2");
                            #endif

                            // Nop out block otherwise problems arise
                            for (int k = 1; k < j; k++)
                                instructionList.RemoveAt(i + 1);

                            yield return instruction; // num
                            yield return new CodeInstruction(OpCodes.Ldloc_S, coverInfoLocalIndex); // coverInfo
                            yield return new CodeInstruction(OpCodes.Call, coverBlockChanceInfo); // CoverBlockChance(coverInfo)
                            instruction = new CodeInstruction(OpCodes.Add); // num + CoverBlockChance(coverInfo)
                        }
                    }

                    yield return instruction;
                }
            }

            private static float CoverBlockChance(CoverInfo coverInfo)
            {
                // Some weird hack to get coverInfo.BlockChance to return properly
                float blockChance = coverInfo.BlockChance;
                blockChance += 0; // this part specifically
                return blockChance;
            }

        }

        [HarmonyPatch(typeof(CoverUtility), nameof(CoverUtility.CalculateCoverGiverSet))]
        public static class CalculateCoverGiverSet
        {

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                #if DEBUG
                    Log.Message("Transpiler start: CoverUtility.CalculateCoverGiverSet (2 matches)");
                #endif

                var instructionList = instructions.ToList();

                var getBlockChanceInfo = AccessTools.Property(typeof(CoverInfo), nameof(CoverInfo.BlockChance)).GetGetMethod();

                for (int i = 0; i < instructionList.Count; i++)
                {
                    var instruction = instructionList[i];

                    // Look for greater-than comparisons with coverInfo.BlockChance and 0 - change this to equal-to comparisons
                    if (instruction.opcode == OpCodes.Ble_Un_S)
                    {
                        #if DEBUG
                            Log.Message("CoverUtility.CalculateCoverGiverSet match 1 of 2");
                        #endif

                        // Check previous instructions - look for coverInfo.BlockChance
                        for (int j = -1; i + j >= 0; j--)
                        {
                            var instructionBehind = instructionList[i + j];
                            if (instructionBehind.opcode == OpCodes.Call || instructionBehind.opcode == OpCodes.Callvirt ||
                                instructionBehind.opcode == OpCodes.Ldfld || instructionBehind.opcode == OpCodes.Ldflda)
                            {
                                #if DEBUG
                                    Log.Message("CoverUtility.CalculateCoverGiverSet match 2 of 2");
                                #endif

                                if (instructionBehind.OperandIs(getBlockChanceInfo))
                                    instruction.opcode = OpCodes.Beq;
                                break;
                            }
                        }

                    }

                    yield return instruction;
                }
            }

        }



    }

}
