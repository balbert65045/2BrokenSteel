using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class CameraControl : MonoBehaviour {

        PlayerController PlayerController;

        // Use this for initialization
        void Start()
        {
            PlayerController = FindObjectOfType<PlayerController>();
        }

        // Update is called once per frame
        void Update()
        {
           // transform.position = PlayerController.transform.position;
        }
}
}
