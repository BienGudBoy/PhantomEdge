using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
	[System.Serializable]
	public class SpawnWeights
	{
		public GameObject goblinPrefab;
		public GameObject skeletonPrefab;
		[Range(0f, 5f)] public float goblinWeight = 1.0f;
		[Range(0f, 5f)] public float skeletonWeight = 0.4f;
		public int goblinCost = 1;
		public int skeletonCost = 3;
		public int skeletonCap = 8;
	}
	
	[Header("Dynamic Spawn Tuning")]
	[SerializeField] private bool useWeightedSpawns = true;
	[SerializeField] private SpawnWeights weights;
	[SerializeField] private int waveBudgetBase = 20;
	[SerializeField] private float budgetGrowthPerWave = 0.15f; // +15% per wave
	
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
    
    [Header("Boss Spawning")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private int bossSpawnScore = 100;
    [SerializeField] private bool bossSpawned = false;
    private GameObject currentBoss = null;
    
    [Header("Current State")]
    [SerializeField] private int currentWaveIndex = 0;
    [SerializeField] private int enemiesAlive = 0;
    [SerializeField] private bool isSpawning = false;
    
    private List<GameObject> activeEnemies = new List<GameObject>();
	private int skeletonAlive = 0;
    
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
        
        // Subscribe to score changes for boss spawning
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += OnScoreChanged;
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
        
		if (!useWeightedSpawns || weights == null || (weights.goblinPrefab == null && weights.skeletonPrefab == null))
		{
			foreach (EnemySpawn enemySpawn in wave.enemies)
			{
				for (int i = 0; i < enemySpawn.count; i++)
				{
					SpawnEnemy(enemySpawn.enemyPrefab);
					yield return new WaitForSeconds(wave.timeBetweenSpawns);
				}
			}
		}
		else
		{
			// Budget-based weighted spawning
			int budget = Mathf.RoundToInt(waveBudgetBase * Mathf.Pow(1f + budgetGrowthPerWave, waveIndex));
			float wGoblin = Mathf.Max(0f, weights.goblinWeight);
			float wSkeleton = Mathf.Max(0f, weights.skeletonWeight);
			
			while (budget > 0)
			{
				// Build candidate list based on weights and caps
				List<(GameObject prefab, int cost, float weight)> choices = new List<(GameObject, int, float)>();
				if (weights.goblinPrefab != null && weights.goblinCost <= budget)
				{
					choices.Add((weights.goblinPrefab, weights.goblinCost, wGoblin));
				}
				if (weights.skeletonPrefab != null && weights.skeletonCost <= budget && skeletonAlive < weights.skeletonCap)
				{
					choices.Add((weights.skeletonPrefab, weights.skeletonCost, wSkeleton));
				}
				
				if (choices.Count == 0) break;
				
				// Weighted pick
				float totalW = 0f;
				foreach (var c in choices) totalW += c.weight;
				float r = Random.Range(0f, totalW);
				GameObject pick = null;
				int pickCost = 0;
				foreach (var c in choices)
				{
					r -= c.weight;
					if (r <= 0f)
					{
						pick = c.prefab;
						pickCost = c.cost;
						break;
					}
				}
				if (pick == null)
				{
					pick = choices[choices.Count - 1].prefab;
					pickCost = choices[choices.Count - 1].cost;
				}
				
				SpawnEnemy(pick);
				budget -= pickCost;
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
		
		// Track skeletons for cap
		if (enemyPrefab.name.ToLowerInvariant().Contains("skeleton"))
		{
			skeletonAlive++;
		}
        
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
			
			// Decrement skeleton count if needed
			if (enemy != null && enemy.name.ToLowerInvariant().Contains("skeleton"))
			{
				skeletonAlive = Mathf.Max(0, skeletonAlive - 1);
			}
            
            Debug.Log($"Enemy defeated. Remaining: {enemiesAlive}");
            
            // Check if this is the boss that died
            if (enemy == currentBoss)
            {
                Debug.Log("BOSS DEFEATED! Victory achieved!");
                currentBoss = null;
                
                // Notify GameManager about boss death
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.HandleBossDeath();
                }
            }
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
    
    private void OnScoreChanged(int newScore)
    {
        // Spawn boss when score threshold is reached (only once)
        if (!bossSpawned && newScore >= bossSpawnScore && bossPrefab != null)
        {
            SpawnBoss();
        }
    }
    
    private void SpawnBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogWarning("Boss prefab is not assigned!");
            return;
        }
        
        bossSpawned = true;
        Vector3 spawnPosition = GetSpawnPosition();
        
        // Spawn boss
        GameObject boss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
        activeEnemies.Add(boss);
        enemiesAlive++;
        currentBoss = boss; // Track the boss
        
        // Subscribe to boss death
        EnemyHealth bossHealth = boss.GetComponent<EnemyHealth>();
        if (bossHealth != null)
        {
            bossHealth.OnDeath += () => OnEnemyDeath(boss);
        }
        
        Debug.Log($"BOSS SPAWNED: {bossPrefab.name} at {spawnPosition}!");
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
    
    private void OnDestroy()
    {
        // Unsubscribe from score changes
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged -= OnScoreChanged;
        }
    }
}