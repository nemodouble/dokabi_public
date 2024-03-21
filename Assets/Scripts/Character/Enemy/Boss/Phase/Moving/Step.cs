using System.Collections;
using UnityEngine;

namespace Boss.Phase.Moving
{
    public class Step : BossPhase
    {
        private Vector2 relativePos;
        private float maxSpeed;
        private float accel;
        private float decelLengthRate;
        
        public Step(string phaseName,Vector2 relativePos,float maxSpeed, float accel, float decelLengthRate) : base(phaseName)
        {
            this.relativePos = relativePos;
            this.maxSpeed = maxSpeed;
            this.accel = accel;
            this.decelLengthRate = decelLengthRate;
        }

        protected internal override IEnumerator DoPhase(BossController bossController)
        {
            var startingPos = (Vector2)bossController.transform.position;
            var targetPos = startingPos + relativePos;
            var bossPos = startingPos;
            
            while ((targetPos - bossPos).magnitude > 0.1f)
            {
                bossPos = bossController.transform.position;
                var leftLength = (targetPos - bossPos).magnitude;
                
                // 벽 정지
                if (bossController.IsHeading(targetPos - bossPos, 0.1f))
                {
                    break;
                }
                
                // 감속
                if (leftLength < relativePos.magnitude * decelLengthRate)
                {
                    bossController.Rigid2D.velocity = maxSpeed * leftLength / relativePos.magnitude * (targetPos - bossPos).normalized;
                    if (bossController.Rigid2D.velocity == Vector2.zero)
                        break;
                }
                // 가속
                else
                {
                    var force = (maxSpeed - bossController.Rigid2D.velocity.magnitude) * accel *
                                (targetPos - bossPos).normalized;
                    // bossController.Rigid2D.AddForce(force);
                    bossController.Rigid2D.velocity = (targetPos - bossPos) * 10;
                }
                yield return null;
            }
        }
    }
}