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

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    private bool isDashing = false;
    private float lastTapTime;
    private float doubleTapDelay = 0.3f;

    [Header("Combat")]
    public float attackCooldown = 0.5f;
    private bool canAttack = true;
    private bool isDead = false;

    [Header("UI")]
    public int monedas = 0;
    public TextMeshProUGUI contadorMonedas;
    public int vidas = 3;
    public TextMeshProUGUI contadorVidas;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        anim.SetBool("isGrounded", true);
    }

    void Update()
    {
        if (isDead || isDashing) return;

        CheckGround();
        Movement();
        Jump();
        Attack();
        Crouch();
        DetectDashInput();
        UpdateAnimations();
        cambioescena();
    }

    // ---------- Ground ----------
    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    // ---------- Movement ----------
    void Movement()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);

        anim.SetBool("velocidad", horizontalInput != 0);

        if (horizontalInput > 0 && !isFacingRight) Flip();
        if (horizontalInput < 0 && isFacingRight) Flip();
    }

    // ---------- Jump ----------
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            anim.SetTrigger("Jump");
        }
    }

    // ---------- Attack ----------
    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Q) && canAttack && isGrounded)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    System.Collections.IEnumerator AttackCoroutine()
    {
        canAttack = false;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // ---------- Crouch ----------
    // ---------- Crouch ----------
    private bool wasCrouching = false;

    void Crouch()
    {
        bool crouchInput = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        if (crouchInput && isGrounded)
        {
            if (!wasCrouching)
            {
                // Comenzamos crouch
                anim.SetBool("isCrouching", true);
                wasCrouching = true;
            }
            // Mientras se mantiene presionado, la animación de hold se hace en loop automáticamente
        }
        else
        {
            if (wasCrouching)
            {
                // Salimos de crouch
                anim.SetTrigger("exitCrouch");
                anim.SetBool("isCrouching", false);
                wasCrouching = false;
            }
        }
    }


    // ---------- Dash ----------
    void DetectDashInput()
    {
        if (!isGrounded) return;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) ||
            Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (Time.time - lastTapTime <= doubleTapDelay)
            {
                StartCoroutine(Dash());
            }

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

    // ---------- Animations ----------
    void UpdateAnimations()
    {
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("verticalSpeed", rb.linearVelocity.y);
    }

    // ---------- Utils ----------
    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        isFacingRight = !isFacingRight;
    }

    void cambioescena()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Pruebica");
        }
    }

    // ---------- Damage ----------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PickUp"))
        {
            monedas++;
            contadorMonedas.text = "Monedas: " + monedas;
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Pincho"))
        {
            TakeDamage();
        }
    }

    void TakeDamage()
    {
        if (isDead) return;

        vidas--;
        contadorVidas.text = "Vidas: " + vidas;
        anim.SetTrigger("Hurt");

        if (vidas <= 0)
        {
            Die();
        }
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
        SceneManager.LoadScene("Scene2");
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
