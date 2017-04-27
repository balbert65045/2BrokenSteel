using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    public class PauseManager : MonoBehaviour
    {


        public PlayerUserControl PlayerUserControl;
        public GameObject PauseMenu;

        private float OriginalTimeScale;

        public bool GamePaused = false;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (PlayerUserControl.Paused && !GamePaused)
            {
                GamePaused = true;
                SetPause(true);
            }
            else if (PlayerUserControl.Paused && GamePaused)
            {
                GamePaused = false;
                SetPause(false);
            }

        }


        public void SetPause(bool value)
        {
            PlayerUserControl.SetCursorLock(!value);
            PlayerUserControl.SetCameraLock(value);
            PauseMenu.SetActive(value);
            if (value == true)
            {
                OriginalTimeScale = Time.timeScale;
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = OriginalTimeScale;
            }
        }
    }
}

