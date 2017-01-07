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

        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        private bool n_Jump;

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
            else
            {
                Debug.Log("Jumped");
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
                //m_Move = v * m_CamForward + h * m_Cam.right;
                m_Move = v * m_CamForward + h * m_Cam.right;
              //  m_Move = v * m_CamForward;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v * Vector3.forward + h * Vector3.right;
            }
            if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
            m_Player.Move(m_Move, m_Jump);
            m_Jump = false;
            m_MouseLook.UpdateCursorLock();

        }

        private void RotateView()
        {
            m_MouseLook.LookRotation(transform, m_Cam.transform);
        }

       

    }
}
