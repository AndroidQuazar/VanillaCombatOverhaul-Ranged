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
                float recoilOffset = FinalRecoilOffset(req.Thing);
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
                val += FinalRecoilOffset(req.Thing);
            }
        }

        private float FinalRecoilOffset(Thing reqThing)
        {
            float offset = 0;

            // Determine recoil amount based on how many shots in a burst have been done so far
            if (VCORangedSettings.weaponRecoil && reqThing is IAttackTargetSearcher searcher)
            {
                var curVerb = searcher.CurrentEffectiveVerb;
                offset = RecoilOffset(curVerb);

                // Dual wield - factor in off-hand verb
                if (ModActive.DualWield && searcher is Pawn pawn)
                {
                    var offhandVerb = NonPublicMethods.DualWield.Ext_Pawn_TryGetOffhandAttackVerb(pawn, pawn.LastAttackedTarget.Thing, true);
                    offset = Mathf.Min(offset, RecoilOffset(offhandVerb));
                }
            }
            return offset;
        }

        private float RecoilOffset(Verb verb)
        {
            if (verb != null && verb.Bursting && verb.EquipmentSource is ThingWithComps eq)
            {
                int burstShotsDone = NonPublicProperties.Verb_get_ShotsPerBurst(verb) - (int)NonPublicFields.Verb_burstShotsLeft.GetValue(verb);
                return -1 * burstShotsDone * eq.GetStatValue(StatDefOf.VCOR_RecoilAmount);
            }
            return 0;
        }

    }

}
