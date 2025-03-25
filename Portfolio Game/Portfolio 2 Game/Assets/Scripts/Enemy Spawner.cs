using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public GameObject[] enemyPrefabs;
        public int enemyCount;

    }

    [Header("Spawner Settings")]
    [SerializeField] private Wave[] waves;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float timeBetweenWaves = 10f;
    [SerializeField] public GameObject[] bossPrefabs;
    [SerializeField] private float bossVulnerabilityTime = 5f;

    private int currentWaveIndex = 0;
    private int enemiesRemainingInWave;
    private int enemiesAlive;
    private Wave currentWave;
    private GameObject currentBoss;
    private bool bossVulnerable = false;

    private void Start()
    {
        SpawnBoss();
        StartNextWave();
    }

    private void SpawnBoss()
    {
        if (bossPrefabs.Length > 0 && spawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomIndex];

            GameObject bossPrefab = bossPrefabs[Random.Range(0, bossPrefabs.Length)];
            currentBoss = Instantiate(bossPrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log($"Spawned boss at {spawnPoint.name}");
            StartCoroutine(EnableEnemyMovement(currentBoss));
        }
        else
        {
            Debug.LogWarning("No boss prefabs or spawn points assigned for boss spawn");
        }
    }

    private void StartNextWave()
    {
        if (currentWaveIndex >= waves.Length)
        {
            Debug.Log("All waves completed!");
            return;
        }

        currentWave = waves[currentWaveIndex];

        enemiesRemainingInWave = currentWave.enemyCount;
        enemiesAlive = 0;

        Debug.Log($"Starting Wave {currentWaveIndex + 1} with {enemiesRemainingInWave} enemies.");

        SpawnEnemiesForWave();
    }

    private void SpawnEnemiesForWave()
    {
        if (enemiesRemainingInWave > 0)
        {
            if (spawnPoints.Length > 0)
            {
                int randomIndex = Random.Range(0, spawnPoints.Length);
                Transform spawnPoint = spawnPoints[randomIndex];

                GameObject enemyPrefab = currentWave.enemyPrefabs[Random.Range(0, currentWave.enemyPrefabs.Length)];

                GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                enemiesRemainingInWave--;
                enemiesAlive++;

                Debug.Log($"Spawned enemy at {spawnPoint.name}. Enemies remaining in wave: {enemiesRemainingInWave}");

                StartCoroutine(EnableEnemyMovement(enemy));
                Invoke(nameof(SpawnEnemiesForWave), 1f);
            }
            else
            {
                Debug.LogWarning("No spawn points assigned to the spawner!");
            }
        }
    }

    private IEnumerator EnableEnemyMovement(GameObject enemy)
    {
        yield return new WaitForSeconds(0.5f);

        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = true;
            Debug.Log($"Enemy {enemy.name} movement enabled.");
        }
    }

    public void OnEnemyDestroyed()
    {
        enemiesAlive--;

        Debug.Log($"Enemy destroyed. Enemies alive: {enemiesAlive}");

        if (enemiesAlive <= 0 && enemiesRemainingInWave <= 0)
        {
            currentWaveIndex++;
            Invoke(nameof(StartNextWave), timeBetweenWaves);
        }
    }

    private IEnumerator MakeBossVulnerable()
    {
        bossVulnerable = true;
        Debug.Log("Boss is Vulnerable!");

        yield return new WaitForSeconds(bossVulnerabilityTime);
        bossVulnerable = false;
        Debug.Log("Boss is no longer vulnerable");

        currentWaveIndex++;
        Invoke(nameof(StartNextWave), timeBetweenWaves - bossVulnerabilityTime);
    }
    public bool IsBossVulnerable()
    {
        return bossVulnerable;
    }
}
