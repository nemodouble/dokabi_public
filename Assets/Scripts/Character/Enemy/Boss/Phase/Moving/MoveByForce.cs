using System.Collections;
using UnityEngine;

namespace Boss.Phase.Moving
{
    public class MoveByForce :BossPhase
    {
        private readonly Vector2 forceDir;
        private readonly float forcePower;
        private readonly float time;
        public MoveByForce(string phaseName, Vector2 forceDir, float forcePower = 0, float time = 0) : base(phaseName)
        {
            this.forceDir = forceDir;
            this.forcePower = forcePower == 0 ? forceDir.magnitude : forcePower;
            this.time = time;
        }

        protected internal override IEnumerator DoPhase(BossController bossController)
        {
            var timeCur = 0f;
            while (timeCur <= time)
            {
                Debug.DrawRay(bossController.transform.position, forceDir.normalized * forcePower * Time.deltaTime);
                timeCur += Time.deltaTime;
                bossController.Rigid2D.AddForce(forceDir.normalized * forcePower * Time.deltaTime);
                yield return null;
            }
        }
    }
}