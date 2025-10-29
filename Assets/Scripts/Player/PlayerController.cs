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
    
    [Header("State")]
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isSprinting;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (groundCheckPoint == null)
        {
            // Create ground check point if not assigned
            GameObject groundCheck = new GameObject("GroundCheck");
            groundCheck.transform.SetParent(transform);
            groundCheck.transform.localPosition = Vector2.zero;
            groundCheckPoint = groundCheck.transform;
        }
    }
    
    private void Update()
    {
        CheckGrounded();
        HandleMovement();
        UpdateAnimations();
        FlipSprite();
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            Jump();
        }
    }
    
    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.performed || context.started;
    }
    
    private void HandleMovement()
    {
        float currentSpeed = moveSpeed;
        if (isSprinting)
        {
            currentSpeed *= sprintMultiplier;
        }
        
        rb.velocity = new Vector2(moveInput.x * currentSpeed, rb.velocity.y);
    }
    
    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        
        if (animator != null)
        {
            animator.SetTrigger("IsJump");
        }
    }
    
    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        
        if (animator != null)
        {
            animator.SetBool("IsGrounded", isGrounded);
        }
    }
    
    private void UpdateAnimations()
    {
        if (animator == null) return;
        
        bool isMoving = Mathf.Abs(moveInput.x) > 0.1f;
        animator.SetBool("IsRun", isMoving && isSprinting);
        animator.SetBool("IsWalk", isMoving && !isSprinting);
    }
    
    private void FlipSprite()
    {
        if (spriteRenderer == null || moveInput.x == 0) return;
        
        spriteRenderer.flipX = moveInput.x < 0;
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

