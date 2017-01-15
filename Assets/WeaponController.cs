﻿using System.Collections;
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
        else
        {
            Debug.LogWarning("Type used in animation cuncussion animation out of range, Use 0-2");
        }
    }

}
