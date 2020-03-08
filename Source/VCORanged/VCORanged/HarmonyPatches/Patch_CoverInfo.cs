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

    public static class Patch_CoverInfo
    {

        [HarmonyPatch(typeof(CoverInfo), nameof(CoverInfo.BlockChance), MethodType.Getter)]
        public static class get_BlockChance
        {

            public static void Postfix(ref float __result)
            {
                // Use score system
                __result *= VCORangedTuning.AccuracyScoreCover;
            }
        
        }


    }

}
