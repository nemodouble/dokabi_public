using Character.Enemy.Normal;
using UnityEngine;

namespace Enemy.IdleLogic
{
    public class FlyIdleLogic : EnemyIdleLogic
    {
        private readonly float flyingSpeed;

        private Vector2 centerPos;
        private readonly Vector2 moveAbleSize;
        private Vector2 dir;
        private readonly float maxMoveDirChangeTime;
        private float curMoveDirChangeTime = float.MaxValue;

        private readonly bool returnToOriginPos;

        public FlyIdleLogic(float flyingSpeed, Vector2 centerPos, Vector2 idleMoveSize, float movingTime, bool returnToOriginPos)
        {
            this.flyingSpeed = flyingSpeed;
            this.centerPos = centerPos;
            this.moveAbleSize = idleMoveSize;
            this.maxMoveDirChangeTime = movingTime;
            this.returnToOriginPos = returnToOriginPos;
        }
        public override Vector2 GetIdleDir(EnemyController enemyController)
        {
            if (curMoveDirChangeTime > maxMoveDirChangeTime)
            {
                dir = GetRandomDir() * flyingSpeed;
                curMoveDirChangeTime = 0;
            }
            else
            {
                curMoveDirChangeTime += Time.deltaTime;
            }
            if (enemyController.IsHeading(dir))
                dir = GetRandomDir() * flyingSpeed;
            else if (IsOverMoveSize(enemyController))
            {
                dir = GetRandomDir() * flyingSpeed;
                curMoveDirChangeTime = 0;
            }
            return dir;
        }

        public override void InitIdleState(EnemyController enemyController)
        {
            enemyController.rigid2D.gravityScale = 0;
            if (!returnToOriginPos)
                SetCenterPos(enemyController.transform.position);
        }

        public override void DrawDebug(EnemyController enemyController)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(centerPos,moveAbleSize);
            Gizmos.color = Color.white;
        }

        private bool IsOverMoveSize(EnemyController enemyController)
        {
            return Mathf.Abs(enemyController.rigid2D.position.x + dir.x - centerPos.x) >= +(moveAbleSize.x / 2) ||
                   Mathf.Abs(enemyController.rigid2D.position.y + dir.y - centerPos.y) >= +(moveAbleSize.y / 2);
        }

        private static Vector2 GetRandomDir()
        {
            var randomAngle = Random.Range(0, 2f * Mathf.PI);
            var randDir = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
            return randDir;
        }

        private void SetCenterPos(Vector2 centerPos)
        {
            this.centerPos = centerPos;
        }
    }
}
