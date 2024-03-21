using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Util
{
    public class Debugger : MonoBehaviour
    {
        public bool debugging = false;
        public float TimeScale = 1f;
        
        void Update()
        {
            if(debugging)
            {
                Time.timeScale = TimeScale;
            }
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            Debug.Log( Math.Round( context.ReadValue<Vector2>().x,5) + " / " + Math.Round(context.ReadValue<Vector2>().y, 5) );
        }
        
        public static float GetVector2Length(Vector2 v)
        {
            return (float)Math.Sqrt(v.x * v.x + v.y * v.y);
        }
        public static Vector3 DrawGizmosCircleXZ(Vector3 pos, float radius,
            int circleStep = 20, float ratioLastPt = 1f)
        {
            float theta, step = (2f * Mathf.PI) / (float)circleStep;
            Vector3 p0 = pos;
            Vector3 p1 = pos;
            for (int i = 0; i < circleStep; ++i)
            {
                theta = step * (float)i;
                p0.x = pos.x + radius * Mathf.Sin(theta);
                p0.y = pos.y + radius * Mathf.Cos(theta);

                theta = step * (float)(i + 1);
                p1.x = pos.x + radius * Mathf.Sin(theta);
                p1.y = pos.y + radius * Mathf.Cos(theta);
                Gizmos.DrawLine(p0, p1);
            }

            theta = step * ((float)circleStep * ratioLastPt);
            p0.x = pos.x + radius * Mathf.Sin(theta);
            p0.y = pos.y + radius * Mathf.Cos(theta);

            return p0;
        }
        public static RaycastHit2D BoxCast(Vector2 origen, Vector2 size, float angle, Vector2 direction, float distance,
            int mask, float duration = 0f)
        {
            RaycastHit2D hit = Physics2D.BoxCast(origen, size, angle, direction, distance, mask);

            //Setting up the points to draw the cast
            Vector2 p1, p2, p3, p4, p5, p6, p7, p8;
            float w = size.x * 0.5f;
            float h = size.y * 0.5f;
            p1 = new Vector2(-w, h);
            p2 = new Vector2(w, h);
            p3 = new Vector2(w, -h);
            p4 = new Vector2(-w, -h);

            Quaternion q = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
            p1 = q * p1;
            p2 = q * p2;
            p3 = q * p3;
            p4 = q * p4;

            p1 += origen;
            p2 += origen;
            p3 += origen;
            p4 += origen;

            Vector2 realDistance = direction.normalized * distance;
            p5 = p1 + realDistance;
            p6 = p2 + realDistance;
            p7 = p3 + realDistance;
            p8 = p4 + realDistance;


            //Drawing the cast
            Color castColor = hit ? Color.red : Color.blue;
            Debug.DrawLine(p1, p2, castColor, duration);
            Debug.DrawLine(p2, p3, castColor, duration);
            Debug.DrawLine(p3, p4, castColor, duration);
            Debug.DrawLine(p4, p1, castColor, duration);

            Debug.DrawLine(p5, p6, castColor, duration);
            Debug.DrawLine(p6, p7, castColor, duration);
            Debug.DrawLine(p7, p8, castColor, duration);
            Debug.DrawLine(p8, p5, castColor, duration);

            Debug.DrawLine(p1, p5, Color.grey, duration);
            Debug.DrawLine(p2, p6, Color.grey, duration);
            Debug.DrawLine(p3, p7, Color.grey, duration);
            Debug.DrawLine(p4, p8, Color.grey, duration);
            if (hit)
            {
                Debug.DrawLine(hit.point, hit.point + hit.normal.normalized * 0.2f, Color.yellow, duration);
            }

            return hit;
        }
    }
}
