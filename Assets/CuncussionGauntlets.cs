using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{


    public class CuncussionGauntlets : MonoBehaviour
    {

        PlayerController PlayerController;
        Rigidbody PlayerRigidBody;

        public float m_RelativeLaunchPower = 1000;
        // Use this for initialization
        void Start()
        {
            PlayerController = FindObjectOfType<PlayerController>();
            PlayerRigidBody = PlayerController.GetComponent<Rigidbody>();
        }

        // Update is called once per frame

        public void LaunchUp()
        {
            PlayerRigidBody.AddRelativeForce(0, m_RelativeLaunchPower, 0);
            PlayerController.m_IsGrounded = false;
            PlayerController.m_GroundCheckDistance = 0.1f;
            BroadcastMessage("BlastLaunchUp");
        }

        public void LaunchLeft()
        {
            PlayerRigidBody.AddRelativeForce(-m_RelativeLaunchPower * .75f, m_RelativeLaunchPower * .75f, 0);
            PlayerController.m_IsGrounded = false;
            PlayerController.m_GroundCheckDistance = 0.1f;
            BroadcastMessage("BlastLaunchLeft");
        }

        public void LaunchRight()
        {
            PlayerRigidBody.AddRelativeForce(m_RelativeLaunchPower * .75f, m_RelativeLaunchPower * .75f, 0);
            PlayerController.m_IsGrounded = false;
            PlayerController.m_GroundCheckDistance = 0.1f;
            BroadcastMessage("BlastLaunchRight");
        }

    }
}
