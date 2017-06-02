using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class PC : MonoBehaviour
    {
		
		//PUBLIC EXPOSED
        [SerializeField]
        public float m_JumpPower = 12f;
        [Range(1f, 4f)]
        [SerializeField]
        float m_GravityMultiplier = 2f;
        [SerializeField]
        public float m_MoveSpeedMultiplier = 1f;
        [SerializeField]
        float m_MaxSpeed = 20f; 

        public float m_GroundCheckDistance = 0.1f;
        public bool m_IsGrounded;
        public float FrictionForce = .2f;

        public bool Sliding = false;
		public float SlideSpeed = 13.34f;
        public bool Atacking = false; 

		//PRIVATE EXPOSED
        private Rigidbody m_Rigidbody;
		private Transform m_Cam;
		private CapsuleCollider m_Collider;

		private bool MovementOverride;
		public bool RotationOverride;
        private bool RotationOverride1;
        private bool RotationOverride2;

        private float m_OrigGroundCheckDistance;
        private Vector3 m_GroundNormal;
        

        private bool LandingHard = false;
        private Vector3 AirSpeed;

        private bool DisableFritction = false;

        // Use this for initialization
        void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        
            m_Collider = GetComponent<CapsuleCollider>();

            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            m_OrigGroundCheckDistance = m_GroundCheckDistance;
            //StartXMin = m_MouseLook.MinimumX; 

            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.Log("Camera is not found");
            }

        }

            public void Move(Vector3 move, bool jump, bool Locked)
        {

            // convert the world relative moveInput vector into a local-relative
            // turn amount and forward amount required to head in the desired
            // direction.


          //  Debug.Log("Movement Overrride " + RotationOverride);
			
            if (move.magnitude > 1f) move.Normalize();
            CheckGroundStatus();


            if (m_IsGrounded && !MovementOverride)
            {
                SendMessage("UpdateXMin", -10);
                HandleGroundedMovement(jump, move);
            }
            else if (!MovementOverride)
            {
                HandleAirborneMovement(move);
               
            }


            RotationOverride = RotationOverride1 || RotationOverride2;

            if (!RotationOverride){
				if (Locked){
                    HandleRotation(m_Cam.transform.rotation.eulerAngles.x, m_Cam.transform.rotation.eulerAngles.y, 0, true, false, true, false);
				}
                else if (Sliding)
                {
                    HandleRotation(0, transform.rotation.eulerAngles.y, 0, true, true, true, false);
                }
				else{
                    HandleRotation(0, m_Cam.transform.rotation.eulerAngles.y, 0, true, false, true, false);
				}
				
			}
			

        }
		
		
		
		public void HandleRotation(float Xrot, float Yrot, float Zrot, bool XFreeze, bool YFreeze, bool ZFreeze, bool SLERP){



			if (XFreeze && YFreeze && ZFreeze){
				m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
			}
			else if (XFreeze && YFreeze && !ZFreeze){
				m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
			}
			else if (XFreeze && !YFreeze && ZFreeze){
				m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
			}
			else if (!XFreeze && YFreeze && ZFreeze){
				m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			}
			else if (XFreeze && !YFreeze && !ZFreeze){
				m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX;
			}
			else if (!XFreeze && YFreeze && !ZFreeze){
				m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationY;
			}
			else if (!XFreeze && !YFreeze && ZFreeze){
				m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
			}
			else{
				m_Rigidbody.constraints = RigidbodyConstraints.None;
			}
			
			if (SLERP){
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Xrot, Yrot, Zrot), Time.deltaTime * 40f); 
			}
			else{
                transform.rotation = Quaternion.Euler(Xrot, Yrot, Zrot);
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

            if (jump && !Sliding)
            {
             //   Debug.Log("Jumped");
                m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
                m_IsGrounded = false;
                m_GroundCheckDistance = 0.1f;
            }

            
            if (m_Rigidbody.velocity.magnitude > SlideSpeed)
            {
                SendMessage("ChargePotential", m_Rigidbody.velocity.magnitude);
                 Sliding = true;
				 m_Rigidbody.AddForce(-FrictionForce * m_Rigidbody.velocity, ForceMode.Force);
            }
            else
            {
                Sliding = false;
                m_Rigidbody.velocity = (move * m_MoveSpeedMultiplier);
                m_Rigidbody.velocity = new Vector3(Mathf.Clamp(m_Rigidbody.velocity.x, -m_MaxSpeed, m_MaxSpeed), 0, Mathf.Clamp(m_Rigidbody.velocity.z, -m_MaxSpeed, m_MaxSpeed));
            }
  
        }
            

        /// <summary>
        /// Airoborne Movement and Rotation
        /// </summary>
        /// <param name="move"></param>
        void HandleAirborneMovement(Vector3 move)
        {

            Sliding = false;
            // readjust ground check distance
            m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
			
			 SendMessage("UpdateXMin", -90);
			
            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
            m_Rigidbody.AddForce(extraGravityForce);
            m_Rigidbody.velocity = new Vector3(Mathf.Clamp(m_Rigidbody.velocity.x, -m_MaxSpeed, m_MaxSpeed), m_Rigidbody.velocity.y, Mathf.Clamp(m_Rigidbody.velocity.z, -m_MaxSpeed, m_MaxSpeed));

          
        }


       private void CheckGroundStatus()
        {

                bool WasGrounded = m_IsGrounded;

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

                if (m_IsGrounded == true && WasGrounded == false)
            {
                //Debug.Log("Landed");
                BroadcastMessage("Landed");
            }
        }

        public void CheckForBadLanding()
        {
        //    if (Mathf.Abs(m_Rigidbody.velocity.y) > (Mathf.Abs(m_Rigidbody.velocity.x) + Mathf.Abs(m_Rigidbody.velocity.z)))
            {
               // Debug.Log("Landed Hard");
                LandingHard = true;
                AirSpeed = new Vector3(m_Rigidbody.velocity.x, 0, m_Rigidbody.velocity.z);
            }
        }

        


        public void Hit()
        {
            m_IsGrounded = false;
            m_GroundCheckDistance = 0.1f;
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
		
		
		
		
		
		public void  AdjustGroundDistance(float distance){
			 m_OrigGroundCheckDistance += distance;
		}
		
		public void ShieldGlideMovement(Vector3 GlideVector, float TurnForce, bool jump, bool Enabled){
			if (Enabled){
					MovementOverride = true;
					m_Rigidbody.AddRelativeForce(TurnForce, 0, 0);
                if (m_IsGrounded)
                {
                    m_Rigidbody.velocity = GlideVector;
                }
                    //HandleGroundedMovement(false, GlideVector);
			}
			else{
			
				MovementOverride = false;
			}
		}
		
		public void OutsideRotation1(bool Enabled)
		{
			RotationOverride1 = Enabled;				
		}
		
        public void OutsideRotation2(bool Enabled)
        {
            RotationOverride2 = Enabled;
        }

    }
}