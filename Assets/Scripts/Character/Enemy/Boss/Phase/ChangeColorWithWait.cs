using System.Collections;
using UnityEngine;

namespace Boss.Phase
{
    // ReSharper disable once InconsistentNaming
    public class ChangeColorWithWait : BossPhase
    {
        private readonly Color color;
        private readonly float time;
        private readonly bool reChangeAfterTime;

        public ChangeColorWithWait(string phaseName, Color color, float time, bool reChangeAfterTime = true) : base(phaseName)
        {
            this.color = color;
            this.time = time;
            this.reChangeAfterTime = reChangeAfterTime;
        }

        protected internal override IEnumerator DoPhase(BossController bossController)
        {
            var spriteRenderer = bossController.GetComponent<SpriteRenderer>();
            var timeCur = 0f;
            while (timeCur < time)
            {
                spriteRenderer.color = color;
                timeCur += Time.deltaTime;
                yield return null;
            }
            if(reChangeAfterTime)
                spriteRenderer.color = Color.white;
        }
    }
}