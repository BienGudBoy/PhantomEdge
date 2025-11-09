using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float patrolDistance = 5f;
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float attackRange = 1.5f;
    
    [Header("Combat Settings")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackDamageDelay = 0.4f; // When in the attack animation to deal damage
    
    [Header("Night Buff Settings")]
    [SerializeField] private float nightSpeedMultiplier = 1.5f; // Speed multiplier at night (1.5 = 50% faster)
    [SerializeField] private float nightDamageMultiplier = 1.5f; // Damage multiplier at night (1.5 = 50% more damage)
    [SerializeField] private float nightDetectionRangeMultiplier = 1.2f; // Detection range multiplier at night
    [SerializeField] private float nightAttackCooldownMultiplier = 0.8f; // Attack cooldown multiplier (0.8 = 20% faster attacks)
    
    // Base values (stored to restore when day comes)
    private float basePatrolSpeed;
    private float baseChaseSpeed;
    private int baseAttackDamage;
    private float baseAttackCooldown;
    private float baseDetectionRange;
    
    // Current buffed values
    private float currentPatrolSpeed;
    private float currentChaseSpeed;
    private int currentAttackDamage;
    private float currentAttackCooldown;
    private float currentDetectionRange;
    
    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform player;
    private DayNightCycle dayNightCycle;
    
    [Header("Patrol")]
    private Vector2 startPosition;
    private Vector2 patrolTarget;
    private bool isMovingRight = true;
    
    [Header("State")]
    private EnemyState currentState = EnemyState.Idle;
    private float stateTimer = 0f;
    private float idleDuration = 2f;
    private float lastAttackTime = 0f;
    
    // Cache animator parameter hashes
    private int isRunHash;
    private int isAttackHash;
    private int isTakehitHash;
    private int isDeathHash;
    private bool hasAnimator;
    private bool hasRunParameter;
    private bool hasAttackParameter;
    private bool hasTakehitParameter;
    private bool hasDeathParameter;
    
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
        
        if (rb == null) Debug.LogError($"{gameObject.name}: Rigidbody2D not found!");
        
        startPosition = transform.position;
        // Initialize patrol target to the right
        patrolTarget = startPosition + Vector2.right * patrolDistance;
        
        // Store base values
        basePatrolSpeed = patrolSpeed;
        baseChaseSpeed = chaseSpeed;
        baseAttackDamage = attackDamage;
        baseAttackCooldown = attackCooldown;
        baseDetectionRange = detectionRange;
        
        // Initialize current values to base
        currentPatrolSpeed = basePatrolSpeed;
        currentChaseSpeed = baseChaseSpeed;
        currentAttackDamage = baseAttackDamage;
        currentAttackCooldown = baseAttackCooldown;
        currentDetectionRange = baseDetectionRange;
        
        // Cache animator parameter hashes
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            hasAnimator = true;
            
            // Try different variations of run/walk parameter names
            string[] runParams = { "IsRun", "IsRunning", "IsWalk", "IsFlight" };
            foreach (string param in runParams)
            {
                if (HasParameter(param))
                {
                    isRunHash = Animator.StringToHash(param);
                    hasRunParameter = true;
                    Debug.Log($"{gameObject.name}: Found run parameter '{param}'");
                    break;
                }
            }
            
            // Check for attack parameter
            if (HasParameter("IsAttack"))
            {
                isAttackHash = Animator.StringToHash("IsAttack");
                hasAttackParameter = true;
                Debug.Log($"{gameObject.name}: Found attack parameter 'IsAttack'");
            }
            
            // Check for takehit parameter
            if (HasParameter("IsTakehit"))
            {
                isTakehitHash = Animator.StringToHash("IsTakehit");
                hasTakehitParameter = true;
                Debug.Log($"{gameObject.name}: Found takehit parameter 'IsTakehit'");
            }
            
            // Check for death parameter
            if (HasParameter("IsDeath"))
            {
                isDeathHash = Animator.StringToHash("IsDeath");
                hasDeathParameter = true;
                Debug.Log($"{gameObject.name}: Found death parameter 'IsDeath'");
            }
            
            if (!hasRunParameter && !hasAttackParameter)
            {
                Debug.LogWarning($"{gameObject.name}: No animation parameters found. Check animator controller.");
            }
        }
        else
        {
            hasAnimator = false;
            Debug.LogWarning($"Animator or RuntimeAnimatorController not found on {gameObject.name}. Animations will not work.");
        }
    }
    
    private bool HasParameter(string parameterName)
    {
        if (animator == null) return false;
        
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == parameterName)
            {
                return true;
            }
        }
        return false;
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
        
        // Subscribe to day/night cycle events
        dayNightCycle = FindObjectOfType<DayNightCycle>();
        if (dayNightCycle != null)
        {
            dayNightCycle.OnDayNightChanged += OnDayNightChanged;
            
            // Apply initial state (if already night)
            if (!dayNightCycle.IsDay)
            {
                ApplyNightBuff();
            }
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: DayNightCycle not found! Night buffs will not work.");
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (dayNightCycle != null)
        {
            dayNightCycle.OnDayNightChanged -= OnDayNightChanged;
        }
    }
    
    private void OnDayNightChanged(bool isDay)
    {
        if (isDay)
        {
            RemoveNightBuff();
            Debug.Log($"{gameObject.name}: Night buff removed");
        }
        else
        {
            ApplyNightBuff();
            Debug.Log($"{gameObject.name}: Night buff applied");
        }
    }
    
    private void ApplyNightBuff()
    {
        // Apply night multipliers
        currentPatrolSpeed = basePatrolSpeed * nightSpeedMultiplier;
        currentChaseSpeed = baseChaseSpeed * nightSpeedMultiplier;
        currentAttackDamage = Mathf.RoundToInt(baseAttackDamage * nightDamageMultiplier);
        currentAttackCooldown = baseAttackCooldown * nightAttackCooldownMultiplier;
        currentDetectionRange = baseDetectionRange * nightDetectionRangeMultiplier;
        
        // Update active values
        patrolSpeed = currentPatrolSpeed;
        chaseSpeed = currentChaseSpeed;
        attackDamage = currentAttackDamage;
        attackCooldown = currentAttackCooldown;
        detectionRange = currentDetectionRange;
    }
    
    private void RemoveNightBuff()
    {
        // Restore base values
        patrolSpeed = basePatrolSpeed;
        chaseSpeed = baseChaseSpeed;
        attackDamage = baseAttackDamage;
        attackCooldown = baseAttackCooldown;
        detectionRange = baseDetectionRange;
        
        // Update current values
        currentPatrolSpeed = basePatrolSpeed;
        currentChaseSpeed = baseChaseSpeed;
        currentAttackDamage = baseAttackDamage;
        currentAttackCooldown = baseAttackCooldown;
        currentDetectionRange = baseDetectionRange;
    }
    
    private void Update()
    {
        // Update non-physics state logic here if needed
        // Most state logic is now in FixedUpdate for physics consistency
    }
    
    private void FixedUpdate()
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
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
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
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            
            // Check if player is alive before chasing
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            bool playerAlive = playerHealth == null || !playerHealth.IsDead;
            
            if (distanceToPlayer <= detectionRange && playerAlive)
            {
                currentState = EnemyState.Chase;
                return;
            }
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
        
        // Check if player is dead
        HealthSystem playerHealth = player.GetComponent<HealthSystem>();
        if (playerHealth != null && playerHealth.IsDead)
        {
            currentState = EnemyState.Patrol;
            return;
        }
        
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
        rb.linearVelocity = Vector2.zero;
        
        // Check if player is still in attack range
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            
            // If player moved out of range, chase them
            if (distanceToPlayer > attackRange)
            {
                currentState = EnemyState.Chase;
                return;
            }
            
            // Attack on cooldown
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                
                // Trigger attack animation
                if (hasAnimator && hasAttackParameter)
                {
                    SafeSetTrigger(isAttackHash);
                }
                
                // Delay damage to match the animation hit frame
                Invoke(nameof(DealDamageToPlayer), attackDamageDelay);
            }
        }
        else
        {
            // No player, go back to patrol
            currentState = EnemyState.Patrol;
        }
    }
    
    private void DealDamageToPlayer()
    {
        if (player == null) return;
        
        // Check if player is still in range (they might have moved)
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            if (playerHealth != null && !playerHealth.IsDead)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log($"{gameObject.name} dealt {attackDamage} damage to player!");
            }
        }
    }
    
    private void HandleHurt()
    {
        rb.linearVelocity = Vector2.zero;
        
        if (hasAnimator && hasTakehitParameter)
        {
            SafeSetTrigger(isTakehitHash);
            Debug.Log($"{gameObject.name} triggered takehit animation");
        }
        
        // Return to chase after being hurt
        Invoke(nameof(ResetToChase), 0.5f);
    }
    
    private void HandleDead()
    {
        rb.linearVelocity = Vector2.zero;
        
        if (hasAnimator && hasDeathParameter)
        {
            SafeSetTrigger(isDeathHash);
            Debug.Log($"{gameObject.name} triggered death animation");
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
        if (rb == null) return;
        
        // Wake up the rigidbody if it's sleeping
        if (rb.IsSleeping())
        {
            rb.WakeUp();
        }
        
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        Vector2 newVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
        
        rb.linearVelocity = newVelocity;
        
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
        if (!hasAnimator) return;
        
        // Update run/walk animation based on movement
        if (hasRunParameter)
        {
            bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
            bool shouldRun = isMoving && (currentState == EnemyState.Chase || currentState == EnemyState.Patrol);
            SafeSetBool(isRunHash, shouldRun);
        }
    }
    
    private void SafeSetBool(int parameterHash, bool value)
    {
        if (animator != null)
        {
            foreach (var param in animator.parameters)
            {
                if (param.nameHash == parameterHash)
                {
                    animator.SetBool(parameterHash, value);
                    return;
                }
            }
        }
    }
    
    private void SafeSetTrigger(int parameterHash)
    {
        if (animator != null)
        {
            foreach (var param in animator.parameters)
            {
                if (param.nameHash == parameterHash)
                {
                    animator.SetTrigger(parameterHash);
                    return;
                }
            }
        }
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

