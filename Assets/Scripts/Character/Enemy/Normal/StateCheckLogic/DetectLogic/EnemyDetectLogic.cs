using Character.Enemy.Normal;

namespace Enemy.StateCheckLogic.DetectLogic
{
    public abstract class EnemyDetectLogic
    {
        public abstract bool CanDetect(EnemyController enemyController);
        public abstract void DrawDebug(EnemyController enemyController);
    }
}