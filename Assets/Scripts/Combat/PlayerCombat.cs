using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private Transform attackPoint;
    
    [Header("Components")]
    private Animator animator;
    private HealthSystem healthSystem;
    
    [Header("State")]
    private float lastAttackTime = 0f;
    private LayerMask enemyLayer;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
        
        enemyLayer = LayerMask.GetMask("Enemy");
        
        if (attackPoint == null)
        {
            GameObject attackObj = new GameObject("AttackPoint");
            attackObj.transform.SetParent(transform);
            attackObj.transform.localPosition = new Vector2(0.5f, 0f);
            attackPoint = attackObj.transform;
        }
    }
    
    public void Attack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        
        lastAttackTime = Time.time;
        
        // Trigger attack animation
        if (animator != null)
        {
            animator.SetTrigger("IsAttack");
        }
        
        // Play attack sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDamageSound();
        }
        
        // Detect enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }
    
    public void OnAttackInput()
    {
        if (healthSystem != null && !healthSystem.IsDead)
        {
            Attack();
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}




