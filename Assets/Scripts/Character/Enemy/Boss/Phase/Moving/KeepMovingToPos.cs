using System.Collections;
using UnityEngine;
using Util;

namespace Boss.Phase.Moving
{
    public class KeepMovingToPos : BossPhase
    {
        private readonly Vector2 movingPos;
        private readonly float movingAccel;
        private readonly float movingTimeMax;
        private readonly float movingSpeedMax;
        private readonly bool movingEnd;
        
        public KeepMovingToPos(string phaseName, Vector2 movingPos, float movingAccel, float movingTime = -1f, float movingSpeedMax = 0f, bool movingEnd = false) : base(phaseName)
        {
            this.movingPos = movingPos;
            this.movingAccel = movingAccel;
            movingTimeMax = movingTime == -1f ? float.MaxValue : movingTime;
            this.movingSpeedMax = movingSpeedMax == 0f ? 200 : movingSpeedMax;
            this.movingEnd = movingEnd;
        }

        protected internal override IEnumerator DoPhase(BossController bossController)
        {
            var movingTimeCur = 0f;
            while (movingTimeCur < movingTimeMax)
            {
                movingTimeCur += Time.deltaTime;
                // 목표 방향
                var targetVelocity = (movingPos - (Vector2)bossController.transform.position).normalized * movingSpeedMax;
                
                // 현 속력에 가해줘야할 힘
                Vector2 forceDir = (targetVelocity - bossController.Rigid2D.velocity) * movingAccel;
                
                // 최대 속도 제한
                if (Util.Debugger.GetVector2Length(forceDir) > movingSpeedMax)
                {
                    forceDir = forceDir.normalized * movingSpeedMax;
                }
                
                // 도달시 종료
                if (movingEnd && (movingPos - (Vector2) bossController.transform.position).magnitude < 0.05f)
                    movingTimeCur = movingTimeMax;
                
                bossController.Rigid2D.AddForce(forceDir);
                Debug.DrawRay(bossController.transform.position,forceDir);
                yield return null;
            }
        }
    }
}