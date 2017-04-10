using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; 
using UnityEngine;

public class WeaponController : MonoBehaviour {

    bool CuncussionGauntlets = true;

    [SerializeField]
    public float LaunchUpManaCost =25;
    public float LaunchSideManaCost = 15;
    public float RapidFireManaCost = 10;
    public float AirManaRechargeRate = 1;

    public Slider HealthSlider;
    public Slider AirManaSlider; 
	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    private void Update()
    {
        AirManaSlider.value += Time.deltaTime * AirManaRechargeRate;
    }

    public void Launch(int type)
    {
        if (type == 0 && AirManaSlider.value > LaunchUpManaCost)
        {
            BroadcastMessage("LaunchUp");
            AirManaSlider.value -= LaunchUpManaCost;
        } else if (type == 1 && AirManaSlider.value > LaunchSideManaCost)
        {
            BroadcastMessage("LaunchLeft");
            AirManaSlider.value -= LaunchSideManaCost;
        } else if (type == 2 && AirManaSlider.value > LaunchSideManaCost)
        {
            BroadcastMessage("LaunchRight");
            AirManaSlider.value -= LaunchSideManaCost;
        }
        else if (type == 3 && AirManaSlider.value > RapidFireManaCost)
        {
            BroadcastMessage("RapidFireRight");
            AirManaSlider.value -= RapidFireManaCost;
        }
        else if(type == 4 && AirManaSlider.value > RapidFireManaCost)
        {
            BroadcastMessage("RapidFireLeft");
            AirManaSlider.value -= RapidFireManaCost;
        }
        else if(type == 5)
        {
            Debug.Log("Still need to code mana with this move");
            BroadcastMessage("LaunchBackward");
        }
        else
        {
            Debug.Log("Out of Mana");
        }
    }

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
    }

    public void Stop()
    {
        BroadcastMessage("StopAttacking");
    }

    public void SlowMo(int on)
    {
        if (on == 1)
        {
            Time.timeScale = .15f;
          //  Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
           // Debug.Log("SlowMo On!");
        }
        else
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
           // Debug.Log("SlowMo off!");
        }
    }
  

}
