using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace Audio
{
    public class SfxManager : MonoBehaviour
    {
        public static SfxManager Instance
        {
            get
            {
                if (instance != null)
                    return instance;
                instance = FindObjectOfType<SfxManager>();
                if (instance != null)
                    return instance;

                return Create();
            }
        }
        protected static SfxManager instance;
        protected static bool quitting;

        public static SfxManager Create()
        {
            var vfxManagerGameObject = new GameObject("SFX Manager");
            DontDestroyOnLoad(vfxManagerGameObject);
            instance = vfxManagerGameObject.AddComponent<SfxManager>();
            return instance;
        }

        public string[] eventName;
        public EventReference[] eventList;
        public Tuple<string, EventReference> eventNameList;

        public void PlayEvent(EventReference eventReference, Vector3 position)
        {
            RuntimeManager.PlayOneShot(eventReference, position);
        }
    }
}