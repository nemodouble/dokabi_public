using System.Collections;
using UnityEngine;

namespace System
{
    public class TimeScaleController : MonoBehaviour
    {
        // Singleton instance
        private static TimeScaleController instance;
        public static TimeScaleController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<TimeScaleController>();
                }
                if (instance == null)
                {
                    var go = new GameObject("TimeScaleController");
                    instance = go.AddComponent<TimeScaleController>();
                }
                return instance;
            }
        }
        
        // Singleton awake 
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        // time scale
        public float TimeScale { get; private set; }
        
        // apply time scale
        public void SetTimeScale(float timeScale)
        {
            TimeScale = timeScale;
            Time.timeScale = timeScale;
        }
        
        // temporary time scale
        public void SetTemporaryTimeScale(float timeScale, float duration)
        {
            StartCoroutine(SetTemporaryTimeScaleCoroutine(timeScale, duration));
        }
        
        // temporary time scale coroutine
        private IEnumerator SetTemporaryTimeScaleCoroutine(float timeScale, float duration)
        {
            SetTimeScale(timeScale);
            yield return new WaitForSecondsRealtime(duration);
            SetTimeScale(1f);
        }
        
    }
}