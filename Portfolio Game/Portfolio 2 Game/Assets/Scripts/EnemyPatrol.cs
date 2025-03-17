using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    public Transform playerTarget; // Reference to the player GameObject
    [SerializeField] NavMeshAgent agent;
    [Range(1, 15), SerializeField] float chaseRange;
    [Range(1, 20), SerializeField] float patrolSpeed;
    [Range(1, 20), SerializeField] float chaseSpeed;
    [SerializeField] Animator animator;

    public int targetPoint;
    public float speed;
    bool isChasing = false;


    public Transform[] waypoints; // Array to store waypoints
    private int currentWaypointIndex = 0;

    bool isPatrolling = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    Debug.DrawLine(transform.position, agent.destination, Color.red);
    //    float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position); // Get distance to player
    //    if (distanceToPlayer <= chaseRange)
    //    {
    //        isPatrolling = false;
    //        agent.speed = chaseSpeed;
    //        agent.SetDestination(playerTarget.position);
    //        animator.SetBool("IsChasing", true); // Make later
    //    }
    //    else
    //    {
    //        // Patrolling Logic
    //        isPatrolling = true;
    //        agent.speed = patrolSpeed;
    //        agent.SetDestination(waypoints[currentWaypointIndex].position); // Move agent to location
    //        animator.SetBool("IsChasing", false);                            //transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, patrolSpeed * Time.deltaTime);

    //        // Waypoint system goes here
    //        //if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 1f) // reached waypoint
    //        if (agent.remainingDistance < 0.1f && !agent.pathPending)
    //        {
    //            currentWaypointIndex++;
    //            if (currentWaypointIndex >= waypoints.Length)
    //            {
    //                currentWaypointIndex = 0; // Loop back to the first waypoint
    //            }
    //        }
    //    }
    //}
    void Update()
    {
        Debug.DrawLine(transform.position, agent.destination, Color.red);

        if (playerTarget != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

            if (distanceToPlayer <= chaseRange)
            {
                agent.speed = chaseSpeed;
                agent.SetDestination(playerTarget.position);
                animator.SetBool("IsChasing", true);
            }
            else
            {
                agent.speed = patrolSpeed;
                animator.SetBool("IsChasing", false);
               //if (waypoints.Length > 0)
                //{
                    if (agent.remainingDistance <= 2.0f && !agent.pathPending)
                    {
                        currentWaypointIndex++;
                        if (currentWaypointIndex == waypoints.Length)
                        {
                            currentWaypointIndex = 0;
                        }
                        agent.SetDestination(waypoints[currentWaypointIndex].position);
                    }
                //}
            }
        }
        else
        {
            Debug.LogError("Player target is not assigned in EnemyPatrol script.");
            //Patrolling logic without chasing.
            agent.speed = patrolSpeed;
            animator.SetBool("IsChasing", false);
            //if (waypoints.Length > 0)
            //{
                if (agent.remainingDistance <= 2.0f && !agent.pathPending)
                {
                //currentWaypointIndex++;
                currentWaypointIndex = Random.Range(0, waypoints.Length);
                    //if (currentWaypointIndex == waypoints.Length)
                    //{
                    //    currentWaypointIndex = 0;
                    //}
                    agent.SetDestination(waypoints[currentWaypointIndex].position);
                }
            //}
        }
    }
}
