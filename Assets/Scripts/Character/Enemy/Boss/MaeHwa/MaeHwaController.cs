using System;
using System.Collections;
using System.Collections.Generic;
using Boss.Phase;
using Boss.Phase.Moving;
using Character.Enemy.Boss.MaeHwa;
using FMODUnity;
using Mechanics.Boss.Phase.Moving;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Boss.MaeHwa
{
    public class MaeHwaController : BossController
    {
        private enum LookingDir
        {
            RightDir = 1, 
            LeftDir = -1
        }
        private LookingDir lookingDir = LookingDir.LeftDir;
        
        private Vector3 centerPos;
        
        // 사운드
        public EventReference horizonAttackEvent;
        public EventReference bodyAttackEvent;
        public EventReference comboFirstAttackEvent;
        public EventReference comboSecondAttackEvent;
        public EventReference comboStingAttackEvent;
        public EventReference downSmashEvent;
        public EventReference rampageWindEvent;
        public EventReference rampageRiseEvent;
        public EventReference dashEvent;
        public EventReference walkEvent;
        public EventReference yell1;
        public EventReference yell2;
        public EventReference yell3;
        public EventReference jump;
        public EventReference land;
        public EventReference deadVoice;
        public EventReference outro;
        [FormerlySerializedAs("flashEvent")] public EventReference teleportEvent;
        
        // 패턴 대기 
        private readonly BossPhase startingWait = new WaitPhase("Start-Wait", 5.5f, false);
        private readonly BossPhase selectAttack = new EmptyPhase("Select-Attack");

        [SerializeField] private float betweenPhaseWaitTime = 1f;
        private BossPhase endPhase;
        private ParticleSystem dashPS;

        #region 걷기

        //walk
        [Header("걷기")] 
        [SerializeField] private float walkSpeed = 3f;
        [SerializeField] private float walkTime = 1f;
        
        private BossPhase walkLeft;
        private BossPhase walkRight;
        private float walkDistance;
        private ParticleSystem walkPS;

        #endregion

        #region 가로베기

        // horizon attack
        [Header("가로베기")] 
        [SerializeField] private float horizonBeforeWaitTime = 1f;
        [SerializeField] private float horizonAfterWaitTime = 1f;
        [SerializeField] private float horizonStepSpeed = 20f;
        private GameObject horizonAttackRange;
        private Vector3 leftEdgePos;
        private Vector3 rightEdgePos;
        
        private readonly BossPhase horizonAttackStart = new EmptyPhase("Horizon-Start", 3);
        private BossPhase horizonStep;
        private BossPhase horizonBeforeWait;
        private BossPhase horizonAttack;
        private BossPhase horizonAfterWait;

        #endregion

        #region 바디태클

        // body attack
        [Header("바디태클")]
        [SerializeField] private float bodyDashSpeed = 20f;
        [SerializeField] private float bodyDashTime = 0.3f;
        [SerializeField] private float bodyAfterDashWaitTime = 1f;
        [SerializeField] private float bodyAfterAttackWaitTime = 1f;
        private GameObject bodyWall;
        private GameObject bodyStrongAttack;
        private GameObject bossDangerRange;
        
        private readonly BossPhase bodyAttackStart = new EmptyPhase("Body-Start");
        private BossPhase bodyLeftDash;
        private BossPhase bodyRightDash;
        private BossPhase bodyAfterDashWait;
        private BossPhase bodyAttack;
        private BossPhase bodyAfterAttackWait;

        #endregion

        #region 콤보
        
        // combo attack
        [Header("콤보")] 
        [SerializeField] private float comboFirstBeforeWaitTime = 0.4f;
        [SerializeField] private float comboAfterFirstWaitTime = 0.1f;
        [SerializeField] private float comboBeforeSecondWaitTime = 0.4f;
        [SerializeField] private float comboAfterSecondWaitTime = 0.1f;
        [SerializeField] private float comboBeforeThirdWaitTime = 0.9f;
        [SerializeField] private float comboAfterThirdWaitTime = 1f;
        [SerializeField] private float comboNormalSpeed = 10f;
        [SerializeField] private float comboNormalLength = 0.2f;
        [SerializeField] private float comboStingSpeed = 20f;
        [SerializeField] private float comboStingTime = 0.2f;
        private GameObject comboNormalAttack;
        private GameObject comboStingAttack;
        
        private readonly BossPhase comboAttackStart = new EmptyPhase("Combo-Start");
        private BossPhase comboFirstAttackStart;
        private BossPhase comboFirstBeforeWait;
        private BossPhase comboFirstNoDash;
        private BossPhase comboFirstLeftDash;
        private BossPhase comboFirstRightDash;
        private BossPhase comboFirstAttack;
        private BossPhase comboFirstAfterWait;
        private BossPhase comboSecondWait;
        private BossPhase comboSecondLeftDash;
        private BossPhase comboSecondRightDash;
        private BossPhase comboSecondNoDash;
        private BossPhase comboSecondAttack;
        private BossPhase comboSecondAfterWait;
        private BossPhase comboThirdBeforeWait;
        private BossPhase comboThirdAttack;
        private BossPhase comboThirdLeftDash;
        private BossPhase comboThirdRightDash;
        private BossPhase comboThirdAfterWait;
        
        

        #endregion

        #region 난무

        //Rampage
        [Header("난무")] 
        [SerializeField] private float rampageRiseSpeed = 5f;
        [SerializeField] private float rampageRiseTime = 0.3f;
        [SerializeField] private float rampageRiseWaitTime = 0.3f;
        [FormerlySerializedAs("rampageBeforeNoticeWait")] [SerializeField] private float rampageBeforeNoticeWaitTime = 1f;
        [SerializeField] private float rampageBlinkWait = 2f;
        [SerializeField] private float rampageNoticeInterval = 0.1f;
        [SerializeField] private float rampageBeforeAttackTime = 0.5f;
        [SerializeField] private float rampageAttackTime = 0.2f;
        [SerializeField] private float rampageAttackAfterWaitTime = 0.3f;
        [SerializeField] private float rampageStaggerTime = 3f;
        private ParticleSystem rampagePS;
        private GameObject rampageRange;
        private float originGravity;
        
        private readonly BossPhase rampageAttackStart = new EmptyPhase("Rampage-Start", 10);
        private BossPhase rampageRise;
        private BossPhase rampageRiseWait;
        private BossPhase rampageBeforeNoticeWait;
        private BossPhase rampageBlink;
        private BossPhase rampageNotice;
        private BossPhase rampageToDown;

        #endregion

        #region 다운스매쉬

        //downSmash
        [FormerlySerializedAs("downAirWait")]
        [Header("다운스매싱")] 
        [SerializeField] private float downAirWaitTime;
        [SerializeField] private float downAccel;
        [SerializeField] private float downAccelTime;
        [SerializeField] private float downAfterSmashTime;
        private bool haveMoreStagger;
        private GameObject downEffect;
        private ParticleSystem teleportPS;
        private ParticleSystem landPS;

        private readonly BossPhase downStart = new EmptyPhase("Down-Start", 7);
        private BossPhase downPlayPS = new EmptyPhase("Down-PlayPS");
        private BossPhase downBlink = new EmptyPhase("Down-Blink");
        private BossPhase downAirWait;
        private BossPhase downGetAccel;
        private BossPhase downSmashWait;
        private BossPhase downSmashRampageWait;

        #endregion
        
        public bool isBackStep;
        
        
        protected override void Start()
        {
            // transform, gameObject 할당
            var parent = gameObject.transform.parent;
            
            bodyWall = transform.Find("PushWall").gameObject;
            bossDangerRange = transform.Find("DangerRange").gameObject;
            bodyStrongAttack = transform.Find("StrongAttack").gameObject;
            comboNormalAttack = transform.Find("NormalAttack").gameObject;
            comboStingAttack = transform.Find("StingAttack").gameObject;
            downEffect = transform.Find("DownEffect").gameObject;
            
            teleportPS = transform.Find("TeleportPS").GetComponent<ParticleSystem>();
            hitPS = parent.Find("HitPS").GetComponent<ParticleSystem>();
            deadPS = transform.Find("DeadPs").GetComponent<ParticleSystem>();
            dashPS = transform.Find("PlatDashPS").GetComponent<ParticleSystem>();
            walkPS = transform.Find("WalkPS").GetComponent<ParticleSystem>();
            landPS = transform.Find("LandPS").GetComponent<ParticleSystem>();
            
            rampageRange = parent.Find("RampageRange").gameObject;
            horizonAttackRange = parent.Find("HorizonAttack").gameObject;
            
            leftEdgePos = parent.Find("LeftEdge").transform.position;
            rightEdgePos = parent.Find("RightEdge").transform.position;
            
            rampagePS = parent.Find("RampagePS").GetComponent<ParticleSystem>();
            centerPos = parent.position;

            #region 패턴할당

            // 패턴 할당
            
            // 걷기
            walkLeft = new MoveByVelocity("Walk", Vector2.left, walkSpeed, walkTime);
            walkRight = new MoveByVelocity("Walk",Vector2.right, walkSpeed, walkTime);
            
            // 가로 베기
            horizonBeforeWait = new WaitPhase("Horizon-BeforeWait", horizonBeforeWaitTime, true);
            horizonAttack = new AttackFixedRange("Horizon-Attack", horizonAttackRange);
            horizonAfterWait = new WaitPhase("Horizon-AfterWait", horizonAfterWaitTime, true);
            
            // 바디태클
            bodyAfterDashWait = new WaitPhase("Body-AfterDashWait", bodyAfterDashWaitTime);
            bodyLeftDash = new MoveByVelocity("Body-Dash", Vector2.left, bodyDashSpeed, bodyDashTime);
            bodyRightDash = new MoveByVelocity("Body-Dash", Vector2.right, bodyDashSpeed, bodyDashTime);
            bodyAttack = new AttackFixedRange("Body-Attack", bodyStrongAttack);
            bodyAfterAttackWait = new WaitPhase("Body-AfterAttackWait", bodyAfterAttackWaitTime);
            
            // 콤보 공격
            comboFirstAttackStart = new EmptyPhase("Combo-FirstAttackStart");
            comboFirstBeforeWait = new WaitPhase("Combo-First-BeforeWait", comboFirstBeforeWaitTime);
            comboFirstAttack = new AttackFixedRange("Combo-First-Attack", comboNormalAttack);
            comboFirstNoDash = new WaitPhase("Combo-First-DashOrWait", comboNormalLength);
            comboFirstLeftDash = new MoveByVelocity("Combo-First-DashOrWait", Vector2.left, comboNormalSpeed, comboNormalLength);
            comboFirstRightDash = new MoveByVelocity("Combo-First-DashOrWait", Vector2.right, comboNormalSpeed, comboNormalLength);
            comboFirstAfterWait = new WaitPhase("Combo-First-AfterWait", comboAfterFirstWaitTime);
            comboSecondWait = new WaitPhase("Combo-Second-BeforeWait", comboBeforeSecondWaitTime);
            comboSecondNoDash = new WaitPhase("Combo-Second-DashOrWait", comboNormalLength);
            comboSecondLeftDash = new MoveByVelocity("Combo-Second-DashOrWait", Vector2.left, comboNormalSpeed, comboNormalLength);
            comboSecondRightDash = new MoveByVelocity("Combo-Second-DashOrWait", Vector2.right, comboNormalSpeed, comboNormalLength);
            comboSecondAttack = new AttackFixedRange("Combo-Second-Attack", comboNormalAttack);
            comboSecondAfterWait = new WaitPhase("Combo-Second-AfterWait", comboAfterSecondWaitTime);
            comboThirdBeforeWait = new WaitPhase("Combo-Third-BeforeWait", comboBeforeThirdWaitTime);
            comboThirdAttack = new AttackFixedRange("Combo-Third-Attack", comboStingAttack);
            comboThirdLeftDash = new MoveByVelocity("Combo-Third-Dash", Vector2.left, comboStingSpeed, comboStingTime);
            comboThirdRightDash = new MoveByVelocity("Combo-Third-Dash", Vector2.right, comboStingSpeed, comboStingTime);
            comboThirdAfterWait = new WaitPhase("Combo-AfterWait", comboAfterThirdWaitTime);
            
            // 난무
            rampageRise = new MoveLikeJump("Rampage-Rise", rampageRiseSpeed, rampageRiseTime);
            rampageRiseWait = new WaitPhase("Rampage-RiseWait", rampageRiseWaitTime, true);
            rampageBeforeNoticeWait = new WaitPhase("Rampage-BeforeNoticeWait", rampageBeforeNoticeWaitTime, true);
            rampageBlink = new WaitPhase("Rampage-Blink", rampageBlinkWait, true);
            rampageNotice = new RampageAttackPhase("Rampage-Notice", rampageNoticeInterval, rampageBeforeAttackTime, rampageAttackTime, rampageAttackAfterWaitTime);
            rampageToDown = new EmptyPhase("Down-AirWait");
            
            // 다운어택
            downAirWait = new WaitPhase("Down-AirWait", downAirWaitTime, true);
            downGetAccel = new MoveByVelocity("Down-GetAccel", Vector2.down, downAccel, downAccelTime);
            downSmashWait = new WaitPhase("Down-SmashWait", downAfterSmashTime);
            downSmashRampageWait = new WaitPhase("Down-SmashWait", rampageStaggerTime);
            
            // 사망
            DeadPhase = new DeadNormal("Dead");
            
            endPhase = new WaitPhase("EndPhase", betweenPhaseWaitTime);

            #endregion
            
            base.Start();
        }

        private void Update()
        {
            transform.localScale = new Vector3((int)lookingDir, 1, 1);
            if (prevPhase == "Walk")
            {
                walkDistance += Time.deltaTime;
                if(walkDistance >= 0.3f)
                {
                    RuntimeManager.PlayOneShot(walkEvent);
                    walkDistance = 0f;
                }
            }
        }

        protected override IEnumerator Dead()
        {
            RuntimeManager.PlayOneShot(deadVoice);
            yield return base.Dead();
        }

        protected override List<BossPhase> GetAblePhaseList()
        {
            var ablePhaseList = new List<BossPhase>();
            if (prevPhase == "")
            {
                SetLookingDir();
                ablePhaseList.Add(startingWait);
            }
            else switch (prevPhase)
            {
                case "Start-Wait":
                case "EndPhase":
                    SetLookingDir();
                    ablePhaseList.Add(new EmptyPhase("Select-Walk"));
                    if(IsInDistance(0f,3f))
                        ablePhaseList.Add(new EmptyPhase("Select-Step", 2));
                    ablePhaseList.Add(selectAttack);
                    break;
                
                case "Select-Walk":
                    lookingDir = IsInDistance(0f, 4f) ^ (Player.transform.position.x > transform.position.x)
                        ? LookingDir.RightDir
                        : LookingDir.LeftDir;
                    walkPS.Play();
                    ablePhaseList.Add(lookingDir == LookingDir.RightDir ? walkRight : walkLeft);
                    break;
                case "Walk":
                    walkPS.Stop();
                    ablePhaseList.Add(selectAttack);
                    break;
                
                case "Select-Step":
                    RuntimeManager.PlayOneShot(dashEvent);
                    ablePhaseList.Add(IsInDistance(0f, 3f)
                        ? GetStepToKeepDistance("BackStep", 6f, out isBackStep)
                        : GetStepToKeepDistance("FrontStep", 6f, out isBackStep));
                    break;
                case "FrontStep":
                case "BackStep":
                    ablePhaseList.Add(selectAttack);
                    break;
                
                case "Select-Attack":
                    SetLookingDir();
                    Rigid2D.velocity = Vector2.zero;
                    ablePhaseList.Add(comboAttackStart);
                    ablePhaseList.Add(bodyAttackStart);
                    if(Mathf.Abs(Player.transform.position.x - transform.parent.position.x) <= Mathf.Abs(rightEdgePos.x - transform.parent.position.x))
                        ablePhaseList.Add(horizonAttackStart);
                    ablePhaseList.Add(rampageAttackStart);
                    ablePhaseList.Add(downStart);
                    break;
                
                
                // 가로베기
                case "Horizon-Start":
                    SetHorizonStep();
                    RuntimeManager.PlayOneShot(dashEvent);
                    dashPS.Play();
                    ablePhaseList.Add(horizonStep);
                    break;
                case "Horizon-Step":
                    var bossTransform = transform;
                    SetLookingDir(bossTransform.position.x > bossTransform.parent.position.x
                        ? LookingDir.LeftDir
                        : LookingDir.RightDir); 
                    RuntimeManager.PlayOneShot(horizonAttackEvent);
                    RuntimeManager.PlayOneShot(yell2);
                    dashPS.Stop();
                    ablePhaseList.Add(horizonBeforeWait);
                    break;
                case "Horizon-BeforeWait":
                    horizonAttackRange.transform.localScale = transform.localScale;
                    RuntimeManager.PlayOneShot(yell3);
                    ablePhaseList.Add(horizonAttack);
                    break;
                case "Horizon-Attack":
                    ablePhaseList.Add(horizonAfterWait);
                    break;
                case "Horizon-AfterWait":
                    ablePhaseList.Add(endPhase);
                    break;
                        
                // 몸통 박치기
                case "Body-Start":
                    bossDangerRange.SetActive(false);
                    bodyWall.SetActive(true);
                    RuntimeManager.PlayOneShot(dashEvent);
                    dashPS.Play();
                    ablePhaseList.Add(Player.transform.position.x > transform.position.x
                        ? bodyRightDash
                        : bodyLeftDash);
                    break;
                case "Body-Dash":
                    Rigid2D.velocity = Vector2.zero;
                    dashPS.Stop();
                    ablePhaseList.Add(bodyAfterDashWait);
                    break;
                case "Body-AfterDashWait":
                    bossDangerRange.SetActive(true);
                    bodyWall.SetActive(false);
                    RuntimeManager.PlayOneShot(bodyAttackEvent);
                    ablePhaseList.Add(bodyAttack);
                    break;
                case "Body-Attack":
                    ablePhaseList.Add(bodyAfterAttackWait);
                    break;
                case "Body-AfterAttackWait":
                    ablePhaseList.Add(endPhase);
                    break;
                    
                // 3연격
                case "Combo-Start":
                case "Combo-Step":
                    ablePhaseList.Add(comboFirstAttackStart);
                    break;
                case "Combo-FirstAttackStart":
                    ablePhaseList.Add(comboFirstBeforeWait);
                    break;
                case "Combo-First-BeforeWait":
                    ablePhaseList.Add(IsInDistance(0f, 2.5f) ? comboFirstNoDash : 
                        lookingDir == LookingDir.LeftDir ? comboFirstLeftDash : comboFirstRightDash);
                    break;
                case "Combo-First-DashOrWait":
                    Rigid2D.velocity = Vector2.zero;
                    RuntimeManager.PlayOneShot(comboFirstAttackEvent);
                    RuntimeManager.PlayOneShot(yell1);
                    ablePhaseList.Add(comboFirstAttack);
                    break;
                case "Combo-First-Attack":
                    ablePhaseList.Add(comboFirstAfterWait);
                    break;
                case "Combo-First-AfterWait":
                    SetLookingDir();
                    ablePhaseList.Add(IsInDistance(0f, 5f) ? comboSecondWait : comboThirdBeforeWait);
                    break;
                case "Combo-Second-BeforeWait":
                    ablePhaseList.Add(IsInDistance(0f, 2.5f) ? comboSecondNoDash :
                        lookingDir == LookingDir.LeftDir ? comboSecondLeftDash : comboSecondRightDash);
                    break;
                case "Combo-Second-DashOrWait":
                    Rigid2D.velocity = Vector2.zero;
                    RuntimeManager.PlayOneShot(comboSecondAttackEvent);
                    RuntimeManager.PlayOneShot(yell2);
                    ablePhaseList.Add(comboSecondAttack);
                    break;
                case "Combo-Second-Attack":
                    ablePhaseList.Add(comboSecondAfterWait);
                    break;
                case "Combo-Second-AfterWait":
                    SetLookingDir();
                    ablePhaseList.Add(comboThirdBeforeWait);
                    break;
                case "Combo-Third-BeforeWait":
                    RuntimeManager.PlayOneShot(comboStingAttackEvent);
                    RuntimeManager.PlayOneShot(yell3);
                    dashPS.Play();
                    ablePhaseList.Add(comboThirdAttack);
                    break;
                case "Combo-Third-Attack":
                    ablePhaseList.Add(lookingDir == LookingDir.RightDir? comboThirdRightDash : comboThirdLeftDash);
                    break;
                case "Combo-Third-Dash":
                    Rigid2D.velocity = Vector2.zero;
                    dashPS.Stop();
                    ablePhaseList.Add(comboThirdAfterWait);
                    break;
                case "Combo-AfterWait":
                    ablePhaseList.Add(endPhase);
                    break;
                
                
                // 난무
                case "Rampage-Start":
                    Rigid2D.velocity = Vector2.zero;
                    RuntimeManager.PlayOneShot(rampageRiseEvent);
                    RuntimeManager.PlayOneShot(yell1);
                    RuntimeManager.PlayOneShot(jump);
                    RuntimeManager.PlayOneShot(rampageWindEvent);
                    rampagePS.Play();
                    ablePhaseList.Add(rampageRise);
                    break;
                case "Rampage-Rise":
                    ablePhaseList.Add(rampageRiseWait);
                    break;
                case "Rampage-RiseWait":
                    originGravity = Rigid2D.gravityScale;
                    Rigid2D.gravityScale = 0;
                    ablePhaseList.Add(rampageBeforeNoticeWait);
                    break;
                case "Rampage-BeforeNoticeWait":
                    // ablePhaseList.Add(rampageBlink);
                    // break;
                case "Rampage-Blink":
                    transform.position += new Vector3(0, 10000);
                    ablePhaseList.Add(rampageNotice);
                    break;
                case "Rampage-Notice":
                    rampagePS.Stop();
                    Rigid2D.gravityScale = originGravity;
                    transform.position = new Vector2(Player.transform.position.x, centerPos.y + 7f);
                    haveMoreStagger = true;
                    ablePhaseList.Add(rampageToDown);
                    break;
                
                // 상단 내려찍기
                case "Down-Start":
                    teleportPS.Play();
                    RuntimeManager.PlayOneShot(teleportEvent);
                    RuntimeManager.PlayOneShot(yell1);
                    originGravity = Rigid2D.gravityScale;
                    Rigid2D.gravityScale = 0;
                    transform.position = new Vector2(Player.transform.position.x, centerPos.y + 7f);
                    ablePhaseList.Add(downBlink);
                    break;
                case "Down-Blink":
                    ablePhaseList.Add(downAirWait);
                    break;
                case "Down-AirWait":
                    downEffect.SetActive(true);
                    RuntimeManager.PlayOneShot(downSmashEvent);
                    GetComponent<ParticleTrigger>().Reset();
                    ablePhaseList.Add(downGetAccel);
                    break;
                case "Down-GetAccel":
                    RuntimeManager.PlayOneShot(land);
                    // landPS.Play();
                    if (haveMoreStagger)
                    {
                        haveMoreStagger = false;
                        ablePhaseList.Add(downSmashRampageWait);
                    }
                    else
                        ablePhaseList.Add(downSmashWait);
                    break;
                case "Down-SmashWait":
                    ablePhaseList.Add(endPhase);
                    break;
                
                
                default:
                    Debug.Log("보스 상태 지정 안됨");
                    ablePhaseList.Add(startingWait);
                    break;
            }

            return ablePhaseList;
        }

        private void SetHorizonStep()
        {
            var position = transform.position;
            Vector3 targetEdgePosition;
            Vector2 stepDir;
            if (Player.transform.position.x < transform.position.x)
            {
                targetEdgePosition = rightEdgePos;
                stepDir = transform.position.x > rightEdgePos.x ? Vector2.left : Vector2.right;
            }
            else
            {
                targetEdgePosition = leftEdgePos;
                stepDir = transform.position.x < leftEdgePos.x ? Vector2.right : Vector2.left;
            }
            horizonStep = new MoveByVelocityToPos("Horizon-Step", stepDir, horizonStepSpeed, 10, targetEdgePosition);
            lookingDir = stepDir == Vector2.right? LookingDir.RightDir : LookingDir.LeftDir;
        }

        private bool IsInDistance(float minDistance, float maxDistance)
        {
            var distance = Mathf.Abs(transform.position.x - Player.transform.position.x);
            return distance >= minDistance && distance <= maxDistance;
        }
        private BossPhase GetStepToKeepDistance(string stepName, float targetDistance, out bool isBackStep)
        {
             
            BossPhase rightStep = new Step(stepName, Vector2.right * 3f, 20f, 10, 0.3f);
            BossPhase leftStep = new Step(stepName, Vector2.left * 3f, 20f, 10, 0.3f);
            if (Mathf.Abs(transform.position.x - Player.transform.position.x) >= targetDistance)
            {
                isBackStep = false;
                return transform.position.x > Player.transform.position.x ? leftStep : rightStep;
            }
            else
            {
                isBackStep = true;
                return transform.position.x > Player.transform.position.x ? rightStep : leftStep;
            }
        }

        public MaeHwaRampageRange InstantiateRampageRange(Vector2 rampagePos, Vector3 rotation)
        {
            return Instantiate(rampageRange, rampagePos,Quaternion.Euler(rotation)).GetComponent<MaeHwaRampageRange>();
        }
        /// <summary>
        /// Set Boss's LookingDir to Player direction
        /// </summary>
        private void SetLookingDir()
        {
            lookingDir = transform.position.x > Player.transform.position.x ? LookingDir.LeftDir : LookingDir.RightDir;
        }

        private void SetLookingDir(LookingDir dir)
        {
            lookingDir = dir;
        }
    }
}