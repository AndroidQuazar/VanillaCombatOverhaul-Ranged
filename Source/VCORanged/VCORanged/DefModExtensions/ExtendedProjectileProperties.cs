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

    public class ExtendedProjectileProperties : DefModExtension
    {

        public const string PelletTexPathStandard = "Things/Projectile/Pellet_Shotgun";
        public const string PelletTexPathAdvanced = "Things/Projectile/Pellet_ChargeShotgun";

        private static readonly ExtendedProjectileProperties defaultValues = new ExtendedProjectileProperties();

        public static ExtendedProjectileProperties Get(Def def) => def.GetModExtension<ExtendedProjectileProperties>() ?? defaultValues;

        public override IEnumerable<string> ConfigErrors()
        {
            // Used in place of ResolveReferences since DefModExtension doesn't yet have an override for it
            if (shotgunPelletGraphicData == null)
            {
                shotgunPelletGraphicData = new GraphicData()
                {
                    graphicClass = typeof(Graphic_Single)
                };
            }

            return base.ConfigErrors();
        }

        public Graphic PelletGraphic => shotgunPelletGraphicData.Graphic;

        public int shotgunPelletCount = 6;
        public GraphicData shotgunPelletGraphicData;

    }

}
