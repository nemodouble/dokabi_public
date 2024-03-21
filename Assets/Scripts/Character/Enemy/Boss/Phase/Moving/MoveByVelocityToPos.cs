using Boss.Phase;
using UnityEngine;

namespace Mechanics.Boss.Phase.Moving
{
    public class MoveByVelocityToPos : MoveByVelocity
    {
        public MoveByVelocityToPos(string phaseName, Vector2 dir, float velocity, float timeMax, Vector2 targetPos, float length = 0) : base(phaseName, dir, velocity, timeMax, length)
        {
            this.targetPos = targetPos;
            haveTargetPos = true;
            Debug.DrawRay((Vector3)targetPos,Vector2.up,Color.blue,3f);
        }
    }
}