using UnityEngine;

namespace Mechanics.System
{
    public class ActiveTemporary : MonoBehaviour
    {
        [SerializeField] private float duration = 0.2f;
        [SerializeField] private bool startWithActive;
        [SerializeField] private bool destroyItself;
        private float durationNow;


        private void Start()
        {
            if(!startWithActive)
                gameObject.SetActive(false);
        }

        protected virtual void Update()
        {
            if (durationNow < duration)
                durationNow += Time.deltaTime;
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            if(destroyItself)
                Destroy(gameObject);
            else
                durationNow = 0;
        }



    }
}
