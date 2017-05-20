using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Player
{
    [RequireComponent(typeof(PC))]
    public class PUC : MonoBehaviour
    {
        private PC m_Player;
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private AnimC AnimC;
        private ShieldSlidingController ShieldSlidingController;

        private EnemyLockCollider m_EnemyLockCollider; 
        public Vector3 m_CamForward;             // The current forward direction of the camera
        public Vector3 m_Move;

        private bool ActiveGauntlets = true;
        private bool ActiveSword = false;
        private bool WeaponSwitch = false; 

        private bool m_Jump;                     
        private bool m_QuickMove = false;
        private bool m_LeftMove = false;
        private bool m_RightMove = false;
        private bool m_SpecialMove = false;
        private bool m_BackwardsMove = false;
        private bool m_ForwardsMove = false;
        private bool Slidinginput = false;
        private bool m_SpecialMoveCharge = false;
        private bool m_DodgeLeft = false;
        private bool m_DodgeRight = false;
        private bool TogglePotential = false;

        public bool Paused = false;

        private bool Locked = false;
        private int NumE = 0; 
		private TargetArrow TargetArrow;

        [SerializeField]
        private MouseLook m_MouseLook;
     
        private GameObject LockedEnemy;
        private bool GamePaused = false;


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

			if (FindObjectOfType<TargetArrow>() != null)
			{
				          TargetArrow = FindObjectOfType<TargetArrow>();
						  TargetArrow.gameObject.SetActive(false);	
			}
			else{
				Debug.LogWarning("TargetArrowNotFound");
			}
			
			if (FindObjectOfType<EnemyLockCollider>() != null){
				m_EnemyLockCollider = FindObjectOfType<EnemyLockCollider>(); 
			}
			else{
				Debug.LogWarning("EnemyLockColliderNotFound");
			}
            
            m_Player = GetComponent<PC>();
            AnimC = GetComponent<AnimC>();
            ShieldSlidingController = GetComponent<ShieldSlidingController>();
            m_MouseLook.Init(transform, m_Cam);
        }

        // Update is called once per frame
        void Update()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
			
			 if (!Paused)
            {
                Paused = CrossPlatformInputManager.GetButtonDown("pausebutton");
             //   Debug.Log(Paused);
            }

            if (!TogglePotential)
            {
                TogglePotential = CrossPlatformInputManager.GetButtonDown("TogglePotential");
                if (TogglePotential)
                {
                    Debug.Log("PotentialOn");
                }
            }

            if (!GamePaused)          
            {
                RotateView();
            }
		   
		   
		   if (CrossPlatformInputManager.GetButtonDown("ShiftWeapon"))
            {
                CheckWeaponChange();
            }
			
			if (CrossPlatformInputManager.GetButtonDown("LockEnemy"))
			{
				 CheckForLock(Locked);
			}
			
           

            WeaponInput();

          
			
		//	Slidinginput = CrossPlatformInputManager.GetButton("Jump");
        }

        private void FixedUpdate()
        {
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");

            if (m_Cam != null)
            {
                // calculate camera relative direction to move

                m_CamForward = Vector3.Scale(transform.position - m_Cam.transform.position, new Vector3(1, 0, 1)).normalized;
                m_Move = v * m_CamForward + h * m_Cam.right;

            }
            else
            {
                // we use world-relativew directions in the case of no main camera
                m_Move = v * Vector3.forward + h * Vector3.right;
            }


         

            AnimC.UpdateAnimator(m_Move, m_QuickMove, m_LeftMove, m_RightMove, m_SpecialMove, ActiveGauntlets, ActiveSword, WeaponSwitch, m_BackwardsMove, m_ForwardsMove, m_SpecialMoveCharge, m_DodgeLeft, m_DodgeRight, TogglePotential);
            ShieldSlidingController.CheckSlide(Slidinginput, h);
            m_Player.Move(m_Move, m_Jump, Locked);

            m_Jump = false;
            Paused = false;

            m_QuickMove = false;
            m_DodgeLeft = false;
            m_DodgeRight = false;
            m_SpecialMove = false;
            WeaponSwitch = false;
            TogglePotential = false;


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
                m_SpecialMove = CrossPlatformInputManager.GetButtonUp("SpecialMove");
            }
            if (!m_DodgeLeft)
            {
                m_DodgeLeft = CrossPlatformInputManager.GetButtonDown("DodgeLeft");
            }
            if (!m_DodgeRight)
            {
                m_DodgeRight = CrossPlatformInputManager.GetButtonDown("DodgeRight");
            }
            m_SpecialMoveCharge = CrossPlatformInputManager.GetButton("SpecialMove");
            m_LeftMove = CrossPlatformInputManager.GetButton("LeftMove");
            m_RightMove = CrossPlatformInputManager.GetButton("RightMove");
            m_BackwardsMove = CrossPlatformInputManager.GetButton("BackwardsMove");
            m_ForwardsMove = CrossPlatformInputManager.GetButton("ForwardsMove");
    
        }
		
		
		private void CheckForLock( bool Status){
			if (!Status)
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
            else
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

        public void SetCursorLock(bool value)
        {
            m_MouseLook.SetCursorLock(value);
        }

        public void SetCameraLock(bool Locked)
        {
            GamePaused = Locked;
        }
    }
}