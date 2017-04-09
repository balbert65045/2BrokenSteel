using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {

    // Use this for initialization
    private Enemy Enemy;
    private BoxCollider ShieldCollider;
    private bool BlockedRecently = false;
    private float TimeBlocked;


    void Start () {

        if (GetComponentInParent<Enemy>() == null)
        {
            Debug.LogError("Shield must be child to object with Enemy");
        }

        ShieldCollider = GetComponent<BoxCollider>();
        ShieldCollider.enabled = false;
        Enemy = GetComponentInParent<Enemy>();

    }
	
	// Update is called once per frame
	void Update () {
		
        if (BlockedRecently)
        {
            TimeBlocked = Time.realtimeSinceStartup;
            BlockedRecently = false;
        }

        if (TimeBlocked + 2f > Time.realtimeSinceStartup)
        {
            Enemy.Recover();
            TimeBlocked = 0; 
        }
	}

    public void Blocking()
    {
        Debug.Log("Blocking");
        ShieldCollider.enabled = true;
    }

    public void NotBlocking()
    {
        ShieldCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered");
        if (other.gameObject.GetComponent<weapon>())
        {
            BlockedRecently = true;
            Enemy.DisablePathing();
            Enemy.gameObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, -100), ForceMode.Impulse);
        }

    }

}
