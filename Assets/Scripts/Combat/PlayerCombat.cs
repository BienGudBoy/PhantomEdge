using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackDuration = 0.85f; // Match animation length
    [SerializeField] private float damageDelay = 0.3f; // When in the animation to deal damage (seconds)
    [SerializeField] private Transform attackPoint;
    
    [Header("Components")]
    private Animator animator;
    private HealthSystem healthSystem;
    
    [Header("State")]
    private float lastAttackTime = 0f;
    private LayerMask enemyLayer;
    private bool isAttacking = false;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
        
        enemyLayer = LayerMask.GetMask("Enemy");
        Debug.Log($"PlayerCombat initialized. Enemy layer mask value: {enemyLayer.value}");
        
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
        if (Time.time < lastAttackTime + attackDuration) return;
        if (isAttacking) return; // Prevent attacking while already attacking
        
        lastAttackTime = Time.time;
        isAttacking = true;
        
        Debug.Log($"Attack started at time: {Time.time}, duration: {attackDuration}");
        
        // Notify PlayerController to stop movement animations
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.IsAttacking = true;
        }
        
        // Trigger attack animation
        if (animator != null)
        {
            // First reset ALL other animation parameters
            animator.SetBool("IsRun", false);
            animator.SetBool("IsWalk", false);
            animator.SetBool("IsJump", false);
            
            // Use SetBool instead of Trigger for better control
            // But first check if parameter exists as bool or trigger
            bool isAttackParamExists = false;
            foreach (var param in animator.parameters)
            {
                if (param.name == "IsAttack")
                {
                    isAttackParamExists = true;
                    // If it's a trigger, use trigger, otherwise use bool
                    if (param.type == AnimatorControllerParameterType.Trigger)
                    {
                        animator.ResetTrigger("IsAttack");
                        animator.SetTrigger("IsAttack");
                    }
                    else if (param.type == AnimatorControllerParameterType.Bool)
                    {
                        animator.SetBool("IsAttack", true);
                    }
                    break;
                }
            }
            Debug.Log($"Attack animation triggered. IsAttack param exists: {isAttackParamExists}");
        }
        
        // Delay damage to match the animation hit frame
        Invoke(nameof(DealDamage), damageDelay);
        
        // Reset attack state after animation completes
        Invoke(nameof(ResetAttack), attackDuration);
    }
    
    private void DealDamage()
    {
        // Play attack sound at hit moment
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDamageSound();
        }
        
        // Detect enemies in range at the moment of impact
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        
        Debug.Log($"Attack detected {hitEnemies.Length} enemies in range. Enemy layer mask: {enemyLayer.value}");
        
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log($"Hit enemy: {enemy.gameObject.name} on layer {LayerMask.LayerToName(enemy.gameObject.layer)}");
            
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
                Debug.Log($"Dealt {attackDamage} damage to {enemy.gameObject.name}");
            }
        }
    }
    
    private void ResetAttack()
    {
        isAttacking = false;
        Debug.Log("ResetAttack called - attack should be done now");
        
        // Reset the IsAttack parameter
        if (animator != null)
        {
            // Reset both trigger and bool versions
            animator.ResetTrigger("IsAttack");
            animator.SetBool("IsAttack", false);
            Debug.Log("IsAttack parameter reset to false");
        }
        
        // Notify PlayerController that attack is done
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.IsAttacking = false;
            Debug.Log("PlayerController.IsAttacking set to false");
        }
    }
    
    public void OnAttackInput()
    {
        // Don't attack if player is dead
        if (healthSystem != null && healthSystem.IsDead)
        {
            return;
        }
        
        // Don't attack if game is paused
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameManager.GameState.Paused)
        {
            return;
        }
        
        Attack();
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




