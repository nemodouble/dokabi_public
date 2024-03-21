using System.Collections;
using UnityEngine;

namespace Boss.Phase.Moving
{
    public class MoveStraight : BossPhase
    {
        public enum WallDirChangeType
        {
            Stop,
            Reflection,
            JustGo
        }
        public const int Stop = 0;
        public const int Reflection = 1;

        private readonly Vector2 targetPos;
        private readonly float movingSpeed;
        private readonly bool isFallWhileMoving;
        private readonly float movingTime;
        private readonly WallDirChangeType wallDirChangeType;
        private readonly bool stopWhenMovingEnd;

        public MoveStraight(string phaseName, Vector2 targetPos, float movingSpeed, bool isFallWhileMoving = false,
            WallDirChangeType wallDirChangeType = Stop, float movingTime = -1f, bool stopWhenMovingEnd = true) : base(phaseName)
        {
            this.targetPos = targetPos;
            this.movingSpeed = movingSpeed;
            this.isFallWhileMoving = isFallWhileMoving;
            this.movingTime = movingTime;
            this.wallDirChangeType = wallDirChangeType;
            this.stopWhenMovingEnd = stopWhenMovingEnd;
        }

        protected internal override IEnumerator DoPhase(BossController bossController)
        {
            // 무중력 이동
            var originGravity = bossController.Rigid2D.gravityScale;
            if (!isFallWhileMoving)
                bossController.Rigid2D.gravityScale = 0;
            
            // 이동 방향 설정
            Vector2 bossPos = bossController.transform.position;
            var forceDir = (targetPos - bossPos).normalized * movingSpeed;
            
            // 이동 루프 종료 조건
            var targetDistance = Mathf.Sqrt((targetPos.x - bossPos.x) * (targetPos.x - bossPos.x) +
                                            (targetPos.y - bossPos.y) * (targetPos.y - bossPos.y));
            var movingTimeCur = 0f;
            
            // 이동 전 속력 초기화
            bossController.Rigid2D.velocity = Vector2.zero;
            
            // 이동
            while(Mathf.Abs(targetDistance) >= 1f)
            {
                var raycastHit2D = bossController.IsHeading(forceDir, 0.01f);
                // 벽 충돌시 방향 변경
                if (raycastHit2D.collider != null)
                {
                    switch (wallDirChangeType)
                    {
                        case WallDirChangeType.Reflection:
                            forceDir = raycastHit2D.normal.normalized * movingSpeed;
                            break;
                        case WallDirChangeType.Stop:
                            forceDir = Vector2.zero;
                            bossController.Rigid2D.velocity = Vector2.zero;
                            movingTimeCur = movingTime;
                            break;
                        case WallDirChangeType.JustGo:
                            break;
                    }
                }
                Debug.DrawRay(bossController.transform.position, forceDir, Color.black);
                bossController.Rigid2D.velocity = forceDir;
                if (movingTimeCur != -1f)
                {
                    movingTimeCur += Time.deltaTime;
                    if (movingTimeCur > movingTime) break;
                }
                yield return null;
            }

            if (stopWhenMovingEnd)
                bossController.Rigid2D.velocity = Vector2.zero;
            if (!isFallWhileMoving)
                bossController.Rigid2D.gravityScale = originGravity;
        }
    }
}