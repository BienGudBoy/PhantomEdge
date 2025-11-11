using UnityEngine;
using System;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    
    [Header("Visual Feedback")]
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private int flashCount = 2;
    
    [Header("Components")]
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    // Events
    public event Action<int, int> OnHealthChanged; // current, max
    public event Action OnDeath;
    public event Action<int> OnDamageTaken; // damage amount
    
    private bool isDead = false;
    private bool isFlashing = false;
	private float invulnerableUntil = 0f;
    
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => isDead;
    public float HealthPercentage => (float)currentHealth / maxHealth;
	public bool IsInvulnerable => Time.time < invulnerableUntil;
	public float RemainingInvulnerability => Mathf.Max(0f, invulnerableUntil - Time.time);
    
    private void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }
    
    private void Start()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
	public void TakeDamage(int damage)
    {
		if (isDead || damage <= 0) return;
		if (IsInvulnerable) return;
        
        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnDamageTaken?.Invoke(damage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // Start damage flash effect
        if (!isFlashing && spriteRenderer != null)
        {
            StartCoroutine(FlashDamage());
        }
        
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }
    
    public void Heal(int amount)
    {
        if (isDead || amount <= 0) return;
        
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
	
	public void GrantInvulnerability(float duration)
	{
		if (duration <= 0f) return;
		float newExpiry = Time.time + duration;
		if (newExpiry > invulnerableUntil)
		{
			invulnerableUntil = newExpiry;
		}
	}
    
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    // Increase max health without restoring current health to full
    public void IncreaseMaxHealth(int amount)
    {
        if (amount <= 0) return;
        maxHealth += amount;
        // Keep current health the same (don't restore to full)
        currentHealth = Mathf.Clamp(currentHealth, 1, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void SetHealthState(int newCurrentHealth, int newMaxHealth)
    {
        maxHealth = Mathf.Max(1, newMaxHealth);
        currentHealth = Mathf.Clamp(newCurrentHealth, 1, maxHealth);
        isDead = false;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void RefreshHealthEvent()
    {
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
		invulnerableUntil = 0f;
        
        // Stop any ongoing flash effect
        StopAllCoroutines();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        
        OnDeath?.Invoke();
        
        // Handle death animation
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            // IsDeath is a Bool parameter, not a Trigger
            animator.SetBool("IsDeath", true);
        }
        
        // Disable player controller
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = false;
        }
        
        // Disable physics
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }
        
        // Disable all colliders to prevent enemies from detecting/attacking the dead player
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            if (col != null)
            {
                col.enabled = false;
            }
        }
        
        // Also disable colliders in children (e.g., attack points, hurtboxes)
        Collider2D[] childColliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in childColliders)
        {
            if (col != null && col.gameObject != gameObject)
            {
                col.enabled = false;
            }
        }
    }
    
    public void Revive(int healthAmount)
    {
        if (!isDead) return;
        
        isDead = false;
		invulnerableUntil = 0f;
        currentHealth = healthAmount;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        // Re-enable player controller
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = true;
        }
        
        // Re-enable physics
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
        
        // Re-enable all colliders
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            if (col != null)
            {
                col.enabled = true;
            }
        }
        
        // Re-enable colliders in children
        Collider2D[] childColliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in childColliders)
        {
            if (col != null && col.gameObject != gameObject)
            {
                col.enabled = true;
            }
        }
        
        // Reset death animation
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            // Re-enable animator if it was disabled
            if (!animator.enabled)
            {
                animator.enabled = true;
            }
            // IsDeath is a Bool parameter, not a Trigger
            animator.SetBool("IsDeath", false);
        }
        
        // Reset sprite color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
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
}




