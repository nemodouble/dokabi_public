using System;
using System.Collections;
using System.Collections.Generic;
using SceneManagement;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Util;

namespace Character.Player
{
    public class PlayerController : MonoBehaviour, IDataPersister
    {
        private const float Accelerate = 100;
        private float m_Decelerate = 50;

        private const float WalkSpeed = 8.6f;

        [Header("점프")]
        public float jumpSpeed = 980;
        public float fallGravityScale = 2f;
        public float jumpBufferLength = 0.5f;
        

        [Header("벽 점프")]
        public float wallDragSpeed = 0.5f;
        public float wallJumpLengthMax = 0.1f;
        public float wallJumpSpeed = 1f;

        [Header("코요테 타임")]
        public float coyoteTime = 0.1f;
    
        [Header("대쉬")]
        public float dashAccel = 600f;
        public float dashLengthMax = 0.08f;
        public float dashCoolMax = 0.3f;

        [Header("아래찍기")]
        public float downSmashBeforeDelay = 0.2f;
        public float downSmashSpeed = 30f;
        public float downSmashAfterDelay = 0.3f;

        [Header("굴러가기")]
        public float ballMass = 10;
        public float ballLinearDrag = 0.1f;
        public float ballAngularDrag;
        public float ballAccel = 20f;
        public float ballSpeedMax = 20f;
        public float ballCoolMax = 0.1f;

        [Header("비행")]
        public float flyingTimeMax = 0.3f;
        public float flyingSpeed = 15f;
        public float flyingStartAccel = 700;
        public float flyingTimeReduceScale = 0.5f;
        
        public enum MeleeType
        {
            Short,
        }
        [Header("공격")] 
        public MeleeType playerMeleeType = MeleeType.Short;

        [Header("피격")]
        public float invincibleTimeMax = 1.5f;
        public float staggerTimeMax = 0.15f;
        public float knockBackSpeed = 5;
        public int hp = 7;
        
        public Rigidbody2D m_Rigidbody2D
        {
            get;
            private set;
        }

        public enum ActionStatus
        {
            Normal,
            Dash,
            Fly,
            DownSmash,
            Ball,
            Stun,
            /// <summary>
            /// 피해 입어 움직일 수 없는 상태
            /// </summary>
            Stagger,
        }

        public ActionStatus m_PlayerActionStatus
        {
            get;
            private set;
        }
        private Coroutine m_NowStateCoroutine;
        
        public enum JumpStatus
        {
            CanJump,
            CantJump,
            JumpBuffered,
            Jumping
        }

        public JumpStatus m_PlayerJumpStatus
        {
            get;
            private set;
        }
        
        public enum PlatformStatus
        {
            Flat,
            OverSlope,
            NotOnPlatform,
        }

        public PlatformStatus PlayerPlatformStatus
        {
            get;
            private set;
        }
        private bool m_IsNearJumpAble;
        private bool m_CanCoyoteJump;
        private float m_PlatformOffTime;
        
        // 캐릭터 조작
        private bool m_HaveControl;
        public bool m_CanDecelX = true;
        
        // 캐릭터 이동
        private Vector2 m_MoveDir;
        
        // 캐릭터 액션
        // 비행
        private bool m_CanFly = true;
        private bool m_CanFlyAccel = true;
        private float m_FlyTimeNow;
        // 대쉬
        private bool m_IsDashCool;
        private bool m_CanAirDash = true;
        // 공격
        public PlayerAttackController m_AttackController
        {
            get;
            private set;
        }
        private bool m_IsAttackCool;
        // 정화
        private bool m_CanCleanse = true;
        private bool m_IsCleansed = false;
        private float m_CleanseDuration = 0.5f;
        private float m_CleanseCoolTime = 2f;
        
        // 캐릭터 상태
        public int LookingDir
        {
            get;
            private set;
        }
        private bool m_IsInvincible;
        private bool m_CanInitStunTime;
        

        // 물리 관련
        private float m_OriginGravityScale;
        public float platformCheckDistance = 0.4f;
        public float platformCheckWeight = 0.51f;
       
