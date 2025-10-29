using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int currentHealth;
    
    [Header("Components")]
    private EnemyController enemyController;
    
    // Events
    public event Action OnDeath;
    public event Action<int, int> OnHealthChanged; // current, max
    
    private bool isDead = false;
    
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => isDead;
    
    private void Awake()
    {
        currentHealth = maxHealth;
        enemyController = GetComponent<EnemyController>();
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0) return;
        
        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
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
    
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
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
        
        // Add score (if GameManager exists)
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddScore(10); // 10 points per enemy
        }
        
        // Destroy after delay
        Destroy(gameObject, 3f);
    }
}




