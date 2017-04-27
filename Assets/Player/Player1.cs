using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1 : MonoBehaviour {

    // Use this for initialization

    private Rigidbody PlayerRigidBody;

	void Start () {

        if (GetComponent<Rigidbody>() == null)
        {
            Debug.LogError("No Rigidbody attached to object for Player1");
        }

        PlayerRigidBody = GetComponent<Rigidbody>();

	}
	
	// Update is called once per frame

    public void PlayerHit()
    {
        SendMessage("Hit");
    }

    public void ImpulseForceController(Vector3 ForceVector, float ForceAmount)
    {
        PlayerRigidBody.AddForce(ForceVector * (ForceAmount), ForceMode.Impulse);
    }

    public void NormalForceController(Vector3 ForceVector)
    {
        PlayerRigidBody.AddRelativeForce(ForceVector);
    }

    public void NormalImpulseForceController(Vector3 ForceVector)
    {
        PlayerRigidBody.AddRelativeForce(ForceVector, ForceMode.Impulse);
    }


    public void TorqueController(Vector3 TourqueVector)
    {
        PlayerRigidBody.constraints = RigidbodyConstraints.None;
        PlayerRigidBody.AddRelativeTorque(TourqueVector);
    }

}
