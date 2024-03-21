using System;
using Character.Player;
using UnityEngine;

namespace Mechanics.System
{
    public class TemporaryDangerRange : ActiveTemporary
    {
        [SerializeField] private int damage = 1;

        // private void FixedUpdate()
        // {
        //     var boxCollider2D = GetComponent<BoxCollider2D>();
        //     var atkBox = boxCollider2D.size;
        //     var hit = Physics2D.OverlapBox((Vector2)boxCollider2D.transform.position + boxCollider2D.offset, atkBox, 0, LayerMask.GetMask("Player"));
        //     if (hit != null)
        //     {
        //         var playerController = hit.gameObject.GetComponent<PlayerController>();
        //         var attackDir = playerController.GetAttackedDir(transform.position);
        //         playerController.Hit(1, attackDir);
        //     }
        // }

        private void OnTriggerEnter2D(Collider2D col)
        {
            var playerController = col.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                var attackDir = playerController.GetAttackedDir(transform.position);
                playerController.Hit(1, attackDir);
            }
        }
    } 
}
