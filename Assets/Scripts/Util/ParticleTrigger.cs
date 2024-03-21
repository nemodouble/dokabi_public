using System;
using UnityEngine;

namespace Character.Enemy.Boss.MaeHwa
{
    public class ParticleTrigger : MonoBehaviour
    {
        public ParticleSystem particle;
        
        public bool triggerOnCollision = true;
        public bool triggerOnTrigger = true;
        
        public bool playOnce = true;
        public bool hasPlayed;

        private void Start()
        {
            if(particle == null)
                particle = GetComponent<ParticleSystem>();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (triggerOnTrigger && !hasPlayed || !playOnce)
            {
                particle.Play();
                hasPlayed = true;
            }
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (triggerOnCollision && !hasPlayed || !playOnce)
            {
                particle.Play();
                hasPlayed = true;
            }
        }

        public void Reset()
        {
            hasPlayed = false;
        }
    }
}