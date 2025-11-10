using UnityEngine;

public class DeathHole : MonoBehaviour
{
    [Header("Death Settings")]
    [SerializeField] private int instantKillDamage = 9999;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it's a player
        if (other.CompareTag("Player"))
        {
            HealthSystem health = other.GetComponent<HealthSystem>();
            if (health != null && !health.IsDead)
            {
                // Deal massive damage to instantly kill the player
                health.TakeDamage(instantKillDamage);
                Debug.Log($"Player fell into hole and died!");
            }
        }
        // Check if it's an enemy
        else if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null && !enemyHealth.IsDead)
            {
                // Deal massive damage to instantly kill the enemy
                enemyHealth.TakeDamage(instantKillDamage);
                Debug.Log($"Enemy fell into hole and died!");
            }
        }
    }
}

