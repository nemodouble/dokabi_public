using System;
using Character.Player;
using UnityEngine;

namespace Mechanics.System
{
    public class EnemyBody : MonoBehaviour
    {
        public bool isTriggerDamage = false;
        public bool isCollisionDamage = true;
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("Player") && isTriggerDamage)
            {
                var playerController = col.gameObject.GetComponent<PlayerController>();
                var attackDir = playerController.GetAttackedDir(transform.position);
                playerController.Hit(1, attackDir);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player") && isCollisionDamage)
            {
                var playerController = collision.gameObject.GetComponent<PlayerController>();
                var attackDir = playerController.GetAttackedDir(transform.position);
                playerController.Hit(1, attackDir);
            }
        }
    }
}