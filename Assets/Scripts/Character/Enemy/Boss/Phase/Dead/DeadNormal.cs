using System;
using System.Collections;
using Audio;
using UnityEngine;

namespace Boss.Phase
{
    public class DeadNormal : BossPhase
    {
        public DeadNormal(string phaseName, int rarity = 1) : base(phaseName, rarity)
        {
        }

        protected internal override IEnumerator DoPhase(BossController bossController)
        {
            //tmp-start
            GameObject.Find("BGMPlayer").GetComponent<BgmManager>().StopBGM();
            //tmp-end
            
            var boss = bossController.gameObject;
            boss.layer = LayerMask.NameToLayer("Dummy");
            boss.tag = "Dummy";
            yield return null;
        }
    }
}