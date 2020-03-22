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

    public class FiringModeDef : Def
    {

        public override void PostLoad()
        {
            base.PostLoad();
            LongEventHandler.ExecuteWhenFinished(() => uiIcon = ContentFinder<Texture2D>.Get(uiIconTexPath));
        }

        public int AdjustedBurstShotCount(int burstShotCount)
        {
            return Mathf.Clamp(Mathf.RoundToInt(burstShotCount * burstShotCountFactor), minBurstShotCount, maxBurstShotCount);
        }

        public bool Allows(IAttackTargetSearcher searcher)
        {
            if (searcher.CurrentEffectiveVerb is Verb_LaunchProjectile verb && verb.EquipmentSource != null)
            {
                var highestBurstShotCountVerb = verb.EquipmentSource.def.Verbs.MaxBy(v => v.burstShotCount);
                return highestBurstShotCountVerb.burstShotCount >= minBurstShotCount;
            }

            return false;
        }

        public int minBurstShotCount = 1;
        public int maxBurstShotCount = Int32.MaxValue;
        public float burstShotCountFactor = 1;
        public string uiIconTexPath;
        public int displayOrder;

        [Unsaved]
        public Texture2D uiIcon = BaseContent.BadTex;

    }

}
