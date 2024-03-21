using System;
using System.Collections;
using UnityEngine;

namespace Character.Player
{
    public class PlayerSpriteController : MonoBehaviour
    {
        private PlayerController m_PlayerController;
        private PlayerAttackController m_PlayerAttackController;
        private PlayerEffectController m_PlayerEffectController;
        private PlayerParticleController m_PlayerParticleController;

        private SpriteRenderer m_SpriteRenderer;
        private Animator m_Animator;

        public bool onComboAttackTime;
        public bool isFirstFrameAttackTrigger;
        
        private static readonly int Dash = Animator.StringToHash("Dash");
        private static readonly int Fly = Animator.StringToHash("Fly");
        private static readonly int DownSmash = Animator.StringToHash("DownSmash");
        private static readonly int Ball = Animator.StringToHash("Ball");
        private static readonly int Stun = Animator.StringToHash("Stun");
        private static readonly int Stagger = Animator.StringToHash("Stagger");
        private static readonly int MeleeSide1 = Animator.StringToHash("MeleeSide1");
        private static readonly int MeleeSide2 = Animator.StringToHash("MeleeSide2");
        private static readonly int MeleeUp = Animator.StringToHash("MeleeUp");
        private static readonly int MeleeDown = Animator.StringToHash("MeleeDown");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int Fall = Animator.StringToHash("Fall");
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Idle = Animator.StringToHash("Idle");

        private void Start()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_Animator = GetComponent<Animator>();
            try
            {
                var parent = transform.parent;
                m_PlayerController = parent.GetComponent<PlayerController>();
                m_PlayerAttackController = parent.Find("AttackController").GetComponent<PlayerAttackController>();
                m_PlayerEffectController = transform.Find("EffectController").GetComponent<PlayerEffectController>();
            }
            catch (Exception e)
            {
                Debug.LogError("PlayerAnimatorController : 플레이어 컨트롤러들 할당 실패");
            }
        }
        private void Update()
        {
            ResetAllTrigger();
            switch (m_PlayerController.m_PlayerActionStatus)
            {
                case PlayerController.ActionStatus.Normal:
                    switch (m_PlayerAttackController.nowAttackState)
                    {
                        case PlayerAttackController.AttackState.None:
                            var v = m_PlayerController.m_Rigidbody2D.velocity;
                            if (v.y >= 0.05f)
                            {
                                m_Animator.SetTrigger(Jump);
                            }
                            else if(v.y <= -0.05f)
                            {
                                m_Animator.SetTrigger(Fall);
                            }
                            else if(m_PlayerController.PlayerPlatformStatus == PlayerController.PlatformStatus.Flat)
                            {
                                m_Animator.SetTrigger(Mathf.Abs(v.x) >= 0.05f ? Walk : Idle);
                            }
                            break;
                        case PlayerAttackController.AttackState.MeleeRight:
                        case PlayerAttackController.AttackState.MeleeLeft:
                            if (!isFirstFrameAttackTrigger) break;
                            isFirstFrameAttackTrigger = false;
                            if (!onComboAttackTime)
                            {
                                m_Animator.SetTrigger(MeleeSide1);
                                m_PlayerEffectController.SetTrigger("MeleeSide1");
                            }
                            else
                            {
                                m_Animator.SetTrigger(MeleeSide2);
                                m_PlayerEffectController.SetTrigger("MeleeSide2");
                            }
                            break;
                        case PlayerAttackController.AttackState.MeleeUp:
                            if (!isFirstFrameAttackTrigger) break;
                            isFirstFrameAttackTrigger = false;
                            m_Animator.SetTrigger(MeleeUp);
                            m_PlayerEffectController.SetTrigger("MeleeUp");
                            break;
                        case PlayerAttackController.AttackState.MeleeDown:
                            if (!isFirstFrameAttackTrigger) break;
                            isFirstFrameAttackTrigger = false;
                            m_Animator.SetTrigger(MeleeDown);
                            m_PlayerEffectController.SetTrigger("MeleeDown");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case PlayerController.ActionStatus.Dash:
                    m_Animator.SetTrigger(Dash);
                    break;
                case PlayerController.ActionStatus.Fly:
                    m_Animator.SetTrigger(Fly);
                    break;
                case PlayerController.ActionStatus.DownSmash:
                    m_Animator.SetTrigger(DownSmash);
                    break;
                case PlayerController.ActionStatus.Ball:
                    m_Animator.SetTrigger(Ball);
                    break;
                case PlayerController.ActionStatus.Stun:
                    m_Animator.SetTrigger(Stun);
                    break;
                case PlayerController.ActionStatus.Stagger:
                    m_Animator.SetTrigger(Stagger);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        // set specific trigger
        public void SetTrigger(string triggerName)
        {
            ResetAllTrigger();
            m_Animator.SetTrigger(triggerName);
        }

        // reset all trigger
        private void ResetAllTrigger()
        {
            m_Animator.ResetTrigger(Dash);
            m_Animator.ResetTrigger(Fly);
            m_Animator.ResetTrigger(DownSmash);
            m_Animator.ResetTrigger(Ball);
            m_Animator.ResetTrigger(Stun);
            m_Animator.ResetTrigger(Stagger);
            m_Animator.ResetTrigger(MeleeSide1);
            m_Animator.ResetTrigger(MeleeSide2);
            m_Animator.ResetTrigger(MeleeUp);
            m_Animator.ResetTrigger(MeleeDown);
            m_Animator.ResetTrigger(Jump);
            m_Animator.ResetTrigger(Fall);
            m_Animator.ResetTrigger(Walk);
            m_Animator.ResetTrigger(Idle);
        }

        public void StartBlink(float maxDelaySecond = 0, float blinkDelay = 0.2f)
        {
            StartCoroutine(SpriteBlink(maxDelaySecond, blinkDelay));
        }
        private IEnumerator SpriteBlink(float maxDelaySecond = 0, float blinkDelay = 0.2f)
        {
            var nowDelaySecond = 0f;
            while (nowDelaySecond <= maxDelaySecond)
            {
                m_SpriteRenderer.color = Color.gray;
                yield return new WaitForSeconds(blinkDelay);
                m_SpriteRenderer.color = Color.white;
                yield return new WaitForSeconds(blinkDelay);
                nowDelaySecond += blinkDelay * 2;
            }
        }
    }
}