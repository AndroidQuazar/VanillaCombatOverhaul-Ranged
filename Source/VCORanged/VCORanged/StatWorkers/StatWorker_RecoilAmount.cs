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

    public class StatWorker_RecoilAmount : StatWorker
    {

        public override bool ShouldShowFor(StatRequest req)
        {
            // Only Show for weapons that have a burst count of greater than 1
            return VCORangedSettings.weaponRecoil && base.ShouldShowFor(req) && req.Def is ThingDef tDef && tDef.IsRangedWeapon && tDef.Verbs.Any(v => v.burstShotCount > 1);
        }

    }

}
