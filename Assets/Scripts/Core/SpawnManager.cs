using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName = "Wave 1";
        public List<EnemySpawn> enemies = new List<EnemySpawn>();
        public float timeBetweenSpawns = 2f;
        public float delayBeforeNextWave = 5f;
    }
    
    [System.Serializable]
    public class EnemySpawn
    {
        public GameObject enemyPrefab;
        public int count = 1;
    }
    
    [Header("Wave Settings")]
    [SerializeField] private List<Wave> waves = new List<Wave>();
    [SerializeField] private bool autoStartWaves = true;
    [SerializeField] private bool loopWaves = false;
    
    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private bool randomizeSpawnPoints = true;
    
    [Header("Spawn Behavior")]
    [SerializeField] private float spawnRadius = 0.5f;
    [SerializeField] private LayerMask obstacleLayer;
    
    [Header("Current State")]
    [SerializeField] private int currentWaveIndex = 0;
    [SerializeField] private int enemiesAlive = 0;
    [SerializeField] private bool isSpawning = false;
    
    private List<GameObject> activeEnemies = new List<GameObject>();
    
    public int CurrentWave => currentWaveIndex + 1;
    public int TotalWaves => waves.Count;
    public int EnemiesAlive => enemiesAlive;
    public bool IsSpawning => isSpawning;
    
    private void Start()
    {
        // Auto-populate spawn points if not set
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            GameObject spawnParent = GameObject.Find("SpawnPoints");
            if (spawnParent != null)
            {
                spawnPoints = spawnParent.GetComponentsInChildren<Transform>();
            }
        }
        
        if (autoStartWaves && waves.Count > 0)
        {
            StartCoroutine(StartWavesSequence());
        }
    }
    
    private IEnumerator StartWavesSequence()
    {
        while (currentWaveIndex < waves.Count)
        {
            yield return StartCoroutine(SpawnWave(currentWaveIndex));
            
            // Wait for all enemies to be defeated
            while (enemiesAlive > 0)
            {
                yield return new WaitForSeconds(0.5f);
            }
            
            // Wait before next wave
            if (currentWaveIndex < waves.Count - 1)
            {
                float delay = waves[currentWaveIndex].delayBeforeNextWave;
                Debug.Log($"Wave {currentWaveIndex + 1} complete! Next wave in {delay} seconds...");
                yield return new WaitForSeconds(delay);
            }
            
            currentWaveIndex++;
        }
        
        // All waves complete
        Debug.Log("All waves complete!");
        
        if (loopWaves)
        {
            currentWaveIndex = 0;
            yield return new WaitForSeconds(5f);
            StartCoroutine(StartWavesSequence());
        }
        else
        {
            OnAllWavesComplete();
        }
    }
    
    private IEnumerator SpawnWave(int waveIndex)
    {
        if (waveIndex < 0 || waveIndex >= waves.Count) yield break;
        
        isSpawning = true;
        Wave wave = waves[waveIndex];
        
        Debug.Log($"Starting {wave.waveName}");
        
        foreach (EnemySpawn enemySpawn in wave.enemies)
        {
            for (int i = 0; i < enemySpawn.count; i++)
            {
                SpawnEnemy(enemySpawn.enemyPrefab);
                yield return new WaitForSeconds(wave.timeBetweenSpawns);
            }
        }
        
        isSpawning = false;
    }
    
    private void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Enemy prefab is null!");
            return;
        }
        
        Vector3 spawnPosition = GetSpawnPosition();
        
        // Spawn enemy
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        activeEnemies.Add(enemy);
        enemiesAlive++;
        
        // Subscribe to enemy death
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.OnDeath += () => OnEnemyDeath(enemy);
        }
        
        Debug.Log($"Spawned {enemyPrefab.name} at {spawnPosition}");
    }
    
    private Vector3 GetSpawnPosition()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            // Default spawn at manager position
            return transform.position;
        }
        
        Transform spawnPoint;
        
        if (randomizeSpawnPoints)
        {
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        }
        else
        {
            // Cycle through spawn points
            int index = enemiesAlive % spawnPoints.Length;
            spawnPoint = spawnPoints[index];
        }
        
        // Add random offset within radius
        Vector3 randomOffset = Random.insideUnitCircle * spawnRadius;
        Vector3 position = spawnPoint.position + randomOffset;
        
        return position;
    }
    
    private void OnEnemyDeath(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            enemiesAlive--;
            
            Debug.Log($"Enemy defeated. Remaining: {enemiesAlive}");
        }
    }
    
    public void StartNextWave()
    {
        if (!isSpawning && currentWaveIndex < waves.Count)
        {
            StartCoroutine(SpawnWave(currentWaveIndex));
            currentWaveIndex++;
        }
    }
    
    public void StopSpawning()
    {
        StopAllCoroutines();
        isSpawning = false;
    }
    
    public void ClearAllEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        
        activeEnemies.Clear();
        enemiesAlive = 0;
    }
    
    private void OnAllWavesComplete()
    {
        Debug.Log("All waves defeated! Level complete!");
        
        // Notify GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(1000); // Bonus score
            // Could trigger level complete/victory here
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw spawn points
        if (spawnPoints != null)
        {
            Gizmos.color = Color.red;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, spawnRadius);
                }
            }
        }
    }
}