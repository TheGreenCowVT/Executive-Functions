using UnityEngine;
using UnityEngine.AI;

public class EnemyAIV2 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] NavMeshAgent agent;

    public Transform player;
    [Range(1, 15), SerializeField] int HP;
    public LayerMask whatIsGround, whatIsPlayer;
    public GameObject projectile;
    // Patrolling Vars
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    bool alreadyAttacked;
    [Range(0, 35), SerializeField] public float timeBetweenAttacks;

    // States
    [Range(0, 35), SerializeField] public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Archer").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        // See if player is in sight or attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange)
        {
            patrolling();
        }

        if (playerInSightRange && !playerInAttackRange)
        {
            chasePlayer();
        }

        if (playerInSightRange && playerInAttackRange)
        {
            attackPlayer();
        }
    }

    private void patrolling()
    {
        if (!walkPointSet)
        {
            searchWalkPoint();
        }
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        // Get distance to walkpoint
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Walkpoint Reached
        if (distanceToWalkPoint.magnitude <1f)
        {
            walkPointSet = false;
        }
    }
    private void searchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomx = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomx, transform.position.y, transform.position.z + randomZ);

        // Check to see if the ground is within the map via raycast
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void chasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void attackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if(!alreadyAttacked)
        {
            // Attack code
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();

            rb.AddForce(transform.forward * 32f, ForceMode.Impulse); 
            //rb.AddForce(transform.up * 8f, ForceMode.Impulse);


            alreadyAttacked = true;
            Invoke(nameof(resetAttack), timeBetweenAttacks);
        }
    }

    private void resetAttack()
    {
        alreadyAttacked = false;
    }

    public void takeTamage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            Invoke(nameof(destroyEnemy), .5f);
        }
    }

    private void destroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
