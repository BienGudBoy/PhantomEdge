using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
	[SerializeField] private float attackRange = 1.6f;
	[SerializeField] private int attackDamage = 13;
	[SerializeField] private float attackDuration = 0.70f; // Total attack animation time
	[SerializeField] private float damageDelay = 0.22f; // When in the animation to deal damage (seconds)
    [SerializeField] private Transform attackPoint;
	
	[Header("Advanced Mechanics")]
	[SerializeField] private bool enableAttackIFrames = true;
	[SerializeField] private float attackInvulnerabilityDuration = 0.15f;
	[SerializeField] private bool enableBackstabBonus = true;
	[SerializeField] private float backstabMultiplier = 1.25f;
	[SerializeField, Range(0f, 180f)] private float backstabAngleThreshold = 60f;
	[SerializeField] private bool enableMicroStagger = true;
	[SerializeField] private float microStaggerDuration = 0.35f;
	[SerializeField] private bool microStaggerSkeletonsRequireBackstab = true;
	[SerializeField] private bool microStaggerOtherEnemies = false;
    
    [Header("Components")]
    private Animator animator;
    private HealthSystem healthSystem;
	private PlayerController playerController;
    
    [Header("State")]
    private float lastAttackTime = 0f;
    private LayerMask enemyLayer;
    private bool isAttacking = false;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
		playerController = GetComponent<PlayerController>();
        
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
		
		if (enableAttackIFrames && healthSystem != null)
		{
			healthSystem.GrantInvulnerability(attackInvulnerabilityDuration);
		}
        
        // Notify PlayerController to stop movement animations
		if (playerController != null)
        {
			playerController.IsAttacking = true;
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
				bool isBackstab = enableBackstabBonus && IsBackstab(enemy.transform);
				float damageScale = isBackstab ? backstabMultiplier : 1f;
				int appliedDamage = Mathf.Max(1, Mathf.RoundToInt(attackDamage * damageScale));
				
				enemyHealth.TakeDamage(appliedDamage);
				
				if (isBackstab)
				{
					Debug.Log($"Backstab! Dealt {appliedDamage} damage (x{backstabMultiplier:F2}) to {enemy.gameObject.name}");
				}
				else
				{
					Debug.Log($"Dealt {appliedDamage} damage to {enemy.gameObject.name}");
				}
				
				TryApplyMicroStagger(enemy.gameObject, isBackstab);
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
		if (playerController != null)
        {
			playerController.IsAttacking = false;
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
    
    public void IncreaseDamage(int amount)
    {
        attackDamage += amount;
        Debug.Log($"PlayerCombat: Damage increased by {amount}. New damage: {attackDamage}");
    }
    
    public void SetDamage(int newDamage)
    {
        attackDamage = newDamage;
        Debug.Log($"PlayerCombat: Damage set to {attackDamage}");
    }
    
    public void SetAttackDuration(float newDuration)
    {
        attackDuration = Mathf.Max(0.1f, newDuration);
        Debug.Log($"PlayerCombat: Attack duration set to {attackDuration}");
    }
    
	// Multiplicatively reduce attackDuration to increase attack speed.
	// Example: multiplier=0.9f reduces duration by 10% per upgrade. Clamped by minDuration.
	public void ApplyAttackSpeedUpgrade(float multiplier = 0.9f, float minDuration = 0.45f)
	{
		float previous = attackDuration;
		attackDuration = Mathf.Max(minDuration, attackDuration * multiplier);
		Debug.Log($"PlayerCombat: Attack speed upgraded. Duration {previous:F3}s -> {attackDuration:F3}s (mult={multiplier}, min={minDuration})");
	}
	
	public float GetAttackDuration()
	{
		return attackDuration;
	}
	
	// Reduce attack duration by a fixed amount (for Blade Empower upgrade)
	public void ReduceAttackDuration(float reduction)
	{
		float previous = attackDuration;
		attackDuration = Mathf.Max(0.1f, attackDuration - reduction); // Minimum 0.1s
		Debug.Log($"PlayerCombat: Attack duration reduced by {reduction:F3}s. {previous:F3}s -> {attackDuration:F3}s");
	}
	
	private bool IsBackstab(Transform enemyTransform)
	{
		Vector2 toPlayer = (Vector2)(transform.position - enemyTransform.position);
		if (toPlayer.sqrMagnitude < Mathf.Epsilon)
		{
			return false;
		}
		
		Vector2 enemyForward = GetEnemyFacingVector(enemyTransform);
		if (enemyForward.sqrMagnitude < Mathf.Epsilon)
		{
			enemyForward = Vector2.right;
		}
		
		float angleThreshold = Mathf.Clamp(backstabAngleThreshold, 0f, 180f);
		float backstabDot = Vector2.Dot(enemyForward.normalized, toPlayer.normalized);
		float requiredDot = -Mathf.Cos(angleThreshold * Mathf.Deg2Rad);
		
		if (backstabDot > requiredDot)
		{
			return false;
		}
		
		Vector2 toEnemy = -toPlayer;
		if (toEnemy.sqrMagnitude < Mathf.Epsilon)
		{
			return false;
		}
		
		Vector2 playerFacing = GetPlayerFacingVector();
		float facingDot = Vector2.Dot(playerFacing.normalized, toEnemy.normalized);
		
		return facingDot > 0.1f;
	}
	
	private Vector2 GetEnemyFacingVector(Transform enemyTransform)
	{
		if (enemyTransform.TryGetComponent<EnemyController>(out var enemyController))
		{
			return new Vector2(enemyController.FacingDirection, 0f);
		}
		
		if (enemyTransform.TryGetComponent<FlyingEnemyController>(out var flyingController))
		{
			return new Vector2(flyingController.FacingDirection, 0f);
		}
		
		SpriteRenderer sprite = enemyTransform.GetComponent<SpriteRenderer>();
		if (sprite != null)
		{
			return sprite.flipX ? Vector2.left : Vector2.right;
		}
		
		return enemyTransform.lossyScale.x >= 0f ? Vector2.right : Vector2.left;
	}
	
	private Vector2 GetPlayerFacingVector()
	{
		if (playerController != null)
		{
			return new Vector2(playerController.FacingDirection, 0f);
		}
		
		return transform.localScale.x >= 0f ? Vector2.right : Vector2.left;
	}
	
	private void TryApplyMicroStagger(GameObject enemyObject, bool isBackstab)
	{
		if (!enableMicroStagger)
		{
			return;
		}
		
		string nameLower = enemyObject.name.ToLowerInvariant();
		bool isGoblin = nameLower.Contains("goblin") || nameLower.Contains("globlin");
		bool isSkeleton = nameLower.Contains("skeleton");
		
		bool shouldApply = false;
		
		if (isGoblin)
		{
			shouldApply = true;
		}
		else if (isSkeleton)
		{
			shouldApply = microStaggerSkeletonsRequireBackstab ? isBackstab : true;
		}
		else if (microStaggerOtherEnemies)
		{
			shouldApply = true;
		}
		
		if (!shouldApply)
		{
			return;
		}
		
		if (enemyObject.TryGetComponent<EnemyController>(out var enemyController))
		{
			enemyController.ApplyExternalStagger(microStaggerDuration);
		}
		else if (enemyObject.TryGetComponent<FlyingEnemyController>(out var flyingController))
		{
			flyingController.ApplyExternalStagger(microStaggerDuration);
		}
	}
	
    public int GetAttackDamage()
    {
        return attackDamage;
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




