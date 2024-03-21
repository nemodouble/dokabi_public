using Character.Enemy.Normal.AttackLogic;
using Enemy;
using Enemy.StateCheckLogic.DetectLogic;

namespace Character.Enemy.Normal.StateCheckLogic
{
    public class EnemyStateSelectLogic
    {
        private EnemyDetectLogic traceDetectLogic;
        private EnemyAttackLogic attackLogic;

        private bool canTrace;
        private bool canAttack;
        private AttackAbleDir attackAbleDir;

        private float unDetectTimeCur;
        private float unDetectTimeMax;
        
        public enum AttackAbleDir
        {
            Up,
            Side,
            UpAndSide,
            None
        }

        public EnemyStateSelectLogic(EnemyDetectLogic traceDetectLogic, EnemyAttackLogic attackLogic, float unDetectTimeMax)
        {
            this.traceDetectLogic = traceDetectLogic;
            this.attackLogic = attackLogic;
            this.unDetectTimeMax = unDetectTimeMax;
        }

        private void DetectUpdate(EnemyController enemyController)
        {
            canTrace = traceDetectLogic.CanDetect(enemyController);
            attackLogic.SetAbleAttackStates(enemyController);
            canAttack = attackLogic.CanAttack(enemyController);
        }
        public EnemyController.EnemyState GetEnemyState(EnemyController enemyController)
        {
            DetectUpdate(enemyController);
            if (enemyController.isDead)
                return EnemyController.EnemyState.Dead;
            if (canAttack)
            {
                return EnemyController.EnemyState.Attack;
            }
            
            if (canTrace)
            {
                unDetectTimeCur = 0;
                return EnemyController.EnemyState.Trace;
            }
            if (enemyController.curState == EnemyController.EnemyState.Trace)
            {
                unDetectTimeCur += enemyController.stateCheckTime;
                if (unDetectTimeCur > unDetectTimeMax)
                {
                    return EnemyController.EnemyState.Idle;
                }
                return EnemyController.EnemyState.Trace;
            }
            return EnemyController.EnemyState.Idle;
        }
    }
}
