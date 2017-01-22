using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLandingManager : MonoBehaviour {

    public ParticleSystem Shockwave; 
	// Use this for initialization

   public void SchockwaveLanding()
    {
       // Debug.Log("SchockwaveRecieved");
        Shockwave.Play();
    }
}
