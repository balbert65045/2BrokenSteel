using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{


    public class FlipC : MonoBehaviour
    {


        private PC PlayerController;
        private AnimC AnimC;
        private Rigidbody M_rigidbody;


        private bool ShieldSliding;

        public float LandingTime = 4f;
        public float SpinHeightDistance = .3f;

        public bool LaunchedLeft;
        public bool LaunchedRight;
        public bool LaunchedForward;
        public bool LaunchedBackward;

        public bool AllowShieldSlideRotation;

        private int spinCount = 0;
        public bool P_Flip = false;
        private bool JustSpun = false;

        private float LaunchBegginingRotation = 0;
        private Quaternion OldRoatation;


        void Start()
        {
            AnimC = GetComponent<AnimC>();
            M_rigidbody = GetComponent<Rigidbody>();
            PlayerController = GetComponent<PC>();

            ShieldSliding = GetComponent<ShieldSlidingController>().ShieldSliding;
            OldRoatation = transform.rotation;
        }

        void Update()
        {

            if (PlayerController.m_IsGrounded)
            {
                ResetLaunches();
            }

        }


        void FixedUpdate()
        {
            if (LaunchedLeft | LaunchedRight | LaunchedForward | LaunchedBackward)
            {
                PlayerController.OutsideRotation1(true);
                HandleAirborneRotation();
            }
            else
            {
                PlayerController.OutsideRotation1(false);
            }
        }

        void ResetLaunches()
        {
            LaunchedLeft = false;
            LaunchedRight = false;
            LaunchedForward = false;
            LaunchedBackward = false;
            spinCount = 0;
            P_Flip = false;
            JustSpun = false;
        }


        void HandleAirborneRotation()
        {

            bool AbouttoLand = LandGuider(M_rigidbody.velocity.y, LandingTime);

            // LaunchLeftAirborneMovement
            if (LaunchedLeft)
            {
               
                if (P_Flip)
                {
                    AllowShieldSlideRotation = false;
                    JustSpun = true;

                    M_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                    if (transform.rotation.eulerAngles.z < 349 && spinCount == 0)
                    {
                        transform.Rotate(Vector3.forward * Time.deltaTime * 500, Space.Self);
                    }
                    else
                    {
                        spinCount = 1;
                        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                        P_Flip = false;
                    }
                }
                else if (AbouttoLand && !ShieldSliding && JustSpun)
                {
                    AllowShieldSlideRotation = false;
                    PlayerController.HandleRotation(0, LaunchBegginingRotation + 90, 0, true, false, true, true);

                }

                else if (ShieldSliding)
                {
                    AllowShieldSlideRotation = true;
                }

                else
                {
                    PlayerController.HandleRotation(0, transform.rotation.eulerAngles.y, 0, true, true, true, false);
                }
            //    PlayerController.CheckForBadLanding();

            }


            else if (LaunchedRight)
            {

                if (P_Flip)
                {
                    AllowShieldSlideRotation = false;
                    JustSpun = true;

                    M_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                    if ((transform.rotation.eulerAngles.z > 11 || transform.rotation.eulerAngles.z <= 0) && spinCount == 0)
                    {
                        transform.Rotate(Vector3.forward * Time.deltaTime * -500, Space.Self);
                    }
                    else
                    {
                        P_Flip = false;
                        spinCount = 1;
                        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
                    }
                }
                else if (AbouttoLand && !ShieldSliding && JustSpun)
                {
                    AllowShieldSlideRotation = false;
                    PlayerController.HandleRotation(0, LaunchBegginingRotation - 90, 0, true, false, true, true);
                }

                else if (ShieldSliding)
                {
                    AllowShieldSlideRotation = true;
                }

                else
                {
                    PlayerController.HandleRotation(0, transform.rotation.eulerAngles.y, 0, true, true, true, false);
                }
               // PlayerController.CheckForBadLanding();

            }

            else if (LaunchedForward)
            {

                if (P_Flip)
                {
                    AllowShieldSlideRotation = false;
                    JustSpun = true;

                    M_rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
                    bool IncreasingAngle = (OldRoatation.eulerAngles.x < transform.rotation.eulerAngles.x);
                    OldRoatation = transform.rotation;

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
                        P_Flip = false;
                    }
                }
                else if (AbouttoLand && !ShieldSliding && JustSpun)
                {
                    AllowShieldSlideRotation = false;
                    PlayerController.HandleRotation(0, transform.rotation.eulerAngles.y, 0, true, true, true, false);
                }

                else if (ShieldSliding)
                {
                    AllowShieldSlideRotation = true;
                }

                else
                {
                    PlayerController.HandleRotation(0, transform.rotation.eulerAngles.y, 0, true, true, true, false);
                }
           //     PlayerController.CheckForBadLanding();

            }

            else if (LaunchedBackward)
            {

                if (P_Flip)
                {
                    AllowShieldSlideRotation = false;
                    JustSpun = true;

                    bool DecreasingAngle = (OldRoatation.eulerAngles.x > transform.rotation.eulerAngles.x);

                    M_rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
                    OldRoatation = transform.rotation;

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
                        P_Flip = false;
                    }
                }
                else if (AbouttoLand && !ShieldSliding && JustSpun)
                {
                    AllowShieldSlideRotation = false;
                    PlayerController.HandleRotation(0, transform.rotation.eulerAngles.y, 0, true, true, true, false);
                }

                else if (ShieldSliding)
                {
                    AllowShieldSlideRotation = true;
                }

                else
                {
                    PlayerController.HandleRotation(0, transform.rotation.eulerAngles.y, 0, true, true, true, false);
                }
              //  PlayerController.CheckForBadLanding();

            }



        }


        public void LaunchStatus(int direction)
        {
            if (direction == 0)
            {
                BroadcastMessage("GauntletsIdle");
                LaunchedLeft = true;
                P_Flip = !CheckIfToCloseToSpin(SpinHeightDistance);
                LaunchBegginingRotation = transform.rotation.eulerAngles.y;
            }
            else if (direction == 1)
            {
                BroadcastMessage("GauntletsIdle");
                LaunchedRight = true;
                P_Flip = !CheckIfToCloseToSpin(SpinHeightDistance);
                if (P_Flip)
                    LaunchBegginingRotation = transform.rotation.eulerAngles.y;
            }
            else if (direction == 2)
            {
                BroadcastMessage("GauntletsIdle");
                LaunchedForward = true;
                P_Flip = !CheckIfToCloseToSpin(SpinHeightDistance);
                if (P_Flip)
                    LaunchBegginingRotation = transform.rotation.eulerAngles.y;
            }
            else if (direction == 3)
            {
                BroadcastMessage("GauntletsIdle");
                LaunchedBackward = true;
                P_Flip = !CheckIfToCloseToSpin(SpinHeightDistance);
                LaunchBegginingRotation = transform.rotation.eulerAngles.y;
            }
        }



        private bool LandGuider(float yVelocity, float time)
        {
            if (yVelocity < 0)
            {
                float LandingDistance = -yVelocity * time;
                if (Physics.Raycast(transform.position - (Vector3.up * .5f), Vector3.down, LandingDistance))
                {
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

    }
}