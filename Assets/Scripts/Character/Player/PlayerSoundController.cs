using System;
using FMODUnity;
using UnityEngine;

namespace Character.Player
{
    public class PlayerSoundController : MonoBehaviour
    {
        private PlayerController playerController;

        private float footstepCool = 0;
        
        public float footstepInterval = 100f;
        
        private void Start()
        {
            playerController = transform.parent.GetComponent<PlayerController>();
        }

        private void FixedUpdate()
        {
            if (playerController.PlayerPlatformStatus == PlayerController.PlatformStatus.Flat && playerController.m_PlayerActionStatus == PlayerController.ActionStatus.Normal)
            {
                footstepCool += Math.Abs(playerController.m_Rigidbody2D.velocity.x);
                if (footstepCool > footstepInterval)
                {
                    footstepCool = 0;
                    RuntimeManager.PlayOneShot("event:/character/move/character_walk");
                }
            }
        }
        
        // play dash sound
        public void PlayDashSound()
        {
            RuntimeManager.PlayOneShot("event:/character/move/character_dash");
        }
        
        // play jump sound
        public void PlayJumpSound()
        {
            RuntimeManager.PlayOneShot("event:/character/move/character_jump");
        }
        
        // play land sound
        public void PlayLandSound()
        {
            RuntimeManager.PlayOneShot("event:/character/move/character_land");
        }
        
        // play flight sound
        public void PlayFlightSound()
        {
            RuntimeManager.PlayOneShot("event:/character/move/character_flight");
        }
        
        // play damage sound
        public void PlayDamageSound()
        {
            RuntimeManager.PlayOneShot("event:/character/attack/character_damage");
        }
        
        // play slash sound
        public void PlaySlashSound()
        {
            RuntimeManager.PlayOneShot("event:/character/attack/character_slash");
        }
    }
}