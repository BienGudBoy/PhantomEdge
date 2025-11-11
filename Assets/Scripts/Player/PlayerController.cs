using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundLayer;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerCombat playerCombat;
    private PlayerInput playerInput;
    private HealthSystem healthSystem;
    private Collider2D bodyCollider;

    [Header("State")]
    private float horizontalInput;
    private bool isGrounded;
    private bool isSprinting;
    private Vector2 moveInput;
    public bool IsAttacking { get; set; } = false;
	public int FacingDirection => spriteRenderer != null && spriteRenderer.flipX ? -1 : 1;

    // Animator hash
    private int isRunHash;
    private int isWalkHash;
    private int isGroundedHash;
    private int isJumpHash;
    private bool hasAnimator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCombat = GetComponent<PlayerCombat>();
        playerInput = GetComponent<PlayerInput>();
        healthSystem = GetComponent<HealthSystem>();
        bodyCollider = GetComponent<Collider2D>();
        if (bodyCollider == null)
        {
            bodyCollider = GetComponentInChildren<Collider2D>();
        }
        
        if (playerInput == null)
        {
            Debug.LogWarning("PlayerInput component not found. Adding it now.");
            playerInput = gameObject.AddComponent<PlayerInput>();
        }

        if (animator != null && animator.runtimeAnimatorController != null)
        {
            hasAnimator = true;
            isRunHash = Animator.StringToHash("IsRun");
            isWalkHash = Animator.StringToHash("IsWalk");
            isGroundedHash = Animator.StringToHash("IsGrounded");
            isJumpHash = Animator.StringToHash("IsJump");
        }
        else
        {
            hasAnimator = false;
            Debug.LogWarning($"Animator or RuntimeAnimatorController not found on {gameObject.name}. Animations will not work.");
        }

        if (groundCheckPoint == null)
        {
            GameObject groundCheck = new GameObject("GroundCheck");
            groundCheck.transform.SetParent(transform);
            groundCheck.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheckPoint = groundCheck.transform;
        }

        // Auto-detect Ground layer if not set
        if (groundLayer == 0)
        {
            groundLayer = LayerMask.GetMask("Ground");
            if (groundLayer == 0)
            {
                Debug.LogWarning("Ground layer not found! Make sure 'Ground' layer exists in Project Settings > Tags and Layers");
            }
            else
            {
                Debug.Log($"Auto-detected Ground layer mask: {groundLayer.value}");
            }
        }
    }

    private void Update()
    {
        // Use the stored move input from OnMove callback
        horizontalInput = moveInput.x;

        // Read sprint state directly from input action to ensure it's always current
        if (playerInput != null && playerInput.actions != null)
        {
            var sprintAction = playerInput.actions.FindAction("Sprint");
            if (sprintAction != null)
            {
                isSprinting = sprintAction.IsPressed();
            }
        }

        // Flip sprite
        if (horizontalInput > 0)
            spriteRenderer.flipX = false;
        else if (horizontalInput < 0)
            spriteRenderer.flipX = true;
    }

    // Input System callback methods (called by PlayerInput component)
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnAttack(InputValue value)
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
        
        // Only trigger attack on button press, not while holding
        if (playerCombat != null && value.isPressed)
        {
            playerCombat.OnAttackInput();
        }
    }

    public void OnJump(InputValue value)
    {
        // Don't jump if player is dead
        if (healthSystem != null && healthSystem.IsDead)
        {
            Debug.Log("Jump blocked: Player is dead");
            return;
        }
        
        if (value.isPressed)
        {
            Debug.Log($"Jump input received. isGrounded={isGrounded}, groundLayer={groundLayer.value}");
        }
        
        if (isGrounded && value.isPressed)
        {
            Jump();
        }
        else if (value.isPressed && !isGrounded)
        {
            Debug.LogWarning($"Jump blocked: Not grounded. Ground check: position={groundCheckPoint.position}, radius={groundCheckRadius}");
        }
    }

    public void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        HandleMovement();
        UpdateAnimations();
    }

    private void HandleMovement()
    {
        // Don't move if player is dead
        if (healthSystem != null && healthSystem.IsDead)
        {
            return;
        }
        
        float currentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;

        rb.linearVelocity = new Vector2(horizontalInput * currentSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        isGrounded = false;

        if (hasAnimator)
            SafeSetTrigger(isJumpHash);
    }

    private void CheckGrounded()
    {
        bool overlapHit = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        bool colliderTouching = bodyCollider != null && bodyCollider.IsTouchingLayers(groundLayer);
        isGrounded = overlapHit || colliderTouching;

        // Debug logging (can be removed later)
        if (Time.frameCount % 60 == 0) // Log every 60 frames to avoid spam
        {
            Collider2D hit = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
            Debug.Log($"Ground check: position={groundCheckPoint.position}, radius={groundCheckRadius}, layerMask={groundLayer.value}, overlapHit={overlapHit}, colliderTouching={colliderTouching}, isGrounded={isGrounded}, hit={hit?.name ?? "null"}");
        }

        if (hasAnimator)
            SafeSetBool(isGroundedHash, isGrounded);
    }

    private void UpdateAnimations()
    {
        if (!hasAnimator) return;
        
        // Don't update movement animations if player is dead
        if (healthSystem != null && healthSystem.IsDead)
        {
            return;
        }
        
        // Don't update movement animations while attacking
        if (IsAttacking)
        {
            Debug.Log("Skipping UpdateAnimations - IsAttacking is true");
            return;
        }

        bool isMoving = Mathf.Abs(horizontalInput) > 0.1f;
        SafeSetBool(isRunHash, isMoving && isSprinting);
        SafeSetBool(isWalkHash, isMoving && !isSprinting);
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

    public void IncreaseMoveSpeed(float amount)
    {
        moveSpeed += amount;
        Debug.Log($"PlayerController: Move speed increased by {amount}. New speed: {moveSpeed}");
    }
    
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
        Debug.Log($"PlayerController: Move speed set to {moveSpeed}");
    }
    
    public float GetMoveSpeed()
    {
        return moveSpeed;
    }
    
    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}
