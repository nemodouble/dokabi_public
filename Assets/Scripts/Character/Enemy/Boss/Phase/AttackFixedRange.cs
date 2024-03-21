using System.Collections;
using UnityEngine;

namespace Boss.Phase
{
    public class AttackFixedRange : BossPhase
    {
        private readonly GameObject attackRange;
        
        public AttackFixedRange(string phaseName, GameObject attackRange, int rarity = 1) : base(phaseName, rarity)
        {
            this.attackRange = attackRange;
        }

        protected internal override IEnumerator DoPhase(BossController bossController)
        {
            attackRange.SetActive(true);
            yield break;
        }
    }
}