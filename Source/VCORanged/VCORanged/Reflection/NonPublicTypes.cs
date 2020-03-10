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

    [StaticConstructorOnStartup]
    public static class NonPublicTypes
    {

        [StaticConstructorOnStartup]
        public static class VanillaFurnitureExpandedSecurity
        {

            static VanillaFurnitureExpandedSecurity()
            {

                if (ModCompatibilityCheck.VanillaFurnitureExpandedSecurity)
                {
                    CompThingTracker = GenTypes.GetTypeInAnyAssembly("VFESecurity.CompThingTracker", "VFESecurity");
                }

            }

            public static Type CompThingTracker;

        }

    }

}