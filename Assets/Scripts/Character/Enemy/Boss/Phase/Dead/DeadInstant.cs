using System.Collections;
using UnityEngine;

namespace Boss.Phase
{
    public class DeadInstant : BossPhase
    {
        private readonly float deadDelay;
        public DeadInstant(string phaseName, float deadDelay = 0) : base(phaseName)
        {
            this.deadDelay = deadDelay;
        }
        protected internal override IEnumerator DoPhase(BossController bossController)
        {
            var deadAnimTime = 0f;
            var runtimeAnimatorController = bossController.Animator.runtimeAnimatorController;
            foreach (var animationClip in runtimeAnimatorController.animationClips)
            {
                if (animationClip.name == "Dead")
                    deadAnimTime = animationClip.length;
            }
            yield return new WaitForSeconds(deadAnimTime);
            bossController.gameObject.layer = LayerMask.NameToLayer("Dummy");
            bossController.gameObject.tag = "Dummy";
            var spriteRenderer = bossController.GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.gray;
        }

    }
}