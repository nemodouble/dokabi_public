using System.Collections;
using UnityEngine;

namespace Character
{
    public interface IHitAble
    { 
        IEnumerator Hit(int attackDamage, Vector2 attackDir, float attackForceScale = 1);
        
    }
}