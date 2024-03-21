using Character.Enemy.Normal;
using UnityEngine;

namespace Enemy.IdleLogic
{
    public abstract class EnemyIdleLogic
    {
        public abstract Vector2 GetIdleDir(EnemyController enemyController);
        public abstract void InitIdleState(EnemyController enemyController);

        public abstract void DrawDebug(EnemyController enemyController);
    }
}
