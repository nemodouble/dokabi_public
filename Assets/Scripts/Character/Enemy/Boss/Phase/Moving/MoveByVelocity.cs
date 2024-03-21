using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Boss.Phase
{
    public class MoveByVelocity : BossPhase
    {
        private readonly float velocity;
        private readonly Vector2 dir;
        private readonly float timeMax;
        protected float length;
        protected bool haveTargetPos = true;
        
        private float timeNow;
        protected Vector3? targetPos = null;
        public MoveByVelocity(string phaseName, Vector2 dir, float velocity, float timeMax, float length = 0) : base(phaseName)
        {
            this.velocity = velocity;
            this.dir = dir;
            this.timeMax = timeMax;
            if (length != 0)
                this.length = length;
            else
                haveTargetPos = false;
        }

        protected internal override IEnumerator DoPhase(BossController bossController)
        {
            var startPos = bossController.transform.position;
            // 최대 길이가 주어진 경우 목표지점 할당
            if(targetPos==null && haveTargetPos)
            {
                targetPos = (Vector2) startPos + dir.normalized * length;
            }
            // 목표 지점이 주어진 경우 최대길이 할당
            else if (targetPos != null && length == 0)
            {
                length = ((Vector3)targetPos - startPos).magnitude;
            }
            
            
            timeNow = 0;
            while (timeNow < timeMax)
            {
                timeNow += Time.deltaTime;
                if(targetPos != null && (bossController.transform.position - startPos).magnitude > length)
                {
                    bossController.Rigid2D.position = (Vector2) targetPos;
                    break;
                }
                bossController.Rigid2D.velocity = dir * velocity;
                yield return null;
            }
        }
    }
}