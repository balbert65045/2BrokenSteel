using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {

	// Use this for initialization
	void Start () {

        gameObject.GetComponent<BoxCollider>().enabled = false;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Blocking()
    {
        gameObject.GetComponent<BoxCollider>().enabled = true;
    }

    public void NotBlocking()
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }

}
