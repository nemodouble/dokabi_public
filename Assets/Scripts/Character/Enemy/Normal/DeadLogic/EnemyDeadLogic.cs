using System.Collections;
using System.Collections.Generic;
using Enemy;

namespace Character.Enemy.Normal.DeadLogic
{
    public class EnemyDeadLogic
    {
        private readonly Queue<global::Enemy.DeadLogic.EnemyDeadState.EnemyDeadState> deadStates;

        public EnemyDeadLogic(Queue<global::Enemy.DeadLogic.EnemyDeadState.EnemyDeadState> queue)
        {
            deadStates = queue;
        }

        public IEnumerator Dead(EnemyController enemyController)
        {
            foreach (var deadState in deadStates)
            {
                yield return deadState.Dead(enemyController);
            }
        }
    }
}