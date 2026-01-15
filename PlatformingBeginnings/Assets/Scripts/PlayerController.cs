using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerrb;
    private Animator anim;
    private float horizontalInput;

    public float speed = 5f;
    private bool isFacingRight = true;

    [Header("Jump Parameters")]
    public float jumpForce = 10f;
    public bool isGrounded;
    private bool doubleJump;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Attack")]
    public float attackCooldown = 0.5f;
    private bool canAttack = true;

    void Start()
    {
        playerrb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Movimiento();
        CheckGround();
        HandleJump();
        HandleAttack();
    }

    void Movimiento()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        playerrb.linearVelocity = new Vector2(horizontalInput * speed, playerrb.linearVelocity.y);

        if (horizontalInput > 0)
        {
            anim.SetBool("velocidad", true);
            if (!isFacingRight) Flipeo();
        }
        else if (horizontalInput < 0)
        {
            anim.SetBool("velocidad", true);
            if (isFacingRight) Flipeo();
        }
        else
        {
            anim.SetBool("velocidad", false);
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump();
                doubleJump = true;
            }
            else if (doubleJump)
            {
                Jump();
                doubleJump = false;
            }
        }
    }

    void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.Q) && canAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;
        anim.SetTrigger("attack");
        Debug.Log("Ataque ejecutado");

        // Aquí puedes añadir hitbox o daño

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void Jump()
    {
        // Reinicia la velocidad vertical antes de saltar
        playerrb.linearVelocity = new Vector2(playerrb.linearVelocity.x, 0);
        playerrb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        anim.SetTrigger("jump");
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        anim.SetBool("isGrounded", isGrounded);
        // Debug para ver si detecta el suelo
        Debug.Log("Grounded: " + isGrounded);
    }

    void Flipeo()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
