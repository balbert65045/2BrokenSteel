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

    void Start () {
		
	}

    private void Update()
    {
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
            if (CheckForEnemy()){
                Charging = false;
                StopCharging();
            }
            if (Time.timeSinceLevelLoad > TimeStored + ChargeTime)
            {
             
                Charging = false;
                StopCharging();
            }
        }
        


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
          //  Debug.Log("Still need to code mana with this move");
            BroadcastMessage("LaunchBackward");
        }
        else if(type == 6)
        {
            BroadcastMessage("LaunchForwards");
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


    private bool CheckForEnemy()
    {
        Debug.Log("CheckingForEnemy");
        float AttackDistance = (GetComponent<Rigidbody>().velocity.magnitude) * TimeForChargedAttack;


        Vector3 ForwardsDirection = new Vector3(transform.forward.x, 0, transform.forward.z); 

        Debug.DrawLine(transform.position + transform.forward *.1f, transform.position + (ForwardsDirection * AttackDistance));

        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position + transform.forward * .1f, ForwardsDirection, out hitInfo, AttackDistance) )
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

    public void ColorChange()
    {
        BroadcastMessage("ChangeColorSword");
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


}
