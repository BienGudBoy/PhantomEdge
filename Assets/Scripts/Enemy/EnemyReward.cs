using UnityEngine;

public class EnemyReward : MonoBehaviour
{
	[Header("Rewards")]
	[SerializeField] private int coinMin = 0;
	[SerializeField] private int coinMax = 0;
	[SerializeField] private int scoreOnDeath = 10;
	
	public int GetCoins()
	{
		if (coinMax <= 0 && coinMin <= 0) return 0;
		int min = Mathf.Max(0, Mathf.Min(coinMin, coinMax));
		int max = Mathf.Max(0, Mathf.Max(coinMin, coinMax));
		return Random.Range(min, max + 1);
	}
	
	public int GetScore()
	{
		return Mathf.Max(0, scoreOnDeath);
	}
}


