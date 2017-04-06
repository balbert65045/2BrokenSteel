using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetArrow : MonoBehaviour {

    // Use this for initialization
    private GameObject Enemy;
    float StartHeight = 3;
    bool MovingUp = true;
    float moveSpeed = .5f; 

	void Start () {
        //gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (Enemy != null)
        {
            
            transform.position = new Vector3(Enemy.transform.position.x, Enemy.transform.position.y  + StartHeight, Enemy.transform.position.z);
            if (StartHeight < 4 && MovingUp)
            {
                Mathf.Clamp(3 ,4 ,StartHeight += moveSpeed * Time.deltaTime);
            }
            else if(StartHeight > 3 && !MovingUp)
            {
                Mathf.Clamp(3, 4, StartHeight -= moveSpeed * Time.deltaTime);
            }
            else if (StartHeight >= 4 && MovingUp)
            {
                MovingUp = false;
                Mathf.Clamp(3, 4, StartHeight -= moveSpeed * Time.deltaTime);
            }
            else if (StartHeight <= 3 && !MovingUp)
            {
                MovingUp = true;
                Mathf.Clamp(3, 4, StartHeight += moveSpeed * Time.deltaTime);
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
