using System.Collections;
using Enemy;
using Enemy.StateCheckLogic.DetectLogic;
using UnityEngine;

namespace Character.Enemy.Normal.AttackLogic.AttackState
{
    public class InstantiateAttackState : EnemyAttackState
    {
        private readonly GameObject summonObject;
        
        public InstantiateAttackState(EnemyDetectLogic attackDetectLogic, GameObject summonObject) : base(attackDetectLogic)
        {
            this.summonObject = summonObject;
        }
        public override IEnumerator Attack(EnemyController enemyController)
        {
            Object.Instantiate(summonObject);
            yield return null;
        }
    }
}