        private PlayerSpriteController m_SpriteController;
        private PlayerEffectController m_EffectController;
        private PlayerParticleController m_ParticleController;
        private PlayerSoundController m_SoundController;
        private float m_OriginDrag;

        private void Start()
        {
            m_Rigidbody2D = GetComponent<Rigidbody2D>();

            m_AttackController = transform.Find("AttackController").GetComponent<PlayerAttackController>();
            m_SpriteController = transform.Find("AnimationController").GetComponent<PlayerSpriteController>();
            m_SoundController = transform.Find("SoundController").GetComponent<PlayerSoundController>();
            m_EffectController = transform.Find("AnimationController").Find("EffectController").GetComponent<PlayerEffectController>();
            m_ParticleController = transform.Find("AnimationController").Find("ParticleController").GetComponent<PlayerParticleController>();
            
            m_PlayerActionStatus = ActionStatus.Normal;
            m_PlayerJumpStatus = JumpStatus.CanJump;
            m_HaveControl = true;

            m_OriginGravityScale = m_Rigidbody2D.gravityScale;

            m_NowStateCoroutine = StartCoroutine(NormalState());
        }

        // 매 프레임 플랫폼 상태, 점프 상태 확인
        private void Update()
        {
            PlayerPlatformStatus = CheckPlatformState();
            m_PlayerJumpStatus = CheckJumpState();
        }
  
        // 좌 스틱 방향 입력
        public void OnMove(InputAction.CallbackContext context)
        {
            m_MoveDir = context.ReadValue<Vector2>();
        }

        // 근접 공격 시도
        public void OnMeleeAttack(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            if(m_PlayerActionStatus == ActionStatus.Fly || m_PlayerActionStatus == ActionStatus.Ball)
            {
                ChangeActionState(ActionStatus.Normal);
            }
            if (m_PlayerActionStatus != ActionStatus.Normal) return;
            
            m_AttackController.TryMeleeAttack(m_MoveDir);
        }
        
        // 점프 시도
        public void OnJump(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                switch (m_PlayerJumpStatus)
                {
                    case JumpStatus.CanJump:
                        Jump();
                        break;
                    case JumpStatus.CantJump when Time.time - m_PlatformOffTime < coyoteTime && m_CanCoyoteJump:
                        m_CanCoyoteJump = false;
                        Jump();
                        break;
                    case JumpStatus.CantJump when m_IsNearJumpAble:
                        m_PlayerJumpStatus = JumpStatus.JumpBuffered;
                        break;
                    case JumpStatus.CantJump when CanChangeActionState(ActionStatus.Fly):
                        ChangeActionState(ActionStatus.Fly);
                        m_ParticleController.PlayFlyPS();
                        break;
                    case JumpStatus.CantJump:
                        break;
                    case JumpStatus.JumpBuffered:
                        break;
                    case JumpStatus.Jumping:
                        break;
                    default:
                        Debug.Log(m_PlayerJumpStatus);
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (context.canceled)
            {
                if (m_PlayerJumpStatus == JumpStatus.Jumping)
                {
                    StopJump();
                }
                if (m_PlayerActionStatus == ActionStatus.Fly)
                {
                    m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
                    ChangeActionState(ActionStatus.Normal);
                }

                if (m_PlayerJumpStatus == JumpStatus.JumpBuffered)
                {
                    m_PlayerJumpStatus = JumpStatus.CantJump;
                }
            }
        }
        
        // 대쉬 시도
        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.performed && !m_IsDashCool && (PlayerPlatformStatus == PlatformStatus.Flat || m_CanAirDash))
            {
                if (CanChangeActionState(ActionStatus.Dash))
                {
                    ChangeActionState(ActionStatus.Dash);
                    m_ParticleController.PlayDashPS();
                }
            }
        }
        
        // 정화 시도
        public void OnCleanse(InputAction.CallbackContext context)
        {
            if (context.performed && m_CanCleanse)
            {
                StartCoroutine(Cleanse());
            }
        }
        
