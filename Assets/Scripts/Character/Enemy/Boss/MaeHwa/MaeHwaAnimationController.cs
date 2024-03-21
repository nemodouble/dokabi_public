using System;
using System.Collections;
using UnityEngine;

namespace Boss.MaeHwa
{
    public class MaeHwaAnimationController : MonoBehaviour
    {
        private MaeHwaController maeHwaController;
        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();
            maeHwaController = GetComponent<MaeHwaController>();
        }

        void Update()
        {
            StartCoroutine(SetTrigger(PhaseNameToTrigger(maeHwaController.prevPhase)));
        }

        private string PhaseNameToTrigger(string phaseName)
        {
            switch (phaseName)
            {
                case "Down-StartStep":
                case "FrontStep":
                case "BackStep":
                    return maeHwaController.isBackStep ? "BackStep" : "FrontStep";
                
                
                case "Select-Attack":
                case "EndPhase":
                    return "EndPhase";
                
                
                case "Horizon-Step":
                case "Horizon-BeforeWait":
                case "Horizon-Attack":
                    
                case "Body-Dash":
                case "Body-AfterDashWait":
                case "Body-AfterAttackWait":
                    
                case "Combo-First-BeforeWait":
                case "Combo-First-DashOrWait":                
                case "Combo-First-AfterWait":
                case "Combo-Second-BeforeWait":
                case "Combo-Second-DashOrWait":
                case "Combo-Second-AfterWait":
                case "Combo-Third-BeforeWait":
                case "Combo-Third-Attack":
                    
                case "Rampage-Rise":
                     
                case "Down-AirWait":
                case "Down-GetAccel":
                case "Down-SmashWait":
                    
                case "Start-Wait":
                case "Walk":
                    
                case "Dead":
                    return phaseName;
                
                
                default:
                    return "null";
            }
        }
        
        private IEnumerator SetTrigger(string triggerName)
        {
            animator.SetTrigger(triggerName);
            yield return null;
            animator.ResetTrigger(triggerName);
        }
    }
}
