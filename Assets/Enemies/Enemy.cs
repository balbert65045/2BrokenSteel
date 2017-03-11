using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    // Use this for initialization
    private Rigidbody m_Rigidbody;
    private bool m_IsGrounded;
    private Vector3 m_GroundNormal;
    public float m_GroundCheckDistance = 0.1f;



    void Start () {

        m_Rigidbody = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        Debug.Log(m_IsGrounded);
        //Debug.Log(m_Rigidbody.velocity.magnitude);

        if (m_IsGrounded && m_Rigidbody.velocity.magnitude <= .5)
        {
            transform.rotation = Quaternion.Euler( 0, transform.rotation.eulerAngles.y, 0);

        }
		
	}

    private void CheckGroundStatus()
    {

        RaycastHit hitInfo;
#if UNITY_EDITOR
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
        {
            m_GroundNormal = hitInfo.normal;
            m_IsGrounded = true;
            //Debug.Log(m_Rigidbody.velocity);

                // Debug.Log("SendtSchockwave");

        }
        else
        {
            m_IsGrounded = false;

            m_GroundNormal = Vector3.up;
        }
    }


}
