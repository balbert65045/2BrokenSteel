using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Player
{
    [Serializable]
    public class MouseLook
    {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool smooth;
        public float smoothTime = 5f;
        public bool lockCursor = true;


        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;
        private bool m_cursorIsLocked = true;


        private float initZpos;
        private float zPos; 
        private float initYpos;
        private float yPos;
        private float initXpos;
        private float xPos;
        private float r;
        private float initR;
        private Quaternion CameraAngles;
        private Vector3 CharPos;

        public void Init(Transform character, Transform camera)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.rotation;
            zPos = camera.localPosition.z;
            initYpos = camera.localPosition.y;
            xPos = camera.localPosition.x;
           
            initR = Mathf.Sqrt(Mathf.Pow(zPos, 2) + Mathf.Pow(xPos, 2));
            //Debug.Log(initZpos);
        }


        public void LookRotation(Transform character, Transform camera)
        {
            float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
            float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;
            //Debug.Log(yRot);
          //  m_CharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
            //m_CameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-xRot, yRot, 0);
            CharPos = character.transform.position;

            //float initR = Mathf.Sqrt(Mathf.Pow(zPos, 2) + Mathf.Pow(xPos, 2));


            if (smooth)
            {
                // character.localRotation = Quaternion.Slerp (character.localRotation, m_CharacterTargetRot,
                //    smoothTime * Time.deltaTime);
                float xAngle;
                if (m_CameraTargetRot.eulerAngles.x > 180)
                {
                    xAngle = -(360 - m_CameraTargetRot.eulerAngles.x);
                }
                else
                {
                    xAngle = m_CameraTargetRot.eulerAngles.x;
                }

                CameraAngles = Quaternion.Euler(Mathf.Clamp(xAngle, MinimumX, MaximumX), m_CameraTargetRot.eulerAngles.y, 0);
                
                camera.rotation = Quaternion.Slerp (camera.localRotation, CameraAngles,
                   smoothTime * Time.deltaTime);
            }
            else
            {
                float xAngle = m_CameraTargetRot.eulerAngles.x; 
               // if (m_CameraTargetRot.eulerAngles.x > 180)
               // {
               //     xAngle = -(360 - m_CameraTargetRot.eulerAngles.x);
               // }
             //   else
             //   {
             //       xAngle = m_CameraTargetRot.eulerAngles.x;
             //   }

                //  float xAngle = Mathf.Clamp(m_CameraTargetRot.eulerAngles.x + 90, MinimumX + 90, MaximumX + 90) - 90f;
                CameraAngles = Quaternion.Euler(Mathf.Clamp(xAngle, MinimumX, MaximumX), m_CameraTargetRot.eulerAngles.y, 0);
                camera.localRotation = CameraAngles;

            }

            CreateCameraPosition(camera);
            UpdateCursorLock();
        }

        public void SetCursorLock(bool value)
        {
            lockCursor = value;
            if(!lockCursor)
            {//we force unlock the cursor if the user disable the cursor locking helper
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void UpdateCursorLock()
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (lockCursor)
                InternalLockUpdate();
        }

        private void InternalLockUpdate()
        {
            if(Input.GetKeyUp(KeyCode.Escape))
            {
                m_cursorIsLocked = false;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        void CreateCameraPosition(Transform camera)
        {
            float adjustmentFactorR = .1f;
            float SecondAsjust = .1f;
            float adjustmentFactorY = .17f;
            if (CameraAngles.eulerAngles.x > 180)
            {
               // Debug.Log(CameraAngles.eulerAngles.x);
                r = Mathf.Clamp(((CameraAngles.eulerAngles.x) - 380) * (adjustmentFactorR) + initR, 1f, 20f);
                yPos = Mathf.Clamp(((CameraAngles.eulerAngles.x) - 380) * (adjustmentFactorY) + initYpos, -3, 20);

            }
            else if (CameraAngles.eulerAngles.x <= 20)
            {
                r = ( (CameraAngles.eulerAngles.x) - 20) * adjustmentFactorR + initR;
                yPos = ((CameraAngles.eulerAngles.x - 20)) * adjustmentFactorY + initYpos; ;
            }
            else
            {
                r = (20 - CameraAngles.eulerAngles.x) * adjustmentFactorR + initR;
                yPos = initYpos;
            }
            
            //pythagrion theorm

            xPos = r * Mathf.Sin(Mathf.Deg2Rad * -m_CameraTargetRot.eulerAngles.y);
            zPos = -r * Mathf.Cos(Mathf.Deg2Rad * -m_CameraTargetRot.eulerAngles.y);

            //camera.localPosition = new Vector3(camera.localPosition.x, yPos, zPos);
            camera.position = new Vector3(CharPos.x + xPos, CharPos.y + yPos - 1, CharPos.z + zPos);
        }
    }
}
