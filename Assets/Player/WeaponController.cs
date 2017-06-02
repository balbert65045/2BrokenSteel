using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; 
using UnityEngine;

namespace Player
{

    public class WeaponController : MonoBehaviour
    {

        bool CuncussionGauntlets = true;
        PC PlayerController;


        [SerializeField]
        public float LaunchUpManaCost = 25;
        public float LaunchSideManaCost = 15;
        private float InitialLaunchUpManaCost;
        private float InitialLauncSidepManaCost;

        public float RapidFireManaCost = 10;
        public float AirManaRechargeRate = 1;

        public float ChargePotentialFactor = 1f;
        public float PotentialDepletionRate = 1;
        private bool DepletelBar = false;

        public float TimeForChargedAttack = .5f;

        public Slider HealthSlider;
        public Slider AirManaSlider;
        public Slider PotentialSlider;
        // Use this for initialization

        public float ChargeTime = 2f;
        private float TimeStored;
        public bool Charging = false;

        private bool potentialOn = false;
        private bool ManaDrain = false;
        public float SppedIncrease = 10f;
        public float ManaDepletionRate = 2f;
        public float IncreaseFactor = 3f;

        private bool dead; 

        void Start()
        {
            PlayerController = GetComponent<PC>();
            InitialLaunchUpManaCost = LaunchUpManaCost;
            InitialLauncSidepManaCost = LaunchSideManaCost;
        }

        private void Update()
        {

            if (ManaDrain)
            {
                if (AirManaSlider.value <= 0)
                {
                    ToggleManaDrain();
                }
                AirManaSlider.value -= Time.deltaTime * ManaDepletionRate;
            }
            else
            {
                AirManaSlider.value += Time.deltaTime * AirManaRechargeRate;
            }
           
            if (DepletelBar && PotentialSlider.value != 0)
            {
                PotentialSlider.value -= PotentialDepletionRate * Time.deltaTime;
            }
            else if (PotentialSlider.value == 0 && potentialOn)
            {
                SendMessage("TogglePotentialSword");
                BroadcastMessage("ChangeColorSword");
                DepletelBar = false;
            }
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (Charging)
            {
                if (CheckForEnemy())
                {
                    Charging = false;
                    StopCharging();
                }
                if (Time.timeSinceLevelLoad > TimeStored + ChargeTime)
                {

                    Charging = false;
                    StopCharging();
                }
            }
        }



        /// <summary>
        /// Gauntlets//
        /// </summary>

        public void Launch(int type)
        {
            if (type == 0)
            {
                if (AirManaSlider.value > LaunchUpManaCost)
                {
                    BroadcastMessage("LaunchUp");
                    AirManaSlider.value -= LaunchUpManaCost;
                }
                ResetManaCharge();
            }
            else if (type == 1)
            {
                if (AirManaSlider.value > LaunchSideManaCost)
                {
                    BroadcastMessage("LaunchLeft");
                    AirManaSlider.value -= LaunchSideManaCost;
                }
                ResetManaCharge();
            }
            else if (type == 2 )
            {
                if (AirManaSlider.value > LaunchSideManaCost)
                {
                    BroadcastMessage("LaunchRight");
                    AirManaSlider.value -= LaunchSideManaCost;
                }
                ResetManaCharge();
            }
            else if (type == 3 && AirManaSlider.value > RapidFireManaCost)
            {

                BroadcastMessage("RapidFireRight");
                AirManaSlider.value -= RapidFireManaCost;
                LaunchSideManaCost = InitialLauncSidepManaCost;
            }
            else if (type == 4 && AirManaSlider.value > RapidFireManaCost)
            {
                BroadcastMessage("RapidFireLeft");
                AirManaSlider.value -= RapidFireManaCost;
            }


            else if (type == 5)
            {
                //  Debug.Log("Still need to code mana with this move");
                if (AirManaSlider.value > LaunchSideManaCost)
                {
                    BroadcastMessage("LaunchBackward");
                    AirManaSlider.value -= LaunchSideManaCost;
                }
                ResetManaCharge(); 
            }
            else if (type == 6)
            {
                if (AirManaSlider.value > LaunchSideManaCost)
                {
                    BroadcastMessage("LaunchForwards");
                    AirManaSlider.value -= LaunchSideManaCost;
                }
                ResetManaCharge();
            }
            else if (type == 7)
            {
                BroadcastMessage("ShieldSlideBoost");
            }
            else if (type == 8)
            {
                BroadcastMessage("ShieldSlideBoostInv");
            }
            else
            {
                Debug.Log("Out of Mana");
            }
        }

