using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class EnemySword : MonoBehaviour
    {
         Enemy Enemy; 

        Rigidbody EnemyRigidBody;

        [Header ("Quick Attack Settings")]
        public float QuickAttackForce = 100f;

         [Header("Strong Attack Settings")]
         public float StrongAttackForce = 200f;
    
        [Header("Force Settings")]
        public float InitialLaunchForce = 100;

         private BoxCollider SwordBox;


        // Use this for initialization
        void Start()
        {

        if (GetComponentInParent<Rigidbody>() == null)
        {
            Debug.LogError("EnemySword must be child to object with RigidBody");
        }
        if (GetComponentInParent<Enemy>() == null)
        {
            Debug.LogError("EnemySword must be child to object with Enemy");
        }

            Enemy = GetComponentInParent<Enemy>();
            EnemyRigidBody = GetComponentInParent<Rigidbody>();

            SwordBox = GetComponent<BoxCollider>();
            SwordBox.enabled = false;
        }




        public void QuickAttack()
        {
        //Debug.Log("QuickAttack!!!");
            Enemy.DisablePathing();
            SwordBox.enabled = true;

        }

    // NOT USED AT THE MOMENT //
        public void StrongAttack()
        {

                Enemy.DisablePathing() ;
                Vector3 ForceVector = new Vector3(0, StrongAttackForce * .3f, StrongAttackForce * .8f);
                Enemy.GetComponent<LaunchObject>().NormalForceController(ForceVector);
                SwordBox.enabled = true;

        }

      

        public void StopAttacking()
        {
            Enemy.Recover();
            SwordBox.enabled = false;
        }

        private void OnTriggerEnter(Collider col)
        {

            
            if (col.gameObject.GetComponent<Player1>())
            {
          
                col.gameObject.GetComponent<Player1>().PlayerHit();


                Vector3 objLoc = col.gameObject.transform.position;
                Vector3 LaunchVector = new Vector3(objLoc.x - Enemy.transform.position.x, 1, objLoc.z - Enemy.transform.position.z).normalized;

                float LaunchForce = InitialLaunchForce + EnemyRigidBody.velocity.magnitude;
                col.GetComponent<Player1>().ImpulseForceController(LaunchVector, LaunchForce);

        }


        }

    }


