using System;
using System.Collections;
using UnityEngine;
using Util;
using Object = System.Object;

namespace Character.Player
{
    public class PlayerEffectController : MonoBehaviour
    {
        private PlayerController m_PlayerController;
        private Animator m_Animator;
        private static readonly int MeleeUpShort = Animator.StringToHash("MeleeUp-Short");
        private static readonly int MeleeSide1Short = Animator.StringToHash("MeleeSide1-Short");
        private static readonly int MeleeSide2Short = Animator.StringToHash("MeleeSide2-Short");
        private static readonly int MeleeDownShort = Animator.StringToHash("MeleeDown-Short");
        private static readonly int Hit = Animator.StringToHash("Hit");

        public GameObject m_JumpEffect;
        private GameObject m_JumpEffectInstance;
        public GameObject m_LandEffect;
        private GameObject m_LandEffectInstance;

        private void Start()
        {
            m_Animator = GetComponent<Animator>();
            m_PlayerController = transform.parent.parent.GetComponent<PlayerController>();

            m_JumpEffectInstance = Instantiate(m_JumpEffect);
            m_LandEffectInstance = Instantiate(m_LandEffect);
        }

        public void SetTrigger(string id)
        {
            switch (m_PlayerController.playerMeleeType)
            {
                case PlayerController.MeleeType.Short:
                    switch (id)
                    {
                        case "MeleeUp":
                            m_Animator.SetTrigger(MeleeUpShort);
                            break;
                        case "MeleeSide1":
                            m_Animator.SetTrigger(MeleeSide1Short);
                            break;
                        case "MeleeSide2":
                            m_Animator.SetTrigger(MeleeSide2Short);
                            break;
                        case "MeleeDown":
                            m_Animator.SetTrigger(MeleeDownShort);
                            break;
                        case "Hit":
                            m_Animator.SetTrigger(Hit);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            StartCoroutine(ReSetAllTrigger());
        }

        private IEnumerator ReSetAllTrigger()
        {
            yield return null;
            m_Animator.ResetTrigger(MeleeUpShort);
            m_Animator.ResetTrigger(MeleeSide1Short);
            m_Animator.ResetTrigger(MeleeSide2Short);
            m_Animator.ResetTrigger(MeleeDownShort);
            m_Animator.ResetTrigger(Hit);
        }
        
        // jump effect
        public void JumpEffect()
        {
            m_JumpEffectInstance.SetActive(true);
            m_JumpEffectInstance.transform.position = transform.position;
        }
        
        // land effect
        public void LandEffect()
        {
            m_LandEffectInstance.SetActive(true);
            m_LandEffectInstance.transform.position = transform.position;
        }
    }
}