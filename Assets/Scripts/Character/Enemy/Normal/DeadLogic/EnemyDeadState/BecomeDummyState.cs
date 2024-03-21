using System.Collections;
using Enemy;
using UnityEngine;

namespace Character.Enemy.Normal.DeadLogic.EnemyDeadState
{
    public class BecomeDummyState : global::Enemy.DeadLogic.EnemyDeadState.EnemyDeadState
    {
        private const float DummyGravity = 1.5f;
        public override IEnumerator Dead(EnemyController enemyController)
        {
            enemyController.rigid2D.gravityScale = DummyGravity;
            enemyController.gameObject.layer = LayerMask.NameToLayer("Dummy");
            enemyController.gameObject.tag = "Dummy";
            enemyController.spriteRenderer.color = Color.gray;
            while (!enemyController.IsOnPlatform(0))
            {
                yield return null;
            }

            enemyController.rigid2D.bodyType = RigidbodyType2D.Static;
        }
    }
}