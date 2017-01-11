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
        float m_GroundCheckDistance = 0.1f;
        [SerializeField]
        float m_RelativeLaunchPower = 240f;

        Rigidbody m_Rigidbody;
        Animator m_Animator;
        bool m_IsGrounded;
        float m_OrigGroundCheckDistance;
        float m_TurnAmount;
        float m_ForwardAmount;
        Vector3 m_GroundNormal;
        //CapsuleCollider m_Capsule;
        // Use this for initialization
        void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Animator = GetComponent<Animator>();
            //        m_boxCollide = GetComponent<BoxCollider>();

            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            m_OrigGroundCheckDistance = m_GroundCheckDistance;
        }

        public void Move(Vector3 move, bool jump, bool UpLaunch, bool LeftLaunch, bool RightLaunch)
        {

            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.
            if (move.magnitude > 1f) move.Normalize();
           // move = transform.InverseTransformDirection(move);
            CheckGroundStatus();
          //  move = Vector3.ProjectOnPlane(move, m_GroundNormal);
            m_TurnAmount = Mathf.Atan2(move.x, move.z);
            m_ForwardAmount = move.z;

            // ApplyExtraTurnRotation();

            // control and velocity handling is different when grounded and airborne:
            if (m_IsGrounded)
            {
                HandleGroundedMovement(jump, move);
            }
            else
            {
                HandleAirborneMovement();
            }

            //control Launch direction

            //HandleLaunch(move, UpLaunch);

            //Update animator
            UpdateAnimator(move, UpLaunch, LeftLaunch, RightLaunch);

        }

        void HandleAirborneMovement()
        {
            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
            m_Rigidbody.AddForce(extraGravityForce);

            m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
        }

        void HandleGroundedMovement(bool jump, Vector3 move)
        {
            if (jump)
            {
                m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
                m_IsGrounded = false;
                m_GroundCheckDistance = 0.1f;
            }
            else
            {
         //       Debug.Log("On Ground");
                m_Rigidbody.velocity = move * m_MoveSpeedMultiplier;
            }
        }

        public void LaunchUp()
        {
               m_Rigidbody.AddRelativeForce(0, m_RelativeLaunchPower, 0);
                m_IsGrounded = false;
                m_GroundCheckDistance = 0.1f;
                BroadcastMessage("BlastLaunchUp");
        }

        public void LaunchLeft()
        {
            m_Rigidbody.AddRelativeForce(-m_RelativeLaunchPower*.75f, m_RelativeLaunchPower*.75f, 0);
            m_IsGrounded = false;
            m_GroundCheckDistance = 0.1f;
            BroadcastMessage("BlastLaunchLeft");
        }

        public void LaunchRight()
        {
            m_Rigidbody.AddRelativeForce(m_RelativeLaunchPower * .75f, m_RelativeLaunchPower * .75f, 0);
            m_IsGrounded = false;
            m_GroundCheckDistance = 0.1f;
            BroadcastMessage("BlastLaunchRight");
        }

        void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
            transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
        }

        void UpdateAnimator(Vector3 move, bool UpLaunch, bool LeftLaunch, bool RightLaunch)
        {
            if (move == Vector3.zero)
            {
                m_Animator.SetBool("Moving", false);
            }
            else
            {
                m_Animator.SetBool("Moving", true);
                float RunSpeed = 1.35f;
                float SlowDownSpeed = 0.8f;
                if (move.magnitude == 1)
                {
                    m_Animator.speed = RunSpeed;
                } else
                {
                    m_Animator.speed = SlowDownSpeed;
                }
            }
            if (UpLaunch)
            {
                m_Animator.SetTrigger("UpLaunch");
            } else if (LeftLaunch)
            {
                m_Animator.SetTrigger("LeftLaunch");
            } else if (RightLaunch)
            {
                m_Animator.SetTrigger("RightLaunch");
            }
        }

        void CheckGroundStatus()
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
            {
                m_GroundNormal = hitInfo.normal;
                m_IsGrounded = true;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundNormal = Vector3.up;
            }

        }
    }
}