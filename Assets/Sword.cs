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

        private BoxCollider SwordBox;


        // Use this for initialization
        void Start()
        {
            PlayerController = FindObjectOfType<PlayerController>();
            PlayerRigidBody = PlayerController.GetComponent<Rigidbody>();
            SwordBox = GetComponent<BoxCollider>();
            SwordBox.enabled = false;
        }

        public void QuickAttack()
        {
            //Debug.Log("QuickAttack!!!");
            PlayerRigidBody.AddRelativeForce(0, 0, QuickAttackForce);
            SwordBox.enabled = true;

        }

        public void StrongAttack()
        {
            if (!PlayerController.m_IsGrounded)
            {
                PlayerController.Atacking = true; 
                PlayerRigidBody.AddRelativeForce(0, -StrongAttackForce, 0);
                PlayerRigidBody.AddRelativeTorque(StrongAttackTorque, 0, 0);
            }
            SwordBox.enabled = true;
            //Debug.Log("StrongAttack!!!");
        }

       public void SideStepRight()
        {
            //PlayerRigidBody.AddRelativeTorque(0, SideStepTorque, 0);
            //Possibly change this to a spinning attack but must adjust mouse look snap
            PlayerController.Atacking = true;
            PlayerRigidBody.AddRelativeForce(-10* SideStepForce, 700, 5*SideStepForce);
            PlayerRigidBody.AddRelativeTorque(0, StrongAttackTorque, 0);
            SwordBox.enabled = true;
            //      Debug.Log("SideStepedRight!");
        }

        public void SideStepLeft()
        {
            //Possibly change this to a spinning attack but must adjust mouse look snap
            PlayerController.Atacking = true;
            PlayerRigidBody.AddRelativeForce(10 * SideStepForce, 700, 5 * SideStepForce);
            PlayerRigidBody.AddRelativeTorque(0, -StrongAttackTorque, 0);
            SwordBox.enabled = true;
        }

        public void StopAttacking()
        {
            PlayerController.Atacking = false;
            SwordBox.enabled = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            //Debug.Log("Entered Collision");
            if (collision.gameObject.GetComponent<LaunchObject>())
            {
                //Debug.Log("Launch Object Found");
                Vector3 objLoc = collision.gameObject.transform.position;
                Vector3 LaunchVector = new Vector3(objLoc.x - PlayerController.transform.position.x, 0, objLoc.z - PlayerController.transform.position.z).normalized;
                float InitialLaunchForce = 100;
                Debug.Log(PlayerRigidBody.velocity);
                //Debug.Log("LaunchVector: " + LaunchVector);
                collision.gameObject.GetComponent<Rigidbody>().AddForce(LaunchVector * (InitialLaunchForce + PlayerRigidBody.velocity.magnitude));
            }
        }
    }
}

