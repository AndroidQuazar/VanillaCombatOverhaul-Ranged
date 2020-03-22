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

    [DefOf]
    public static class FiringModeDefOf
    {

        public static FiringModeDef VCOR_SingleShot;
        public static FiringModeDef VCOR_FullAuto;

    }

}
