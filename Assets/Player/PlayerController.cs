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
        public float SpinHeightDistance = 7f;

        private Quaternion OldRotation;
        private float BackwardsAngle;
        private float CurrentAngleY;
        private float CurrentAngleZ = 0;
        private float CurrentAngleX = 0;

        private bool FullRotation = false;
    //    private bool FullRotationZ = false;
    //    private bool FullRotationX = false;

        private bool RecentlyLaunched = false;
        private bool initialLaunch = false; 
        public bool LaunchedLeft = false;
        private bool LaunchedRight = false;
        private bool LaunchedForward = false;
        private bool LaunchedBackward = false;

        private float LandingTime = .25f;
        private bool AbouttoLand = false;
        private Vector3 LaunchBegginingRotation; 

        private bool M_Blocked = false;
        public bool P_Flip = false;
        private int spinCount = 0;
        private bool Spining = false;
        private bool JustSpun = false;
        private bool LandingHard = false;
        private Vector3 AirSpeed;

        private CapsuleCollider m_Collider;
        public float ShieldTransitionTime = .3f;
        public bool ShieldSliding = false;
        private Quaternion ShieldBegginingRotation;
        public float ShieldTurnSpeed = 5f;
        private float OldLookingAngle;
        public bool InvertSlide = false; 

        // Use this for initialization
        void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Animator = GetComponent<Animator>();
            m_Collider = GetComponent<CapsuleCollider>();

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

        public void Move(Vector3 move, bool jump, bool QuickMove, bool LeftMove, bool RightMove, bool SpecialMove, bool ActiveGauntlets, bool ActiveSword, bool WeaponSwitch, bool m_BackwardsMove, bool Locked, bool  m_ForwardMovement, bool Slidinginput,float h)
        {

            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            if (move.magnitude > 1f) move.Normalize();
            CheckGroundStatus();
          //  move = Vector3.ProjectOnPlane(move, m_GroundNormal);
            //m_TurnAmount = Mathf.Atan2(move.x, move.z);
            //m_ForwardAmount = move.z;


            // control and velocity handling is different when grounded and airborne:
            if (ShieldSliding)
            {
              
                if (!m_IsGrounded)
                {
                    HandleAirborneRotation(move, Locked,  LaunchedLeft, LaunchedRight, LaunchedForward, LaunchedBackward);
                }
                else {
                    HandleShieldSlidingMovement(h);
                  
                   
                }
                if (!Slidinginput)
                {
                    m_OrigGroundCheckDistance -= .7f;
                    m_GroundCheckDistance -= .7f;
                    ShieldSliding = false;

                    //Jump
                    if (m_IsGrounded)
                    {
                        m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
                        m_IsGrounded = false;
                        m_GroundCheckDistance = 0.1f;
                    }
                }
            }

            else if (m_IsGrounded)
            {
                SendMessage("UpdateXMin", -10);
                HandleGroundedMovement(jump, move);
            }
            else
            {
                HandleAirborneMovement(move, Locked, Slidinginput);
               
            }
            //Update animator
            UpdateAnimator(move, QuickMove, LeftMove, RightMove, SpecialMove, ActiveGauntlets, ActiveSword, WeaponSwitch, m_BackwardsMove, m_ForwardMovement);

        }



        void HandleShieldSlidingMovement(float h)
        {

            bool PastLaunchedForward;
            bool PastLaunchedBackward;
            if (LaunchedForward)
            {
                PastLaunchedForward = true;
            }
            else
            {
                PastLaunchedForward = false;
            }
            if (LaunchedBackward)
            {
                PastLaunchedBackward = true; 
            }
            else
            {
                PastLaunchedBackward = false;
            }

            if (InvertSlide)
            {
                h = -h;
            }
            else{
                h = +h;
            }

            ResetLaunches();
            HandleShieldSlideRotation(h, PastLaunchedForward, PastLaunchedBackward);

            //      m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, 0, h * ShieldTurnSpeed);
            if (m_Rigidbody.velocity.magnitude > 10)
            {
                m_Rigidbody.AddRelativeForce(0, 0, h * ShieldTurnSpeed * m_Rigidbody.velocity.magnitude * .1f);
            }
         
            if (LandingHard && m_IsGrounded)
            {
                LandingHard = false;
                m_Rigidbody.velocity = AirSpeed;
            }
        }

        void HandleShieldSlideRotation(float TurnDirection, bool ForwardsLaunch, bool BackwardsLaunch)
        {
            if (ForwardsLaunch)
            {
                Debug.Log("TurnedForward");
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                transform.rotation = Quaternion.Euler(0, ShieldBegginingRotation.eulerAngles.y - 90, 0); 
            }
            else if (BackwardsLaunch)
            {
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                transform.rotation = Quaternion.Euler(0, ShieldBegginingRotation.eulerAngles.y - 90, 0);
                OldLookingAngle += 180;
            }
            else if(TurnDirection > 0 && m_Rigidbody.velocity.magnitude > 10)
            {
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(10, transform.rotation.eulerAngles.y, 0), Time.deltaTime * 20f);
                if (transform.rotation.eulerAngles.x > 0 && transform.rotation.eulerAngles.x <= 10)
                {
                    transform.rotation = Quaternion.Euler(Mathf.Clamp(transform.rotation.eulerAngles.x, 0, 10), transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                }
            }
            else if(TurnDirection < 0 && m_Rigidbody.velocity.magnitude > 10)
            {
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(-10, transform.rotation.eulerAngles.y, 0), Time.deltaTime*20f);
                if (transform.rotation.eulerAngles.x > 350 && transform.rotation.eulerAngles.x <= 360)
                {
                    transform.rotation = Quaternion.Euler(Mathf.Clamp(transform.rotation.eulerAngles.x, 350, 360), transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                }   
            }
            else if (m_Rigidbody.velocity.magnitude > 10)
            {
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), Time.deltaTime * 20f); ;
            }
            else
            {
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
                transform.rotation =  Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) ;
            }
           

        }


        /// <summary>
        /// Airoborne Movement and Rotation
        /// </summary>
        /// <param name="move"></param>
        void HandleAirborneMovement(Vector3 move, bool Locked, bool SlidingInput)
        {

            //Debug.Log(CheckForShieldSlide(m_Rigidbody.velocity.y, ShieldTransitionTime));

            if (SlidingInput)
            {
                ShieldSliding = CheckForShieldSlide(m_Rigidbody.velocity.y, ShieldTransitionTime);
                if (ShieldSliding)
                {
                    ShieldBegginingRotation = transform.rotation;
                    m_OrigGroundCheckDistance += .7f;
                }
            }
            

            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
            m_Rigidbody.AddForce(extraGravityForce);
            m_Rigidbody.velocity = new Vector3(Mathf.Clamp(m_Rigidbody.velocity.x, -m_MaxSpeed, m_MaxSpeed), m_Rigidbody.velocity.y, Mathf.Clamp(m_Rigidbody.velocity.z, -m_MaxSpeed, m_MaxSpeed));

         //   Debug.Log("Y " + m_Rigidbody.velocity.y);
        //    Debug.Log("XZ " + m_Rigidbody.velocity.x + m_Rigidbody.velocity.z);
            HandleAirborneRotation(move, Locked, LaunchedLeft, LaunchedRight, LaunchedForward, LaunchedBackward);
          
          
        }

        void HandleAirborneRotation(Vector3 move, bool Locked, bool LaunchedLeft, bool LaunchedRight, bool LaunchedForward, bool LaunchedBackward)
        {

            m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
            // allow rotation
            m_Rigidbody.constraints = RigidbodyConstraints.None;

            AbouttoLand = LandGuider(m_Rigidbody.velocity.y, LandingTime);
            SendMessage("UpdateXMin", -90);


            // LaunchLeftAirborneMovement
            if (LaunchedLeft)
            {
                if (P_Flip && Spining)
                {
                    JustSpun = true;

                    m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                    if (transform.rotation.eulerAngles.z < 349 && spinCount == 0)
                    {
                        transform.Rotate(Vector3.forward * Time.deltaTime * 500, Space.Self);
                    }
                    else
                    {
                        spinCount = 1;
                        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                        Spining = false;
                    }
                }
                else if (AbouttoLand && !ShieldSliding)
                {
                    m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
                    RotatetoLand();
                }
                else if (ShieldSliding)
                {
                    m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                    transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                    //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, ShieldBegginingRotation.eulerAngles.y + 90, 0), Time.deltaTime * 20);
                }
                else
                {
                    m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                    transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                }
                CheckForBadLanding();

            }


            else if (LaunchedRight)
            {
                if (P_Flip && Spining)
                {
                    JustSpun = true;

                    m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                    if ((transform.rotation.eulerAngles.z > 11 || transform.rotation.eulerAngles.z <= 0) && spinCount == 0)
                    {
                        transform.Rotate(Vector3.forward * Time.deltaTime * -500, Space.Self);
                    }
                    else
                    {
                        Spining = false;
                        spinCount = 1;
                        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                    }
                }
                else if (AbouttoLand && !ShieldSliding)
                {
                    m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
                    RotatetoLand();
                }
                else if (ShieldSliding)
                {
                    m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                    transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                    //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, ShieldBegginingRotation.eulerAngles.y + 90, 0), Time.deltaTime * 20);
                }
                else
                {
                    m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                    transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                }
                CheckForBadLanding();

            }

            else if (LaunchedForward)
            {
                if (P_Flip && Spining)
                {
                    JustSpun = true;

                    m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
                    bool IncreasingAngle = (OldRotation.eulerAngles.x < transform.rotation.eulerAngles.x);
                    OldRotation = transform.rotation;

                    if (transform.rotation.eulerAngles.x < 280 && transform.rotation.eulerAngles.x > 270 && IncreasingAngle)
                    {
                        spinCount = 1;
                    }
                    if (transform.rotation.eulerAngles.x < 360 && transform.rotation.eulerAngles.x > 350 && IncreasingAngle && spinCount == 1)
                    {
                        spinCount = 2;
                    }

                    if ((spinCount < 2))
                    {

                        transform.Rotate(Vector3.right * Time.deltaTime * 400, Space.Self);


                    }
                    else
                    {
                        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                        Spining = false;
                    }
                }
                else if (AbouttoLand && !ShieldSliding)
                {
                    m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                    transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                }
                else if (ShieldSliding)
                {
                    // transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                    m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, ShieldBegginingRotation.eulerAngles.y - 90, 0), Time.deltaTime * 20);
                }
                else
                {
                    m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                    transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                }
                CheckForBadLanding();

            }

            else if (LaunchedBackward)
            {
                if (P_Flip && Spining)
                {
                    JustSpun = true;
                    // Debug.Log(transform.rotation.eulerAngles.x);
                    bool DecreasingAngle = (OldRotation.eulerAngles.x > transform.rotation.eulerAngles.x);
                    // Debug.Log(DecreasingAngle);
                    //   Debug.Log(spinCount);
                    m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
                    OldRotation = transform.rotation;

                    if (transform.rotation.eulerAngles.x < 90 && transform.rotation.eulerAngles.x > 80 && !DecreasingAngle)
                    {
                        spinCount = 1;
                    }
                    if (transform.rotation.eulerAngles.x < 10 && transform.rotation.eulerAngles.x > 0 && DecreasingAngle && spinCount == 1)
                    {
                        spinCount = 2;
                    }
                    if (spinCount < 2)
                    {

                        transform.Rotate(Vector3.right * Time.deltaTime * -400, Space.Self);
                    }

                    else
                    {
                        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                        Spining = false;
                    }
                }
                else if (AbouttoLand && !ShieldSliding)
                {
                    m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                    transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                }
                else if (ShieldSliding)
                {
                    // transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                    m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, ShieldBegginingRotation.eulerAngles.y - 90, 0), Time.deltaTime * 20);
                }
                else
                {
                    m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                    transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                }
                CheckForBadLanding();

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

            else
            {
                // FullRotation = false;
                //    transform.rotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, m_Cam.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
                CurrentAngleY = transform.rotation.eulerAngles.y;
                if (Locked)
                {
                    transform.rotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, m_Cam.transform.rotation.eulerAngles.y, m_Cam.transform.rotation.eulerAngles.z);
                }
                else
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, OldLookingAngle, 0), Time.deltaTime*10f);
                }
            }



            //FOR RAPID FIRE//
            //    else if (turnBackward)
            //    {
            //        BackwardsAngle = CalculateDesiredAngle(m_Cam.transform.rotation.eulerAngles.y, true, 180);
            //        CurrentAngleY = CalculateCurrentAngle(transform.rotation.eulerAngles.y, CurrentAngleY);

            //    if (CurrentAngleY < BackwardsAngle)
            //    {
            //        OldRotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, (OldRotation.eulerAngles.y + 10f), transform.rotation.eulerAngles.z);
            //        transform.rotation = Quaternion.Slerp(transform.rotation, OldRotation, 20f * Time.deltaTime);
            //    }
            //}
            //else if (Backwards)
            //{
            //    //  Debug.Log("RapidFiring");
            //    // Debug.Log("Backwards");
            //    FullRotation = false;
            //    transform.rotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, m_Cam.transform.rotation.eulerAngles.y + 180, transform.rotation.eulerAngles.z);
            //    OldRotation = transform.rotation;
            //    CurrentAngleY = transform.rotation.eulerAngles.y;
            //}

            //else if (turnForward)
            //{
            //    BackwardsAngle = CalculateDesiredAngle(m_Cam.transform.rotation.eulerAngles.y, false, 180);
            //    CurrentAngleY = CalculateCurrentAngle(transform.rotation.eulerAngles.y, CurrentAngleY);

            //    if (CurrentAngleY < BackwardsAngle)
            //    {
            //        //  Debug.Log("TurnForward");
            //        OldRotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, (OldRotation.eulerAngles.y + 10f), transform.rotation.eulerAngles.z);
            //        transform.rotation = Quaternion.Slerp(transform.rotation, OldRotation, 20f * Time.deltaTime);
            //    }
            //}



            //else if ((AllowMovement))
            //{
            //    //   m_Rigidbody.constraints = RigidbodyConstraints.None;
            //    OldRotation = transform.rotation;
            //    if (Locked)
            //    {
            //        transform.rotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, m_Cam.transform.rotation.eulerAngles.y, m_Cam.transform.rotation.eulerAngles.z);
            //    }
            //    else
            //    {
            //        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            //    }
            //    //   transform.rotation = Quaternion.Euler(m_Cam.transform.rotation.eulerAngles.x, m_Cam.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            //    FullRotation = false;
            //    CurrentAngleY = transform.rotation.eulerAngles.y;



            //}



        }


        void RotatetoLand()
        {
          //  Debug.Log("Landing");
            Sliding = true;
        

            CurrentAngleY = transform.rotation.eulerAngles.y;
            // Debug.Log(CurrentAngleY);
            // Debug.Log(SideAngle);

            if (LaunchedLeft)
            {
                transform.Rotate(Vector3.up * Time.deltaTime * 400, Space.Self);
            }

            else if (LaunchedRight)
            {
                transform.Rotate(Vector3.up * Time.deltaTime * -400, Space.Self);
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

            if (jump)
            {
                Debug.Log("Jumped");
                m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
                m_IsGrounded = false;
                m_GroundCheckDistance = 0.1f;
            }

            HandleGroundedRotation();

            if (LandingHard)
            {
                LandingHard = false;
                m_Rigidbody.velocity = AirSpeed;
            }
            if (m_Rigidbody.velocity.magnitude > SlideSpeed)
            {
                Sliding = true;
            }
            else
            {
                ResetLaunches();
                Sliding = false;
            }

            if (Sliding)
            {
                m_Rigidbody.AddForce(-FrictionForce * m_Rigidbody.velocity, ForceMode.Force);
            }
            else
            {
                m_Rigidbody.velocity = (move * m_MoveSpeedMultiplier);
                m_Rigidbody.velocity = new Vector3(Mathf.Clamp(m_Rigidbody.velocity.x, -m_MaxSpeed, m_MaxSpeed), 0, Mathf.Clamp(m_Rigidbody.velocity.z, -m_MaxSpeed, m_MaxSpeed));
            }
       

                //Rigidbody input movement
                //Debug.Log(m_Rigidbody.velocity.magnitude);
                

                //if (RapidFiring)
                //{
                //   m_Rigidbody.AddForce((move * m_MoveSpeedMultiplier), ForceMode.Force);
                //}

                //Land Friction
                //m_Rigidbody.AddForce(new Vector3(-m_Rigidbody.velocity.x * FrictionGround, 0, -m_Rigidbody.velocity.z * FrictionGround));
                //m_Rigidbody.AddForce(-move * FrictionGround * 3);

              //  Debug.Log(m_Rigidbody.velocity);
            
        }

        void ResetLaunches()
        {
            LaunchedLeft = false;
            LaunchedRight = false;
            LaunchedForward = false;
            LaunchedBackward = false;
            spinCount = 0;
            Spining = false;
            JustSpun = false;



        }


        //void ResetRotation()
        //{
        //    {
        //        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        //        transform.rotation = (Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0));

        //    }
        //}

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

            else if (Sliding)
            {
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                if (LaunchedLeft)
                {
                    transform.rotation = Quaternion.Euler(0, LaunchBegginingRotation.y + 90, 0);

                }
                else if (LaunchedRight)
                {
                    transform.rotation = Quaternion.Euler(0, LaunchBegginingRotation.y - 90, 0);

                }
                else if (LaunchedForward)
                {
                    Debug.Log("SlidingForward");
                    transform.rotation = Quaternion.Euler(0, LaunchBegginingRotation.y, 0);
                }
                else if (LaunchedBackward)
                {
                    transform.rotation = Quaternion.Euler(0, LaunchBegginingRotation.y, 0);
                }
                //        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);


            }

            else
            {
               
                FullRotation = false;
                CurrentAngleY = transform.rotation.eulerAngles.y;
                transform.rotation = Quaternion.Euler(0, m_Cam.transform.rotation.eulerAngles.y, 0);
                //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, m_Cam.transform.rotation.eulerAngles.y, 0), Time.deltaTime*20);
                //Debug.Log("Idle");
                OldRotation = transform.rotation;
                OldLookingAngle = transform.rotation.eulerAngles.y;
            }



            //FOR RAPID FIREING//
            //else if (turnBackward)
            //{

            //    BackwardsAngle = CalculateDesiredAngle(m_Cam.transform.rotation.eulerAngles.y, true, 180);
            //    CurrentAngleY = CalculateCurrentAngle(transform.rotation.eulerAngles.y, CurrentAngleY);



            //    if (CurrentAngleY < BackwardsAngle)
            //    {
            //        OldRotation = Quaternion.Euler(0, (OldRotation.eulerAngles.y + 10f), 0);
            //        transform.rotation = Quaternion.Slerp(transform.rotation, OldRotation, 20f * Time.deltaTime);
            //    }
            //}
            //else if (Backwards)
            //{
            //    FullRotation = false;
            //    transform.rotation = Quaternion.Euler(0, m_Cam.transform.rotation.eulerAngles.y + 180, 0);
            //    OldRotation = transform.rotation;
            //    CurrentAngleY = transform.rotation.eulerAngles.y;
            //}


            //else if (turnForward)
            //{
            //    BackwardsAngle = CalculateDesiredAngle(m_Cam.transform.rotation.eulerAngles.y, false, 180);
            //    CurrentAngleY = CalculateCurrentAngle(transform.rotation.eulerAngles.y, CurrentAngleY);

            //    if (CurrentAngleY < BackwardsAngle)
            //    {
            //        OldRotation = Quaternion.Euler(0, (OldRotation.eulerAngles.y + 10f), 0);
            //        transform.rotation = Quaternion.Slerp(transform.rotation, OldRotation, 20f * Time.deltaTime);
            //    }
            //}



            //  else if (moving && AllowMovement)
            //else if (AllowMovement)
            //{
            //    // Debug.Log("Moving");
            //    FullRotation = false;
            //    CurrentAngleY = transform.rotation.eulerAngles.y;
            //    m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            //    transform.rotation = (Quaternion.Euler(0, m_Cam.transform.rotation.eulerAngles.y, 0));
            //    OldRotation = transform.rotation;
            //}

        }






        //private float CalculateDesiredAngle(float Angle, bool GoingBackwards, float DesiredAmount)
        //{
        //    if (GoingBackwards)
        //    {
        //       return (Angle + DesiredAmount);
        //    }
        //    else
        //    {
        //        if (Angle < DesiredAmount)
        //        {
        //            return(Angle + 360);
        //        }
        //        else
        //        {
        //            return (Angle);
        //        }
        //    }
        //}

        //private float CalculateCurrentAngle(float AngleNow, float AngleBefore)
        //{
        //    if (AngleNow < 90 && AngleBefore > 270)
        //    {
        //        FullRotation = true;
        //    }

        //    if (FullRotation)
        //    {
        //        return (AngleNow + 360);
        //    }
        //    else
        //    {
        //        return (AngleNow);
        //    }
        

        //public void PlayerTurn(int turn)
        //{
        //    // Turn Backwards 
        //      if (turn == 1)
        //      {
        //       // Debug.Log("Turning");
        //        turnBackward = true;
        //        Backwards = false;
        //        turnForward = false;
        //        AllowMovement = false;
        //    }
        //    else if (turn == 2)
        //    {
        //        Backwards = true;
        //        turnBackward = false;
        //        turnForward = false;
        //        AllowMovement = true;
        //    }
        //    else if (turn == 3)
        //      {
        //       // Debug.Log("Turning Forwards!");
        //          turnForward = true;
        //          Backwards = false;
        //          turnBackward = false;
        //          AllowMovement = false;
        //    }
            
        //    else
        //      {
        //       // Debug.Log("Stoped Turning!");
        //        Backwards = false;
        //          AllowMovement = true;
        //          turnBackward = false;
        //          turnForward = false;
        //      }
        //}







        void UpdateAnimator(Vector3 move, bool QuickMove, bool LeftMove, bool RightMove, bool SpecialMove, bool ActiveGauntlets, bool ActiveSword, bool WeaponSwitch, bool m_BackwardsMove,
            bool m_ForwardMovement)
        {
            // Debug.Log(move);

            //
            // animation bools!
            //
            // Debug.Log(SpecialMove);
            bool LeftInput = false;
            bool RightInput = false;
            bool ForwardInput = false; 
            bool BackwardInput = false;
          

            if (m_IsGrounded)
            {
                RecentlyLaunched = false;
            }


            if (LeftMove)
            {
                LeftInput = true;
                RightInput = false;
                ForwardInput = false;
                BackwardInput = false;
            } 
            else if (RightMove)
            {
                LeftInput = false;
                RightInput = true;
                ForwardInput = false;
                BackwardInput = false;
            }
            else if (m_ForwardMovement)
            {
                LeftInput = false;
                RightInput = false;
                ForwardInput = true;
                BackwardInput = false;
            }
            else if (m_BackwardsMove)
            {
                LeftInput = false;
                RightInput = false;
                ForwardInput = false;
                BackwardInput = true;
            }
          else
            {
                LeftInput = false;
                RightInput = false;
                ForwardInput = false;
                BackwardInput = false;
            }




            //    if (SpecialMove && ActiveGauntlets)
            //      {
            //       RapidFiring = true;
            //       m_Animator.SetBool(("RapidFire"), true);
            //   }

            if (ShieldSliding)
            {
                m_Animator.SetBool("ShieldSliding", true);
                if (LaunchedRight || (LaunchedForward && !JustSpun) || (LaunchedBackward && JustSpun))
                {
                    BroadcastMessage("ShieldSlidingRight");
                    InvertSlide = true;
                }
                else if (LaunchedLeft || (LaunchedForward && JustSpun) || (LaunchedBackward && !JustSpun))
                {
                    InvertSlide = false;
                    BroadcastMessage("ShieldSlidingLeft");
                }
               
            }

           else if (Sliding)
            {
                m_Animator.SetBool(("RapidFire"), false);
                m_Animator.SetBool("Moving", false);
                m_Animator.SetBool("ShieldSliding", false);
                RapidFiring = false;
                moving = true;
                BroadcastMessage("NotSliding");
                m_Animator.SetBool("Sliding", true);
              
            }

            else if (move != Vector3.zero && m_IsGrounded)
            {
                m_Animator.SetBool("Sliding", false);
                m_Animator.SetBool("ShieldSliding", false);
                m_Animator.SetBool(("RapidFire"), false);
                RapidFiring = false;
                moving = true;
                BroadcastMessage("NotSliding");
                m_Animator.SetBool("Moving", true);
           //     float RunSpeed = 1.35f;
            //    float SlowDownSpeed = 0.8f;

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
                    m_Animator.SetBool("ShieldSliding", false);
                    BroadcastMessage("NotSliding");
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

                if (!ShieldSliding && !Sliding)
                {

                    if (SpecialMove  && LeftInput && !RecentlyLaunched)
                    {
                        if (m_IsGrounded)
                        {
                            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, .8f * m_JumpPower, m_Rigidbody.velocity.z);
                        }
                        m_Animator.SetTrigger("LeftLaunch");
                        RecentlyLaunched = true;
                    }
                    else if (SpecialMove && RightInput && !RecentlyLaunched)
                    {
                        if (m_IsGrounded)
                        {
                            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, .8f * m_JumpPower, m_Rigidbody.velocity.z);
                        }
                        m_Animator.SetTrigger("RightLaunch");
                        RecentlyLaunched = true;
                    }
                    else if (SpecialMove && BackwardInput && !RecentlyLaunched)
                    {
                        if (m_IsGrounded)
                        {
                            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, .8f * m_JumpPower, m_Rigidbody.velocity.z);
                        }

                        m_Animator.SetTrigger("BackwardLaunch");
                        RecentlyLaunched = true;
                    }
                    else if (SpecialMove && ForwardInput && !RecentlyLaunched)
                    {
                        if (m_IsGrounded)
                        {
                            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, .8f * m_JumpPower, m_Rigidbody.velocity.z);
                        }
                        m_Animator.SetTrigger("LaunchForwards");
                        RecentlyLaunched = true;
                    }

                    else if (SpecialMove && m_IsGrounded && !initialLaunch)
                    {
                        m_Animator.SetTrigger("UpLaunch");
                        initialLaunch = true;

                    }

                }

                else
                {
                    if (SpecialMove && ForwardInput)
                    {
                        if (InvertSlide)
                        {
                            m_Animator.SetTrigger("ShieldSlideBoostInv");
                            RecentlyLaunched = true;
                        }
                        else
                        {
                            m_Animator.SetTrigger("ShieldSlideBoost");
                            RecentlyLaunched = true;
                        }
                    }
                  
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
                        if (InvertSlide && ShieldSliding)
                        {
                            m_Animator.SetTrigger(("QuickAttack2"));
                        }
                        else
                        {
                            m_Animator.SetTrigger(("QuickAttack"));
                        }
                       
                    }
                    else if (SpecialMove && !ShieldSliding)
                    {
                        m_Animator.SetTrigger("StrongAttack");
                    }
                  //  else if (RightMove && m_IsGrounded && !Sliding)
               //     {
                //        m_Animator.SetTrigger("SideStepRight");
                 //   }
                 //   else if (LeftMove && m_IsGrounded && !Sliding)
                  //  {
                  //      m_Animator.SetTrigger("SideStepLeft");
                  //  }
                  

                    
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

        private void CheckForBadLanding()
        {
        //    if (Mathf.Abs(m_Rigidbody.velocity.y) > (Mathf.Abs(m_Rigidbody.velocity.x) + Mathf.Abs(m_Rigidbody.velocity.z)))
            {
               // Debug.Log("Landed Hard");
                LandingHard = true;
                AirSpeed = new Vector3(m_Rigidbody.velocity.x, 0, m_Rigidbody.velocity.z);
            }
        }

        private bool CheckForShieldSlide(float yVelocity, float time)
        {
                float LandingDistance = -yVelocity * time;
                if (yVelocity < 0)
                {
                    if (Physics.Raycast(transform.position - (Vector3.up * .5f), Vector3.down, LandingDistance))
                    {
                        // Debug.Log("About to hit ground");
                        return false;
                    }
                    return true;
                }
                return false;
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
        

        private bool LandGuider(float yVelocity, float time)
        {
            if (yVelocity < 0)
            {
                float LandingDistance = -yVelocity * time;
                if (Physics.Raycast(transform.position - (Vector3.up * .5f), Vector3.down, LandingDistance)){
                   // Debug.Log("About to hit ground");
                    return true; 
                }
            }
            return false;
        }

        private bool CheckIfToCloseToSpin(float LandingDistance)
        {
            
                if (Physics.Raycast(transform.position - (Vector3.up * .5f), Vector3.down, LandingDistance))
                {
                    // Debug.Log("About to hit ground");
                    return true;
                }
            
            return false;
        }


        public void LaunchStatus(int direction)
        {
            if (direction == 0)
            {
                LaunchedLeft = true;
                P_Flip = !CheckIfToCloseToSpin(SpinHeightDistance);
                if (P_Flip)
                {
                    Spining = true;
                }
                LaunchBegginingRotation = transform.rotation.eulerAngles;
            }
            else if (direction == 1)
            {
                LaunchedRight = true;
                P_Flip = !CheckIfToCloseToSpin(SpinHeightDistance);
                if (P_Flip)
                {
                    Spining = true;
                }
                LaunchBegginingRotation = transform.rotation.eulerAngles;
            }
            else if (direction == 2)
            {
                LaunchedForward = true;
                P_Flip = !CheckIfToCloseToSpin(SpinHeightDistance);
                if (P_Flip)
                {
                    Spining = true;
                }
                LaunchBegginingRotation = transform.rotation.eulerAngles;
            }
            else if (direction == 3)
            {
                LaunchedBackward = true;
                P_Flip = !CheckIfToCloseToSpin(SpinHeightDistance);
                if (P_Flip)
                {
                    Spining = true;
                }
                LaunchBegginingRotation = transform.rotation.eulerAngles;
            }
        }

        public void Launched()
        {
            initialLaunch = false; 
        }

        public void ExpandCollider()
        {
            m_Collider.height = 1.2f;
            m_Collider.center = new Vector3(0, -.1f, 0);

        }

        public void ShrinkCollider()
        {
            m_Collider.height = 1f;
            m_Collider.center = new Vector3(0, 0, 0);
        }

    }
}