using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchObject : MonoBehaviour {



    public void ImpulseForceController(Vector3 ForceVector, float ForceAmount)
    {
        GetComponent<Rigidbody>().AddForce(ForceVector * (ForceAmount), ForceMode.Impulse);
    }

    public void NormalForceController(Vector3 ForceVector)
    {
        GetComponent<Rigidbody>().AddRelativeForce(ForceVector);
    }


}
