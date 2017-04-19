using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchArm : MonoBehaviour {

    public GameObject Blast;
    public GameObject Smoke;
    private ParticleSystem SmokeParticle;
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

        Blast.transform.localRotation = Quaternion.Euler(0, 0, 0);
       
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
    public void BlastLaunchBackward()
    {

            BlastParticles.Play();
    }

    public void BlastLaunchForwards()
    {
        BlastParticles.Play();
    }

    public void BlastRapidFireRight()
    {
        if (name == "RightLaunchArm")
        {
            BlastParticles.Play();
        }
    }

    public void BlastRapidFireLeft()
    {
        if (name == "LeftLaunchArm")
        {
            BlastParticles.Play();
        }
    }

    public void BlastShieldSlide()
    {
        if (name == "RightLaunchArm")
        {
            BlastParticles.Play();
        }
    }

    public void BlastShieldSlideInv()
    {
        if (name == "LeftLaunchArm")
        {
            BlastParticles.Play();
        }
    }
}
