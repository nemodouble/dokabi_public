using UnityEngine;

namespace Util
{
    public class CopyScale : MonoBehaviour
    {
        public Transform target;
        public bool copyX = true;
        public bool copyY = true;
        public bool copyZ = true;
        
        // Update
        void Update()
        {
            if (transform == null)
                throw new UnityException("CopyScale 대상 설정되지 않음");
            Vector3 scale = transform.localScale;
            if (copyX) scale.x = target.localScale.x;
            if (copyY) scale.y = target.localScale.y;
            if (copyZ) scale.z = target.localScale.z;
            transform.localScale = scale;
        }
    }
}