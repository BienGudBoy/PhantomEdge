using UnityEngine;

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
    private float horizontalInput;
    private bool isGrounded;
    private bool isSprinting;

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
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // ✅ Jump bằng Space
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Flip sprite
        if (horizontalInput > 0)
            spriteRenderer.flipX = false;
        else if (horizontalInput < 0)
            spriteRenderer.flipX = true;
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        HandleMovement();
        UpdateAnimations();
    }

    private void HandleMovement()
    {
        float currentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;

        // ✅ SỬA LẠI CHỖ NÀY
        rb.linearVelocity = new Vector2(horizontalInput * currentSpeed, rb.linearVelocity.y);
    }

    private void Jump()
    {
        // ✅ Reset Y trước khi nhảy
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

        // ✅ Add lực nhảy
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        isGrounded = false;

        if (hasAnimator)
            SafeSetTrigger(isJumpHash);
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        if (hasAnimator)
            SafeSetBool(isGroundedHash, isGrounded);
    }

    private void UpdateAnimations()
    {
        if (!hasAnimator) return;

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

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}
