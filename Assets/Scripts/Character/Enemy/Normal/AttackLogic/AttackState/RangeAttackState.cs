using System;
using System.Collections;
using Character.Player;
using Enemy.StateCheckLogic.DetectLogic;
using UnityEngine;

namespace Character.Enemy.Normal.AttackLogic.AttackState
{
    public class RangeAttackState : EnemyAttackState
    {
        private readonly Vector2 attackPos;
        private readonly Vector2 attackSize;

        public RangeAttackState(EnemyDetectLogic attackDetectLogic, Vector2 attackPos, Vector2 attackSize) : base(attackDetectLogic)
        {
            this.attackPos = attackPos;
            this.attackSize = attackSize;
        }
        public override IEnumerator Attack(EnemyController enemyController)
        {
            throw new NotImplementedException();
            // var attackDir = PlayerController.GetPosToPlayerDir(enemyController.transform.position);
            // yield return AttackType.RangeAttack(attackPos, attackSize, attackDir);
        }
    }
}