        private IEnumerator Cleanse()
        {
            Debug.Log("정화 시도");
            if(m_PlayerActionStatus == ActionStatus.Stun)
            {
                Debug.Log("정화됨");
                ChangeActionState(ActionStatus.Normal);
            }
            // m_EffectController.PlayCleanseEffect();
            // m_ParticleController.PlayCleansePS();
            m_CanCleanse = false;
            m_IsCleansed = true; 
            yield return new WaitForSeconds(m_CleanseDuration);
            m_IsCleansed = false;
            yield return new WaitForSeconds(m_CleanseCoolTime - m_CleanseDuration);
            m_CanCleanse = true;
            Debug.Log("정화 준비됨");
        }

        public void OnAction(InputAction.CallbackContext context)
        {
            
        }

        private PlatformStatus CheckPlatformState()
        {
            var rayBoxSize = new Vector2(platformCheckWeight, 0.05f);
            var rayLayerMask = LayerMask.GetMask("Platform");
            var rayHit = Debugger.BoxCast(transform.position, rayBoxSize, 0f, Vector2.down,
                platformCheckDistance + jumpBufferLength, rayLayerMask);
            
            m_IsNearJumpAble = rayHit.distance > platformCheckDistance;
            
            if (rayHit.collider == null || rayHit.distance > platformCheckDistance)
            {
                m_ParticleController.StopWalkPS();
                return PlatformStatus.NotOnPlatform;
            }
            if (rayHit.normal.x == 0)
            {
                m_CanAirDash = true;
                m_CanCoyoteJump = true;
                m_ParticleController.PlayWalkPS();
                if(PlayerPlatformStatus != PlatformStatus.Flat)
                    Land();
                return PlatformStatus.Flat;
            }
            else
            {
                m_ParticleController.PlayWalkPS();
                return PlatformStatus.OverSlope;
            }
        }

        private JumpStatus CheckJumpState()
        {
            if (!m_HaveControl) return JumpStatus.CantJump;
            // 점프 버퍼 처리
            if (m_PlayerJumpStatus == JumpStatus.JumpBuffered)
            {
                if(PlayerPlatformStatus == PlatformStatus.Flat)
                    Jump();
                return m_PlayerJumpStatus;
            }

            // 플랫폼 확인
            switch (PlayerPlatformStatus)
            {
                case PlatformStatus.Flat when m_PlayerJumpStatus == JumpStatus.CantJump:
                    m_CanFly = true;
                    return JumpStatus.CanJump;
                
                case PlatformStatus.NotOnPlatform
                    when m_Rigidbody2D.velocity.y < 0 || m_PlayerJumpStatus != JumpStatus.Jumping:
                    if(m_PlayerJumpStatus == JumpStatus.CanJump)
                        m_PlatformOffTime = Time.time;
                    return JumpStatus.CantJump;
                
                case PlatformStatus.OverSlope:
                default:
                    return m_PlayerJumpStatus;
            }
        }

        private void Land()
        {
            m_SoundController.PlayLandSound();
            m_EffectController.LandEffect();
        }
        
        private void Jump()
        {
            m_PlayerJumpStatus = JumpStatus.Jumping;
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
            m_Rigidbody2D.AddForce(jumpSpeed * Vector2.up);
            m_SoundController.PlayJumpSound();
            m_EffectController.JumpEffect();
            m_ParticleController.PlayJumpPS();
        }
        
