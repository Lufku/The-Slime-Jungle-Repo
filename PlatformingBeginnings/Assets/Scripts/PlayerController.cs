using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // ================= MOVEMENT =================
    [Header("Movement")]
    public float speed = 5f;
    private float horizontalInput;
    private bool isFacingRight = true;

    private Rigidbody2D rb;
    private Animator anim;

    // ================= CROUCH =================
    [Header("Crouch")]
    private bool isCrouching = false;

    // ================= JUMP =================
    [Header("Jump")]
    public float jumpForce = 8f;
    private int jumpCount = 0; // 0 = en suelo, 1 = primer salto, 2 = doble salto
    private bool isGrounded;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    public float groundRadius = 0.3f;

    // ================= DASH =================
    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    private bool isDashing;
    private float lastTapTime;
    private float doubleTapDelay = 0.3f;

    // ================= DASH ATTACK =================
    [Header("Dash Attack")]
    public float dashAttackSpeed = 20f;
    public float dashAttackDuration = 0.2f;
    private bool isDashAttacking;

    // ================= COMBAT =================
    [Header("Combat")]
    public float attackCooldown = 0.5f;
    private bool canAttack = true;
    private bool isDead = false;

    [Header("Attack Hitbox")]
    public GameObject attackHitbox;

    // ================= UI =================
    [Header("UI")]
    public int monedas = 0;
    public int vidas = 3;
    public TextMeshProUGUI contadorMonedas;
    public TextMeshProUGUI contadorVidas;
    public TextMeshProUGUI textoTemporal;

    // ================= DARKNESS =================
    [Header("Darkness")]
    public DarknessController darknessController;

    // ================= AUDIO =================
    [Header("Audio")]
    public AudioClip coinSound;
    public AudioClip attackSound;
    public AudioClip dashSound;
    public AudioClip jumpSound;
    public AudioClip damageSound;
    public AudioClip deathSound;
    public AudioClip doorSound;
    public AudioClip keySound;
    public AudioClip pickUpSound;
    public AudioClip unlockSound;
    private AudioSource audioSource;

    [Header("Key")]
    public KeyController key;
    public bool keyCollected = false;
    private bool keyAppeared = false;

    [Header("Door")]
    public DoorController doorController;

    [Header("Combat Stats")]
    public int extraDamage = 0;

    // ================= START =================
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (attackHitbox != null)
            attackHitbox.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        UpdateUI();
    }

    // ================= UPDATE =================
    void Update()
    {
        if (isDead) return;

        CheckGround();
        DetectDashInput();
        DetectDashAttackInput();
        Movement();
        Jump();
        Attack();
        Crouch();
        UpdateAnimations();
        CheckCoins();
    }

    // ================= GROUND =================
    void CheckGround()
    {
        // Detecta suelo SOLO si estás cayendo o quieto verticalmente
        bool groundedNow = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );

        // Si estás subiendo, NO estás en el suelo aunque el círculo toque algo
        if (rb.linearVelocity.y > 0.1f)
            groundedNow = false;

        anim.SetBool("isGrounded", groundedNow);

        // Reset de saltos SOLO cuando aterrizas de verdad
        if (!isGrounded && groundedNow)
            jumpCount = 0;

        isGrounded = groundedNow;
    }



    // ================= JUMP =================
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < 2)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            if (jumpCount == 0)
                anim.SetTrigger("Jump");
            else
                anim.SetTrigger("DoubleJump");

            jumpCount++;
        }
    }



    void DoJump()
    {
        // Reset vertical para saltos consistentes
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        // Aplicar fuerza
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // Sonido
        if (jumpSound != null && audioSource != null)
            audioSource.PlayOneShot(jumpSound);
    }

    // ================= CROUCH =================
    void Crouch()
    {
        if (!isGrounded) return;

        bool crouchInput = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        if (crouchInput && !isCrouching)
        {
            isCrouching = true;
            anim.SetBool("isCrouching", true);
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else if (!crouchInput && isCrouching)
        {
            isCrouching = false;
            anim.SetBool("isCrouching", false);
            anim.SetTrigger("exitCrouch");
        }
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

    // ================= ATTACK =================
    void Attack()
    {
        if (Input.GetMouseButtonDown(0) && canAttack && isGrounded && !isDashAttacking)
        {
            StartCoroutine(AttackCoroutine());

            if (attackSound != null && audioSource != null)
                audioSource.PlayOneShot(attackSound);
        }
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

    // ================= DASH =================
    void DetectDashInput()
    {
        // No permitir dash si estás atacando con dash
        if (isDashAttacking) return;

        // Solo permitir dash si estás en el suelo
        if (!isGrounded) return;

        // Detectar doble tap SOLO si el jugador pulsa A o D MANUALMENTE
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            float timeSinceLastTap = Time.time - lastTapTime;

            if (timeSinceLastTap <= doubleTapDelay)
            {
                // Evitar que el dash se active si vienes de un dash attack
                if (!isDashAttacking)
                    StartCoroutine(Dash());
            }

            lastTapTime = Time.time;
        }
    }


    IEnumerator Dash()
    {
        if (dashSound != null && audioSource != null)
            audioSource.PlayOneShot(dashSound);

        isDashing = true;
        anim.SetTrigger("Dash");

        float dir = isFacingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(dir * dashSpeed, 0);

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero;
        isDashing = false;
    }

    // ================= DASH ATTACK =================
    void DetectDashAttackInput()
    {
        if (!isGrounded || isDashAttacking) return;

        if (Input.GetMouseButtonDown(1))
            StartCoroutine(DashAttack());
    }

    IEnumerator DashAttack()
    {
        isDashAttacking = true;
        isDashing = false;
        canAttack = false;

        anim.SetTrigger("DashAttack");

        float dir = isFacingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(dir * dashAttackSpeed, 0);

        // Espera un poco para sincronizar con la animación
        yield return new WaitForSeconds(0.1f);

        // Activar hitbox igual que el ataque normal
        attackHitbox.SetActive(true);

        // Mantener hitbox activo durante el ataque
        yield return new WaitForSeconds(0.2f);

        attackHitbox.SetActive(false);

        // Termina el dash attack
        rb.linearVelocity = Vector2.zero;
        isDashAttacking = false;
        canAttack = true;

        lastTapTime = 0;
    }



    // ================= PICKUPS =================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PickUp"))
        {
            if (coinSound != null && audioSource != null)
                audioSource.PlayOneShot(coinSound);

            monedas++;
            UpdateUI();

            if (darknessController != null)
                darknessController.UpdateScreenIcon(monedas);

            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Pincho"))
        {
            TakeDamage();
        }
        if (collision.CompareTag("Key") && key.isVisible)
        {
            ShowText("You can now unlock the door.");
            keyCollected = true;
            Destroy(collision.gameObject);
            if (pickUpSound != null && audioSource != null)
                audioSource.PlayOneShot(pickUpSound);
        }
        if (collision.CompareTag("Door_closed"))
        {
            if (!keyCollected)
            {
                ShowText("You must collect 10 coins to summon the key.");
            }
            else
            {
                doorController.OpenDoor();
                if (unlockSound != null && audioSource != null) ;
                audioSource.PlayOneShot(unlockSound);
            }

        }
    }

    // ================= DAMAGE =================
    public void TakeDamage()
    {
        if (isDead) return;

        if (damageSound != null && audioSource != null)
            audioSource.PlayOneShot(damageSound);

        vidas--;
        UpdateUI();
        anim.SetTrigger("Hurt");

        if (vidas <= 0)
            Die();
    }

    void Die()
    {
        if (deathSound != null && audioSource != null)
            audioSource.PlayOneShot(deathSound);

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

    void CheckCoins()
    {
        key.CheckCoins(monedas);
        if (!keyAppeared && key.isVisible && keySound != null && audioSource != null)
        {
            keyAppeared = true;
            ShowText("The key has been summoned.");
            audioSource.PlayOneShot(keySound);
        }

    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
    private void ShowText(string mensaje)
    {
        textoTemporal.text = mensaje;
        Invoke("HideText", 5f);
    }

    void HideText()
    {
        textoTemporal.text = "";
    }
}
