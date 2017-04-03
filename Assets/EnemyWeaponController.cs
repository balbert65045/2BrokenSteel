using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponController : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
       
    }


    public void Slash(int type)
    {
        if (type == 0)
        {
            BroadcastMessage("QuickAttack");
        }
        else if (type == 1)
        {
            BroadcastMessage("StrongAttack");
        }
      
    }

    public void Stop()
    {
        BroadcastMessage("StopAttacking");
    }

   


}
