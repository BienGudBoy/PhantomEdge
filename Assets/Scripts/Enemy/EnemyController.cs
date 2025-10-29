using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float patrolDistance = 5f;
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float attackRange = 1.5f;
    
    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform player;
    
    [Header("Patrol")]
    private Vector2 startPosition;
    private Vector2 patrolTarget;
    private bool isMovingRight = true;
    
    [Header("State")]
    private EnemyState currentState = EnemyState.Idle;
    private float stateTimer = 0f;
    private float idleDuration = 2f;
    
    private enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Hurt,
        Dead
    }
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
        patrolTarget = startPosition;
    }
    
    private void Start()
    {
        // Find player if exists
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        
        currentState = EnemyState.Patrol;
    }
    
    private void Update()
    {
        UpdateState();
    }
    
    private void UpdateState()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdle();
                break;
            case EnemyState.Patrol:
                HandlePatrol();
                break;
            case EnemyState.Chase:
                HandleChase();
                break;
            case EnemyState.Attack:
                HandleAttack();
                break;
            case EnemyState.Hurt:
                HandleHurt();
                break;
            case EnemyState.Dead:
                HandleDead();
                break;
        }
        
        UpdateAnimations();
    }
    
    private void HandleIdle()
    {
        stateTimer += Time.deltaTime;
        
        if (stateTimer >= idleDuration)
        {
            currentState = EnemyState.Patrol;
            stateTimer = 0f;
        }
    }
    
    private void HandlePatrol()
    {
        // Check if player is in range
        if (player != null && Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            currentState = EnemyState.Chase;
            return;
        }
        
        // Move towards patrol target
        float distanceToTarget = Vector2.Distance(transform.position, patrolTarget);
        
        if (distanceToTarget < 0.5f)
        {
            // Reached patrol point, wait
            currentState = EnemyState.Idle;
            stateTimer = 0f;
            
            // Set new patrol target
            SetNewPatrolTarget();
        }
        else
        {
            MoveTowards(patrolTarget, patrolSpeed);
        }
    }
    
    private void HandleChase()
    {
        if (player == null)
        {
            currentState = EnemyState.Patrol;
            return;
        }
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Lost player, go back to patrol
        if (distanceToPlayer > detectionRange * 1.5f)
        {
            currentState = EnemyState.Patrol;
            return;
        }
        
        // Attack if in range
        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
            return;
        }
        
        // Chase player
        MoveTowards(player.position, chaseSpeed);
    }
    
    private void HandleAttack()
    {
        rb.velocity = Vector2.zero;
        
        if (animator != null)
        {
            animator.SetTrigger("IsAttack");
        }
        
        // Reset to chase after attack
        Invoke(nameof(ResetToChase), 1f);
    }
    
    private void HandleHurt()
    {
        rb.velocity = Vector2.zero;
        
        if (animator != null)
        {
            animator.SetTrigger("IsTakehit");
        }
        
        // Return to chase after being hurt
        Invoke(nameof(ResetToChase), 0.5f);
    }
    
    private void HandleDead()
    {
        rb.velocity = Vector2.zero;
        
        if (animator != null)
        {
            animator.SetTrigger("IsDeath");
        }
    }
    
    private void ResetToChase()
    {
        if (currentState == EnemyState.Hurt || currentState == EnemyState.Attack)
        {
            currentState = EnemyState.Chase;
        }
    }
    
    private void MoveTowards(Vector2 target, float speed)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
        
        // Flip sprite based on direction
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction.x < 0;
        }
    }
    
    private void SetNewPatrolTarget()
    {
        if (isMovingRight)
        {
            patrolTarget = startPosition + Vector2.right * patrolDistance;
        }
        else
        {
            patrolTarget = startPosition + Vector2.left * patrolDistance;
        }
        
        isMovingRight = !isMovingRight;
    }
    
    private void UpdateAnimations()
    {
        if (animator == null) return;
        
        bool isMoving = Mathf.Abs(rb.velocity.x) > 0.1f;
        animator.SetBool("IsRun", isMoving && currentState == EnemyState.Chase);
    }
    
    public void TakeDamage()
    {
        if (currentState == EnemyState.Dead) return;
        
        currentState = EnemyState.Hurt;
    }
    
    public void Die()
    {
        if (currentState == EnemyState.Dead) return;
        
        currentState = EnemyState.Dead;
        
        // Disable collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
        
        // Destroy after delay
        Destroy(gameObject, 3f);
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw patrol distance
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * patrolDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * patrolDistance);
    }
}

