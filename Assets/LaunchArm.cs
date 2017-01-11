using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchArm : MonoBehaviour {

    public GameObject Blast;
    private ParticleSystem BlastParticles; 

	// Use this for initialization
	void Start () {
        BlastParticles = Blast.GetComponent<ParticleSystem>(); 

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void BlastLaunchUp()
    {

        Blast.transform.localRotation = Quaternion.Euler(90, 0, 90);
        BlastParticles.Play();
    }

    public void BlastLaunchLeft()
    {
        if (name == "RightLaunchArm")
        {
            Blast.transform.localRotation = Quaternion.Euler(120, 0, 90);
            BlastParticles.Play();
        }
    }

    public void BlastLaunchRight()
    {
        if (name == "LeftLaunchArm")
        {
            Blast.transform.localRotation = Quaternion.Euler(60, 0, 90);
            BlastParticles.Play();
        }
    }
}
