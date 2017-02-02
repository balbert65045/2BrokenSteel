using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class Sword : MonoBehaviour
    {

        PlayerController PlayerController;
        Rigidbody PlayerRigidBody;
        public float QuickAttackForce = 100f;
        public float QuickAttackTourque = 100f; 
        public float StrongAttackForce = 200f;
        public float StrongAttackTorque = 100f;
        public float SideStepForce = 100f;
        public float SideStepTorque = 100f; 


        // Use this for initialization
        void Start()
        {
            PlayerController = FindObjectOfType<PlayerController>();
            PlayerRigidBody = PlayerController.GetComponent<Rigidbody>();
        }

        public void QuickAttack()
        {
            //Debug.Log("QuickAttack!!!");
            PlayerRigidBody.AddRelativeForce(0, 0, QuickAttackForce);

        }

        public void StrongAttack()
        {
            if (!PlayerController.m_IsGrounded)
            {
                PlayerRigidBody.AddRelativeForce(0, -StrongAttackForce, 0);
               // PlayerRigidBody.AddRelativeTorque(StrongAttackTorque, 0, 0);
            }
            //Debug.Log("StrongAttack!!!");
        }

     //   public void SideStepRight()
     //   {
            //PlayerRigidBody.AddRelativeTorque(0, SideStepTorque, 0);
            //Possibly change this to a spinning attack but must adjust mouse look snap
      //      PlayerRigidBody.AddRelativeForce(-10* SideStepForce, 0, SideStepForce);
      //      Debug.Log("SideStepedRight!");
      //  }

     //   public void SideStepLeft()
     //   {
            //Possibly change this to a spinning attack but must adjust mouse look snap
      //      PlayerRigidBody.AddRelativeForce(10*SideStepForce, 0, SideStepForce);
      //      Debug.Log("SideStepedLeft!");
      //  }
    }
}

