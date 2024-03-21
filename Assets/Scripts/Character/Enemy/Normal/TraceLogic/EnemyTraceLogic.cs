using Character.Enemy.Normal;
using UnityEngine;

namespace Enemy.TraceLogic
{
    public abstract class EnemyTraceLogic
    {
        public abstract Vector2 GetTraceDir(EnemyController enemyController);
        public abstract void DrawDebug(EnemyController enemyController);
    }
}
