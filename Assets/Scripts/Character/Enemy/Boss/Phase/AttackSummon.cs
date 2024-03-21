using System.Collections;
using UnityEngine;

namespace Boss.Phase
{
    public class AttackSummon : BossPhase
    {
        private readonly GameObject gameObject;
        private readonly Vector2 relativePos;
        
        public AttackSummon(string phaseName, GameObject gameObject, Vector2? relativePos = null) : base(phaseName)
        {
            this.gameObject = gameObject;
            if(relativePos != null)
                this.relativePos = (Vector2)relativePos;
            else
                this.relativePos = Vector2.zero;
        }

        protected internal override IEnumerator DoPhase(BossController bossController)
        {
            bossController.CallInstantiate(gameObject, relativePos);
            yield return null;
        }
    }
}