using UnityEngine;

public class BossEncounterController : MonoBehaviour
{
	[SerializeField] private float engageRange = 10f;
	[SerializeField] private string bossDisplayName = "Boss";
	
	private EnemyHealth bossHealth;
	private Transform player;
	private bool engaged = false;
	private BossHPBarController hpBar;
	private GameObject coinUIRoot;
	
	private void Start()
	{
		bossHealth = GetComponent<EnemyHealth>();
		if (bossHealth == null)
		{
			Debug.LogWarning("BossEncounterController: EnemyHealth not found on boss.");
			enabled = false;
			return;
		}
		
		var playerObj = GameObject.FindGameObjectWithTag("Player");
		player = playerObj != null ? playerObj.transform : null;
		
		// Find HP bar controller in scene
		hpBar = FindFirstObjectByType<BossHPBarController>(FindObjectsInactive.Include);
		if (hpBar == null)
		{
			Debug.LogWarning("BossEncounterController: BossHPBarController not found in scene.");
		}
		
		// Find coin UI root
		var coinUI = FindFirstObjectByType<CoinUI>(FindObjectsInactive.Include);
		if (coinUI != null)
		{
			coinUIRoot = coinUI.gameObject;
		}
		
		// On death: hide HP bar and show coins again
		bossHealth.OnDeath += OnBossDeath;
	}
	
	private void Update()
	{
		if (engaged || player == null) return;
		
		float dist = Vector2.Distance(transform.position, player.position);
		if (dist <= engageRange)
		{
			engaged = true;
			
			// Hide coin UI
			if (coinUIRoot != null)
			{
				coinUIRoot.SetActive(false);
			}
			
			// Show and bind HP bar
			if (hpBar != null)
			{
				hpBar.Bind(bossHealth, bossDisplayName);
				hpBar.Show();
			}
		}
	}
	
	private void OnBossDeath()
	{
		// Restore coin UI
		if (coinUIRoot != null)
		{
			coinUIRoot.SetActive(true);
		}
		
		// Hide HP bar
		if (hpBar != null)
		{
			hpBar.Hide();
			hpBar.Unbind();
		}
	}
}


