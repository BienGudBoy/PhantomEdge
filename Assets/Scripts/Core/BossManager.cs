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
				// assign via reflection of private fields
				var cfgField = typeof(BossScaler).GetField("scalingConfig", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				cfgField?.SetValue(scaler, cfg);
				var baseHpField = typeof(BossScaler).GetField("overrideBaseMaxHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				baseHpField?.SetValue(scaler, mushroomBaseHP);
				var baseDmgField = typeof(BossScaler).GetField("overrideBaseAttackDamage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				baseDmgField?.SetValue(scaler, mushroomBaseDamage);
				scaler.SetRoundIndex(roundIndex);
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
				var cfgField = typeof(BossScaler).GetField("scalingConfig", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				cfgField?.SetValue(scaler, cfg);
				var baseHpField = typeof(BossScaler).GetField("overrideBaseMaxHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				baseHpField?.SetValue(scaler, swordBaseHP);
				var baseDmgField = typeof(BossScaler).GetField("overrideBaseAttackDamage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				baseDmgField?.SetValue(scaler, swordBaseDamage);
				scaler.SetRoundIndex(roundIndex);
				scaler.ApplyScaling();
			}
		}
	}
	
	private void OnBossDefeated()
	{
		switch (bossToSpawn)
		{
			case BossType.Mushroom:
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


