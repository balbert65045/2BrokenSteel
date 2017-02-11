using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {

        [SerializeField]
        float m_MovingTurnSpeed = 360;
        [SerializeField]
        float m_StationaryTurnSpeed = 180;
        [SerializeField]
        float m_JumpPower = 12f;
        [Range(1f, 4f)]
        [SerializeField]
        float m_GravityMultiplier = 2f;
        [SerializeField]
        float m_MoveSpeedMultiplier = 1f;

        public float m_GroundCheckDistance = 0.1f;
        public bool m_IsGrounded;
        public float TurnTime = 5;

        [SerializeField]
        float FrictionGround = .2f;

        Rigidbody m_Rigidbody;
        Animator m_Animator;
        float m_OrigGroundCheckDistance;
        float m_TurnAmount;
        float m_ForwardAmount;
        Vector3 m_GroundNormal;

        private bool moving;
        public bool Atacking = false; 
        private Transform m_Cam;

        public bool RapidFiring = false;
        public bool turnBackward = false;
        private bool turnForward = false;
        public bool AllowMovement = true;

        private Quaternion OldRotation;


        // Use this for initialization
        void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Animator = GetComponent<Animator>();

            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            m_OrigGroundCheckDistance = m_GroundCheckDistance;
            //StartXMin = m_MouseLook.MinimumX; 
            OldRotation = transform.rotation;

            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.Log("Camera is not found");
            }

        }

        public void Move(Vector3 move, bool jump, bool QuickMove, bool LeftMove, bool RightMove, bool SpecialMove, bool ActiveGauntlets, bool ActiveSword, bool WeaponSwitch)
        {

            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            if (move.magnitude > 1f) move.Normalize();
            CheckGroundStatus();
          //  move = Vector3.ProjectOnPlane(move, m_GroundNormal);
            m_TurnAmount = Mathf.Atan2(move.x, move.z);
            m_ForwardAmount = move.z;


            // control and velocity handling is different when grounded and airborne:
            if (m_IsGrounded)
            {
                SendMessage("UpdateXMin", -10);
                HandleGroundedMovement(jump, move);
            }
            else
            {
                HandleAirborneMovement(move);
            }
            //Update animator
            UpdateAnimator(move, QuickMove, LeftMove, RightMove, SpecialMove, ActiveGauntlets, ActiveSword, WeaponSwitch);

        }

        void HandleAirborneMovement(Vector3 move)
        {
         
            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
            m_Rigidbody.AddForce(extraGravityForce);

            HandleAirborneRotation(move);
            m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
        }

        void HandleAirborneRotation(Vector3 move)
        {
            // allow rotation
            m_Rigidbody.constraints = RigidbodyConstraints.None;


            SendMessage("UpdateXMin", -90);
            // rotate self in mid air

           
            if (Atacking)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            }
            else if (RapidFiring)
            {
                if (turnBackward)
                {
                    float YAngle;

                    if (OldRotation.eulerAngles.y > 270)
                    {
                        YAngle = OldRotation.eulerAngles.y - 360;
                    }
                    else
                    {
                        YAngle = OldRotation.eulerAngles.y;
                    }
                    // Debug.Log(YAngle);
                    OldRotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, Mathf.Clamp((YAngle + 10f), YAngle, YAngle + 180), transform.rotation.eulerAngles.z);
                    transform.rotation = Quaternion.Slerp(transform.rotation, OldRotation, 10f * Time.deltaTime);
                    //transform.rotation = Quaternion.Slerp(OldRotation, Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, m_Cam.transform.rotation.eulerAngles.y - 180, transform.rotation.eulerAngles.z), 5);
                }
                else
                {
                    OldRotation = transform.rotation;
                    transform.rotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, m_Cam.transform.rotation.eulerAngles.y - 180, transform.rotation.eulerAngles.z);
                }
            }
            else if (turnForward)
            {
                //Debug.Log("RotatingForward");
                float YAngle;

                if (OldRotation.eulerAngles.y < 90)
                {
                    YAngle = OldRotation.eulerAngles.y + 360;
                }
                else
                {
                    YAngle = OldRotation.eulerAngles.y;
                }
                // Debug.Log(YAngle);
                OldRotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, Mathf.Clamp((YAngle + 10f), YAngle, YAngle + 180), transform.rotation.eulerAngles.z);
                transform.rotation = Quaternion.Slerp(transform.rotation, OldRotation, 10f * Time.deltaTime);
              //  transform.rotation = Quaternion.Slerp(OldRotation, Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, m_Cam.transform.rotation.eulerAngles.y - 180, transform.rotation.eulerAngles.z), 5);
            }
            else if ((AllowMovement))
            {
                OldRotation = transform.rotation;
                transform.rotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, m_Cam.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            }
           // transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            {
             //   transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, m_Cam.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            }

        }

        void HandleGroundedMovement(bool jump, Vector3 move)
        {
           


            AlignWithCamera();

            if (jump)
            {
                m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
                m_IsGrounded = false;
                m_GroundCheckDistance = 0.1f;
            }
            else
            {
              
                //Rigidbody input movement
                m_Rigidbody.AddForce((move * m_MoveSpeedMultiplier * 3) , ForceMode.Force);
                //Land Friction
                m_Rigidbody.AddForce(new Vector3(-m_Rigidbody.velocity.x * FrictionGround, 0, -m_Rigidbody.velocity.z * FrictionGround));
                //m_Rigidbody.AddForce(-move * FrictionGround * 3);
                m_Rigidbody.velocity = new Vector3 (Mathf.Clamp(m_Rigidbody.velocity.x, -20, 20), 0, Mathf.Clamp(m_Rigidbody.velocity.z, -20, 20));
            }
        }

        void ResetRotation()
        {
            {
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                transform.rotation = (Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0));

            }
        }

        void AlignWithCamera()
        {

            //   transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, m_Cam.transform.rotation.eulerAngles.y, 0), Quaternion.Euler(0, transform.rotation.y, 0), TurnTime * Time.deltaTime);
            if (Atacking)
            {
                m_IsGrounded = false;
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; ;
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                OldRotation = transform.rotation; 
            }
            else if (RapidFiring)
            {
                if (turnBackward)
                {
                   // Debug.Log("TurnBackwards");
                    //Debug.Log(m_Cam.eulerAngles.y);
                    float YAngle;

                    if (OldRotation.eulerAngles.y < 90)
                    {
                        YAngle = OldRotation.eulerAngles.y + 360;
                    }
                    else
                    {
                        YAngle = OldRotation.eulerAngles.y;
                    }
                   // Debug.Log(YAngle);
                    OldRotation = Quaternion.Euler(0, Mathf.Clamp((YAngle + 10f), YAngle, YAngle + 180), 0);
                    transform.rotation = Quaternion.Slerp(transform.rotation, OldRotation, 10f * Time.deltaTime);
                }
                else
                {
                  //  Debug.Log("RapidFiring");
                    transform.rotation =  Quaternion.Euler(0, m_Cam.transform.rotation.eulerAngles.y - 180, 0);
                    OldRotation = transform.rotation;
                }
                
            }
            else if (turnForward)
            {
                float YAngle;

                if (OldRotation.eulerAngles.y > 270)
                {
                    YAngle = OldRotation.eulerAngles.y - 360;
                }
                else
                {
                    YAngle = OldRotation.eulerAngles.y;
                }
               //  Debug.Log("TurnForward");
                OldRotation = Quaternion.Euler(0, Mathf.Clamp((YAngle + 10f), YAngle, YAngle + 180), 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, OldRotation, 10f * Time.deltaTime);
            }

            else if (moving && AllowMovement)
            {
                // Debug.Log("Moving");
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
               // transform.rotation = (Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0));
                transform.rotation = (Quaternion.Euler(0, m_Cam.transform.rotation.eulerAngles.y, 0));
                OldRotation = transform.rotation;
                //  ResetRotation();
            }
            else
            {
                //Debug.Log("Idle");
                OldRotation = transform.rotation;
            }

            Debug.Log(OldRotation.eulerAngles.y);
        }

        public void PlayerTurn(int turn)
        {
              if (turn == 1 && RapidFiring)
              {
               // Debug.Log("Turning");
                turnBackward = true;
                  turnForward = false;
                AllowMovement = false;
            }
              else if (turn == 1 && !RapidFiring)
              {
                  turnForward = true;
                  turnBackward = false;
                AllowMovement = false;
            }
              else if(turn == 2 && RapidFiring)
            {
                AllowMovement = true; 
            }
              else if(turn == 2 && !RapidFiring)
            {
                turnBackward = false;
                turnForward = false;
                AllowMovement = true;
            }
              else
              {
                //  Debug.Log("StopTurning");
                  AllowMovement = false;
                    turnBackward = false;
                  turnForward = false;
              }
        }


        void UpdateAnimator(Vector3 move, bool QuickMove, bool LeftMove, bool RightMove, bool SpecialMove, bool ActiveGauntlets, bool ActiveSword, bool WeaponSwitch)
        {

            //
            // animation bools!
            //
            if (SpecialMove && ActiveGauntlets)
            {
                RapidFiring = true;
                m_Animator.SetBool(("RapidFire"), true);
            }

            else if (move != Vector3.zero)
            {
                m_Animator.SetBool(("RapidFire"), false);
                RapidFiring = false;
                moving = true;
                m_Animator.SetBool("Moving", true);
                float RunSpeed = 1.35f;
                float SlowDownSpeed = 0.8f;

                if (move.magnitude == 1)
                {
                    m_Animator.speed = RunSpeed;
                }
                else
                {
                    m_Animator.speed = SlowDownSpeed;
                }
            }

            else 
               {
                    m_Animator.SetBool("Moving", false);
                    m_Animator.SetBool(("RapidFire"), false);
                    RapidFiring = false;
            }

            //
            // animation Triggers!
            //

            if (ActiveGauntlets)
            {
                if (WeaponSwitch)
                {
                    m_Animator.SetTrigger("GauntletsOutTrig");
                }
                else if (QuickMove)
                {
                    m_Animator.SetTrigger("UpLaunch");

                }
                else if (LeftMove)
                {
                    m_Animator.SetTrigger("LeftLaunch");

                }
                else if (RightMove)
                {
                
                    m_Animator.SetTrigger("RightLaunch");
                }
               
                }

                if (ActiveSword)
                {
                    //  Debug.Log("ActiveSword");
                    if (WeaponSwitch)
                    {
                        m_Animator.SetTrigger("SwordOutTrig");
                    }
                    else if (QuickMove)
                    {
                        m_Animator.SetTrigger(("QuickAttack"));
                    }
                    else if (SpecialMove)
                    {
                        m_Animator.SetTrigger("StrongAttack");
                    }
                    else if (RightMove)
                    {
                        m_Animator.SetTrigger("SideStepRight");
                    }
                    else if (LeftMove)
                    {
                        m_Animator.SetTrigger("SideStepLeft");
                    }
                  

                    
                }
            }
        


       private void CheckGroundStatus()
        {

                RaycastHit hitInfo;
#if UNITY_EDITOR
                Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
                if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
                {
                    m_GroundNormal = hitInfo.normal;
                    m_IsGrounded = true;
                    //Debug.Log(m_Rigidbody.velocity);
                    if (-m_Rigidbody.velocity.y >= 20f)
                    {
                        BroadcastMessage("SchockwaveLanding");
                        // Debug.Log("SendtSchockwave");
                    }
                }
                else
                {
                    m_IsGrounded = false;

                m_GroundNormal = Vector3.up;
                }
        }

        


    }
}