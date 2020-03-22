using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;
using HarmonyLib;

namespace VCORanged
{

    public class StatPart_WeaponRecoil : StatPart
    {

        public override string ExplanationPart(StatRequest req)
        {
            if (req.HasThing)
            {
                float recoilOffset = RecoilOffset(req.Thing);
                if (recoilOffset != 0)
                    return $"{"VCO.RangedModule.StatsReport_WeaponRecoil".Translate()}: {recoilOffset.ToStringByStyle(parentStat.toStringStyle, ToStringNumberSense.Offset)}";
            }
            return null;
        }

        public override void TransformValue(StatRequest req, ref float val)
        {
            // Adjust value based on recoil and burst shots left
            if (req.HasThing)
            {
                val += RecoilOffset(req.Thing);
            }
        }

        private float RecoilOffset(Thing thing)
        {
            // Determine recoil amount based on how many shots in a burst have been done so far
            if (VCORangedSettings.weaponRecoil && thing is IAttackTargetSearcher searcher)
            {
                var curVerb = searcher.CurrentEffectiveVerb;
                if (curVerb != null && curVerb.Bursting && curVerb.EquipmentSource is ThingWithComps eq)
                {
                    int burstShotsDone = NonPublicProperties.Verb_get_ShotsPerBurst(curVerb) - (int)NonPublicFields.Verb_burstShotsLeft.GetValue(curVerb);
                    return -1 * burstShotsDone * eq.GetStatValue(StatDefOf.VCOR_RecoilAmount);
                }
            }
            return 0;
        }

    }

}
