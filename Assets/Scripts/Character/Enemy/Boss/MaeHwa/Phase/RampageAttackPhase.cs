using System.Collections;
using System.Collections.Generic;
using Boss.Phase;
using UnityEngine;
using Util;

namespace Boss.MaeHwa
{
    public class RampageAttackPhase : BossPhase
    {
        private readonly float noticeWaitTime;
        private readonly float attackBeforeWaitTime;
        private readonly float attackTime;
        private readonly float attackAfterWaitTime;
        public RampageAttackPhase(string phaseName,float noticeWaitTime, float attackBeforeWaitTime, float attackTime, float attackAfterWaitTime, int rarity = 1) : base(phaseName, rarity)
        {
            this.noticeWaitTime = noticeWaitTime;
            this.attackBeforeWaitTime = attackBeforeWaitTime;
            this.attackTime = attackTime;
            this.attackAfterWaitTime = attackAfterWaitTime;
        }

        protected internal override IEnumerator DoPhase(BossController bossController)
        {
            var playerPos = GameObject.Find("Player").transform.position;

            
            float firstAngle;
            RaycastHit2D raycastHit2D;
            do
            {
                InfiniteLoopDetector.Run();
                firstAngle = Random.Range(0, 360);
                raycastHit2D = Physics2D.Raycast(playerPos, new Vector2(Mathf.Cos(firstAngle), Mathf.Sin(firstAngle)),
                    3f, LayerMask.GetMask("Platform"));
            } while (raycastHit2D.collider != null);
            
            float secondAngle;
            do
            {
                InfiniteLoopDetector.Run();
                secondAngle = Random.Range(0, 360);
                raycastHit2D = Physics2D.Raycast(playerPos, new Vector2(Mathf.Cos(secondAngle), Mathf.Sin(secondAngle)),
                    5f, LayerMask.GetMask("Platform"));
            } while (raycastHit2D.collider != null);
            
            var posList = new List<Vector2>
            {
                playerPos,
                new Vector2(playerPos.x + 3f * Mathf.Cos(firstAngle), playerPos.y + 3f * Mathf.Sin(firstAngle)),
                new Vector2(playerPos.x + 5f * Mathf.Cos(secondAngle), playerPos.y + 5f * Mathf.Sin(secondAngle))
            };
            
            var rotList = new List<Vector3>
            {
                new Vector3(0, 0, Random.Range(10f,-10f)),
                new Vector3(0, 0, Random.Range(20f,40f)),
                new Vector3(0, 0, Random.Range(-20f,-40f))
            };

            var rangeList = new List<MaeHwaRampageRange>();
            for(var i = 0; i<3; i++)
            {
                var pos = posList[Random.Range(0, posList.Count)];
                posList.Remove(pos);
                
                var rot = rotList[Random.Range(0, posList.Count)];
                rotList.Remove(rot);
                
                var range =  ((MaeHwaController) bossController).InstantiateRampageRange(pos, rot);
                range.SetActive(true);
                rangeList.Add(range);
                
                yield return new WaitForSeconds(noticeWaitTime);
            }
            yield return new WaitForSeconds(attackBeforeWaitTime);

            foreach (var range in rangeList)
            {
                range.GetComponent<MaeHwaRampageRange>().SetDestroyTime(attackTime);
                range.SetDanger();
                // TO-DO : range 애니메이션 설정
            }
            yield return new WaitForSeconds(attackAfterWaitTime);
        }
    }
}