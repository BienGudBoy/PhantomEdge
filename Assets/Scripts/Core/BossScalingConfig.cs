using UnityEngine;

[CreateAssetMenu(fileName = "BossScalingConfig", menuName = "Configs/Boss Scaling Config")]
public class BossScalingConfig : ScriptableObject
{
	[Header("Base Values")]
	public int baseMaxHealth = 500;
	public int baseAttackDamage = 20;
	
	[Header("Round Scaling")]
	[Tooltip("Additional multiplier per round index: final = (1 + roundIndex * roundScale)")]
	[Range(0f, 1f)] public float roundScale = 0.15f;
	[Range(1f, 10f)] public float roundMaxMultiplier = 3f;
	
	[Header("Player Power Scaling")]
	[Tooltip("Weight applied to normalized player power score")]
	[Range(0f, 2f)] public float playerPowerWeight = 0.5f;
	[Tooltip("Cap for player power contribution (as a fraction)")]
	[Range(0f, 1f)] public float playerPowerCap = 0.5f;
	[Tooltip("Baseline values to normalize player power")]
	public int baselinePlayerDamage = 13;
	public int baselinePlayerMaxHealth = 100;
	
	[Header("Farming Time Scaling")]
	[Tooltip("Extra per-minute scaling for lingering in Scene1")]
	[Range(0f, 0.2f)] public float minutesScale = 0.03f;
	[Tooltip("Cap for minutes-based contribution (as a fraction)")]
	[Range(0f, 1f)] public float minutesCap = 0.3f;
	
	[Header("Final Caps")]
	[Range(1f, 10f)] public float hpMaxMultiplier = 4f;
	[Range(1f, 5f)] public float damageMaxMultiplier = 2f;
}


