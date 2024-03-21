using System.Collections;
using UnityEngine;

namespace Boss.Phase.Moving
{
    public class MoveToRelativePos : BossPhase
    {
        private readonly GameObject targetObject;
        private readonly Vector2 relativeVector;
        private readonly float movingSpeed;
        private readonly float movingTimeMax;
        public MoveToRelativePos(string phaseName, GameObject targetObject, Vector2 relativeVector, float movingSpeed, float movingTime) : base(phaseName)
        {
            this.targetObject = targetObject;
            this.relativeVector = relativeVector;
            this.movingSpeed = movingSpeed;
            movingTimeMax = movingTime;
        }

        protected internal override IEnumerator DoPhase(BossController bossController)
        {
            var movingTimeCur = 0f;
            while (movingTimeCur <= movingTimeMax)
            {
                movingTimeCur += Time.deltaTime;
                var targetPos = (Vector2)targetObject.transform.position + relativeVector;
                var boosPosition = bossController.transform.position;
                var forceDir = targetPos - (Vector2)boosPosition;
                var force = forceDir.normalized * movingSpeed - bossController.Rigid2D.velocity;
                
                bossController.Rigid2D.AddForce(force);
                Debug.DrawRay(boosPosition,force);

                yield return null;
            }
        }
    }
}