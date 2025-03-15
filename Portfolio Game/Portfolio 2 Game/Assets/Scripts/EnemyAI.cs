using UnityEngine;
using System.Collections;
using UnityEngine.AI;

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

    int maxHP;


    float shootTimer;

    Vector3 playerDir;
    bool playerInRage;
    private EnemyLoot loot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        loot = GetComponent<EnemyLoot>();
        gamemanager.instance.updateGameGoal(1);
        maxHP = HP;
        updateenemyUI();
    }
    // Update is called once per frame
    void Update()
    {
        updateenemyUI();

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
        updateenemyUI();
        StartCoroutine(flashRed());


        agent.SetDestination(gamemanager.instance.player.transform.position);

        if (HP <= 0)
        {

            gamemanager.instance.updateGameGoal(-1);
            loot.Die();
        }
        else
        {
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

    public void updateenemyUI()
    {
        gamemanager.instance.enemyHPBar.fillAmount = (float)HP / maxHP;
    }
}
