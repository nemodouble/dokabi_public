using Character.Player;
using UnityEngine;

namespace System
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private float speedMultipler;
        [SerializeField] private float playerSpeedRate;
        private PlayerController playerController;

        [SerializeField] private bool isStop;
    
        void Start()
        {
            player = GameObject.Find("Player");
            playerController = player.GetComponent<PlayerController>();
        }

        void FixedUpdate()
        {
            if (player.gameObject != null && !isStop)
            {
                var position = player.transform.position;
                transform.position = new Vector3(position.x, position.y, transform.position.z);
                // float playerXSpeed = Mathf.Abs(player.GetComponent<PlayerController>().velocityX);
                // float cameraMoveSpeed = (playerXSpeed + 1) * playerSpeedRate + speedMultipler;
                // Vector3 targetPos = new Vector3(player.transform.position.x, player.transform.position.y, this.transform.position.z);
                // //transform.position = Vector3.Lerp(this.transform.position, targetPos, cameraMoveSpeed * Time.deltaTime);
                // transform.position = Vector3.Lerp(this.transform.position, targetPos, speedMultipler * Time.deltaTime);
            }
        }
    }
}
