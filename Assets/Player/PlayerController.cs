﻿using System.Collections;
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
        [SerializeField]
        float m_MaxSpeed = 20f; 

        public float m_GroundCheckDistance = 0.1f;
        public bool m_IsGrounded;
        public float TurnTime = 5;

      //  [SerializeField]
        public float FrictionForce = .2f;

        Rigidbody m_Rigidbody;
        Animator m_Animator;
        float m_OrigGroundCheckDistance;
        float m_TurnAmount;
        float m_ForwardAmount;
        Vector3 m_GroundNormal;

        private bool moving;
        private bool Sliding = false;
        public bool Atacking = false; 
        private Transform m_Cam;

        public bool RapidFiring = false;
        public bool turnBackward = false;
        public bool turnForward = false;
        public bool Backwards = false;
        public bool AllowMovement = true;

        public float SlideSpeed = 13.34f;

        private Quaternion OldRotation;
        private float BackwardsAngle;
        private float CurrentAngleY;
        private float CurrentAngleZ = 0; 
        private bool FullRotation = false;
        private bool FullRotationZ = false;

        private bool LaunchedLeft = false;
        private bool LaunchedRight = false;
        private float LandingTime = .25f;
        private bool AbouttoLand = false;
        private Vector3 LaunchBegginingRotation; 

        private bool M_Blocked = false;


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

        public void Move(Vector3 move, bool jump, bool QuickMove, bool LeftMove, bool RightMove, bool SpecialMove, bool ActiveGauntlets, bool ActiveSword, bool WeaponSwitch, bool m_BackwardsMove, bool Locked)
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
                HandleAirborneMovement(move, Locked);
               
            }
            //Update animator
            UpdateAnimator(move, QuickMove, LeftMove, RightMove, SpecialMove, ActiveGauntlets, ActiveSword, WeaponSwitch, m_BackwardsMove);

        }




        /// <summary>
        /// Airoborne Movement and Rotation
        /// </summary>
        /// <param name="move"></param>
        void HandleAirborneMovement(Vector3 move, bool Locked)
        {
         
            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
            m_Rigidbody.AddForce(extraGravityForce);
            m_Rigidbody.velocity = new Vector3(Mathf.Clamp(m_Rigidbody.velocity.x, -m_MaxSpeed, m_MaxSpeed), m_Rigidbody.velocity.y, Mathf.Clamp(m_Rigidbody.velocity.z, -m_MaxSpeed, m_MaxSpeed));

            AbouttoLand = LandGuider(m_Rigidbody.velocity.y, LandingTime, LaunchedLeft || LaunchedRight);

            HandleAirborneRotation(move, Locked);
            m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
        }

        void HandleAirborneRotation(Vector3 move, bool Locked)
        {
            // allow rotation
            m_Rigidbody.constraints = RigidbodyConstraints.None;
            //Debug.Log("Airborne");

            SendMessage("UpdateXMin", -90);
            // rotate self in mid air

            if (AbouttoLand)
            {
                Debug.Log("Landing");
                FullRotationZ = false;
                Sliding = true;
                float SideAngle;
                if (LaunchedLeft)
                {
                    SideAngle = LaunchBegginingRotation.y + 90;
                }
                else
                {
                    SideAngle = LaunchBegginingRotation.y - 90;
                }

                CurrentAngleY = transform.rotation.eulerAngles.y;
               // Debug.Log(CurrentAngleY);
               // Debug.Log(SideAngle);

                if (CurrentAngleY < SideAngle && LaunchedLeft)
                {
                    OldRotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, (OldRotation.eulerAngles.y + 15f), transform.rotation.eulerAngles.z);
                    transform.rotation = Quaternion.Slerp(transform.rotation, OldRotation, 20f * Time.deltaTime);
                }

                else if (CurrentAngleY > SideAngle && LaunchedRight)
                {
                    OldRotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, (OldRotation.eulerAngles.y - 15f), transform.rotation.eulerAngles.z);
                    transform.rotation = Quaternion.Slerp(transform.rotation, OldRotation, 20f * Time.deltaTime);
                }
               
            }

            else if (LaunchedLeft)
            {
                CurrentAngleZ = transform.rotation.eulerAngles.z;
                float FlipAngle = 359;
               // Debug.Log(CurrentAngleZ);
              //  Debug.Log(FlipAngle);
                if (CurrentAngleZ < FlipAngle)
                {
                    OldRotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Mathf.Clamp(OldRotation.eulerAngles.z + 15, 0, 359));
                    transform.rotation = Quaternion.Slerp(transform.rotation, OldRotation, 20f * Time.deltaTime);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
                }
            }

            else if (LaunchedRight)
            {
                float FlipAngle;
                CurrentAngleZ = transform.rotation.eulerAngles.z;
                if (!FullRotationZ)
                {
                     FlipAngle = -360;
                     FullRotationZ = true;
                }
                else if (CurrentAngleZ >= 0 && CurrentAngleZ <= 20)
                {
                    FlipAngle = 360;
                }
                else
                {
                    FlipAngle = 0;
                }

              


               // Debug.Log(CurrentAngleZ);
               // Debug.Log(FlipAngle);
                if (CurrentAngleZ > FlipAngle)
                {   
                    OldRotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, OldRotation.eulerAngles.z - 15);
                    transform.rotation = Quaternion.Slerp(transform.rotation, OldRotation, 20f * Time.deltaTime);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
                }
            }


            else if (turnBackward)
            {
                BackwardsAngle = CalculateDesiredAngle(m_Cam.transform.rotation.eulerAngles.y, true, 180);
                CurrentAngleY = CalculateCurrentAngle(transform.rotation.eulerAngles.y, CurrentAngleY);

                if (CurrentAngleY < BackwardsAngle)
                {
                    OldRotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, (OldRotation.eulerAngles.y + 10f), transform.rotation.eulerAngles.z);
                    transform.rotation = Quaternion.Slerp(transform.rotation, OldRotation, 20f * Time.deltaTime);
                }
            }
            else if (Backwards)
            {
                //  Debug.Log("RapidFiring");
               // Debug.Log("Backwards");
                FullRotation = false;
                transform.rotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, m_Cam.transform.rotation.eulerAngles.y + 180, transform.rotation.eulerAngles.z);
                OldRotation = transform.rotation;
                CurrentAngleY = transform.rotation.eulerAngles.y;
            }

            else if (turnForward)
            {
                BackwardsAngle = CalculateDesiredAngle(m_Cam.transform.rotation.eulerAngles.y, false, 180);
                CurrentAngleY = CalculateCurrentAngle(transform.rotation.eulerAngles.y, CurrentAngleY);

                if (CurrentAngleY < BackwardsAngle)
                {
                    //  Debug.Log("TurnForward");
                    OldRotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, (OldRotation.eulerAngles.y + 10f), transform.rotation.eulerAngles.z);
                    transform.rotation = Quaternion.Slerp(transform.rotation, OldRotation, 20f * Time.deltaTime);
                }
            }

           else if (Atacking)
            {
                //  Debug.Log(m_Rigidbody.constraints);
                if (Locked)
                {
                    transform.rotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, m_Cam.transform.rotation.eulerAngles.y, m_Cam.transform.rotation.eulerAngles.z);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                }
               
            }

            else if ((AllowMovement))
            {
             //   m_Rigidbody.constraints = RigidbodyConstraints.None;
                OldRotation = transform.rotation;
                if (Locked)
                {
                    transform.rotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, m_Cam.transform.rotation.eulerAngles.y, m_Cam.transform.rotation.eulerAngles.z);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                }
                //   transform.rotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, m_Cam.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                FullRotation = false;
                CurrentAngleY = transform.rotation.eulerAngles.y;
            }

           else
            {
                FullRotation = false;
                //    transform.rotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, m_Cam.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                CurrentAngleY = transform.rotation.eulerAngles.y;
            }

        }






        /// <summary>
        /// Grounded Movement and Rotation
        /// </summary>
        /// <param name="jump"></param>
        /// <param name="move"></param>
        /// 
        void HandleGroundedMovement(bool jump, Vector3 move)
        {
            HandleGroundedRotation();

            if (jump)
            {
                m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
                m_IsGrounded = false;
                m_GroundCheckDistance = 0.1f;
            }
            else
            {

                //Rigidbody input movement

                if (m_Rigidbody.velocity.magnitude > SlideSpeed)
                {
                    Sliding = true;
                }
                else
                {
                    Sliding = false;
                }

                if (RapidFiring)
                {
                   m_Rigidbody.AddForce((move * m_MoveSpeedMultiplier), ForceMode.Force);
                }
                else if (Sliding)
                {
                    m_Rigidbody.AddForce(-FrictionForce * m_Rigidbody.velocity, ForceMode.Force);
                }
                else
                {
                    m_Rigidbody.velocity = (move * m_MoveSpeedMultiplier);
                }

                //Land Friction
                //m_Rigidbody.AddForce(new Vector3(-m_Rigidbody.velocity.x * FrictionGround, 0, -m_Rigidbody.velocity.z * FrictionGround));
                //m_Rigidbody.AddForce(-move * FrictionGround * 3);


                

                m_Rigidbody.velocity = new Vector3 (Mathf.Clamp(m_Rigidbody.velocity.x, -m_MaxSpeed, m_MaxSpeed), 0, Mathf.Clamp(m_Rigidbody.velocity.z, -m_MaxSpeed, m_MaxSpeed));
                //
              //  Debug.Log(m_Rigidbody.velocity);
            }
        }

        void ResetRotation()
        {
            {
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                transform.rotation = (Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0));

            }
        }

        void HandleGroundedRotation()
        {
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; ;
            //   transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, m_Cam.transform.rotation.eulerAngles.y, 0), Quaternion.Euler(0, transform.rotation.y, 0), TurnTime * Time.deltaTime);
            if (Atacking)
            {
                m_IsGrounded = false;
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; ;
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                OldRotation = transform.rotation; 
            }

            

            else if (turnBackward)
            {

                BackwardsAngle = CalculateDesiredAngle(m_Cam.transform.rotation.eulerAngles.y, true, 180);
                CurrentAngleY = CalculateCurrentAngle(transform.rotation.eulerAngles.y, CurrentAngleY);



                if (CurrentAngleY < BackwardsAngle)
                {
                    OldRotation = Quaternion.Euler(0, (OldRotation.eulerAngles.y + 10f), 0);
                    transform.rotation = Quaternion.Slerp(transform.rotation, OldRotation, 20f * Time.deltaTime);
                }
            }
            else if (Backwards)
            {
                FullRotation = false;
                transform.rotation = Quaternion.Euler(0, m_Cam.transform.rotation.eulerAngles.y + 180, 0);
                OldRotation = transform.rotation;
                CurrentAngleY = transform.rotation.eulerAngles.y;
            }


            else if (turnForward)
            {
                BackwardsAngle = CalculateDesiredAngle(m_Cam.transform.rotation.eulerAngles.y, false, 180);
                CurrentAngleY = CalculateCurrentAngle(transform.rotation.eulerAngles.y, CurrentAngleY);

                if (CurrentAngleY < BackwardsAngle)
                {
                    OldRotation = Quaternion.Euler(0, (OldRotation.eulerAngles.y + 10f), 0);
                    transform.rotation = Quaternion.Slerp(transform.rotation, OldRotation, 20f * Time.deltaTime);
                }
            }

            else if (Sliding)
            {
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                if (LaunchedLeft)
                {
                    transform.rotation = Quaternion.Euler(0, LaunchBegginingRotation.y + 90, 0);
                    LaunchedLeft = false;
                }
                else if (LaunchedRight)
                {
                    transform.rotation = Quaternion.Euler(0, LaunchBegginingRotation.y - 90, 0);
                    LaunchedRight = false;
                }
                 transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);


            }

            //  else if (moving && AllowMovement)
            else if (AllowMovement)
            {
                // Debug.Log("Moving");
                FullRotation = false;
                CurrentAngleY = transform.rotation.eulerAngles.y;
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                transform.rotation = (Quaternion.Euler(0, m_Cam.transform.rotation.eulerAngles.y, 0));
                OldRotation = transform.rotation;
            }
            else
            {
                FullRotation = false;
                CurrentAngleY = transform.rotation.eulerAngles.y;
                //Debug.Log("Idle");
                OldRotation = transform.rotation;
            }
        }






        private float CalculateDesiredAngle(float Angle, bool GoingBackwards, float DesiredAmount)
        {
            if (GoingBackwards)
            {
               return (Angle + DesiredAmount);
            }
            else
            {
                if (Angle < DesiredAmount)
                {
                    return(Angle + 360);
                }
                else
                {
                    return (Angle);
                }
            }
        }

        private float CalculateCurrentAngle(float AngleNow, float AngleBefore)
        {
            if (AngleNow < 90 && AngleBefore > 270)
            {
                FullRotation = true;
            }

            if (FullRotation)
            {
                return (AngleNow + 360);
            }
            else
            {
                return (AngleNow);
            }
        }

        public void PlayerTurn(int turn)
        {
            // Turn Backwards 
              if (turn == 1)
              {
               // Debug.Log("Turning");
                turnBackward = true;
                Backwards = false;
                turnForward = false;
                AllowMovement = false;
            }
            else if (turn == 2)
            {
                Backwards = true;
                turnBackward = false;
                turnForward = false;
                AllowMovement = true;
            }
            else if (turn == 3)
              {
               // Debug.Log("Turning Forwards!");
                  turnForward = true;
                  Backwards = false;
                  turnBackward = false;
                  AllowMovement = false;
            }
            
            else
              {
               // Debug.Log("Stoped Turning!");
                Backwards = false;
                  AllowMovement = true;
                  turnBackward = false;
                  turnForward = false;
              }
        }







        void UpdateAnimator(Vector3 move, bool QuickMove, bool LeftMove, bool RightMove, bool SpecialMove, bool ActiveGauntlets, bool ActiveSword, bool WeaponSwitch, bool m_BackwardsMove)
        {

            //
            // animation bools!
            //
           // Debug.Log(SpecialMove);
            if (SpecialMove && ActiveGauntlets)
            {
                RapidFiring = true;
                m_Animator.SetBool(("RapidFire"), true);
            }

            else if (Sliding)
            {
                m_Animator.SetBool(("RapidFire"), false);
                m_Animator.SetBool("Moving", false);
                RapidFiring = false;
                moving = true;
                m_Animator.SetBool("Sliding", true);
              
            }

            else if (move != Vector3.zero && m_IsGrounded)
            {
                m_Animator.SetBool("Sliding", false);
                m_Animator.SetBool(("RapidFire"), false);
                RapidFiring = false;
                moving = true;
                m_Animator.SetBool("Moving", true);
                float RunSpeed = 1.35f;
                float SlowDownSpeed = 0.8f;

               // if (move.magnitude == 1)
             //   {
             //       m_Animator.speed = RunSpeed;
           //     }
         //       else
            //    {
            //        m_Animator.speed = SlowDownSpeed;
             //   }
            }

            else 
               {
                    m_Animator.SetBool("Sliding", false);
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


                else if (m_BackwardsMove)
                {
                    m_Animator.SetTrigger("BackwardLaunch");
                }
            }

                if (ActiveSword)
                {
                    //  Debug.Log("ActiveSword");
                    if (WeaponSwitch)
                    {
                        m_Animator.SetTrigger("SwordOutTrig");
                    }
                    else if (M_Blocked)
                {
                    Debug.Log("GotBlocked");
                    m_Animator.SetTrigger("Blocked");
                    M_Blocked = false;
                }
                    else if (QuickMove)
                    {
                        m_Animator.SetTrigger(("QuickAttack"));
                    }
                    else if (SpecialMove)
                    {
                        m_Animator.SetTrigger("StrongAttack");
                    }
                    else if (RightMove && m_IsGrounded && !Sliding)
                    {
                        m_Animator.SetTrigger("SideStepRight");
                    }
                    else if (LeftMove && m_IsGrounded && !Sliding)
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

        public void Hit()
        {
            m_IsGrounded = false;
            m_GroundCheckDistance = 0.1f;
        }
       
       
        public void Blocked()
        {
            M_Blocked = true;
        }
        

        private bool LandGuider(float yVelocity, float time, bool Launched)
        {
            if (Launched && yVelocity < 0)
            {
                float LandingDistance = -yVelocity * time;
                if (Physics.Raycast(transform.position - (Vector3.up * .5f), Vector3.down, LandingDistance)){
                   // Debug.Log("About to hit ground");
                    return true; 
                }
            }
            return false;
        }

        public void LaunchStatus(int direction)
        {
            if (direction == 0)
            {
                LaunchedLeft = true;
                LaunchBegginingRotation = transform.rotation.eulerAngles;
            }
            else if (direction == 1)
            {
                LaunchedRight = true;
                LaunchBegginingRotation = transform.rotation.eulerAngles;
            }
        }

    }
}