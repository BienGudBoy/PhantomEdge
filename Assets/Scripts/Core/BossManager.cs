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
	}
	
	private void OnBossDefeated()
	{
		switch (bossToSpawn)
		{
			case BossType.Mushroom:
				// Reward money for next round, then return to Scene1
				if (GameManager.Instance != null)
				{
					// Optional: give coin reward here or let EnemyReward handle it
					GameManager.Instance.LoadScene("Scene1");
				}
				else
				{
					SceneManager.LoadScene("Scene1");
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


