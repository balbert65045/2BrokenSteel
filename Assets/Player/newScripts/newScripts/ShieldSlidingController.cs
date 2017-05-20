using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{


    public class ShieldSlidingController : MonoBehaviour
    {


        private FlipC FlipC;
        private PC PlayerController;
        private AnimC AnimC;
        private Rigidbody m_Rigidbody;

        public bool ShieldSliding = false;
        public float ShieldTurnSpeed = .2f;

        private bool ShieldStatusPrevious = false;
        public float ShieldTransitionTime = .3f;
        private float ShieldBegginingRotation = 0;

        private bool LaunchedRight = false;
        private bool LaunchedLeft = false;
        private bool LaunchedBackward = false;

        private Vector3 MoveShieldVeclocity;
        private bool InitialSpeedTrigger;

        public float AdjustedGroundCheckDistance = .7f;


        void Start() {

            FlipC = GetComponent<FlipC>();
            PlayerController = GetComponent<PC>();
            m_Rigidbody = GetComponent<Rigidbody>();
            AnimC = GetComponent<AnimC>();

        }



        public void CheckSlide(bool Active, float h) {
            if (Active && !PlayerController.m_IsGrounded && !FlipC.AllowShieldSlideRotation)
            {
                if (ShieldSliding)
                {
                    InitialSpeedTrigger = true;
                    MoveShieldVeclocity = new Vector3(m_Rigidbody.velocity.x, 0, m_Rigidbody.velocity.z);
                    if (AnimC.InvertSlide) {

                        if (!FlipC.LaunchedRight && !FlipC.LaunchedBackward)
                        {
                          
                            LaunchedRight = false;
                            LaunchedBackward = false;
                            PlayerController.HandleRotation(0, ShieldBegginingRotation - 90, 0, true, false, true, false);
                        }
                        else if (FlipC.LaunchedBackward)
                        {
                            LaunchedRight = false;
                            LaunchedBackward = true;
                            PlayerController.HandleRotation(0, ShieldBegginingRotation + 90, 0, true, false, true, false);
                        }
                        else
                        {
                            PlayerController.HandleRotation(0, ShieldBegginingRotation, 0, true, false, true, false);
                            LaunchedBackward = false;
                            LaunchedRight = true;
                        }
                    }
                    else
                    {
                        if (!FlipC.LaunchedLeft)
                        {
                            PlayerController.HandleRotation(0, ShieldBegginingRotation + 90, 0, true, false, true, false);
                            LaunchedLeft = false;
                       
                        }
                        else
                        {
                            PlayerController.HandleRotation(0, ShieldBegginingRotation, 0, true, false, true, false);
                            LaunchedLeft = true;
                        }
                       
                    }
                }

                else
                {
                    bool  ShieldSlidingTrigger = CheckForShieldSlide(m_Rigidbody.velocity.y, ShieldTransitionTime);
                    if (ShieldSlidingTrigger)
                    {
                        ShieldBegginingRotation = transform.rotation.eulerAngles.y;
                        //  Debug.Log(ShieldBegginingRotation);
                        AnimC.ShieldSlideEnable(true);
                        PlayerController.OutsideRotation2(true);
                        PlayerController.AdjustGroundDistance(AdjustedGroundCheckDistance);
                        ShieldSliding = true;

                    }
                }

            }
            else if (Active && PlayerController.m_IsGrounded) {
                if (ShieldSliding)
                {
                    if (AnimC.InvertSlide) {
                        h = -h;
                    }
                    else {
                        h = +h;
                    }
                    float TurnForce = h * ShieldTurnSpeed * m_Rigidbody.velocity.magnitude * .1f;
                    
                    if (InitialSpeedTrigger)
                    {
                        PlayerController.ShieldGlideMovement(MoveShieldVeclocity, TurnForce, false, true);
                        InitialSpeedTrigger = false;
                    }
                    else
                    {
                        PlayerController.ShieldGlideMovement(m_Rigidbody.velocity, TurnForce, false, true);
                    }
                   
                  //  PlayerController.OutsideRotation(true);
                    if (AnimC.InvertSlide)
                    {
                        if (!LaunchedRight && !LaunchedBackward)
                        {

                            PlayerController.HandleRotation(0, ShieldBegginingRotation - 90, 0, true, false, true, false);
                            
                        }
                        else if (LaunchedBackward)
                        {
                          //  PlayerController.HandleRotation(0, transform.rotation.eulerAngles.y, 0, true, false, true, false);
                            PlayerController.HandleRotation(0, ShieldBegginingRotation + 90, 0, true, false, true, false);
                        }

                        else
                        {
                            PlayerController.HandleRotation(0, ShieldBegginingRotation, 0, true, false, true, false);
                        }


      
                    }
                    else
                    {

                        if (ShieldBegginingRotation + 90 > 360)
                        {
                            ShieldBegginingRotation -= 360;
                        }

                        if (!LaunchedLeft)
                        {
                            PlayerController.HandleRotation(0, ShieldBegginingRotation + 90, 0, true, false, true, false);
                        }
                        else
                        {
                            PlayerController.HandleRotation(0, ShieldBegginingRotation, 0, true, false, true, false);
                        }
                    }
                }
            }
            else if (!Active && !PlayerController.m_IsGrounded) {

                if (ShieldSliding)
                {
                
                    PlayerController.AdjustGroundDistance(-AdjustedGroundCheckDistance);
                    PlayerController.OutsideRotation2(false);
                    ShieldSliding = false;
                    PlayerController.ShieldGlideMovement(m_Rigidbody.velocity, 0, false, false);
                    AnimC.ShieldSlideEnable(false);
                }

            }

            else if (!Active && PlayerController.m_IsGrounded) {
                if (ShieldSliding)
                {
                    PlayerController.AdjustGroundDistance(-AdjustedGroundCheckDistance);
                    PlayerController.OutsideRotation2(false);
                    ShieldSliding = false;
                    PlayerController.ShieldGlideMovement(m_Rigidbody.velocity, 0, true,
                    false);
                    AnimC.ShieldSlideEnable(false);


                }
            }


        }


        private bool CheckForShieldSlide(float yVelocity, float time)
        {
            float LandingDistance = -yVelocity * time;
            if (yVelocity < -2)
            {
                Debug.DrawLine(transform.position - (Vector3.up * .5f), transform.position - (Vector3.up * .5f) - (Vector3.up * LandingDistance), Color.red);
                if (Physics.Raycast(transform.position - (Vector3.up * .5f), Vector3.down, LandingDistance))
                {
                    // Debug.Log("About to hit ground");
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}