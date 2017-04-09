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
    
        // Use this for initialization
        void Start()
        {

            if (GetComponentInParent<Player1>() == null)
            {
                Debug.LogError("CuncussionGauntlets must be child to object with Player1");
            }

            Player1 = GetComponentInParent<Player1>();

        }

        // Update is called once per frame

        public void LaunchUp()
        {
            Vector3 ForceVector = new Vector3(0, m_RelativeLaunchPowerUp, 0);
            Player1.NormalForceController(ForceVector);
     
            BroadcastMessage("BlastLaunchUp");
        }

        public void LaunchLeft()
        {
            Vector3 ForceVector = new Vector3(-m_RelativeLaunchPowerSideX, m_RelativeLaunchPowerSideY, 0);
            Vector3 TorqueVector = new Vector3(0, 0, m_RelativeTourquePower);

            Player1.NormalForceController(ForceVector);
            Player1.TorqueController(TorqueVector);

            BroadcastMessage("BlastLaunchLeft");
        }

        public void LaunchRight()
        {
            Vector3 ForceVector = new Vector3(m_RelativeLaunchPowerSideX, m_RelativeLaunchPowerSideY, 0);
            Vector3 TorqueVector = new Vector3(0, 0, -m_RelativeTourquePower);

            Player1.NormalForceController(ForceVector);
            Player1.TorqueController(TorqueVector);
            BroadcastMessage("BlastLaunchRight");
        }

        public void LaunchBackward()
        {
            Vector3 ForceVector = new Vector3(0, 0, -m_RelativeLaunchPowerUp);
            Player1.NormalForceController(ForceVector);
            BroadcastMessage("BlastLaunchBackward");
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

    }
}
