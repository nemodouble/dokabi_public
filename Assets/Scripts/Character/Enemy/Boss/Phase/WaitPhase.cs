using System.Collections;
using UnityEngine;

namespace Boss.Phase
{
    public class WaitPhase : BossPhase
    {
        private readonly float waitingSecond;
        private readonly bool notMoving;

        public WaitPhase(string phaseName, float waitingSecond, bool notMoving = false) : base(phaseName)
        {
            this.waitingSecond = waitingSecond;
            this.notMoving = notMoving;
        }

        protected internal override IEnumerator DoPhase(BossController bossController)
        {
            if(notMoving)
            {
                var waitingSecondCur = 0f;
                while (waitingSecondCur < waitingSecond)
                {
                    waitingSecondCur += Time.deltaTime;
                    bossController.Rigid2D.velocity = Vector2.zero;
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(waitingSecond);    
            }
            
        }
    }
}