        private void StopJump()
        {
            m_PlayerJumpStatus = JumpStatus.CantJump;
            m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);
        }

        private IEnumerator NormalState()
        {
            while (m_PlayerActionStatus == ActionStatus.Normal)
            {
                // 조작감-낙하 중력 강화
                if (m_Rigidbody2D.velocity.y < 0)
                    m_Rigidbody2D.gravityScale = m_OriginGravityScale * fallGravityScale;
                else
                    m_Rigidbody2D.gravityScale = m_OriginGravityScale;
                
                // 플레이어 처다보는 방향 변경
                if (m_MoveDir.x != 0 && m_AttackController.nowAttackState == PlayerAttackController.AttackState.None)
                {
                    LookingDir = m_MoveDir.x > 0 ? 1 : -1;
                    transform.localScale = new Vector3(LookingDir, 1, 1);
                }
                
                // x축 가속 감속
                if (PlayerPlatformStatus != PlatformStatus.OverSlope)
                {
                    // 가속
                    if (m_MoveDir.x != 0)
                    {
                        if (Math.Abs(m_Rigidbody2D.velocity.x) < WalkSpeed)
                        {
                            m_Rigidbody2D.AddForce(Accelerate * m_MoveDir.x * Vector2.right);
                        }
                        else if(m_CanDecelX)
                        {
                            m_Rigidbody2D.velocity = new Vector2(m_MoveDir.x * WalkSpeed, m_Rigidbody2D.velocity.y);
                        }
                    }
                    // 감속
                    else
                    {
                        if (m_Rigidbody2D.velocity.x != 0 && m_CanDecelX)
                        {
                            m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
                        }
                    }
                }
                yield return new WaitForFixedUpdate();
            }
        }
        
        private IEnumerator FlyState()
        {
            // Fly 시작 처리
            m_Rigidbody2D.gravityScale = 0;
            m_CanFly = false;
            m_SoundController.PlayFlightSound();
            
            while (true)
            {
                var force = m_MoveDir.normalized * flyingSpeed - m_Rigidbody2D.velocity;
                m_Rigidbody2D.AddForce(force * 10f);
                
                // 시작 가속
                if (m_MoveDir != Vector2.zero && m_CanFlyAccel)
                {
                    m_Rigidbody2D.AddForce(m_MoveDir * flyingStartAccel);
                    m_CanFlyAccel = false;
                }

                if (m_MoveDir == Vector2.zero)
                    m_FlyTimeNow += Time.deltaTime *  flyingTimeReduceScale;
                else
                    m_FlyTimeNow += Time.deltaTime;
                if (m_FlyTimeNow >= flyingTimeMax)
                    ChangeActionState(ActionStatus.Normal);
                
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator DashState()
        {
            // Dash 시작 처리
            m_OriginDrag = m_Rigidbody2D.drag;
            m_Rigidbody2D.drag = m_OriginDrag / 3;
            m_Rigidbody2D.gravityScale = 0;
            m_CanAirDash = false;
            m_IsDashCool = true;
            m_CanDecelX = false;
            m_SoundController.PlayDashSound();
            
            var startTime = Time.time;
            
            StartCoroutine(SetDashBool( 0.16f));
            m_Rigidbody2D.AddForce(new Vector2(LookingDir * dashAccel, 0));

            while (true)
            {
                if (Time.time - startTime > dashLengthMax)
                {
                    ChangeActionState(ActionStatus.Normal);
                }
                yield return new WaitForFixedUpdate();
            }
        }
        
        private IEnumerator SetDashBool(float delaySecond = 0)
        {
            yield return new WaitForSeconds(delaySecond);
            m_CanDecelX = true;
            yield return new WaitForSeconds(dashCoolMax - delaySecond);
            m_IsDashCool = false;
        }
        
        private IEnumerator StaggerState(int attackDamage, Vector2 attackDir, float attackForceScale = 1)
        {
            hp -= attackDamage;
            HpUiController.Instace.SetHpUi(hp);
            m_PlayerActionStatus = ActionStatus.Stagger;
            m_SoundController.PlayDamageSound();
            m_EffectController.SetTrigger("Hit");
            TimeScaleController.Instance.SetTemporaryTimeScale(0.1f, 0.1f);

            var nowStaggerTime = 0f;
            m_HaveControl = false;
            StartCoroutine(ApplyInvisible(invincibleTimeMax));
            while (nowStaggerTime <= staggerTimeMax)
            {
                // 플레이어가 피격시 피격방향으로 튕겨나가는 속도
                m_Rigidbody2D.velocity = attackDir * knockBackSpeed + Vector2.up * knockBackSpeed / 2;
                nowStaggerTime += Time.deltaTime;
                yield return null;
            }
            ChangeActionState(ActionStatus.Normal);
        }

        private IEnumerator ApplyInvisible(float maxDelaySecond = 0)
        {
            m_SpriteController.StartBlink(maxDelaySecond);
            
            var playerLayer = LayerMask.NameToLayer("Player");
            var enemyLayer = LayerMask.NameToLayer("Enemy");
            
            m_IsInvincible = true;
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
            
            yield return new WaitForSeconds(maxDelaySecond);
            
            m_IsInvincible = false;
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        }

        public void StartStunState(float stunTime, bool canInitStunTime = false)
        {
            m_CanInitStunTime = canInitStunTime;
            if (CanChangeActionState(ActionStatus.Stun))
                StartCoroutine(StunState(stunTime));
        }
        
        // Player stun state
        private IEnumerator StunState(float stunTime)
        {
            m_PlayerActionStatus = ActionStatus.Stun;
            m_HaveControl = false;
            m_Rigidbody2D.velocity = Vector2.zero;
            yield return new WaitForSeconds(stunTime);
            ChangeActionState(ActionStatus.Normal);
        }

        /// <summary>
        /// 현재 액션 상태에서 targetState로의 전환이 가능한지 여부를 반환합니다.
        /// </summary>
        /// <param name="targetState">목표 상태</param>
        /// <returns>변환 가능 여부</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool CanChangeActionState(ActionStatus targetState)
        {
            switch (targetState)
            {
                case ActionStatus.Normal:
                    return true;
                case ActionStatus.Dash:
                    if (!m_CanAirDash)
                        return false;
                    switch (m_PlayerActionStatus)
                    {
                        case ActionStatus.Normal:
                        case ActionStatus.Fly:
                        case ActionStatus.Ball:
                            return true;
                        case ActionStatus.Dash:
                        case ActionStatus.DownSmash:
                        case ActionStatus.Stagger:
                        case ActionStatus.Stun:
                            return false;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ActionStatus.Fly:
                    if (!m_CanFly || PlayerPlatformStatus != PlatformStatus.NotOnPlatform)
                        return false;
                    switch (m_PlayerActionStatus)
                    {
                        case ActionStatus.Normal:
                        case ActionStatus.Ball:
                            return true;
                        case ActionStatus.Dash:
                        case ActionStatus.Fly:
                        case ActionStatus.DownSmash:
                        case ActionStatus.Stun:
                        case ActionStatus.Stagger:
                            return false;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ActionStatus.DownSmash:
                    switch (m_PlayerActionStatus)
                    {
                        case ActionStatus.Normal:
                        case ActionStatus.Ball:
                        case ActionStatus.Fly:
                            return true;
                        case ActionStatus.Dash:
                        case ActionStatus.DownSmash:
                        case ActionStatus.Stun:
                        case ActionStatus.Stagger:
                            return false;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ActionStatus.Ball:
                    switch (m_PlayerActionStatus)
                    {
                        case ActionStatus.Normal:
                        case ActionStatus.Fly:
                            return true;
                        case ActionStatus.Dash:
                        case ActionStatus.DownSmash:
                        case ActionStatus.Ball:
                        case ActionStatus.Stun:
                        case ActionStatus.Stagger:
                            return false;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ActionStatus.Stun:
                    if (m_IsCleansed) return false;
                    switch (m_PlayerActionStatus)
                    {
                        case ActionStatus.Normal:
                        case ActionStatus.Dash:
                        case ActionStatus.Ball:
                        case ActionStatus.Fly:
                            return true;
                        case ActionStatus.Stun:
                            return m_CanInitStunTime;
                        case ActionStatus.DownSmash:
                        case ActionStatus.Stagger:
                            return false;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ActionStatus.Stagger:
                    if (m_IsInvincible) return false;
                    switch (m_PlayerActionStatus)
                    {
                        case ActionStatus.Normal:
                        case ActionStatus.Dash:
                        case ActionStatus.Fly:
                        case ActionStatus.Ball:
                        case ActionStatus.Stun:
                            return true;
                        case ActionStatus.DownSmash:
                        case ActionStatus.Stagger:
                            return false;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetState), targetState, null);
            }
        }

        /// <summary>
        /// 현재 ActionState를 종료합니다. 이후 반드시 m_NowStateCoroutine를 할당해야 합니다.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">해당 state의 종료가 구현되지 않음</exception>
        private void StopNowActionState()
        {
            // 현 state 종료
            switch (m_PlayerActionStatus)
            {
                case ActionStatus.Normal:
                    m_ParticleController.StopWalkPS();
                    break;
                case ActionStatus.Dash:
                    m_Rigidbody2D.gravityScale = m_OriginGravityScale;
                    m_Rigidbody2D.drag = m_OriginDrag;
                    m_ParticleController.StopDashPS();
                    break;
                case ActionStatus.Fly:
                    m_FlyTimeNow = 0;
                    m_CanFlyAccel = true;
                    m_Rigidbody2D.gravityScale = m_OriginGravityScale;
                    m_ParticleController.StopFlyPS();
                    break;
                case ActionStatus.DownSmash:
                    break;
                case ActionStatus.Ball:
                    break;
                case ActionStatus.Stun:
                    m_HaveControl = true;
                    break;
                case ActionStatus.Stagger:
                    m_HaveControl = true;
                    m_Rigidbody2D.velocity = Vector2.zero;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            StopCoroutine(m_NowStateCoroutine);
        }
        
        /// <summary>
        /// StopNowActionState()을 실행하고 새 액션작업을 시작합니다.
        /// 액션 전환 가능 여부를 CanChangeActionState통해 확인 해야합니다. (이 함수 내부에서는 확인하지 않습니다.)
        /// </summary>
        /// <param name="actionStatus">목표 액션 상태</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void ChangeActionState(ActionStatus actionStatus)
        {
            // 현재 ActionState 종료에 필요한 처리
            StopNowActionState();
            
            // 다음 state 시작
            m_PlayerActionStatus = actionStatus;
            switch (actionStatus)
            {
                case ActionStatus.Normal:
                    m_NowStateCoroutine = StartCoroutine(NormalState());
                    break;
                case ActionStatus.Dash:
                    m_NowStateCoroutine = StartCoroutine(DashState());
                    break;
                case ActionStatus.Fly:
                    m_NowStateCoroutine = StartCoroutine(FlyState());
                    break;
                case ActionStatus.DownSmash:
                    // m_NowStateCoroutine = StartCoroutine();
                    break;
                case ActionStatus.Ball:
                    // m_NowStateCoroutine = StartCoroutine();
                    break;
                case ActionStatus.Stun:
                    Debug.LogError("Stun 상태는 ChangeActionState를 통해 전환할 수 없습니다.");
                    throw new ArgumentOutOfRangeException(nameof(actionStatus), actionStatus, null);
                case ActionStatus.Stagger:
                    Debug.LogError("Stagger 상태는 ChangeActionState를 통해 전환할 수 없습니다.");
                    throw new ArgumentOutOfRangeException(nameof(actionStatus), actionStatus, null);
                default:
                    throw new ArgumentOutOfRangeException(nameof(actionStatus), actionStatus, null);
            }
        }

        public void Hit(int attackDamage, Vector2 attackDir, float attackForceScale = 1)
        {
            if(CanChangeActionState(ActionStatus.Stagger))
            {
                StartStaggerState(attackDamage, attackDir, attackForceScale);
            }
        }
        
        private void StartStaggerState(int attackDamage, Vector2 attackDir, float attackForceScale = 1)
        {
            StopNowActionState();
            m_NowStateCoroutine = StartCoroutine(StaggerState(attackDamage, attackDir, attackForceScale));
        }
        
        // 피격 방향 가져오기
        public Vector2 GetAttackedDir(Vector2 attackerPosition)
        {
            Vector2 dir;
            dir.x = attackerPosition.x > transform.position.x ? -1 : 1;
            dir.y = attackerPosition.y > transform.position.y ? 1 : -1;
            return dir;
        }

        public DataSettings GetDataSettings()
        {
            throw new NotImplementedException();
        }

        public void SetDataSettings(string dataTag, DataSettings.PersistenceType persistenceType)
        {
            throw new NotImplementedException();
        }

        public Data SaveData()
        {
            throw new NotImplementedException();
        }

        public void LoadData(Data data)
        {
            throw new NotImplementedException();
        }
    }
}