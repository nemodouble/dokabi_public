using System;
using System.Collections.Generic;
using Boss.Phase;
using Boss.Phase.Moving;
using UnityEngine;

namespace Boss
{
    public class TmpFlyBossController : BossController
    {
        private const int RightIdle = 1;
        private const int LeftIdle = -1;

        private const int IdleChange = -1;
        
        private int idleDir = LeftIdle;
        private bool disabled;

        private void Awake()
        {
            DeadPhase = new DeadInstant("Dead", 1f);
        }

        private void OnEnable()
        {
            if(disabled)
                StartCoroutine(Action());
        }
        
        private void OnDisable()
        {
            prevPhase = "";
            disabled = true;
        }


        protected override List<BossPhase> GetAblePhaseList()
        {
            var ableStateList = new List<BossPhase>();
            
            var bossPosition = transform.position;
            var idleChange = new MoveToRelativePos("idleChange", Player,
                new Vector2(-idleDir * 8f, 5f), 10f, 3f);
            
            if (prevPhase == "")
            {
                ableStateList.Add(SetAndGetIdle());
            }
            else switch (prevPhase)
            {
                case "idle":
                    ableStateList.Add(new ChangeColorWithWait("dashStart",Color.blue, 0.1f, false));
                    ableStateList.Add(new ChangeColorWithWait("dashStart",Color.blue, 0.1f, false));
                    ableStateList.Add(new ChangeColorWithWait("goingAttackStart",Color.cyan, 0.1f, false));
                    ableStateList.Add(new ChangeColorWithWait("goingAttackStart",Color.cyan, 0.1f, false));
                    ableStateList.Add(idleChange);
                    break;
                case "idleChange":
                    SetIdleWay();
                    ableStateList.Add(new ChangeColorWithWait("dashStart",Color.blue, 0.1f, false));
                    break;
                //대쉬
                case "dashStart":
                    ableStateList.Add(new MoveToRelativePos("dashBefore", Player,
                        new Vector2(idleDir * 9f, 0), 20f, 2f));
                    ableStateList.Add(new MoveToRelativePos("dashBefore", Player,
                        new Vector2(idleDir * 9f, 0), 20f, 2f));
                    ableStateList.Add(new MoveToRelativePos("difDashBefore", Player,
                        new Vector2(-idleDir * 9f, 0), 20f, 2f));
                    break;
                case "dashBefore":
                    Rigid2D.velocity = Vector2.zero;
                    ableStateList.Add(new ChangeColorWithWait("dashWait",Color.green, 0.5f));
                    break;
                case "dashWait":
                    ableStateList.Add(new MoveStraight("dashDoing",
                        new Vector2(bossPosition.x - idleDir * 20, bossPosition.y), 50f, false,
                        MoveStraight.WallDirChangeType.Stop,1f));
                    break;
                // 대쉬 변형
                case "difDashBefore":
                    Rigid2D.velocity = Vector2.zero;
                    ableStateList.Add(new ChangeColorWithWait("difDashWait",Color.green, 0.5f));
                    break;
                case "difDashWait":
                    ableStateList.Add(new MoveStraight("dashDoing",
                        new Vector2(bossPosition.x + idleDir * 20, bossPosition.y), 50f, false,
                        MoveStraight.WallDirChangeType.Stop,1f));
                    break;
                case "dashDoing":
                    ableStateList.Add(SetAndGetIdle());
                    break;
                // 훑고 지나가기
                case "goingAttackStart":
                    ableStateList.Add(new WaitPhase("goingAttackWait",0.7f,true));
                    break;
                case "goingAttackWait":
                    var targetPos = new Vector2(transform.position.x - 5f * idleDir, transform.position.y - 5f);
                    Rigid2D.gravityScale = 1f;
                    ableStateList.Add(new MoveStraight("startGoing",targetPos,30f,true, movingTime:0.001f,stopWhenMovingEnd:false));
                    break;
                case "startGoing":
                    ableStateList.Add(new ChangeColorWithWait("goingWait",Color.white, 0.5f));
                    break;
                case "goingWait":
                    Rigid2D.gravityScale = 0;
                    ableStateList.Add(SetAndGetIdle());
                    break;
                default:
                    Debug.Log("패턴 미지정");
                    ableStateList.Add(new WaitPhase("idle",1));
                    break;
            }
            return ableStateList;
        }

        private BossPhase SetAndGetIdle()
        {
            SetIdleWay();
            return new MoveToRelativePos("idle", Player, new Vector2(idleDir * 8f, 5f), 10f, 3f);
        }
        private void SetIdleWay()
        {
            idleDir = transform.position.x > Player.transform.position.x ? RightIdle : LeftIdle;
        }
        
    }
}