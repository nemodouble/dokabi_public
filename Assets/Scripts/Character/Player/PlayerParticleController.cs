using System;
using System.Collections;
using UnityEngine;

namespace Character.Player
{
    public class PlayerParticleController : MonoBehaviour
    {
        private PlayerController playerController;
        
        private ParticleSystem walkPS;
        private ParticleSystem flyPS;
        private ParticleSystem dashPS;
        private ParticleSystem platDashPS;
        private ParticleSystem jumpPS;

        private void Start()
        {
            playerController = transform.parent.parent.GetComponent<PlayerController>();
            
            walkPS = transform.Find("WalkPS").GetComponent<ParticleSystem>();
            flyPS = transform.Find("FlyPS").GetComponent<ParticleSystem>();
            dashPS = transform.Find("DashPS").GetComponent<ParticleSystem>();
            platDashPS = transform.Find("PlatDashPS").GetComponent<ParticleSystem>();
            jumpPS = transform.Find("JumpPS").GetComponent<ParticleSystem>();
        }
        
        public void PlayWalkPS()
        {
            walkPS.Play();
        }
        
        public void StopWalkPS()
        {
            walkPS.Stop();
        }
        
        public void PlayFlyPS()
        {
            flyPS.Play();
        }
        
        public void StopFlyPS()
        {
            flyPS.Stop();
        }
        
        public void PlayDashPS()
        {
            dashPS.Play();
            if(playerController.PlayerPlatformStatus == PlayerController.PlatformStatus.Flat)
                platDashPS.Play();
        }
        
        public void StopDashPS()
        {
            dashPS.Stop();
            platDashPS.Stop();
        }

        public void PlayJumpPS()
        {
            jumpPS.Play();
        }
    }
}