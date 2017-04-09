using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;




    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    public class Enemy : MonoBehaviour
    {
       
        public float m_GroundCheckDistance = 0.1f;

        [SerializeField]
        private EnemyRotation EnemyRotation;


        Rigidbody m_Rigidbody;
        public bool m_IsGrounded;
        float m_OrigGroundCheckDistance;
        float m_TurnAmount;
        float m_ForwardAmount;
        Vector3 m_GroundNormal;

    public float recoverspeed = 4f; 
        public bool hit;

        private Enemy_AI_Control Enemy_AI_Control;




        void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            Enemy_AI_Control = GetComponent<Enemy_AI_Control>();

            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            m_OrigGroundCheckDistance = m_GroundCheckDistance;



        }

        private void Update()
        {
        CheckGroundStatus();
        CheckRecoverStatus();

        if (!m_IsGrounded)
           {
             HandleAirborne();
           }

        }

        public void RecentlyHit()
        {
            hit = true;
        }



        public void Rotate(Transform player)
        {
          //  Debug.Log("Moving");
            transform.rotation = EnemyRotation.Rotate(player, transform);
        }

        
    private void HandleAirborne()
    {
        m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
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
        }
        else
        {
            m_IsGrounded = false;

            m_GroundNormal = Vector3.up;
        }
    }

    private void CheckRecoverStatus()
    {
        if (hit && m_Rigidbody.velocity.magnitude < recoverspeed)
        {
            hit = false;
            Recover();
        }
    }


    public void Recover()
    {
        Enemy_AI_Control.MoveAgain(m_IsGrounded);
    }

    public void DisablePathing()
    {
        Enemy_AI_Control.DisablePath();
    }




}






