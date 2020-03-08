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
    public static class NonPublicMethods
    {

        #region CoverUtility
        // Had no luck with delegates so this is the best alternative I could come up with

        public static TryFindAdjustedCoverInCell<IntVec3, LocalTargetInfo, IntVec3, Map, CoverInfo> CoverUtility_TryFindAdjustedCoverInCell =
            (TryFindAdjustedCoverInCell<IntVec3, LocalTargetInfo, IntVec3, Map, CoverInfo>)
            Delegate.CreateDelegate(typeof(TryFindAdjustedCoverInCell<IntVec3, LocalTargetInfo, IntVec3, Map, CoverInfo>), AccessTools.Method(typeof(CoverUtility), "TryFindAdjustedCoverInCell"));
        #endregion

        public delegate bool TryFindAdjustedCoverInCell<A, B, C, D, E>(A first, B second, C third, D fourth, out E fifth);

    }

}
