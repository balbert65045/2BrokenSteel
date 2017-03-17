using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    public class Enemy : MonoBehaviour
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
        float m_AnimSpeedMultiplier = 1f;
        [SerializeField]
        float m_GroundCheckDistance = 0.1f;

        Rigidbody m_Rigidbody;
        bool m_IsGrounded;
        float m_OrigGroundCheckDistance;
        float m_TurnAmount;
        float m_ForwardAmount;
        Vector3 m_GroundNormal;

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
        Debug.Log(m_Rigidbody.velocity.magnitude);
        if (hit && m_Rigidbody.velocity.magnitude < 4f)
        {
            hit = false;
            Enemy_AI_Control.MoveAgain();
        }
    }

    public void Readjust()
    {
        hit = true; 
    }

    }