        public void SlowMo(int on)
        {
            if (on == 1)
            {
                //  Time.timeScale = .15f;
                //  //  Time.timeScale = 1f;
                //  Time.fixedDeltaTime = 0.02F * Time.timeScale;
                //  // Debug.Log("SlowMo On!");
            }
            else
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02F * Time.timeScale;
                // Debug.Log("SlowMo off!");
            }
        }

        private void ResetManaCharge()
        {
            LaunchSideManaCost = InitialLauncSidepManaCost;
            LaunchUpManaCost = InitialLaunchUpManaCost;
        }

        public void ChargingGauntlets(bool maxed)
        {
            if (!maxed)
            {
                LaunchSideManaCost += Time.deltaTime * IncreaseFactor;
                LaunchUpManaCost += Time.deltaTime * IncreaseFactor;
            }
        }

        public void ToggleManaDrain()
        {
            ManaDrain = !ManaDrain;
            if (ManaDrain)
            {
                PlayerController.m_MoveSpeedMultiplier += SppedIncrease;
                PlayerController.SlideSpeed += SppedIncrease;
            }
            else
            {
                PlayerController.m_MoveSpeedMultiplier -= SppedIncrease;
                PlayerController.SlideSpeed -= SppedIncrease;
            }
            BroadcastMessage("ChangeColorGauntlets");
        }



        /// <summary>
        /// Sword //
        /// </summary>
        /// 
        public void Slash(int type)
        {
            if (type == 0)
            {
                BroadcastMessage("QuickAttack");
            }

            else if (type == 1)
            {
                BroadcastMessage("StrongAttack");
            }
            else if (type == 2)
            {
                BroadcastMessage("SideStepRight");
            }
            else if (type == 3)
            {
                BroadcastMessage("SideStepLeft");
            }
            //SuperChargeCharging
            else if (type == 4)
            {
                Debug.Log("SuperCharging");
                Charging = true;
                TimeStored = Time.timeSinceLevelLoad;
                BroadcastMessage("SuperCharging");
            }
            else if (type == 5)
            {
                BroadcastMessage("SlashSword");
            }
        }

        public void Stop()
        {
            BroadcastMessage("StopAttacking");
        }


        private bool CheckForEnemy()
        {
            Debug.Log("CheckingForEnemy");
            float AttackDistance = (GetComponent<Rigidbody>().velocity.magnitude) * TimeForChargedAttack;


            Vector3 ForwardsDirection = new Vector3(transform.forward.x, 0, transform.forward.z);

            Debug.DrawLine(transform.position + transform.forward * .1f, transform.position + (ForwardsDirection * AttackDistance));

            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position + transform.forward * .1f, ForwardsDirection, out hitInfo, AttackDistance))
            {
                if (hitInfo.transform.GetComponent<Enemy>())
                {
                    Debug.Log("EnemyFound");
                    return true;
                }
            }
            return false;
        }

        private void StopCharging()
        {
            SendMessage("StopPotential");
        }

        public void ColorChange()
        {
            if (!dead)
            {
                BroadcastMessage("ChangeColorSword");
            }
            // Debug.Log("ChangeColor");
        }

        public void ChargePotential(float speed)
        {
            if (!potentialOn)
            {
                PotentialSlider.value += speed * ChargePotentialFactor * Time.deltaTime;
            }
        }

        public void PotentialStatus(bool value)
        {
            if (value)
            {
                potentialOn = true;
            }
            else
            {
                potentialOn = false;
            }
        }

        public void DepletePotentialBar()
        {
            DepletelBar = true;
        }

        public void Death()
        {
            dead = true;
        }


    }
}
