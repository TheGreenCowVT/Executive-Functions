using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.VisualScripting;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] playerController controller;
    public enum EnemyType { melee, Ranged }
    [SerializeField] EnemyType enemyType;
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] int FOV;
    [SerializeField] int roamPauseTime;
    [SerializeField] int roamDistance;



    [SerializeField] int maxHP;
    [Range(1,100),SerializeField] int expValue;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int animTransSpeed;
    [SerializeField] gamemanager enemyHealthBar;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] Transform headPos;


    float angleToPlayer;
    Vector3 playerDir;
    Vector3 startingPos;
    float roamTimer;
    float stoppingDistance;

    float shootTimer;
    private int HP;
 
    bool playerInRange;
    bool wasDamagedRecently;
    private EnemyLoot loot;

    private EnemySpawner spawner; // Reference to the wave-based spawner

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        loot = GetComponent<EnemyLoot>();
        HP = maxHP;

        enemyHealthBar = gamemanager.instance;
        if (enemyHealthBar != null)
        {
            enemyHealthBar.updateEnemyHPBar(maxHP, HP);
        }
        else
        {
            Debug.LogError("Gamemanager not found");
        }
        if (enemyType == EnemyType.melee)
        {
            bullet = null;
            shootRate = 0f;
        }

        gamemanager.instance.updateNumEnemies(1);
        gamemanager.instance.numEnemiesOrig++;

        spawner = FindObjectOfType<EnemySpawner>();
    }
    // Update is called once per frame
    void Update()
    {
        setAnimLocomotion();

        shootTimer += Time.deltaTime;

        if (agent.remainingDistance <= 0.01f)
        {
            roamTimer += Time.deltaTime;
        }

        if (playerInRange && !canSeePlayer())
        {
            agent.stoppingDistance = 0;
            checkRoam();

        }
        else if (!playerInRange)
        {
            agent.stoppingDistance = 0;
            checkRoam();
        }
        else if (angleToPlayer <= FOV)
        {
            agent.stoppingDistance = stoppingDistance;
            faceTarget();
        }


    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
        }
    }

    void setAnimLocomotion()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        float animSpeedCur = animator.GetFloat("Speed");
        animator.SetFloat("Speed", Mathf.Lerp(animSpeedCur, agentSpeed, Time.deltaTime * animTransSpeed));
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        if (enemyHealthBar != null)
        {
            enemyHealthBar.updateEnemyHPBar(maxHP, HP);
        }

        StartCoroutine(flashRed());


        agent.SetDestination(gamemanager.instance.player.transform.position);
        wasDamagedRecently = true;
        StartCoroutine(ResetDamageFlag());

        if (HP <= 0)
        {

            loot.Die();

            // Notify the spawner that this enemy was destroyed
            if (spawner != null)
            {
                spawner.OnEnemyDestroyed();
            }

            Destroy(gameObject);
            controller.expAmount += expValue;
            controller.updatePlayerUI();
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

    IEnumerator ResetDamageFlag()
    {
        yield return new WaitForSeconds(2F);
        wasDamagedRecently = false;
    }


    bool canSeePlayer()
    {

        playerDir = gamemanager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {

                agent.SetDestination(gamemanager.instance.player.transform.position);

                // Moved to only let the enemy shoot while they can see the player
                if (shootTimer >= shootRate)
                {
                    shoot();
                }
                return true;
            }
            else
            {
                return false;
            }

            // True because we're shooting the player

        }

        // Falce because we did not find the player
        agent.stoppingDistance = 0;
        return false;

    }

    void checkRoam()
    {
        if ((roamTimer > roamPauseTime && agent.remainingDistance < 0.01) || gamemanager.instance.playerScript.HP <= 0)
        {

            roam();

        }
    }
    void roam()
    {
        roamTimer = 0;

        // Go exactly to the spot
        //agent.stoppingDistance = 0;

        Vector3 randomPos = Random.insideUnitSphere * roamDistance;
        randomPos += startingPos;

        NavMeshHit hit;

        NavMesh.SamplePosition(randomPos, out hit, roamDistance, 1);

        agent.SetDestination(hit.position);
    }

}
