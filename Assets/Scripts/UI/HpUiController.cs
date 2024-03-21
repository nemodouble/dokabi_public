using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class HpUiController : MonoBehaviour
    {
        public static HpUiController Instace
        {
            get { return s_Instance; }
        }
        private static HpUiController s_Instance;
        

        public int maxHP;
        private int nowHP;
        private List<Animator> HeartAnimators;
        
        private static readonly int IsAlive = Animator.StringToHash("IsAlive");

        private void Start()
        {
            HeartAnimators = new List<Animator>();
            nowHP = maxHP;
            for (var i = 0; i < transform.childCount; i++)
            {
                HeartAnimators.Add(transform.GetChild(i).GetComponent<Animator>());
            }
        }
        void Awake ()
        {
            if (s_Instance == null)
                s_Instance = this;
            else
                throw new UnityException("There cannot be more than one HeartUIController script.  The instances are " + s_Instance.name + " and " + name + ".");
        }

        void OnEnable()
        {
            if (s_Instance == null)
                s_Instance = this;
            else if(s_Instance != this)
                throw new UnityException("There cannot be more than one HeartUIController script.  The instances are " + s_Instance.name + " and " + name + ".");
        
        }
        
        public void SetHpUi(int hp)
        {
            nowHP = hp;
            for (var i = 0; i < HeartAnimators.Count; i++)
            {
                if (i < nowHP)
                {
                    HeartAnimators[i].SetBool(IsAlive, true);
                }
                else
                {
                    HeartAnimators[i].SetBool(IsAlive, false);
                }
            }
        }
        
        public void HpIncrease(int amount)
        {
            for(var i = 1; i<=amount; i++)
            {
                HeartAnimators[nowHP + i - 1].SetBool(IsAlive, true);
            }

            nowHP += amount;
        }

        public void HpDecrease(int amount)
        {
            for(var i = 0; i<amount; i++)
            {
                HeartAnimators[nowHP - i - 1].SetBool(IsAlive, false);
            }

            nowHP -= amount;
        }
    }
}
