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

    public float AttacksPerSeconds = .5f; 

    private Animator E_Animator;

    private bool JustBlocked = false;
    private bool timeAttack = false;
    private bool InCombatPosition = false;

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
        CheckCombatTime();
        UpdateAnimator();
        if (target != null && agent.enabled == true)
        {
            agent.SetDestination(target.position);

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
       // Debug.Log("Resumed");
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

    private void UpdateAnimator()
    {
        if (target != null && agent.enabled == true)
        {
            if (agent.stoppingDistance > (target.position - transform.position).magnitude)
            {
                E_Animator.SetBool("Running", false);
                InCombatPosition = true;
                if (JustBlocked)
                {
                    E_Animator.SetTrigger("QuickAttack");
                    JustBlocked = false;
                }
                else if (timeAttack)
                {
                    E_Animator.SetTrigger("QuickAttack");
                    timeAttack = false;
                }
                else
                {
                    E_Animator.SetBool("Defending", true);
                }
            }
            else
            {
                InCombatPosition = false;
                E_Animator.SetBool("Defending", false);
                E_Animator.SetBool("Running", true);
            }
        }
        else
        {
          //  Debug.Log("OutofCombat");
            InCombatPosition = false;
            E_Animator.SetBool("Defending", false);
            E_Animator.SetBool("Running", false);
            
        }
    }


        private void CheckCombatTime()
    {
        if (InCombatPosition)
        {
            float probability = Time.deltaTime * AttacksPerSeconds;
            //Debug.Log(probability);

            if (Random.value < probability)
            {
               // Debug.Log("Attacked");
                timeAttack = true;
            }
        }
    }


    }
