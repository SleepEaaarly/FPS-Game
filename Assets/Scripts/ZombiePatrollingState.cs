using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;


public class ZombiePatrollingState : StateMachineBehaviour
{
    float timer;
    public float patrollingTime = 10f;

    Transform player;
    NavMeshAgent agent;

    public float detectionArea = 18f;
    public float patrolSpeed = 2f;

    List<Transform> waypointsList = new List<Transform>();

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // --- Initialization --- //
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();

        agent.speed = patrolSpeed;
        timer = 0;

        // --- Get all waypoints and move to the first one --- //
        GameObject waypointCluster = GameObject.FindGameObjectWithTag("Waypoints");
        if (waypointCluster == null) 
            return;
        
        foreach (Transform t in waypointCluster.transform)
        {
            waypointsList.Add(t);
        }

        Vector3 nextPosition = waypointsList[Random.Range(0, waypointsList.Count)].position;
        agent.SetDestination(nextPosition);

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (SoundManager.Instance.zombieChannel.isPlaying == false)
        {
            SoundManager.Instance.zombieChannel.clip = SoundManager.Instance.zombieWalking;
            SoundManager.Instance.zombieChannel.PlayDelayed(1f);
        }

        // --- If arrived, move to next waypoint --- //
        if (agent.remainingDistance <= agent.stoppingDistance && waypointsList.Count > 0)
        {
            agent.SetDestination(waypointsList[Random.Range(0, waypointsList.Count)].position);
        }

        // --- Transition to Idle State --- //
        timer += Time.deltaTime;
        if (timer > patrollingTime)
        {
            animator.SetBool("isPatrolling", false);
        }

        // --- Transition to Chase State --- //
        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
        if (distanceFromPlayer < detectionArea)
        {
            animator.SetBool("isChasing", true);
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // stop the agent
        agent.SetDestination(agent.transform.position);

        SoundManager.Instance.zombieChannel.Stop();
    }
}
