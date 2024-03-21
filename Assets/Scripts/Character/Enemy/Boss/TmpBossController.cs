using System.Collections;
using System.Collections.Generic;
using Boss.Phase;
using Boss.Phase.Moving;
using UnityEngine;
using UnityEngine.UIElements;

namespace Boss
{
    public class TmpBossController : BossController
    {
        [SerializeField] private GameObject throwingObject;
        private readonly BossPhase dashStart = new ChangeColorWithWait("dashStart", Color.blue, 0.7f);
        private readonly BossPhase jumpStart = new ChangeColorWithWait("jumpStart",Color.green, 0.5f);
        private readonly BossPhase throwingStart = new ChangeColorWithWait("throwingStart", Color.black, 0.7f);
        private readonly BossPhase wait1Sec = new WaitPhase("idle", 1);

        protected override void Start()
        {
            base.Start();
            DeadPhase = new DeadInstant("Dead", 1f);
        }
        protected override List<BossPhase> GetAblePhaseList()
        {
            var ableStateList = new List<BossPhase>();
            if (prevPhase == "")
            {
                ableStateList.Add(wait1Sec);
            }
            else switch (prevPhase)
            {
                case "idle":
                    ableStateList.Add(dashStart);
                    //ableStateList.Add(jumpStart);
                    //ableStateList.Add(throwingStart);
                    break;
                // 대쉬
                case "dashStart":
                    ableStateList.Add(new KeepMovingToPos("toPlayerDash",
                        new Vector2(Player.transform.position.x, transform.position.y), 1000f, 0.7f, 20f, true));
                    break;
                case "toPlayerDash":
                    Rigid2D.velocity = Vector2.zero;
                    ableStateList.Add(new WaitPhase("idle",1f));
                    break;
                // 점프
                case "jumpStart":
                    var position = transform.position;
                    ableStateList.Add(new MoveStraight("jump",new Vector2(position.x + GetBossToPlayerDir().x, position.y + 2f), 20f, false,
                        MoveStraight.Stop, 0.001f, false));
                    break;
                case "jump":
                    ableStateList.Add(new WaitPhase("waitAfterJump", 0.7f));
                    ableStateList.Add(new WaitPhase("jumpLandWait", 2f));
                    break;
                case "waitAfterJump":
                    ableStateList.Add(new WaitPhase("stopAtAir",0.5f,true));
                    break;
                case "stopAtAir":
                    ableStateList.Add(new MoveStraight("jumpDash",Player.transform.position, 30f, false, MoveStraight.Stop, 1f));
                    break;
                // 던지기
                case "throwingStart":
                    ableStateList.Add(new AttackSummon("throw",throwingObject,new Vector2(GetBossToPlayerDir().x , 0)));
                    break;
                case "throw":
                    ableStateList.Add(wait1Sec);
                    break;
                
                default:
                    ableStateList.Add(new WaitPhase("idle",0.5f));
                    break;
            }
            return ableStateList;
        }
    }
}