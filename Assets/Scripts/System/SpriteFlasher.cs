using System.Collections;
using UnityEngine;

namespace System
{
    public class SpriteFlasher : MonoBehaviour
    {
        [SerializeField] private Material flashMaterial;
        [SerializeField] private float duration = 0.5f;
        private float m_NowDuration;

        private SpriteRenderer m_SpriteRenderer;
        private Material m_OriginMaterial;
        private Coroutine m_FlashRoutine;
        private static readonly int FlashPercent = Shader.PropertyToID("FlashPercent");

        private void Start()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_OriginMaterial = m_SpriteRenderer.material;
        }

        public void Flash()
        {
            if (m_FlashRoutine != null)
            {
                StopCoroutine(m_FlashRoutine);
            }

            m_FlashRoutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            m_NowDuration = 0;
            m_SpriteRenderer.material = flashMaterial;
            while(m_NowDuration <= duration)
            {
                // m_SpriteRenderer.material.SetFloat(FlashPercent, 1 - m_NowDuration / duration);
                m_NowDuration += Time.deltaTime;
                yield return null;
            }
            m_SpriteRenderer.material = m_OriginMaterial;
            m_FlashRoutine = null;
        }
    }
}