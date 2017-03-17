using System.Collections;
using System.Collections.Generic;
using UnityEngine;



    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof(Enemy))]
    public class Enemy_AI_Control : MonoBehaviour
    {
        public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
        public Enemy character { get; private set; } // the character we are controlling
        public Transform target;                                    // target to aim for


        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            character = GetComponent<Enemy>();

            agent.updateRotation = true;
            agent.updatePosition = true;
       
        }


    private void Update()
    {
        if (target != null)
            agent.SetDestination(target.position);
    }



        public void SetTarget(Transform target)
        {
            this.target = target;
         }

    public void MoveAgain()
    {
        Debug.Log("Resumed");
        //agent.ResetPath();
        agent.Resume();
        agent.updatePosition = true;

    }
     
    public void DisablePath()
    {
        agent.updatePosition = false;
        agent.Stop();
    }



   
}
