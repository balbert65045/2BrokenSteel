using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Health : MonoBehaviour {

    public Slider HealthSlider;

    [SerializeField]
    float DamageVelocity = 20f;
    private bool dead = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TakeDamage(float damage)
    {
        HealthSlider.value -= damage; 
        if (HealthSlider.value <= 0 && !dead)
        {
            dead = true;
            SendMessage("Death");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
     //   Debug.Log(GetComponent<Rigidbody>().velocity.magnitude);
        if (collision.gameObject.GetComponent<DamageWalls>())
        {
            Debug.Log(GetComponent<Rigidbody>().velocity.magnitude);
            if (GetComponent<Rigidbody>().velocity.magnitude > DamageVelocity)
            {
                Debug.Log("Damaged");
                TakeDamage(GetComponent<Rigidbody>().velocity.magnitude);
            }
        }
    }
}
