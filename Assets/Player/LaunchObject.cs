using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchObject : MonoBehaviour {



    public void ImpulseForceController(Vector3 ForceVector, float ForceAmount)
    {
        GetComponent<Rigidbody>().AddForce( new Vector3(ForceVector.x * (ForceAmount), (ForceAmount * 0.1f), ForceVector.z * (ForceAmount)), ForceMode.Impulse);
    }

    public void NormalForceController(Vector3 ForceVector)
    {
        GetComponent<Rigidbody>().AddRelativeForce(ForceVector);
    }


}
