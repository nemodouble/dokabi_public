using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace tmp.Scripts
{
    public class TmpEventManager : MonoBehaviour
    {
        private bool TMP_toggle;
        private GameObject tmpenemy;
        public void Reset()
        {
            RuntimeManager.PauseAllEvents(false);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void Exit()
        {
            Application.Quit();
        }

        private void Start()
        {
            tmpenemy = GameObject.Find("tmp_enemy2");
        }

        private void Update()
        {
            try
            {
                var changeCheck = TMP_toggle;
                TMP_toggle = GameObject.Find("TestingToggle").GetComponent<Toggle>().isOn;
                if (changeCheck != TMP_toggle)
                    tmpenemy.SetActive(TMP_toggle);
            }
            catch (NullReferenceException)
            {
                
            }
        }
    }
}
