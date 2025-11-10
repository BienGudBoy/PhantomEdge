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
    
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => isDead;
    public float HealthPercentage => (float)currentHealth / maxHealth;
    
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
    
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
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
    }
    
    public void Revive(int healthAmount)
    {
        if (!isDead) return;
        
        isDead = false;
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
        
        // Reset death animation
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
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




