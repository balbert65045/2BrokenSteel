using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class Sword : MonoBehaviour
    {

        PC PlayerController;
        ShieldSlidingController ShieldSlidingController;
        Rigidbody PlayerRigidbody; 
        Player1 Player1;

        public Material ChargeMaterial;
        private Color NormalMaterialColor;
        private bool ChargeStatus = false;

        public GameObject DeathSword;

        [Header("Quick Attack Settings")]
        public float QuickAttackForce = 100f;
   //     public float QuickAttackTourque = 100f;

        [Header("Strong Attack Settings")]
        public float StrongAttackForceY = 200f;
        public float StrongAttackForceZ = 200f;
        //   public float StrongAttackTorque = 100f;

        [Header("Side Move Settings")]
        public float SideStepForceX = 100f;
        public float SideStepForceZ = 100f;
        public float SideStepForceY = 100f;
        public float SideStepTorque = 100f;

        [Header("Force Settings")]
        public float InitialLaunchForce = 100f;
        public float BlockedForce = 100f;

        [Header("SuperCharge Settings")]
        public float ImpulseForwardForce = 200f;


        private BoxCollider SwordBox;
        private bool BlockedRecently = false;
        private bool CanDoDamage = true;
        private float TimeBlocked;


        // Use this for initialization
        void Start()
        {
            if (GetComponentInParent<PC>() == null)
            {
                Debug.LogError("Sword must be child to object with PlayerController");
            }
            if (GetComponentInParent<Rigidbody>() == null)
            {
                Debug.LogError("Sword must be child to object with RigidBody");
            }
            if (GetComponentInParent<Player1>() == null)
            {
                Debug.LogError("Sword must be child to object with Player1");
            }

            PlayerController = GetComponentInParent<PC>();
            ShieldSlidingController = GetComponentInParent<ShieldSlidingController>();
            PlayerRigidbody = GetComponentInParent<Rigidbody>();
            Player1 = GetComponentInParent<Player1>();

            NormalMaterialColor = GetComponent<MeshRenderer>().materials[1].color;

            SwordBox = GetComponent<BoxCollider>();
            SwordBox.enabled = false;
        }

        void Update()
        {

            if (BlockedRecently)
            {
                TimeBlocked = Time.realtimeSinceStartup;
                BlockedRecently = false;
            }

            if (TimeBlocked + 2f > Time.realtimeSinceStartup)
            {
                CanDoDamage = true;
                TimeBlocked = 0;
            }
        }


        public void QuickAttack()
        {
            //Debug.Log("QuickAttack!!!");
            if (PlayerController.m_IsGrounded && !ShieldSlidingController.ShieldSliding)
            {
                Vector3 ForceVector = new Vector3(0, 0, QuickAttackForce);
                Player1.NormalForceController(ForceVector);
            }
            if (ChargeStatus)
            {
                SendMessageUpwards("DepletePotentialBar");
            }
           
            SwordBox.enabled = true;

        }

        public void StrongAttack()
        {

            if (PlayerController.m_IsGrounded && !ShieldSlidingController.ShieldSliding)
            {
                PlayerController.Atacking = true;
                Vector3 ForceVector = new Vector3(0, StrongAttackForceY , StrongAttackForceZ);
                Player1.NormalForceController(ForceVector);
            }
            SwordBox.enabled = true;

        }

        public void SideStepRight()
        {

            //Possibly change this to a spinning attack but must adjust mouse look snap
            PlayerController.Atacking = true;
            Vector3 ForceVector = new Vector3(-SideStepForceX, SideStepForceY, SideStepForceZ);
            Vector3 TorqueVector = new Vector3(0, 0, SideStepTorque);

            Player1.NormalForceController(ForceVector);
            //Player1.TorqueController(TorqueVector);

        }

        public void SideStepLeft()
        {
            //Possibly change this to a spinning attack but must adjust mouse look snap
            PlayerController.Atacking = true;
            Vector3 ForceVector = new Vector3(SideStepForceX, SideStepForceY, SideStepForceZ);
            Vector3 TorqueVector = new Vector3(0, 0, -SideStepTorque);

            Player1.NormalForceController(ForceVector);
         //   Player1.TorqueController(TorqueVector);

        }

        public void ChangeColorSword()
        {
            ChargeStatus = !ChargeStatus;
            if (ChargeStatus)
            {
                SendMessageUpwards("PotentialStatus", true);
                GetComponent<MeshRenderer>().materials[1].color = ChargeMaterial.color;
            }
            else
            {
                SendMessageUpwards("PotentialStatus", false);
                GetComponent<MeshRenderer>().materials[1].color = NormalMaterialColor;
            }
        }

        public void SuperCharging()
        {
            Vector3 ForceVector = new Vector3(0, 0, ImpulseForwardForce);
            Player1.NormalImpulseForceController(ForceVector);
        }

        public void SlashSword()
        {
            if (ChargeStatus)
            {
                SendMessageUpwards("DepletePotentialBar");
            }
            PlayerController.Atacking = true;
            SwordBox.enabled = true;
        }



        public void StopAttacking()
        {
            PlayerController.Atacking = false;
            SwordBox.enabled = false;
        }

        private void OnTriggerEnter(Collider col)
        {
            //Debug.Log("Entered Collision");
            if (col.gameObject.GetComponent<Shield>())
            {
                Vector3 objLoc = col.gameObject.transform.position;
                Vector3 LaunchVector = new Vector3(objLoc.x - PlayerController.transform.position.x, 0, objLoc.z - PlayerController.transform.position.z).normalized;

                Player1.ImpulseForceController(-LaunchVector, BlockedForce);
                //PlayerRigidBody.AddRelativeForce(new Vector3(0,0, -100), ForceMode.Impulse);
                SendMessageUpwards("Blocked");
                CanDoDamage = false;
                BlockedRecently = true;
            }

            else
            {
               
                if (col.gameObject.GetComponent<LaunchObject>() && CanDoDamage)
                {
                    if (col.gameObject.GetComponent<Enemy>())
                    {
                        col.gameObject.GetComponent<Enemy>().DisablePathing();

                        // Part of it damage needs to be scalled//
                        col.gameObject.GetComponent<EnemyHealth>().Hit(100);
                    }

                    //Vector3 objLoc = col.gameObject.transform.position;
                    //Vector3 LaunchVector = new Vector3(objLoc.x - PlayerController.transform.position.x, 1, objLoc.z - PlayerController.transform.position.z).normalized;
                    Vector3 LaunchVector = new Vector3(PlayerController.transform.forward.x, 0, PlayerController.transform.forward.z);


                    float LaunchForce = InitialLaunchForce + PlayerRigidbody.velocity.magnitude;

                    col.GetComponent<LaunchObject>().ImpulseForceController(LaunchVector, LaunchForce);
                 
                }
            }
        }

        private void OnTriggerExit(Collider col)
        {
          //  Debug.Log("TriggerExited");
            if (col.gameObject.GetComponent<Enemy>())
            {
                col.gameObject.GetComponent<Enemy>().RecentlyHit();
            }
        }

        public void Dead()
        {
            gameObject.SetActive(false);
            Instantiate(DeathSword, transform.position, transform.rotation);
        }

    }
}

