using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Character.Enemy.Normal.AttackLogic.AttackState;
using Enemy;
using UnityEngine;

namespace Character.Enemy.Normal.AttackLogic
{
    public class EnemyAttackLogic
    {
        private List<EnemyAttackState> attackStates;
        private List<EnemyAttackState> ableAttackStates = new List<EnemyAttackState>();

        public EnemyAttackLogic(List<EnemyAttackState> attackStates)
        {
            this.attackStates = attackStates;
        }
        public virtual IEnumerator Attack(EnemyController enemyController)
        {
            var nextAttackState = ableAttackStates[Random.Range(1, ableAttackStates.Count)];
            yield return nextAttackState.Attack(enemyController);
        }
        public bool CanAttack(EnemyController enemyController)
        {
            return ableAttackStates.Count != 0;
        }

        public void SetAbleAttackStates(EnemyController enemyController)
        {
            ableAttackStates.Clear();
            foreach (var attackState in attackStates.Where(attackState => attackState.CanAttack(enemyController)))
            {
                ableAttackStates.Add(attackState);
            }
        }
    }
}