using Character.Enemy.Normal;
using UnityEngine;
using Util;

namespace Enemy.StateCheckLogic.DetectLogic
{
    public class CircleDetectLogic : EnemyDetectLogic
    {
        private readonly float detectDistance;

        public CircleDetectLogic(float detectDistance)
        {
            this.detectDistance = detectDistance;
        }

        public override bool CanDetect(EnemyController enemyController)
        {
            var enemyPosition = enemyController.transform.position;
            var playerPosition = enemyController.player.transform.position;
            return Vector2.Distance(playerPosition, enemyPosition) < detectDistance 
                   && CanLookPlayer(enemyPosition, playerPosition);
        }

        public override void DrawDebug(EnemyController enemyController)
        {
            var enemyPosition = enemyController.transform.position;
            var playerPosition = enemyController.player.transform.position;
            Util.Debugger.DrawGizmosCircleXZ(enemyPosition, detectDistance);
            if (Vector2.Distance(playerPosition, enemyPosition) < detectDistance)
            {
                if(CanLookPlayer(enemyPosition, playerPosition))
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = Color.red;
                Gizmos.DrawLine(enemyPosition,playerPosition);
            }
        }
        
        private bool CanLookPlayer(Vector2 position, Vector2 playerPosition)
        {
            var layerMask = ((1 << LayerMask.NameToLayer("Player")) + (1 << LayerMask.NameToLayer("Platform")));
            var lookingPlayerRayHit = Physics2D.Raycast(position, playerPosition - position, detectDistance, layerMask);
            return lookingPlayerRayHit.collider.CompareTag("Player");
        }
    }
}
