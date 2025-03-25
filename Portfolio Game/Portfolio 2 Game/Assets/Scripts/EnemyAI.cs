using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] playerController controller;
    public enum EnemyType { melee, Ranged }
    [SerializeField] EnemyType enemyType;
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;

    [SerializeField] int maxHP;
    [Range(1,100),SerializeField] int expValue;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int animTransSpeed;
    [SerializeField] gamemanager enemyHealthBar;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    
    
    float shootTimer;
    private int HP;
    Vector3 playerDir;
    bool playerInRage;
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

        if (playerInRage || wasDamagedRecently)
        {
            playerDir = gamemanager.instance.player.transform.position - transform.position;
            agent.SetDestination(gamemanager.instance.player.transform.position);

            if (enemyType == EnemyType.Ranged && shootTimer >= shootRate && bullet != null)
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
}
