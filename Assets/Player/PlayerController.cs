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

        private Transform m_Cam;

        // Use this for initialization
        void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Animator = GetComponent<Animator>();

            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            m_OrigGroundCheckDistance = m_GroundCheckDistance;

            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.Log("Camera is not found");
            }

        }

        public void Move(Vector3 move, bool jump, bool UpLaunch, bool LeftLaunch, bool RightLaunch)
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
                HandleGroundedMovement(jump, move);
            }
            else
            {
                HandleAirborneMovement();
            }
            //Update animator
            UpdateAnimator(move, UpLaunch, LeftLaunch, RightLaunch);

        }

        void HandleAirborneMovement()
        {
            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
            m_Rigidbody.AddForce(extraGravityForce);

            m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;

           // Debug.Log(m_Rigidbody.velocity);
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

                //Rigidbody input movement
                m_Rigidbody.AddForce((move * m_MoveSpeedMultiplier * 3) , ForceMode.Force);
                //Land Friction
                m_Rigidbody.AddForce(new Vector3(-m_Rigidbody.velocity.x * FrictionGround, 0, -m_Rigidbody.velocity.z * FrictionGround));
                //m_Rigidbody.AddForce(-move * FrictionGround * 3);
                m_Rigidbody.velocity = new Vector3 (Mathf.Clamp(m_Rigidbody.velocity.x, -20, 20), 0, Mathf.Clamp(m_Rigidbody.velocity.z, -20, 20));
                

            }
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
                AlignWithCamera();
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

        void AlignWithCamera()
        {
         //   transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, m_Cam.transform.rotation.eulerAngles.y, 0), Quaternion.Euler(0, transform.rotation.y, 0), TurnTime * Time.deltaTime);
            transform.rotation = (Quaternion.Euler(0, m_Cam.transform.rotation.eulerAngles.y, 0));
            Debug.Log(m_Cam.transform.rotation.y);
          //  Debug.Log("Cube" + transform.rotation.y);
          //  Debug.Log("Camera" + m_Cam.transform.rotation.y);
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
                //Debug.Log(m_Rigidbody.velocity);
                if (-m_Rigidbody.velocity.y >= 20f)
                {
                    BroadcastMessage("SchockwaveLanding");
                    Debug.Log("SendtSchockwave");
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