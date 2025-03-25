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
    [Range(1, 50)][SerializeField] float rotationSpeed;

    public int targetPoint;
    public float speed;
    bool isChasing = false;

    public GameObject enemy;
    public Transform[] waypoints; // Array to store waypoints
    private int currentWaypointIndex = 0;

    bool isPatrolling = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        enemy = GameObject.FindWithTag("Enemy");
        playerTarget = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
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
                Vector3 direction = (playerTarget.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
            else
            {
                agent.speed = patrolSpeed;
                animator.SetBool("IsChasing", false);
                if (waypoints != null && waypoints.Length > 0)
                {
                    if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending && agent.pathStatus == NavMeshPathStatus.PathComplete)
                    {
                        currentWaypointIndex = Random.Range(0, waypoints.Length);
                        agent.SetDestination(waypoints[currentWaypointIndex].position);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Player target is not assigned in EnemyPatrol script.");
            //Patrolling logic without chasing.
            agent.speed = patrolSpeed;
            animator.SetBool("IsChasing", false);
            if (waypoints != null && waypoints.Length > 0) //Null check here
            {
                if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending && agent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    currentWaypointIndex = Random.Range(0, waypoints.Length);
                    agent.SetDestination(waypoints[currentWaypointIndex].position);
                }
            }
        }
    }
}
