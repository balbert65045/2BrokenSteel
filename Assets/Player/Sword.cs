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
        public float SideStepForceX = 100f;
        public float SideStepForceZ = 100f;
        public float SideStepForceY = 100f;
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
            //   if (!PlayerController.m_IsGrounded)
            //    {
            //       PlayerController.Atacking = true; 
            //         PlayerRigidBody.AddRelativeForce(0, -StrongAttackForce, 0);
            //      PlayerRigidBody.AddRelativeTorque(StrongAttackTorque, 0, 0);
            //    }
            //   else
            {
                //  Debug.Log("Attacked on ground");
                PlayerController.Atacking = true;
                PlayerRigidBody.AddRelativeForce(0, StrongAttackForce * .3f, StrongAttackForce * .8f);
            }
            SwordBox.enabled = true;
            //Debug.Log("StrongAttack!!!");
        }

        public void SideStepRight()
        {
            //PlayerRigidBody.AddRelativeTorque(0, SideStepTorque, 0);
            //Possibly change this to a spinning attack but must adjust mouse look snap
            PlayerController.Atacking = true;
            PlayerRigidBody.constraints = RigidbodyConstraints.None;
            PlayerRigidBody.AddRelativeForce(-SideStepForceX, SideStepForceY, SideStepForceZ);
            PlayerRigidBody.AddRelativeTorque(0, 0, SideStepTorque);
            //SwordBox.enabled = true;
            //      Debug.Log("SideStepedRight!");
        }

        public void SideStepLeft()
        {
            //Possibly change this to a spinning attack but must adjust mouse look snap
            PlayerController.Atacking = true;
            PlayerRigidBody.constraints = RigidbodyConstraints.None;
            PlayerRigidBody.AddRelativeForce(SideStepForceX, SideStepForceY, SideStepForceZ);
            PlayerRigidBody.AddRelativeTorque(0, 0, -SideStepTorque);
            // SwordBox.enabled = true;
        }

        public void StopAttacking()
        {
            PlayerController.Atacking = false;
            SwordBox.enabled = false;
        }

        private void OnTriggerEnter(Collider col)
        {
            //Debug.Log("Entered Collision");
            if (col.gameObject.GetComponent<Enemy_AI_Control>())
            {
                col.gameObject.GetComponent<Enemy_AI_Control>().DisablePath();
            }

            if (col.gameObject.GetComponent<LaunchObject>())
            {
                //Debug.Log("Launch Object Found");
                Vector3 objLoc = col.gameObject.transform.position;
                Vector3 LaunchVector = new Vector3(objLoc.x - PlayerController.transform.position.x, 1, objLoc.z - PlayerController.transform.position.z).normalized;

                float InitialLaunchForce = 100;
                // Debug.Log(LaunchVector);
                //Debug.Log("LaunchVector: " + LaunchVector);
                col.gameObject.GetComponent<Rigidbody>().AddForce(LaunchVector * (InitialLaunchForce + PlayerRigidBody.velocity.magnitude), ForceMode.Impulse);
            }


        }

        private void OnTriggerExit(Collider col)
        {
          //  Debug.Log("TriggerExited");
            if (col.gameObject.GetComponent<Enemy>())
            {
                col.gameObject.GetComponent<Enemy>().Readjust();
            }
        }
    }
}

