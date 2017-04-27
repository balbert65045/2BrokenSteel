using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour {



    private Animator M_Animator;

    public bool isUnderneath = false;
    private float InitialRotation; 

    // Use this for initialization
    void Start () {

        M_Animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    private void Update()
    {

        //RaycastHit hitInfo;

        ////Debug.DrawLine(transform.position + (Vector3.down * -.2f), transform.position + (Vector3.down ) + (Vector3.down * -m_GroundCheckDistance));

        ////Debug.Log(transform.position);
        //isGrounded = (Physics.Raycast(transform.position + (Vector3.down *-.2f), Vector3.down, out hitInfo, -m_GroundCheckDistance));

        //if (isGrounded && isUnderneath)
        //{
        //    Debug.Log("Grounded");
        //    ShieldCollider.enabled = true;
        //}
        //else
        //{
        //    ShieldCollider.enabled = false;
        //}

    }

    public void ShieldSlidingLeft()
    {
        M_Animator.SetBool("ShieldSlidingLeft", true);
       
    }

    public void ShieldSlidingRight()
    {
        M_Animator.SetBool("ShieldSlidingRight", true);

    }

    public void NotSliding()
    {
        M_Animator.SetBool("ShieldSlidingLeft", false);
        M_Animator.SetBool("ShieldSlidingRight", false);
        // ShieldCollider.enabled = false;
    }

    public void Underneath()
    {
        isUnderneath = true;
        SendMessageUpwards("ExpandCollider");
      //  ShieldCollider.enabled = true;
    }

    public void Idle()
    {
       // InitialRotation = 
        isUnderneath = false;
         SendMessageUpwards("ShrinkCollider");
        // ShieldCollider.enabled = false;
    }



}
