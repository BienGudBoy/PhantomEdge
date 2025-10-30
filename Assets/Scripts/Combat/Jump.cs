using UnityEngine;

public class Move : MonoBehaviour
{
    public float jumpForce = 7f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;


    private bool isGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {


        Jump();
        UpdateAnimation();
        CheckGround();
    }



    void Flip(float moveX)
    {
        if (moveX != 0)
        {
            // ✅ Không dùng Vector3 nữa
            spriteRenderer.flipX = (moveX < 0);
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    void UpdateAnimation()
    {
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool("isJump", !isGrounded && rb.linearVelocity.y > 0);
    }

    void CheckGround()
    {
        if (Mathf.Abs(rb.linearVelocity.y) < 0.01f)
            isGrounded = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }
}
