using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {

    bool CuncussionGauntlets = true; 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame

    public void Launch(int type)
    {
        if (type == 0)
        {
            BroadcastMessage("LaunchUp");
        } else if (type == 1)
        {
            BroadcastMessage("LaunchLeft");
        } else if (type == 2)
        {
            BroadcastMessage("LaunchRight");
        }
        else if (type == 3)
        {
            BroadcastMessage("RapidFireRight");
        }
        else if(type == 4)
        {
            BroadcastMessage("RapidFireLeft");
        }
        else
        {
            Debug.LogWarning("Type used in animation cuncussion animation out of range, Use 0-2");
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

}
