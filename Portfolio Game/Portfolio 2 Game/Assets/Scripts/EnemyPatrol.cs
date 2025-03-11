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
    void Update()
    {
        Debug.DrawLine(transform.position, agent.destination, Color.red);
        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position); // Get distance to player
        if (distanceToPlayer <= chaseRange)
        {
            isPatrolling = false;
            agent.speed = chaseSpeed;
            agent.SetDestination(playerTarget.position);
            animator.SetBool("IsChasing", true); // Make later
        }
        else
        {
            // Patrolling Logic
            isPatrolling = true;
            agent.speed = patrolSpeed;
            agent.SetDestination(waypoints[currentWaypointIndex].position); // Move agent to location
            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, patrolSpeed * Time.deltaTime);

            // Waypoint system goes here
            if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 1f) // reached waypoint
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Length)
                {
                    currentWaypointIndex = 0; // Loop back to the first waypoint
                }
            }
        }
    }
}
