using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{

    [RequireComponent(typeof(Rigidbody))]
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

        Rigidbody m_Rigidbody;
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
            //        m_boxCollide = GetComponent<BoxCollider>();

            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            m_OrigGroundCheckDistance = m_GroundCheckDistance;
        }

        public void Move(Vector3 move, bool jump)
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

        void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
            transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
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