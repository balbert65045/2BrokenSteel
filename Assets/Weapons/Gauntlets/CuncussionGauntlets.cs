using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{


    public class CuncussionGauntlets : MonoBehaviour
    {

        Player1 Player1;


        [Header ("Launch Up Settings")]
        public float m_RelativeLaunchPowerUp = 1000;

        [Header("Launch Side Settings")]
        public float m_RelativeLaunchPowerSideX = 1000;
        public float m_RelativeLaunchPowerSideY = 1000;
        public float m_RelativeTourquePower = 100;

        [Header("RapidFire Settings")]
        public float m_RelativeLaunchPowerRapidFire = 250; 

        [Header("ShieldSlide Settings")]
        public float ShieldSlideLaunchForce = 500;

        // Use this for initialization
        public float ChargeTimeForce = 100;

        public bool chargeinit = false;
        private float ChargeTime = 0;
        private float InitialChargetimeForceFactor;
        public bool LaunchedUp = false;

        void Start()
        {

            if (GetComponentInParent<Player1>() == null)
            {
                Debug.LogError("CuncussionGauntlets must be child to object with Player1");
            }

            Player1 = GetComponentInParent<Player1>();
            InitialChargetimeForceFactor = ChargeTimeForce;
            ChargeTimeForce = 0;
        }

        // Update is called once per frame
        public void ChargeGauntlets()
        {
            if (!LaunchedUp)
            {
                if (!chargeinit)
                {
                    ChargeTime = Time.timeSinceLevelLoad;
                   // Debug.Log("TimeCaptured");
                }
                chargeinit = true;
                ChargeTimeForce = InitialChargetimeForceFactor * (Time.timeSinceLevelLoad - ChargeTime);
                BroadcastMessage("ChangeColor");
            }
        }

        public void GauntletsIdle()
        {
            if (!LaunchedUp)
            {
                chargeinit = false;
                ChargeTimeForce = 0;
                BroadcastMessage("ChangeColorBack");
            }
        }


        public void LaunchUp()
        {
            LaunchedUp = true;
            Vector3 ForceVector = new Vector3(0, m_RelativeLaunchPowerUp + ChargeTimeForce, 0);
            Player1.NormalForceController(ForceVector);
     
            BroadcastMessage("BlastLaunchUp");
        }

        public void LaunchLeft()
        {
            LaunchedUp = false;
            Vector3 ForceVector = new Vector3(-(m_RelativeLaunchPowerSideX + ChargeTimeForce), m_RelativeLaunchPowerSideY + .3f * ChargeTimeForce, 0);
            Vector3 TorqueVector = new Vector3(0, 0, m_RelativeTourquePower);

            Player1.NormalForceController(ForceVector);
          //  Player1.TorqueController(TorqueVector);

            BroadcastMessage("BlastLaunchLeft");
        }

        public void LaunchRight()
        {
            LaunchedUp = false;
            Vector3 ForceVector = new Vector3(m_RelativeLaunchPowerSideX + ChargeTimeForce, m_RelativeLaunchPowerSideY + .3f * ChargeTimeForce, 0);
            Vector3 TorqueVector = new Vector3(0, 0, -m_RelativeTourquePower);

            Player1.NormalForceController(ForceVector);
            //Player1.TorqueController(TorqueVector);
            BroadcastMessage("BlastLaunchRight");
        }

        public void LaunchBackward()
        {
            LaunchedUp = false;
            Vector3 ForceVector = new Vector3(0, m_RelativeLaunchPowerSideY + .3f * ChargeTimeForce, -(m_RelativeLaunchPowerSideX + ChargeTimeForce));
            Player1.NormalForceController(ForceVector);
            BroadcastMessage("BlastLaunchBackward");
        }

        public void LaunchForwards()
        {
            LaunchedUp = false;
            Vector3 ForceVector = new Vector3(0, m_RelativeLaunchPowerSideY + .3f * ChargeTimeForce, m_RelativeLaunchPowerSideX + ChargeTimeForce);
            Player1.NormalForceController(ForceVector);
            BroadcastMessage("BlastLaunchForwards");
        }

        public void RapidFireRight()
        {
            Vector3 ForceVector = new Vector3(0, 0, -m_RelativeLaunchPowerRapidFire);
            Player1.NormalForceController(ForceVector);
            BroadcastMessage("BlastRapidFireRight");
        }

        public void RapidFireLeft()
        {
            Vector3 ForceVector = new Vector3(0, 0, -m_RelativeLaunchPowerRapidFire);
            Player1.NormalForceController(ForceVector);
            BroadcastMessage("BlastRapidFireLeft");
        }

        public void ShieldSlideBoost()
        {
            Vector3 ForceVector = new Vector3(-(ShieldSlideLaunchForce + ChargeTimeForce/10), 0, 0);
            Player1.NormalImpulseForceController(ForceVector);
            BroadcastMessage("BlastShieldSlide");
        }
        public void ShieldSlideBoostInv()
        {
            Vector3 ForceVector = new Vector3(ShieldSlideLaunchForce + ChargeTimeForce/10, 0, 0);
            Player1.NormalImpulseForceController(ForceVector);
            BroadcastMessage("BlastShieldSlideInv");
        }

        public void Landed()
        {
            LaunchedUp = false;
            GauntletsIdle();
        }
    }
}
