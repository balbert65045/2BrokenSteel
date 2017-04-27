using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArrow : MonoBehaviour {

    // Use this for initialization
    private GameObject Enemy;
    float StartHeight = 5;
    float initialStartHeight;
    bool MovingUp = true;
    float moveSpeed = .5f; 

	void Start () {
        //gameObject.SetActive(false);
        initialStartHeight = StartHeight;
    }
	
	// Update is called once per frame
	void Update () {
        if (Enemy != null)
        {
            
            transform.position = new Vector3(Enemy.transform.position.x, Enemy.transform.position.y  + StartHeight, Enemy.transform.position.z);
            if (StartHeight < initialStartHeight + 1 && MovingUp)
            {
                Mathf.Clamp(initialStartHeight, initialStartHeight + 1, StartHeight += moveSpeed * Time.deltaTime);
            }
            else if(StartHeight > initialStartHeight && !MovingUp)
            {
                Mathf.Clamp(initialStartHeight, initialStartHeight + 1, StartHeight -= moveSpeed * Time.deltaTime);
            }
            else if (StartHeight >= initialStartHeight + 1 && MovingUp)
            {
                MovingUp = false;
                Mathf.Clamp(3, initialStartHeight + 1, StartHeight -= moveSpeed * Time.deltaTime);
            }
            else if (StartHeight <= initialStartHeight && !MovingUp)
            {
                MovingUp = true;
                Mathf.Clamp(initialStartHeight, initialStartHeight + 1, StartHeight += moveSpeed * Time.deltaTime);
            }
         //   Debug.Log(StartHeight);
          //  Debug.Log(MovingUp);
        }

    }


    public void Targeted(GameObject Target)
    {

            Enemy = Target;
       // Debug.Log(Enemy.transform.position);

    }

}
