using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerUserControl : MonoBehaviour
    {
        private PlayerController m_Player;
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        public Vector3 m_CamForward;             // The current forward direction of the camera
        public Vector3 m_Move;

        private bool ActiveGauntlets = true;

        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        private bool m_UpLaunch = false;
        private bool m_LeftLaunch = false;
        private bool m_RightLaunch = false;
        private bool m_BackwardLaunch = false;

        public Vector3 TestVector; 

        [SerializeField]
        private MouseLook m_MouseLook;

        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            m_Player = GetComponent<PlayerController>();
            m_MouseLook.Init(transform, m_Cam);
        }

        // Update is called once per frame
        void Update()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
            if (ActiveGauntlets)
            {
                GauntletInput();
            }
          

            RotateView();
        }

        private void FixedUpdate()
        {
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");

            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                TestVector = m_Cam.transform.position;

                m_CamForward = Vector3.Scale(transform.position - m_Cam.transform.position, new Vector3(1, 0, 1)).normalized;
                m_Move = v * m_CamForward + h * m_Cam.right;
            }
            else
            {
                // we use world-relativew directions in the case of no main camera
                m_Move = v * Vector3.forward + h * Vector3.right;
            }
            if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
            m_Player.Move(m_Move, m_Jump, m_UpLaunch, m_LeftLaunch, m_RightLaunch, m_BackwardLaunch);
            m_Jump = false;
            m_UpLaunch = false;
            m_LeftLaunch = false;
            m_RightLaunch = false;
            m_BackwardLaunch = false;
            m_MouseLook.UpdateCursorLock();

        }

        private void RotateView()
        {
            m_MouseLook.LookRotation(transform, m_Cam.transform);
        }


        private void GauntletInput()
        {
            if (!m_UpLaunch)
            {
                m_UpLaunch = CrossPlatformInputManager.GetButtonDown("UpLaunch");
            }
            if (!m_LeftLaunch)
            {
                m_LeftLaunch = CrossPlatformInputManager.GetButtonDown("LeftLaunch");
            }
            if (!m_RightLaunch)
            {
                m_RightLaunch = CrossPlatformInputManager.GetButtonDown("RightLaunch");
            }
            if (!m_BackwardLaunch)
            {
                m_BackwardLaunch = CrossPlatformInputManager.GetButton("BackLaunch");
            }
        }

        public void UpdateXMin(float value)
        {
            m_MouseLook.MinimumX = value;
        }

    }
}
