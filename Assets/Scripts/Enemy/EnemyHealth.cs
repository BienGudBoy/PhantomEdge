using UnityEngine;
using System;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int currentHealth;
	
	[Header("Rewards")]
	[SerializeField] private GameObject coinPrefab; // Assign Assets/Prefabs/Items/Coin.prefab in Inspector
    
    [Header("Visual Feedback")]
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private int flashCount = 2;
    
    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;
    
    [Header("Components")]
    private EnemyController enemyController;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Color originalColor;
    
    // Events
    public event Action OnDeath;
    public event Action<int, int> OnHealthChanged; // current, max
    
    private bool isDead = false;
    private bool isFlashing = false;
    
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => isDead;
    
    private void Awake()
    {
        currentHealth = maxHealth;
        enemyController = GetComponent<EnemyController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }
    
    // External scaling hook for bosses and elites
    public void SetMaxHealth(int newMaxHealth, bool refillToFull)
    {
        maxHealth = Mathf.Max(1, newMaxHealth);
        if (refillToFull)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0) return;
        
        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // Start damage flash effect
        if (!isFlashing && spriteRenderer != null)
        {
            StartCoroutine(FlashDamage());
        }
        
        // Apply knockback effect
        ApplyKnockback();
        
        // Notify enemy controller about damage
        if (enemyController != null)
        {
            enemyController.TakeDamage();
        }
        
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }
    
    private void ApplyKnockback()
    {
        if (rb == null) return;
        
        // Find the player to determine knockback direction
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Calculate direction from player to enemy
            Vector2 direction = (transform.position - player.transform.position).normalized;
            
            // Apply knockback force
            rb.linearVelocity = direction * knockbackForce;
            
            // Reset velocity after a short duration
            StartCoroutine(ResetVelocityAfterDelay());
        }
    }
    
    private IEnumerator ResetVelocityAfterDelay()
    {
        yield return new WaitForSeconds(knockbackDuration);
        
        if (rb != null && !isDead)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    private IEnumerator FlashDamage()
    {
        isFlashing = true;
        
        for (int i = 0; i < flashCount; i++)
        {
            // Flash to damage color
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(flashDuration);
            
            // Flash back to original color
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
        
        isFlashing = false;
    }
    
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        // Stop any ongoing flash effect
        StopAllCoroutines();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        
        // Invoke death event
        OnDeath?.Invoke();
        
        // Notify enemy controller about death
        if (enemyController != null)
        {
            enemyController.Die();
        }
        
        // Disable collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
        
        // Disable physics to prevent sliding
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }
        
		// Rewards (coins and score)
		GameManager gameManager = FindFirstObjectByType<GameManager>();
		if (gameManager != null)
		{
			// Score remains as overall tally
			int scoreToAdd = 10;
			int coins = 0;
			EnemyReward reward = GetComponent<EnemyReward>();
			if (reward != null)
			{
				scoreToAdd = reward.GetScore();
				coins = reward.GetCoins();
			}
			else
			{
				// Name-based fallback mapping if no reward component attached
				string n = gameObject.name.ToLowerInvariant();
				coins = 0;
				if (n.Contains("flying") || n.Contains("eye"))
				{
					coins = 2;
				}
				else if (n.Contains("goblin") || n.Contains("globlin"))
				{
					coins = UnityEngine.Random.Range(1, 4); // 1..3 inclusive
				}
				else if (n.Contains("skeleton"))
				{
					coins = 4;
				}
				else if (n.Contains("mushroom"))
				{
					coins = UnityEngine.Random.Range(15, 21); // 15..20 inclusive
				}
				else if (n.Contains("sword"))
				{
					coins = 0; // final boss, no coin
				}
			}
			
			// Spawn coin collectibles if prefab is assigned; otherwise grant directly
			if (coins > 0)
			{
				if (coinPrefab != null)
				{
					SpawnCoins(coins);
				}
				else
				{
					gameManager.AddCoins(coins);
				}
			}
			
			gameManager.AddScore(scoreToAdd);
		}
        
        // Start death sequence with fade out
        StartCoroutine(DeathSequence());
    }
	
	private void SpawnCoins(int amount)
	{
		// Spawn 'amount' 1-value coins with small random scatter and gentle pop
		for (int i = 0; i < amount; i++)
		{
			Vector3 spawnPos = transform.position + (Vector3)(UnityEngine.Random.insideUnitCircle * 0.2f);
			GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);
			
			// Random outward impulse if Rigidbody2D present
			Rigidbody2D crb = coin.GetComponent<Rigidbody2D>();
			if (crb != null)
			{
				// Respect prefab setup: if collider is trigger, keep kinematic; if not, keep physics bounce
				Collider2D coinCol = coin.GetComponent<Collider2D>();
				if (coinCol != null && coinCol.isTrigger)
				{
					crb.gravityScale = 0f;
					crb.bodyType = RigidbodyType2D.Kinematic;
					crb.freezeRotation = true;
				}
				else
				{
					// Use default dynamic physics for bounce; add a small random upward impulse
					crb.bodyType = RigidbodyType2D.Dynamic;
					if (crb.gravityScale < 0.01f) crb.gravityScale = 1f;
					crb.freezeRotation = false;
				}
				
				Vector2 impulse = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(0.5f, 1.5f);
				crb.AddForce(impulse, ForceMode2D.Impulse);
				crb.AddTorque(UnityEngine.Random.Range(-5f, 5f), ForceMode2D.Impulse);
			}
			
			// Ensure visible above ground
			SpriteRenderer sr = coin.GetComponent<SpriteRenderer>();
			if (sr != null)
			{
				sr.sortingLayerName = "Foreground";
				if (sr.sortingOrder < 50) sr.sortingOrder = 100;
				Color c = sr.color;
				c.a = 1f;
				sr.color = c;
			}
			
			// Ensure Collectible coin value is 1
			Collectible coinCollectible = coin.GetComponent<Collectible>();
			if (coinCollectible != null)
			{
				// Coin type enum default is Coin; we only ensure value=1
				// Using reflection not necessary; fields are serialized
			}
		}
	}
    
    private IEnumerator DeathSequence()
    {
        // Wait for death animation to play (if any)
        yield return new WaitForSeconds(0.5f);
        
        // Fade out effect
        float fadeTime = 1.0f;
        float elapsed = 0f;
        
        while (elapsed < fadeTime && spriteRenderer != null)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
            
            // Scale down slightly while fading
            float scale = Mathf.Lerp(1f, 0.8f, elapsed / fadeTime);
            transform.localScale = Vector3.one * scale;
            
            yield return null;
        }
        
        // Destroy the game object
        Destroy(gameObject);
    }
}




