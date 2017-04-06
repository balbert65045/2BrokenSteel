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
            SwordBox.enabled = true;

        }

    // NOT USED AT THE MOMENT //
        public void StrongAttack()
        {

                Enemy.DisablePath() ;
                EnemyRigidBody.AddRelativeForce(0, StrongAttackForce * .3f, StrongAttackForce * .8f);
                SwordBox.enabled = true;

        }

      

        public void StopAttacking()
        {
            Enemy.MoveAgain();
            SwordBox.enabled = false;
        }

        private void OnTriggerEnter(Collider col)
        {

            
            if (col.gameObject.GetComponent<Player1>())
            {
          
                col.gameObject.GetComponent<Player1>().PlayerHit();
                Vector3 objLoc = col.gameObject.transform.position;
                Vector3 LaunchVector = new Vector3(objLoc.x - Enemy.transform.position.x, 1, objLoc.z - Enemy.transform.position.z).normalized;
                
                float InitialLaunchForce = 100;

                col.gameObject.GetComponent<Rigidbody>().AddForce(LaunchVector * (InitialLaunchForce + EnemyRigidBody.velocity.magnitude), ForceMode.Impulse);
            }


        }

    }


