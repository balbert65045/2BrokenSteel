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
        private EnemyLockCollider m_EnemyLockCollider; 
        public Vector3 m_CamForward;             // The current forward direction of the camera
        public Vector3 m_Move;

        private bool ActiveGauntlets = true;
        private bool ActiveSword = false;
        private bool WeaponSwitch = false; 

        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        private bool m_QuickMove = false;
        private bool m_LeftMove = false;
        private bool m_RightMove = false;
        private bool m_SpecialMove = false;
        private bool m_BackwardsMove = false;
        private bool m_ForwardsMove = false;

        private bool Locked = false;
        private int NumE = 0; 

        public Vector3 TestVector;
        private TargetArrow TargetArrow;

        private float SlowMoTimeStart;

        [SerializeField]
        private MouseLook m_MouseLook;

        private GameObject LockedEnemy; 

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

            TargetArrow = FindObjectOfType<TargetArrow>();
            TargetArrow.gameObject.SetActive(false);
            m_EnemyLockCollider = FindObjectOfType<EnemyLockCollider>(); 
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
            if (CrossPlatformInputManager.GetButtonDown("ShiftWeapon"))
            {
            //    SlowMoController(Time.realtimeSinceStartup);
                CheckWeaponChange();
            }

            if (CrossPlatformInputManager.GetButtonDown("LockEnemy") && !Locked)
            {
                if (m_EnemyLockCollider.LocalEnemies.Count > 0)
                {
                    NumE = 0; 
                   // Debug.Log("Locked On Enemy");
                    Locked = true;
                    // Debug.Log(m_EnemyLockCollider.LocalEnemies[NumE].name);
                    LockedEnemy = m_EnemyLockCollider.LocalEnemies[NumE];
                    m_MouseLook.LocktoEnemy(m_EnemyLockCollider.LocalEnemies[NumE].transform);
                    TargetArrow.gameObject.SetActive(true);
                    TargetArrow.Targeted(m_EnemyLockCollider.LocalEnemies[NumE].gameObject);
                }
            }
            else if ((CrossPlatformInputManager.GetButtonDown("LockEnemy") && Locked))
            { 
                NumE++;

              //  Debug.Log("Number of Enemies to lock to" + m_EnemyLockCollider.LocalEnemies.Count);
                if (NumE < m_EnemyLockCollider.LocalEnemies.Count)
                {
                    //    Debug.Log("Switch Lock to new Enemy");
                    LockedEnemy = m_EnemyLockCollider.LocalEnemies[NumE];
                    m_MouseLook.LocktoEnemy(m_EnemyLockCollider.LocalEnemies[NumE].transform);
                    TargetArrow.gameObject.SetActive(true);
                    TargetArrow.Targeted(m_EnemyLockCollider.LocalEnemies[NumE].gameObject);
                }
                else
                {
                     LockedEnemy = null;
                    TargetArrow.gameObject.SetActive(false);
                    //TargetArrow.Targeted(null);
                    Locked = false;
                    Debug.Log("Unlocked");
                    m_MouseLook.Unlock(); 
                }
            }

            WeaponInput();
            RotateView();
        }

        private void FixedUpdate()
        {
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
          //Debug.Log(h);

            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                TestVector = m_Cam.transform.position;

                m_CamForward = Vector3.Scale(transform.position - m_Cam.transform.position, new Vector3(1, 0, 1)).normalized;
                m_Move = v * m_CamForward + h * m_Cam.right;

               // Debug.Log(m_Cam.right);

                //m_Move = new Vector3(h, 0, v);

            }
            else
            {
                // we use world-relativew directions in the case of no main camera
                m_Move = v * Vector3.forward + h * Vector3.right;
            }
            if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
            m_Player.Move(m_Move, m_Jump, m_QuickMove, m_LeftMove, m_RightMove, m_SpecialMove, ActiveGauntlets, ActiveSword, WeaponSwitch, m_BackwardsMove, Locked, m_ForwardsMove);
            m_Jump = false;
            m_QuickMove = false;
           // m_LeftMove = false;
           // m_RightMove = false;
           // m_BackwardsMove = false;
            m_SpecialMove = false;
            // if (ActiveSword)
            //  {
            //      m_SpecialMove = false;
            //  }w
            WeaponSwitch = false;
            m_MouseLook.UpdateCursorLock();

        }

        private void SlowMoController(float time)
        {
            SlowMoTimeStart = time; 
            Time.timeScale = .4f;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
        }

        private void RotateView()
        {
            m_MouseLook.LookRotation(transform, m_Cam.transform);
        }

        private void CheckWeaponChange() {
            WeaponSwitch = true;
            if (ActiveGauntlets)
            {
                ActiveGauntlets = false;
                ActiveSword = true;
            }
            else if (ActiveSword)
            {
                ActiveGauntlets = true;
                ActiveSword = false;
            }

        }




        private void WeaponInput()
        {
            if (!m_QuickMove)
            {
                m_QuickMove = CrossPlatformInputManager.GetButtonDown("QuickMove");
            }
            if (!m_SpecialMove)
            {
                m_SpecialMove = CrossPlatformInputManager.GetButtonDown("SpecialMove");
            }

                m_LeftMove = CrossPlatformInputManager.GetButton("LeftMove");
                m_RightMove = CrossPlatformInputManager.GetButton("RightMove");
                m_BackwardsMove = CrossPlatformInputManager.GetButton("BackwardsMove");
                m_ForwardsMove = CrossPlatformInputManager.GetButton("ForwardsMove");
            


                //   if (ActiveGauntlets)
                //    {
                //            m_SpecialMove = CrossPlatformInputManager.GetButton("SpecialMove");
                //    }
                //    else if (ActiveSword)
                //    {
                //        if (!m_SpecialMove)
                //        {
                //           m_SpecialMove = CrossPlatformInputManager.GetButtonDown("SpecialMove");
                //       }
                //   }


        }

        public void UpdateXMin(float value)
        {
            m_MouseLook.MinimumX = value;
        }

        public void LostLock(GameObject EnemyLeft)
        {
            if (EnemyLeft == LockedEnemy)
            {
               // Debug.Log("EnemyLost");
                LockedEnemy = null;
                TargetArrow.gameObject.SetActive(false);
                //TargetArrow.Targeted(null);
                Locked = false;
             //   Debug.Log("Unlocked");
                m_MouseLook.Unlock();
            }
        }
    }
}
