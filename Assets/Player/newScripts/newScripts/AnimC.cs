using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{


    public class AnimC : MonoBehaviour
    {
        private PC PlayerController;
        private ShieldSlidingController ShieldControllerSlider;
        private FlipC FlipC;
        private Animator m_Animator;
        private Rigidbody m_Rigidbody;

        private bool ShieldSliding = false;
        public bool InvertSlide = false;
        private bool RecentlyLaunched = false;
        private bool initialLaunch = false;
        private bool M_Blocked = false;




        void Start()
        {

            m_Rigidbody = GetComponent<Rigidbody>();
            PlayerController = GetComponent<PC>();
            m_Animator = GetComponent<Animator>();
            FlipC = GetComponent<FlipC>();
            ShieldControllerSlider = GetComponent<ShieldSlidingController>();

        }


        public void UpdateAnimator(Vector3 move, bool QuickMove, bool LeftMove, bool RightMove, bool SpecialMove, bool ActiveGauntlets, bool ActiveSword, bool WeaponSwitch, bool m_BackwardsMove,
                   bool m_ForwardMovement, bool m_SpecialMoveCharge, bool m_DodgeLeft, bool m_DodgeRight, bool TogglePotential)
        {
            // Debug.Log(move);

            // animation bools!
            //

            ShieldSliding = ShieldControllerSlider.ShieldSliding;

            if (PlayerController.m_IsGrounded)
            {
                RecentlyLaunched = false;
            }


            //Moving toggle//
            if (move != Vector3.zero && PlayerController.m_IsGrounded)
            {
                m_Animator.SetBool("Moving", true);
            }

            else
            {
                m_Animator.SetBool("Moving", false);
            }

            if (PlayerController.Sliding)
            {
                m_Animator.SetBool("Sliding", true);
            }
            else
            {
                m_Animator.SetBool("Sliding", false);
            }

            //
            // animation Triggers!
            //

            if (ActiveGauntlets)
            {

                if (WeaponSwitch)
                {
                    m_Animator.SetTrigger("GauntletsOutTrig");
                    BroadcastMessage("GauntletsIdle");
                }

                if (!ShieldSliding && !PlayerController.Sliding)
                {
                    if (m_SpecialMoveCharge)
                    {
                        BroadcastMessage("ChargeGauntlets");
                    }

                    if (SpecialMove && LeftMove && !RecentlyLaunched)
                    {
                        if (PlayerController.m_IsGrounded)
                        {
                            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, .8f * PlayerController.m_JumpPower, m_Rigidbody.velocity.z);
                        }
                        m_Animator.SetTrigger("LeftLaunch");
                        RecentlyLaunched = true;

                    }
                    else if (SpecialMove && RightMove && !RecentlyLaunched)
                    {
                        if (PlayerController.m_IsGrounded)
                        {
                            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, .8f * PlayerController.m_JumpPower, m_Rigidbody.velocity.z);
                        }
                        m_Animator.SetTrigger("RightLaunch");
                        RecentlyLaunched = true;

                    }
                    else if (SpecialMove && m_BackwardsMove && !RecentlyLaunched)
                    {
                        if (PlayerController.m_IsGrounded)
                        {
                            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, .8f * PlayerController.m_JumpPower, m_Rigidbody.velocity.z);
                        }

                        m_Animator.SetTrigger("BackwardLaunch");
                        RecentlyLaunched = true;

                    }
                    else if (SpecialMove && m_ForwardMovement && !RecentlyLaunched)
                    {
                        if (PlayerController.m_IsGrounded)
                        {
                            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, .8f * PlayerController.m_JumpPower, m_Rigidbody.velocity.z);
                        }
                        m_Animator.SetTrigger("LaunchForwards");
                        RecentlyLaunched = true;

                    }

                    else if (SpecialMove && PlayerController.m_IsGrounded && !initialLaunch)
                    {
                        m_Animator.SetTrigger("UpLaunch");
                        initialLaunch = true;
                    }


                }

                else if (ShieldSliding)
                {
                    if (m_SpecialMoveCharge)
                    {
                        BroadcastMessage("ChargeGauntlets");
                    }

                    if (SpecialMove && m_ForwardMovement)
                    {
                        if (InvertSlide)
                        {
                            m_Animator.SetTrigger("ShieldSlideBoostInv");
                            RecentlyLaunched = true;
                            BroadcastMessage("GauntletsIdle");
                        }
                        else
                        {
                            m_Animator.SetTrigger("ShieldSlideBoost");
                            RecentlyLaunched = true;
                            BroadcastMessage("GauntletsIdle");
                        }
                    }

                }
                else
                {

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
                else if(TogglePotential)
                {
                    m_Animator.SetTrigger("PotentialToggle");
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
                    m_Animator.SetBool("SuperCharge", true);
                   // m_Animator.SetTrigger("StrongAttack");
                }
                else if (m_DodgeLeft && PlayerController.m_IsGrounded)
                {
                    m_Animator.SetTrigger("SideStepLeft");
                }
                else if (m_DodgeRight && PlayerController.m_IsGrounded)
                {
                    m_Animator.SetTrigger("SideStepRight");
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

        public void ShieldSlideEnable(bool Enabled)
        {
            if (Enabled)
            {
                m_Animator.SetBool("ShieldSliding", true);
                if (FlipC.LaunchedRight || (FlipC.LaunchedForward) || (FlipC.LaunchedBackward))
                {
                    Debug.Log("Inverted");
                    BroadcastMessage("ShieldSlidingRight");
                    InvertSlide = true;
                }
                //    else if (LaunchedLeft || (LaunchedForward && JustSpun) || (LaunchedBackward && !JustSpun))
                else if (FlipC.LaunchedLeft)
                {
                    InvertSlide = false;
                    BroadcastMessage("ShieldSlidingLeft");
                }
                else
                {
                    Debug.Log("Else Statement");
                    InvertSlide = false;
                    BroadcastMessage("ShieldSlidingLeft");
                }
            }
            else
            {
                BroadcastMessage("NotSliding");
                m_Animator.SetBool("ShieldSliding", false);
            }

        }

        public void StopPotential()
        {
            m_Animator.SetBool("SuperCharge", false);
        }


        public void Launched()
        {
            BroadcastMessage("GauntletsIdle");
            initialLaunch = false;
        }


        public void Blocked()
        {
            M_Blocked = true;
        }

    }
}

