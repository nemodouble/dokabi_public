using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Audio
{
    public class BgmManager : MonoBehaviour
    {
        public static BgmManager Instance
        {
            get
            {
                if (instance != null)
                    return instance;
                instance = FindObjectOfType<BgmManager>();
                if (instance != null)
                    return instance;

                return Create();
            }
        }
        protected static BgmManager instance;
        protected static bool quitting;
        
        public static BgmManager Create ()
        {
            var bgmManagerGameObject = new GameObject("BGM Manager");
            DontDestroyOnLoad(bgmManagerGameObject);
            instance = bgmManagerGameObject.AddComponent<BgmManager>();
            return instance;
        }
        
        public EventReference ambientEvent;
        private EventInstance m_AmbientInstance;

        public EventReference bgmEvent;
        private EventInstance m_BGMInstance;

        void Awake()
        {
            if (Instance != this)
                Destroy(gameObject);
            StartCoroutine(WaitForBankLoad());
        }
        
        IEnumerator WaitForBankLoad()
        {
            while (!RuntimeManager.HasBankLoaded("Master"))
            {
                yield return null;
            }
            StartAmbient();
            StartBGM();
        }

        void OnDestroy()
        {
            if (instance == this)
                quitting = true;
        }

        public void StartAmbient()
        {
            StartAmbient(ambientEvent);
        }
        public void StartAmbient(EventReference ambientEvent)
        {
            StopAmbient();
            this.ambientEvent = ambientEvent;
            m_AmbientInstance = RuntimeManager.CreateInstance(ambientEvent);
            m_AmbientInstance.start();
        }
        public void StopAmbient()
        {
            m_BGMInstance.stop(STOP_MODE.ALLOWFADEOUT);
        }

        public void StartBGM()
        {
            StartBGM(bgmEvent);
        }
        public void StartBGM(EventReference bgmEvent)
        {
            StopBGM();
            this.bgmEvent = bgmEvent;
            m_BGMInstance = RuntimeManager.CreateInstance(bgmEvent);
            m_BGMInstance.start();
        }
        public void StopBGM()
        {
            m_BGMInstance.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }
}