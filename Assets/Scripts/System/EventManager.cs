using Character.Player;
using UnityEngine;

namespace System
{
    public class EventManager : MonoBehaviour
    {
        // Singleton
        private static EventManager instance;
        public static EventManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<EventManager>();
                }
                return instance;
            }
        }
        
        // player controller
        public PlayerController playerController;
        
        // awake 
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
            
            // player controller 초기화
            try
            {
                playerController = FindObjectOfType<PlayerController>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}