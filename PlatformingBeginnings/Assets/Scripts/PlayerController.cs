using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    private Rigidbody2D rb;
    private Animator anim;
    private float horizontalInput;
    public float speed = 5f;
    private bool isFacingRight = true;

    [Header("Jump")]
    public float jumpForce = 8f;
    private bool isGrounded;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    public float groundRadius = 0.3f;
    private int maxJumps = 2;
    private int jumpCount = 0;

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    private bool isDashing = false;
    private float lastTapTime;
    private float doubleTapDelay = 0.3f;

    [Header("Dash Attack")]
    public float dashAttackSpeed = 20f;
    public float dashAttackDuration = 0.2f;
    private bool isDashAttacking = false;

    [Header("Combat")]
    public float attackCooldown = 0.5f;
    private bool canAttack = true;
    private bool isDead = false;

    [Header("Attack Hitbox")]
    public GameObject attackHitbox;

    [Header("UI")]
    public int monedas = 0;
    public TextMeshProUGUI contadorMonedas;
    public int vidas = 3;
    public TextMeshProUGUI contadorVidas;

    private bool wasCrouching = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        anim.SetBool("isGrounded", true);

        // Desactivar hitbox si existe
        if (attackHitbox != null)
            attackHitbox.SetActive(false);

        // Inicializar UI seguro
        if (contadorVidas != null)
            contadorVidas.text = "Vidas: " + vidas;
        if (contadorMonedas != null)
            contadorMonedas.text = "Monedas: " + monedas;
    }

    void Update()
    {
        if (isDead) return;

        CheckGround();
        DetectDashAttackInput();
        Movement();
        Jump();
        Attack();
        Crouch();
        DetectDashInput();
        UpdateAnimations();
    }

    // ---------- Ground ----------
    void CheckGround()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        anim.SetBool("isGrounded", isGrounded);

        if (isGrounded && !wasGrounded)
            jumpCount = 0;
    }

    // ---------- Movement ----------
    void Movement()
    {
        if (isDashing || isDashAttacking) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);

        anim.SetBool("velocidad", horizontalInput != 0);

        if (horizontalInput > 0 && !isFacingRight) Flip();
        if (horizontalInput < 0 && isFacingRight) Flip();
    }

    // ---------- Jump ----------
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            if (jumpCount == 0)
                anim.SetTrigger("Jump");
            else
                anim.SetTrigger("DoubleJump");

            jumpCount++;
        }
    }

    // ---------- Attack ----------
    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Q) && canAttack && isGrounded && !isDashAttacking)
            StartCoroutine(AttackCoroutine());
    }

    System.Collections.IEnumerator AttackCoroutine()
    {
        canAttack = false;
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(0.1f);

        if (attackHitbox != null)
            attackHitbox.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        if (attackHitbox != null)
            attackHitbox.SetActive(false);

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // ---------- Crouch ----------
    void Crouch()
    {
        bool crouchInput = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        if (crouchInput && isGrounded)
        {
            if (!wasCrouching)
            {
                anim.SetBool("isCrouching", true);
                wasCrouching = true;
            }
        }
        else
        {
            if (wasCrouching)
            {
                anim.SetTrigger("exitCrouch");
                anim.SetBool("isCrouching", false);
                wasCrouching = false;
            }
        }
    }

    // ---------- Dash ----------
    void DetectDashInput()
    {
        if (!isGrounded || isDashAttacking) return;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) ||
            Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (Time.time - lastTapTime <= doubleTapDelay)
                StartCoroutine(Dash());

            lastTapTime = Time.time;
        }
    }

    System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        anim.SetTrigger("Dash");

        float direction = isFacingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * dashSpeed, 0);

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero;
        isDashing = false;
    }

    // ---------- Dash Attack ----------
    void DetectDashAttackInput()
    {
        if (!isGrounded || isDashAttacking) return;

        if (Input.GetKey(KeyCode.E))
            StartCoroutine(DashAttack());
    }

    System.Collections.IEnumerator DashAttack()
    {
        isDashAttacking = true;
        canAttack = false;
        anim.SetTrigger("DashAttack");

        float direction = isFacingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * dashAttackSpeed, 0);

        yield return new WaitForSeconds(0.05f);

        if (attackHitbox != null)
            attackHitbox.SetActive(true);

        yield return new WaitForSeconds(dashAttackDuration);

        if (attackHitbox != null)
            attackHitbox.SetActive(false);

        rb.linearVelocity = Vector2.zero;
        isDashAttacking = false;
        canAttack = true;
    }

    // ---------- Utils ----------
    void UpdateAnimations()
    {
        anim.SetFloat("verticalSpeed", rb.linearVelocity.y);
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        isFacingRight = !isFacingRight;

        // Flip hitbox
        if (attackHitbox != null)
        {
            Vector3 pos = attackHitbox.transform.localPosition;
            pos.x *= -1;
            attackHitbox.transform.localPosition = pos;
        }
    }

    // ---------- Damage ----------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PickUp"))
        {
            monedas++;
            if (contadorMonedas != null)
                contadorMonedas.text = "Monedas: " + monedas;

            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Pincho"))
            TakeDamage();
    }

    public void TakeDamage()
    {
        if (isDead) return;

        vidas--;
        if (contadorVidas != null)
            contadorVidas.text = "Vidas: " + vidas;

        anim.SetTrigger("Hurt");

        if (vidas <= 0) Die();
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        anim.SetTrigger("Death");

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        Invoke(nameof(LoadDeathScene), 2f);
    }

    void LoadDeathScene()
    {
        SceneManager.LoadScene("GameOver");
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
