using System.Collections;
using UnityEngine;

public class FlyingEnemyController : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float patrolDistance = 5f;
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float attackRange = 1.5f;
    
    [Header("Flying Settings")]
    [SerializeField] private float minHeightAboveGround = 1.5f;
    [SerializeField] private float maxHeight = 5f;
    [SerializeField] private float verticalSpeed = 2f;
    [SerializeField] private float preferredHeight = 3f;
    
    [Header("Combat Settings")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackDamageDelay = 0.4f;
    
    [Header("Night Buff Settings")]
    [SerializeField] private float nightSpeedMultiplier = 1.5f;
    [SerializeField] private float nightDamageMultiplier = 1.5f;
    [SerializeField] private float nightDetectionRangeMultiplier = 1.2f;
    [SerializeField] private float nightAttackCooldownMultiplier = 0.8f;
    
    // Base values
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
	public int FacingDirection { get; private set; } = 1;
    
    [Header("Patrol")]
    private Vector2 startPosition;
    private Vector2 patrolTarget;
    private bool isMovingRight = true;
    
    [Header("State")]
    private EnemyState currentState = EnemyState.Idle;
    private float stateTimer = 0f;
    private float idleDuration = 2f;
    private float lastAttackTime = 0f;
    
    // Ground detection
    [SerializeField] private LayerMask groundLayerMask = 1; // Default layer
    private float groundCheckDistance = 10f;
    
    // Cache animator parameter hashes
    private int isFlightHash;
    private int isAttackHash;
    private int isTakehitHash;
    private int isDeathHash;
    private bool hasAnimator;
    private bool hasFlightParameter;
    private bool hasAttackParameter;
    private bool hasTakehitParameter;
    private bool hasDeathParameter;
	private Coroutine externalStaggerRoutine;
    
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
        patrolTarget = startPosition + Vector2.right * patrolDistance;
        
        // Store base values
        basePatrolSpeed = patrolSpeed;
        baseChaseSpeed = chaseSpeed;
        baseAttackDamage = attackDamage;
        baseAttackCooldown = attackCooldown;
        baseDetectionRange = detectionRange;
        
        // Initialize current values
        currentPatrolSpeed = basePatrolSpeed;
        currentChaseSpeed = baseChaseSpeed;
        currentAttackDamage = baseAttackDamage;
        currentAttackCooldown = baseAttackCooldown;
        currentDetectionRange = baseDetectionRange;
        
        // Setup ground layer mask (Ground tag or Default layer)
        int groundLayer = LayerMask.NameToLayer("Default");
        if (groundLayer != -1)
        {
            groundLayerMask = 1 << groundLayer;
        }
        
        // Cache animator parameters
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            hasAnimator = true;
            
            if (HasParameter("IsFlight"))
            {
                isFlightHash = Animator.StringToHash("IsFlight");
                hasFlightParameter = true;
                Debug.Log($"{gameObject.name}: Found flight parameter 'IsFlight'");
            }
            
            if (HasParameter("IsAttack"))
            {
                isAttackHash = Animator.StringToHash("IsAttack");
                hasAttackParameter = true;
            }
            
            if (HasParameter("IsTakehit"))
            {
                isTakehitHash = Animator.StringToHash("IsTakehit");
                hasTakehitParameter = true;
            }
            
            if (HasParameter("IsDeath"))
            {
                isDeathHash = Animator.StringToHash("IsDeath");
                hasDeathParameter = true;
            }
        }
        else
        {
            hasAnimator = false;
            Debug.LogWarning($"Animator not found on {gameObject.name}");
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
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        
        currentState = EnemyState.Patrol;
        
        // Subscribe to day/night cycle
        dayNightCycle = FindObjectOfType<DayNightCycle>();
        if (dayNightCycle != null)
        {
            dayNightCycle.OnDayNightChanged += OnDayNightChanged;
            
            if (!dayNightCycle.IsDay)
            {
                ApplyNightBuff();
            }
        }
    }
    
    private void OnDestroy()
    {
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
        }
        else
        {
            ApplyNightBuff();
        }
    }
    
    private void ApplyNightBuff()
    {
        currentPatrolSpeed = basePatrolSpeed * nightSpeedMultiplier;
        currentChaseSpeed = baseChaseSpeed * nightSpeedMultiplier;
        currentAttackDamage = Mathf.RoundToInt(baseAttackDamage * nightDamageMultiplier);
        currentAttackCooldown = baseAttackCooldown * nightAttackCooldownMultiplier;
        currentDetectionRange = baseDetectionRange * nightDetectionRangeMultiplier;
        
        patrolSpeed = currentPatrolSpeed;
        chaseSpeed = currentChaseSpeed;
        attackDamage = currentAttackDamage;
        attackCooldown = currentAttackCooldown;
        detectionRange = currentDetectionRange;
    }
    
    private void RemoveNightBuff()
    {
        patrolSpeed = basePatrolSpeed;
        chaseSpeed = baseChaseSpeed;
        attackDamage = baseAttackDamage;
        attackCooldown = baseAttackCooldown;
        detectionRange = baseDetectionRange;
        
        currentPatrolSpeed = basePatrolSpeed;
        currentChaseSpeed = baseChaseSpeed;
        currentAttackDamage = baseAttackDamage;
        currentAttackCooldown = baseAttackCooldown;
        currentDetectionRange = baseDetectionRange;
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
        rb.linearVelocity = Vector2.zero;
        MaintainHeight();
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
            currentState = EnemyState.Idle;
            stateTimer = 0f;
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
        
        HealthSystem playerHealth = player.GetComponent<HealthSystem>();
        if (playerHealth != null && playerHealth.IsDead)
        {
            currentState = EnemyState.Patrol;
            return;
        }
        
        if (distanceToPlayer > detectionRange * 1.5f)
        {
            currentState = EnemyState.Patrol;
            return;
        }
        
        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
            return;
        }
        
        // Chase player (move towards player position in air)
        MoveTowards(player.position, chaseSpeed);
    }
    
    private void HandleAttack()
    {
        rb.linearVelocity = Vector2.zero;
        MaintainHeight();
        
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            
            if (distanceToPlayer > attackRange)
            {
                currentState = EnemyState.Chase;
                return;
            }
            
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                
                if (hasAnimator && hasAttackParameter)
                {
                    SafeSetTrigger(isAttackHash);
                }
                
                Invoke(nameof(DealDamageToPlayer), attackDamageDelay);
            }
        }
        else
        {
            currentState = EnemyState.Patrol;
        }
    }
    
    private void DealDamageToPlayer()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            if (playerHealth != null && !playerHealth.IsDead)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }
    
    private void HandleHurt()
    {
        rb.linearVelocity = Vector2.zero;
        MaintainHeight();
        
        if (hasAnimator && hasTakehitParameter)
        {
            SafeSetTrigger(isTakehitHash);
        }
        
        Invoke(nameof(ResetToChase), 0.5f);
    }
    
    private void HandleDead()
    {
        rb.linearVelocity = Vector2.zero;
        
        if (hasAnimator && hasDeathParameter)
        {
            SafeSetTrigger(isDeathHash);
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
        
        if (rb.IsSleeping())
        {
            rb.WakeUp();
        }
        
        // Calculate direction (both X and Y for flying)
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        
        // Calculate desired velocity
        Vector2 desiredVelocity = direction * speed;
        
        // Maintain minimum height above ground
        float currentHeight = GetHeightAboveGround();
        float targetY = transform.position.y;
        
        // If too low, add upward velocity
        if (currentHeight < minHeightAboveGround)
        {
            targetY = GetGroundHeight() + minHeightAboveGround;
            float yDiff = targetY - transform.position.y;
            desiredVelocity.y = Mathf.Max(desiredVelocity.y, yDiff * verticalSpeed);
        }
        // If too high, allow downward movement
        else if (currentHeight > maxHeight)
        {
            targetY = GetGroundHeight() + preferredHeight;
            float yDiff = targetY - transform.position.y;
            desiredVelocity.y = yDiff * verticalSpeed;
        }
        // Otherwise, maintain preferred height with some vertical movement
        else
        {
            // Try to maintain preferred height, but allow following player vertically
            float preferredY = GetGroundHeight() + preferredHeight;
            float yDiff = preferredY - transform.position.y;
            
            // Blend between following player and maintaining height
            float heightBlend = 0.3f; // 30% maintain height, 70% follow player
            desiredVelocity.y = Mathf.Lerp(desiredVelocity.y, yDiff * verticalSpeed, heightBlend);
        }
        
        // Clamp Y velocity to prevent going too fast vertically
        desiredVelocity.y = Mathf.Clamp(desiredVelocity.y, -verticalSpeed, verticalSpeed);
        
        rb.linearVelocity = desiredVelocity;
        
        // Flip sprite based on horizontal direction
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction.x < 0;
			if (Mathf.Abs(direction.x) > 0.01f)
			{
				FacingDirection = direction.x >= 0f ? 1 : -1;
			}
        }
    }
    
    private void MaintainHeight()
    {
        float currentHeight = GetHeightAboveGround();
        
        if (currentHeight < minHeightAboveGround)
        {
            // Move up
            float targetY = GetGroundHeight() + minHeightAboveGround;
            float yDiff = targetY - transform.position.y;
            rb.linearVelocity = new Vector2(0, yDiff * verticalSpeed);
        }
        else if (currentHeight > maxHeight)
        {
            // Move down
            float targetY = GetGroundHeight() + preferredHeight;
            float yDiff = targetY - transform.position.y;
            rb.linearVelocity = new Vector2(0, yDiff * verticalSpeed);
        }
        else
        {
            // Hover in place
            rb.linearVelocity = new Vector2(0, 0);
        }
    }
    
    private float GetHeightAboveGround()
    {
        float groundHeight = GetGroundHeight();
        return transform.position.y - groundHeight;
    }
    
    private float GetGroundHeight()
    {
        // Raycast downward to find ground
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayerMask);
        
        if (hit.collider != null)
        {
            return hit.point.y;
        }
        
        // If no ground found, assume ground is at a default height
        return transform.position.y - minHeightAboveGround;
    }
    
    private void SetNewPatrolTarget()
    {
        float groundHeight = GetGroundHeight();
        float patrolY = groundHeight + preferredHeight;
        
        if (isMovingRight)
        {
            patrolTarget = new Vector2(startPosition.x + patrolDistance, patrolY);
        }
        else
        {
            patrolTarget = new Vector2(startPosition.x - patrolDistance, patrolY);
        }
        
        isMovingRight = !isMovingRight;
    }
    
    private void UpdateAnimations()
    {
        if (!hasAnimator) return;
        
        if (hasFlightParameter)
        {
            bool isMoving = rb.linearVelocity.magnitude > 0.1f;
            bool shouldFly = isMoving && (currentState == EnemyState.Chase || currentState == EnemyState.Patrol);
            SafeSetBool(isFlightHash, shouldFly);
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
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
        
        Destroy(gameObject, 3f);
    }
	
	public void ApplyExternalStagger(float duration)
	{
		if (duration <= 0f || currentState == EnemyState.Dead) return;
		
		if (externalStaggerRoutine != null)
		{
			StopCoroutine(externalStaggerRoutine);
		}
		externalStaggerRoutine = StartCoroutine(ExternalStagger(duration));
	}
	
	private IEnumerator ExternalStagger(float duration)
	{
		currentState = EnemyState.Hurt;
		if (rb != null)
		{
			rb.linearVelocity = Vector2.zero;
		}
		
		yield return new WaitForSeconds(duration);
		
		ResetToChase();
		externalStaggerRoutine = null;
	}
    
    private void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Draw height limits
        float groundHeight = Application.isPlaying ? GetGroundHeight() : transform.position.y - minHeightAboveGround;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(transform.position.x - 2, groundHeight + minHeightAboveGround, 0),
                        new Vector3(transform.position.x + 2, groundHeight + minHeightAboveGround, 0));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(transform.position.x - 2, groundHeight + preferredHeight, 0),
                        new Vector3(transform.position.x + 2, groundHeight + preferredHeight, 0));
    }
}



