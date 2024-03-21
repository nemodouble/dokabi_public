using System.Collections;
using Character.Enemy.Normal;

namespace Enemy.DeadLogic.EnemyDeadState
{
    public abstract class EnemyDeadState
    {
        public abstract IEnumerator Dead(EnemyController enemyController);
    }
}