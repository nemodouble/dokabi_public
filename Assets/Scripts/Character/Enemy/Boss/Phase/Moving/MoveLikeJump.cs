using System.Collections;
using Boss.Phase;
using UnityEngine;

namespace Boss.MaeHwa
{
    public class MoveLikeJump : BossPhase
    {
        private readonly float startSpeed;
        private readonly float jumpTime;
        
        public MoveLikeJump(string phaseName, float startSpeed, float jumpTime, int rarity = 1) : base(phaseName, rarity)
        {
            this.startSpeed = startSpeed;
            this.jumpTime = jumpTime;
        }

        protected internal override IEnumerator DoPhase(BossController bossController)
        {
            var jumpTimeNow = 0f;
            while (jumpTimeNow <= jumpTime)
            {
                jumpTimeNow += Time.deltaTime;
                bossController.Rigid2D.velocity = new Vector2(bossController.Rigid2D.velocity.x,
                    startSpeed - (jumpTimeNow / jumpTime) * startSpeed);
                yield return null;
            }
        }
    }
}