using UnityEngine;

public class BossScaler : MonoBehaviour
{
	[Header("Config")]
	[SerializeField] private BossScalingConfig scalingConfig;
	[SerializeField] private int roundIndex = 0;
	
	[Header("Overrides (optional)")]
	[SerializeField] private int overrideBaseMaxHealth = -1;
	[SerializeField] private int overrideBaseAttackDamage = -1;
	
	private void Start()
	{
		ApplyScaling();
	}
	
	public void Configure(BossScalingConfig config, int baseMaxHealth, int baseAttackDamage, int round)
	{
		scalingConfig = config;
		overrideBaseMaxHealth = baseMaxHealth;
		overrideBaseAttackDamage = baseAttackDamage;
		roundIndex = Mathf.Max(0, round);
	}
	
	public void SetRoundIndex(int index)
	{
		roundIndex = Mathf.Max(0, index);
	}
	
	public void ApplyScaling()
	{
		if (scalingConfig == null)
		{
			Debug.LogWarning("BossScaler: ScalingConfig not assigned.");
			return;
		}
		
		// Fetch base stats
		int baseHP = overrideBaseMaxHealth > 0 ? overrideBaseMaxHealth : scalingConfig.baseMaxHealth;
		int baseDmg = overrideBaseAttackDamage > 0 ? overrideBaseAttackDamage : scalingConfig.baseAttackDamage;
		
		// Round scaling
		float roundMul = 1f + (roundIndex * scalingConfig.roundScale);
		roundMul = Mathf.Min(roundMul, scalingConfig.roundMaxMultiplier);
		
		// Player power score
		float playerPowerScore = ComputePlayerPowerScore(scalingConfig.baselinePlayerDamage, scalingConfig.baselinePlayerMaxHealth);
		float playerFactor = 1f + Mathf.Min(playerPowerScore * scalingConfig.playerPowerWeight, scalingConfig.playerPowerCap);
		
		// Time-based scaling
		float minutes = 0f;
		if (GameManager.Instance != null)
		{
			minutes = GameManager.Instance.GetLastFarmingMinutes();
		}
		float timeFactor = 1f + Mathf.Min(minutes * scalingConfig.minutesScale, scalingConfig.minutesCap);
		
		// Final multipliers
		float hpMul = Mathf.Min(roundMul * playerFactor * timeFactor, scalingConfig.hpMaxMultiplier);
		float dmgMul = Mathf.Min((1f + (roundIndex * scalingConfig.roundScale * 0.5f)) * (1f + Mathf.Min(playerPowerScore * (scalingConfig.playerPowerWeight * 0.5f), scalingConfig.playerPowerCap * 0.5f)), scalingConfig.damageMaxMultiplier);
		
		// Apply to components
		var health = GetComponent<EnemyHealth>();
		if (health != null)
		{
			int newMax = Mathf.RoundToInt(baseHP * hpMul);
			health.SetMaxHealth(newMax, true);
		}
		
		// Try grounded enemy
		var enemy = GetComponent<EnemyController>();
		if (enemy != null)
		{
			ScaleEnemyDamage(enemy, baseDmg, dmgMul);
		}
		else
		{
			// Try flying enemy
			var flying = GetComponent<FlyingEnemyController>();
			if (flying != null)
			{
				ScaleFlyingDamage(flying, baseDmg, dmgMul);
			}
		}
	}
	
	private float ComputePlayerPowerScore(int baselineDmg, int baselineHP)
	{
		int dmg = baselineDmg;
		int hp = baselineHP;
		
		// Fetch player components if available
		var player = GameObject.FindGameObjectWithTag("Player");
		if (player != null)
		{
			var pc = player.GetComponent<PlayerCombat>();
			if (pc != null)
			{
				dmg = pc.GetAttackDamage();
			}
			var hs = player.GetComponent<HealthSystem>();
			if (hs != null)
			{
				hp = hs.MaxHealth;
			}
		}
		
		float dmgScore = Mathf.Clamp01((float)dmg / Mathf.Max(1, baselineDmg));
		float hpScore = Mathf.Clamp01((float)hp / Mathf.Max(1, baselineHP));
		
		// Average for simplicity
		return (dmgScore + hpScore) * 0.5f;
	}
	
	private void ScaleEnemyDamage(EnemyController enemy, int baseDmg, float mul)
	{
		// EnemyController stores serialized attackDamage; set via reflection of fields exposed methods
		var atkField = typeof(EnemyController).GetField("attackDamage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		if (atkField != null)
		{
			int scaled = Mathf.Max(1, Mathf.RoundToInt(baseDmg * mul));
			atkField.SetValue(enemy, scaled);
		}
	}
	
	private void ScaleFlyingDamage(FlyingEnemyController enemy, int baseDmg, float mul)
	{
		var atkField = typeof(FlyingEnemyController).GetField("attackDamage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		if (atkField != null)
		{
			int scaled = Mathf.Max(1, Mathf.RoundToInt(baseDmg * mul));
			atkField.SetValue(enemy, scaled);
		}
	}
}


