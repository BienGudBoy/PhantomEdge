using UnityEngine;
using UnityEngine.SceneManagement;

public class BossManager : MonoBehaviour
{
	public enum BossType
	{
		Mushroom,
		Sword
	}
	
	[Header("Boss Selection")]
	[SerializeField] private BossType bossToSpawn = BossType.Mushroom;
	[SerializeField] private GameObject mushroomBossPrefab;
	[SerializeField] private GameObject swordBossPrefab;
	[SerializeField] private Transform bossSpawnPoint;
	[SerializeField] private BossScalingConfig mushroomScaling;
	[SerializeField] private BossScalingConfig swordScaling;
	[SerializeField] private int mushroomBaseHP = 600;
	[SerializeField] private int mushroomBaseDamage = 18;
	[SerializeField] private int swordBaseHP = 1200;
	[SerializeField] private int swordBaseDamage = 25;
	[SerializeField] private int roundIndex = 0;
	
	private GameObject currentBoss;
	
	private void Start()
	{
		if (GameManager.Instance != null)
		{
			bossToSpawn = GameManager.Instance.GetNextBossType();
			roundIndex = GameManager.Instance.GetRoundIndexForBoss(bossToSpawn);
		}
		
		SpawnSelectedBoss();
	}
	
	private void SpawnSelectedBoss()
	{
		GameObject prefab = null;
		switch (bossToSpawn)
		{
			case BossType.Mushroom:
				prefab = mushroomBossPrefab;
				break;
			case BossType.Sword:
				prefab = swordBossPrefab;
				break;
		}
		
		if (prefab == null)
		{
			Debug.LogWarning("BossManager: Boss prefab not assigned!");
			return;
		}
		
		Vector3 spawnPos = bossSpawnPoint != null ? bossSpawnPoint.position : transform.position;
		currentBoss = Instantiate(prefab, spawnPos, Quaternion.identity);
		
		var enemyHealth = currentBoss.GetComponent<EnemyHealth>();
		if (enemyHealth != null)
		{
			enemyHealth.OnDeath += OnBossDefeated;
		}
		
		// Setup encounter controller for UI toggles (HP bar and coins)
		var encounter = currentBoss.GetComponent<BossEncounterController>();
		if (encounter == null)
		{
			encounter = currentBoss.AddComponent<BossEncounterController>();
		}
		// Name
		string displayName = prefab != null ? prefab.name : bossToSpawn.ToString();
		var nameField = typeof(BossEncounterController).GetField("bossDisplayName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		nameField?.SetValue(encounter, displayName);
		
		// Apply scaling
		var scaler = currentBoss.GetComponent<BossScaler>();
		if (scaler == null)
		{
			scaler = currentBoss.AddComponent<BossScaler>();
		}
		if (bossToSpawn == BossType.Mushroom)
		{
			var cfg = mushroomScaling;
			if (cfg == null)
			{
				Debug.LogWarning("BossManager: Mushroom scaling config not assigned.");
			}
			else
			{
				scaler.Configure(cfg, mushroomBaseHP, mushroomBaseDamage, roundIndex);
				scaler.ApplyScaling();
			}
		}
		else
		{
			var cfg = swordScaling;
			if (cfg == null)
			{
				Debug.LogWarning("BossManager: Sword scaling config not assigned.");
			}
			else
			{
				scaler.Configure(cfg, swordBaseHP, swordBaseDamage, roundIndex);
				scaler.ApplyScaling();
			}
		}
	}
	
	private void OnBossDefeated()
	{
		switch (bossToSpawn)
		{
			case BossType.Mushroom:
				if (GameManager.Instance != null)
				{
					GameManager.Instance.RegisterBossVictory(BossType.Mushroom);
				}
				
				// Show VictoryScreen first, then transition via GameManager.NextLevel to Scene1
				if (GameManager.Instance != null)
				{
					GameManager.Instance.HandleBossDeath();
				}
				else
				{
					Debug.Log("Boss defeated. Victory screen sequence expected, but GameManager not found.");
				}
				break;
				
			case BossType.Sword:
				// Final victory
				if (GameManager.Instance != null)
				{
					GameManager.Instance.FinalVictoryPending = true;
					GameManager.Instance.RegisterBossVictory(BossType.Sword);
					GameManager.Instance.HandleBossDeath();
				}
				else
				{
					Debug.Log("Final victory achieved.");
				}
				break;
		}
	}
}


