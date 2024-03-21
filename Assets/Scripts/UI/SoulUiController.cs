using System;
using UnityEngine;

namespace UI
{
    public class SoulUiController : MonoBehaviour
    {
        public float soul = 0.5f;
        public float fullHeight = 120f;
        public float fullPosY = 340f;
        
        private RectTransform rt;
        private float soulBgAmount;

        private void Start()
        {
            rt = GetComponent<RectTransform>();
        }

        private void Update()
        {
            soulBgAmount = soul - 0.22f;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, fullHeight * soulBgAmount);
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, fullPosY - fullHeight * (1 - soulBgAmount) * 0.5f);
        }
    }
}