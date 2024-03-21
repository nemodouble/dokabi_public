using System;
using Character.Enemy.Normal;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy.IdleLogic
{
    public class WalkIdleLogic : EnemyIdleLogic
    {
        private Vector2 originPos;
        private readonly float movingLength;
        private int xDir = 1;
        private bool isMoving;
        private readonly float movingSpeed;
        private readonly float movingTime;
        private float movingTimeCur;
        private float movingTimeMax;
        private readonly float stopTime;
        private float stopTimeCur;
        private float stopTimeMax;
        private readonly bool isRandomDirChange;
        private float randomTime;

        internal WalkIdleLogic(float movingSpeed, float movingTime = 1, float stopTime = 0,float movingLength = -1f, float randomTime = 0 ,bool isRandomDirChange = false)
        {
            this.randomTime = randomTime;
            this.movingSpeed = movingSpeed;
            this.movingTime = movingTime;
            this.stopTime = stopTime;
            movingTimeMax = movingTime + Random.Range(0, randomTime) - randomTime / 2;
            stopTimeMax = stopTime + Random.Range(0, randomTime) - randomTime / 2;
            this.isRandomDirChange = isRandomDirChange;
            this.movingLength = movingLength;
        }
        public override Vector2 GetIdleDir(EnemyController enemyController)
        {
            Vector2 idleDir = new Vector2(0, enemyController.rigid2D.velocity.y);
            var headingCheckLength = enemyController.boxCollider.size.x / 2;
            if (enemyController.IsHeading(Vector2.right * xDir * headingCheckLength))
                xDir *= -1;
            if (isMoving)
            {
                movingTimeCur += Time.deltaTime;
                if (movingTimeCur >= movingTimeMax)
                {
                    movingTimeCur = 0;
                    movingTimeMax = movingTime + Random.Range(0, randomTime) - randomTime / 2;
                    isMoving = false;
                }
                if (!enemyController.IsOnPlatform(xDir) ||
                    Math.Abs(enemyController.transform.position.x + xDir * enemyController.boxCollider.size.x/2 - originPos.x) >= movingLength)
                    xDir *= -1;
                idleDir.x = xDir * movingSpeed;
                return idleDir;
            } 
            stopTimeCur += Time.deltaTime;
            if (stopTimeCur >= stopTimeMax)
            {
                stopTimeCur = 0;
                isMoving = true;
                stopTimeMax = stopTime + Random.Range(0, randomTime) - randomTime / 2;
                if (isRandomDirChange)
                    xDir = (Random.Range(0, 2) * 2) - 1;
            }
            return idleDir;
        }

        public override void InitIdleState(EnemyController enemyController)
        {
            originPos = enemyController.transform.position;
            enemyController.rigid2D.gravityScale = enemyController.originGravityScale;
        }

        public override void DrawDebug(EnemyController enemyController)
        {
            throw new System.NotImplementedException();
        }
        
    }
}
