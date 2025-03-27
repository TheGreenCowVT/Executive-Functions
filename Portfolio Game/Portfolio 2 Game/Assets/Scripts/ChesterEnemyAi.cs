using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.VisualScripting;

public class ChesterEnemyAI : MonoBehaviour, IDamage
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

    [SerializeField] Transform headPos;


    float angleToPlayer;
    Vector3 playerDir;
    Vector3 startingPos;
    float roamTimer;
    float stoppingDistance;
    bool isChasing;
    float shootTimer;
    private int HP;
 
    bool playerInRange;

    private EnemySpawner spawner; // Reference to the wave-based spawner

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        HP = maxHP;
        stoppingDistance = agent.stoppingDistance;
        agent = GetComponent<NavMeshAgent>();
        //enemyHealthBar = gamemanager.instance;


        //if (enemyHealthBar != null)
        //{
        //    enemyHealthBar.updateEnemyHPBar(maxHP, HP);
        //}
        //else
        //{
        //    Debug.LogError("Gamemanager not found");
        //}
        //if (enemyType == EnemyType.melee)
        //{
        //    bullet = null;
        //    shootRate = 0f;
        //}

        spawner = FindObjectOfType<EnemySpawner>();
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>();
    }
    // Update is called once per frame
    void Update()
    {
        setAnimLocomotion();

        if(agent.velocity.magnitude > 0 && animator.GetBool("isAttacking") == false)
        {
            animator.SetBool("isMoving", true);
        }
        else if (agent.velocity.magnitude > 0 && animator.GetBool("isAttacking") == true)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
            shootTimer += Time.deltaTime;

        if (agent.remainingDistance <= 0.01f)
        {
            roamTimer += Time.deltaTime;
        }

        if (playerInRange/* && !canSeePlayer()*/)
        {
            agent.stoppingDistance = stoppingDistance;

            animator.SetBool("isChasing", true);
            agent.SetDestination(gamemanager.instance.player.transform.position);

        }
        else if (!playerInRange)
        {
            agent.stoppingDistance = 0;
            checkRoam();
        }



    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            animator.SetBool("isAttacking", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
            animator.SetBool("isAttacking", false);
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
        StartCoroutine(flashRed());

        //if (enemyHealthBar != null)
        //{
        //    enemyHealthBar.updateEnemyHPBar(maxHP, HP);
        //}

        agent.SetDestination(gamemanager.instance.player.transform.position);
        StartCoroutine(ResetDamageFlag());

        if (HP <= 0)
        {
            // Notify the spawner that this enemy was destroyed
            if (spawner != null)
            {
                spawner.OnEnemyDestroyed();
            }


            gamemanager.instance.enemyKillCountInt++;
            gamemanager.instance.enemyKillCount.text = gamemanager.instance.enemyKillCountInt.ToString("F0");
            Destroy(gameObject);
            controller.expUP();
            controller.updatePlayerUI();
        }
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

   

    IEnumerator ResetDamageFlag()
    {
        yield return new WaitForSeconds(2F);
    }


    bool canSeePlayer()
    {

        playerDir = gamemanager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(gamemanager.instance.player.transform.position);

                // Moved to only let the enemy shoot while they can see the player
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
        if ((roamTimer > roamPauseTime && agent.remainingDistance < 0.01) /*|| gamemanager.instance.playerScript.HP <= 0*/)
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
