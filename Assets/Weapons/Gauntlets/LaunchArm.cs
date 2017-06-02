using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchArm : MonoBehaviour {

    public GameObject Blast;
    public GameObject Smoke;
    private ParticleSystem SmokeParticle;
    private ParticleSystem BlastParticles;
    private bool Toggled = false;
    private Color OriginalColor; 

    private bool colorCharge = false;

	// Use this for initialization
	void Start () {
        BlastParticles = Blast.GetComponent<ParticleSystem>();
        OriginalColor = GetComponent<MeshRenderer>().material.color;
    }
	
	// Update is called once per frame
	void Update () {
		if (colorCharge)
        {
           // Debug.Log("ChangingColor");
            float M_b = Mathf.Clamp(GetComponent<MeshRenderer>().material.color.b - Time.deltaTime, 0, 255);
            float M_g = Mathf.Clamp(GetComponent<MeshRenderer>().material.color.g - Time.deltaTime, 0, 255);
            GetComponent<MeshRenderer>().material.color = new Color(GetComponent<MeshRenderer>().material.color.r, M_g, M_b, GetComponent<MeshRenderer>().material.color.a);
        }
        else if (GetComponent<MeshRenderer>().material.color != OriginalColor)
        {
            GetComponent<MeshRenderer>().material.color = OriginalColor;
        }
	}


    public void GauntletsToggleColor()
    {
        Toggled = !Toggled;
        if (Toggled)
        {

            OriginalColor = Color.blue;
        }
        else
        {
            OriginalColor = Color.white;
        }
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

    public void ChangeColor()
    {
        colorCharge = true;
        
    }

    public void ChangeColorBack()
    {
        colorCharge = false;
    }
}
