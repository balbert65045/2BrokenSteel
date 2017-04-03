using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class EnemySword : MonoBehaviour
    {
         Enemy_AI_Control Enemy; 

        Rigidbody EnemyRigidBody;
        public float QuickAttackForce = 100f;
       
        public float StrongAttackForce = 200f;
    
        

        private BoxCollider SwordBox;


        // Use this for initialization
        void Start()
        {
            Enemy = FindObjectOfType<Enemy_AI_Control>();
            EnemyRigidBody = Enemy.gameObject.GetComponent<Rigidbody>();
            SwordBox = GetComponent<BoxCollider>();
            SwordBox.enabled = false;
        }

        public void QuickAttack()
        {
        //Debug.Log("QuickAttack!!!");
            Enemy.DisablePath();
            EnemyRigidBody.AddRelativeForce(new Vector3(0, 0, QuickAttackForce), ForceMode.Impulse); 
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

        //  Debug.Log("Attacked on ground");
                Enemy.DisablePath() ;
                EnemyRigidBody.AddRelativeForce(0, StrongAttackForce * .3f, StrongAttackForce * .8f);
                SwordBox.enabled = true;
            //Debug.Log("StrongAttack!!!");
        }

      

        public void StopAttacking()
        {
            Enemy.MoveAgain();
            SwordBox.enabled = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
        //Debug.Log("Entered Collision");
        Debug.Log("Collision with " + collision.gameObject.name);
            
            if (collision.gameObject.GetComponent<Player1>())
            {
                //Debug.Log("Launch Object Found");
                Vector3 objLoc = collision.gameObject.transform.position;
                Vector3 LaunchVector = new Vector3(objLoc.x - Enemy.transform.position.x, 1, objLoc.z - Enemy.transform.position.z).normalized;

                float InitialLaunchForce = 100;
                // Debug.Log(LaunchVector);
                //Debug.Log("LaunchVector: " + LaunchVector);
                collision.gameObject.GetComponent<Rigidbody>().AddForce(LaunchVector * (InitialLaunchForce + EnemyRigidBody.velocity.magnitude), ForceMode.Impulse);
            }


        }

    }


