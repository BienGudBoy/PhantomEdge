using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private bool destroyOnHit = true;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            // Deal damage to target
            if (targetTag == "Player")
            {
                HealthSystem health = other.GetComponent<HealthSystem>();
                if (health != null)
                {
                    health.TakeDamage(damageAmount);
                    
                    // Play damage sound
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayDamageSound();
                    }
                }
            }
            else if (targetTag == "Enemy")
            {
                EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damageAmount);
                }
            }
            
            // Destroy projectile if specified
            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}




