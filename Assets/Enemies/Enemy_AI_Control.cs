using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;




    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    [RequireComponent(typeof(Enemy))]
    public class Enemy_AI_Control : MonoBehaviour
    {
        public UnityEngine.AI.NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
        public Enemy character { get; private set; } // the character we are controlling
        public Transform target;                                    // target to aim for

        private Animator E_Animator;


        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            character = GetComponent<Enemy>();
            E_Animator = GetComponent<Animator>();


            agent.updateRotation = true;
            agent.updatePosition = true;

        }


        private void Update()
        {
            if (target != null && agent.enabled == true)
            {
                agent.SetDestination(target.position);
                E_Animator.SetBool("Running", true);
            }
            else
            {
                E_Animator.SetBool("Running", false);
            }
        }

        private void FixedUpdate()
        {
            if (target)
            {
                character.Move(target.transform);
            }
        }



        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public void MoveAgain()
        {
            Debug.Log("Resumed");
            //agent.ResetPath();
            agent.enabled = true;

        }

        public void DisablePath()
        {
            agent.enabled = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (target && agent)
            {
                Gizmos.DrawWireSphere(target.position, agent.stoppingDistance);
            }

        }



    }

