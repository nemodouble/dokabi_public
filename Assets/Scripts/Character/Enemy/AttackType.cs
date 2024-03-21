using System;
using System.Collections;
using System.Linq;
using Character;
using UnityEngine;

public static class AttackType
{
        private static readonly string[] HitAbleTagList = {"Enemy", "Boss"};

        public static IEnumerator RangeAttack(Vector2 point, Vector2 atkSize, Vector2 attackDir)
        {
                var attackedCollider2Ds = Physics2D.OverlapBoxAll(point, atkSize, 0);
                foreach (var attackedCollider in attackedCollider2Ds)
                {
                        if (HitAbleTagList.Any(hitAbleTag => attackedCollider.CompareTag(hitAbleTag)))
                        {
                                yield return attackedCollider.gameObject
                                        .GetComponent<IHitAble>()
                                        .Hit(10, attackDir);
                        }
                }
                
                yield return null;
        }
}