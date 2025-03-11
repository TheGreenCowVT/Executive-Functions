using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Runtime.CompilerServices;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int animTransSpeed;

    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    public Transform playerTarget; // Reference to the player GameObject
    [Range(1,15), SerializeField] float chaseRange;
    [Range(1, 20), SerializeField] float patrolSpeed;
    [Range(1, 20), SerializeField] float chaseSpeed;

    public Transform[] waypoints; // Array to store waypoints
    private int currentWaypointIndex = 0;

    bool isPatrolling = true;

    float shootTimer;

    Vector3 playerDir;
    bool playerInRage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        gamemanager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {

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

            setAnimLocomotion();

        shootTimer += Time.deltaTime;

        if (playerInRage)
        {
            playerDir = gamemanager.instance.player.transform.position - transform.position;
            agent.SetDestination(gamemanager.instance.player.transform.position);



            if (shootTimer >= shootRate)
            {
                shoot();
            }

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                faceTarget();
            }
        }
    }

    void setAnimLocomotion()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        float animSpeedCur = animator.GetFloat("Speed");
        animator.SetFloat("Speed", Mathf.Lerp(animSpeedCur, agentSpeed, Time.deltaTime * animTransSpeed));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRage = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRage = false;
        }
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());

        agent.SetDestination(gamemanager.instance.player.transform.position);

        if (HP <= 0)
        {
            gamemanager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

    void shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPos.position, transform.rotation);
    }
}
