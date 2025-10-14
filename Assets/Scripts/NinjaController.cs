using UnityEngine;

public class NinjaController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float jumpForce = 10f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    
    // Components
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    // State variables
    private bool isGrounded;
    private bool isJumping;
    private float horizontalInput;
    private bool isRunning;
    private bool isAttacking;
    
    // Animation parameter names
    private static readonly int SpeedParam = Animator.StringToHash("Speed");
    private static readonly int IsGroundedParam = Animator.StringToHash("IsGrounded");
    private static readonly int JumpParam = Animator.StringToHash("Jump");
    private static readonly int AttackParam = Animator.StringToHash("Attack");
    private static readonly int IsRunningParam = Animator.StringToHash("IsRunning");
    
    void Start()
    {
        // Get required components
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Create ground check if it doesn't exist
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.parent = transform;
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = groundCheckObj.transform;
        }
        
        // Validate components
        if (rb == null)
        {
            Debug.LogError("NinjaController requires a Rigidbody2D component!");
        }
        if (animator == null)
        {
            Debug.LogError("NinjaController requires an Animator component!");
        }
    }
    
    void Update()
    {
        // Get input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        
        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isAttacking)
        {
            Jump();
        }
        
        // Attack input
        if (Input.GetKeyDown(KeyCode.A) && isGrounded && !isAttacking)
        {
            Attack();
        }
        
        // Flip sprite based on movement direction
        if (horizontalInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (horizontalInput < 0)
        {
            spriteRenderer.flipX = true;
        }
        
        // Update animator parameters
        UpdateAnimator();
    }
    
    void FixedUpdate()
    {
        // Check if grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        
        // Handle movement only if not attacking
        if (!isAttacking)
        {
            Move();
        }
        else
        {
            // Slow down horizontal movement during attack
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.9f, rb.linearVelocity.y);
        }
    }
    
    private void Move()
    {
        // Determine current speed based on running state
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        
        // Apply horizontal movement
        float targetVelocityX = horizontalInput * currentSpeed;
        rb.linearVelocity = new Vector2(targetVelocityX, rb.linearVelocity.y);
    }
    
    private void Jump()
    {
        isJumping = true;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        animator.SetTrigger(JumpParam);
    }
    
    private void Attack()
    {
        isAttacking = true;
        animator.SetTrigger(AttackParam);
        // Attack duration will be handled by animation event or timer
        Invoke(nameof(ResetAttack), 0.5f); // Adjust based on attack animation length
    }
    
    private void ResetAttack()
    {
        isAttacking = false;
    }
    
    private void UpdateAnimator()
    {
        if (animator == null) return;
        
        // Set speed parameter for walk/run/idle transitions
        float speed = Mathf.Abs(horizontalInput);
        animator.SetFloat(SpeedParam, speed);
        
        // Set grounded state
        animator.SetBool(IsGroundedParam, isGrounded);
        
        // Set running state
        animator.SetBool(IsRunningParam, isRunning && speed > 0);
    }
    
    // Optional: Visualize ground check in editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
    
    // Public methods for animation events (optional)
    public void OnAttackComplete()
    {
        isAttacking = false;
    }
    
    public void OnJumpComplete()
    {
        isJumping = false;
    }
}
