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

        private bool LockedtoEnemy = false;
        private Transform m_Enemy;
        private Vector3 EnemyPos; 

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
            float yRot;
            float xRot;
            if (!LockedtoEnemy)
            {
                 yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
                 xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;
                m_CameraTargetRot *= Quaternion.Euler(-xRot, yRot, 0);

            }
            else
            {
                 EnemyPos = m_Enemy.transform.position;
                 CharPos = character.transform.position;
                //Debug.Log("Enemy tranform " + EnemyPos);
              //  Debug.Log("Char transform " + CharPos);

                yRot = CalculateDegree((CharPos.x - EnemyPos.x), (CharPos.z - EnemyPos.z), true);
                float R = Mathf.Sqrt(Mathf.Pow((CharPos.x - EnemyPos.x), 2) + (Mathf.Pow((CharPos.z - EnemyPos.z), 2)));
                xRot =CalculateDegree(R, (CharPos.y - EnemyPos.y), false);
                m_CameraTargetRot = Quaternion.Euler(xRot, yRot, 0);
                //Debug.Log("Xrotation " + xRot + "Yrotation " + yRot);

            }

          
            CharPos = character.transform.position;

            if (smooth)
            {
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

                camera.rotation = Quaternion.Slerp(camera.localRotation, CameraAngles,
                   smoothTime * Time.deltaTime);
            }
            else
            {
                float xAngle = m_CameraTargetRot.eulerAngles.x;
                CameraAngles = Quaternion.Euler(Mathf.Clamp(xAngle, MinimumX, MaximumX), m_CameraTargetRot.eulerAngles.y, 0);
                camera.localRotation = CameraAngles;

            }
            CreateCameraPosition(camera);
            UpdateCursorLock();
        }

        private float CalculateDegree(float c, float a, bool Yrot)
        {
            int Quadrient = 0;
            // Debug.Log("c " + c);
            // Debug.Log("a " + a);
            if (-c >= 0 && -a >= 0)
            {
                Quadrient = 1;
            }
            else if (-c >= 0 && -a < 0)
            {
                Quadrient = 2;
            }
            else if (-c < 0 && -a < 0)
            {
                Quadrient = 3;
            }
            else if (-c < 0 && -a >= 0)
            {
                Quadrient = 4;
            }
            c = Mathf.Abs(c);
            a = Mathf.Abs(a);



            float b = Mathf.Sqrt(Mathf.Pow(c, 2) + Mathf.Pow(a, 2));
            float Numerator = Mathf.Pow(a, 2) + Mathf.Pow(b, 2) - Mathf.Pow(c, 2);
            float Denominator = 2 * b * a;

            float Degree = Mathf.Rad2Deg * (Mathf.Acos(Numerator / Denominator));

            //Debug.Log("Angle " + Degree);

            if (Yrot)
            {
                if (Quadrient == 1)
                {
                    return (Degree);
                }
                else if (Quadrient == 2)
                {
                    return (180 - Degree);
                }
                else if (Quadrient == 3)
                {
                    return (180 + Degree);
                }
                else if (Quadrient == 4)
                {
                    return (360 - Degree);
                }
                else
                {
                    Debug.LogError("Incorrect Quadrient found");
                    return (0);
                }
            }
            else
            {
              //  Debug.Log(Degree);
                if (Quadrient == 1)
                {
                    return (90 - Degree);
                }
                else if (Quadrient == 2)
                {
                    return (90 - Degree);
                }
                else if (Quadrient == 3)
                {
                    return (90 - Degree);
                }
                else if (Quadrient == 4)
                {
                    return (90 - Degree);
                }
                else
                {
                    Debug.LogError("Incorrect Quadrient found");
                    return (0);
                }
            }
        }



        public void LocktoEnemy(Transform Enemy)
        {
            m_Enemy = Enemy;
            LockedtoEnemy = true; 
        }

        public void Unlock()
        {
            LockedtoEnemy = false;
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
