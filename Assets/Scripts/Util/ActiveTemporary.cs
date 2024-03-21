using UnityEngine;

namespace Util
{
    public class ActiveTemporary : MonoBehaviour
    {
        [SerializeField] private float duration = 0.2f;
        [SerializeField] private bool startWithActive;
        [SerializeField] private bool destroyDurationEnd;
        private float durationNow;


        private void Start()
        {
            if(!startWithActive)
                gameObject.SetActive(false);
        }

        private void Update()
        {
            if (durationNow < duration)
                durationNow += Time.deltaTime;
            else
            {
                if(destroyDurationEnd)
                    Destroy(gameObject);
                else
                    gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            durationNow = 0;
        }
    }
}
