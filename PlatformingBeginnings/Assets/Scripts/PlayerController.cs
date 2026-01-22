using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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


    public DarknessController darknessController;

    private bool wasCrouching = false;

    // ================= START =================
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (attackHitbox != null)
            attackHitbox.SetActive(false);

        UpdateUI();
    }

    // ================= UPDATE =================
    void Update()
    {
        if (isDead) return;

        CheckGround();
        DetectDashAttackInput();
        DetectDashInput();
        Movement();
        Jump();
        Attack();
        Crouch();
        UpdateAnimations();
    }

    // ================= GROUND =================
    void CheckGround()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        anim.SetBool("isGrounded", isGrounded);

        if (isGrounded && !wasGrounded)
            jumpCount = 0;
    }

    // ================= MOVEMENT =================
    void Movement()
    {
        if (isDashing || isDashAttacking) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);

        anim.SetBool("velocidad", horizontalInput != 0);

        if (horizontalInput > 0 && !isFacingRight) Flip();
        if (horizontalInput < 0 && isFacingRight) Flip();
    }

    // ================= JUMP =================
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            anim.SetTrigger(jumpCount == 0 ? "Jump" : "DoubleJump");
            jumpCount++;
        }
    }

    // ================= ATTACK (CLICK IZQUIERDO) =================
    void Attack()
    {
        if (Input.GetMouseButtonDown(0) && canAttack && isGrounded && !isDashAttacking)
            StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        canAttack = false;
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(0.1f);
        attackHitbox.SetActive(true);

        yield return new WaitForSeconds(0.2f);
        attackHitbox.SetActive(false);

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // ================= CROUCH =================
    void Crouch()
    {
        bool crouchInput = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        if (crouchInput && isGrounded)
        {
            anim.SetBool("isCrouching", true);
            wasCrouching = true;
        }
        else if (wasCrouching)
        {
            anim.SetTrigger("exitCrouch");
            anim.SetBool("isCrouching", false);
            wasCrouching = false;
        }
    }

    // ================= DASH =================
    void DetectDashInput()
    {
        if (!isGrounded || isDashAttacking) return;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            if (Time.time - lastTapTime <= doubleTapDelay)
                StartCoroutine(Dash());

            lastTapTime = Time.time;
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        anim.SetTrigger("Dash");

        float direction = isFacingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * dashSpeed, 0);

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero;
        isDashing = false;
    }

    // ================= DASH ATTACK (CLICK DERECHO) =================
    void DetectDashAttackInput()
    {
        if (!isGrounded || isDashAttacking) return;

        if (Input.GetMouseButtonDown(1))
            StartCoroutine(DashAttack());
    }

    IEnumerator DashAttack()
    {
        isDashAttacking = true;
        canAttack = false;
        anim.SetTrigger("DashAttack");

        float direction = isFacingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(direction * dashAttackSpeed, 0);

        yield return new WaitForSeconds(0.05f);
        attackHitbox.SetActive(true);

        yield return new WaitForSeconds(dashAttackDuration);
        attackHitbox.SetActive(false);

        rb.linearVelocity = Vector2.zero;
        isDashAttacking = false;
        canAttack = true;
    }

    // ================= DAMAGE =================
    public void TakeDamage()
    {
        if (isDead) return;

        vidas--;
        UpdateUI();
        anim.SetTrigger("Hurt");

        if (vidas <= 0)
            Die();
    }

    void Die()
    {
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

    // ================= UTILS =================
    void UpdateAnimations()
    {
        anim.SetFloat("verticalSpeed", rb.linearVelocity.y);
    }

    void UpdateUI()
    {
        if (contadorVidas != null)
            contadorVidas.text = "Vidas: " + vidas;

        if (contadorMonedas != null)
            contadorMonedas.text = "Monedas: " + monedas;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        if (attackHitbox != null)
        {
            Vector3 pos = attackHitbox.transform.localPosition;
            pos.x *= -1;
            attackHitbox.transform.localPosition = pos;
        }
    }

    // ================= PICKUPS =================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PickUp"))
        {
            monedas++;
            UpdateUI();

            
            if (darknessController != null)
                darknessController.UpdateScreenIcon(monedas);

            Destroy(collision.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }






}
