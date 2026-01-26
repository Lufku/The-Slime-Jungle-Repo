using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Respawn")]
    public Transform respawnPoint;

    [Header("Movement")]
    public float speed = 5f;
    [HideInInspector] public float baseSpeed;

    private float horizontalInput;
    private bool isFacingRight = true;

    private Rigidbody2D rb;
    private Animator anim;

    [Header("Crouch")]
    private bool isCrouching = false;

    [Header("Jump")]
    public float jumpForce = 6f;
    [HideInInspector] public float baseJumpForce;

    private int jumpCount = 0;
    private bool isGrounded;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    public float groundRadius = 0.3f;

    [Header("Dash")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    private bool isDashing;
    private float lastTapTime;
    private float doubleTapDelay = 0.3f;

    [Header("Dash Attack")]
    public float dashAttackSpeed = 20f;
    public float dashAttackDuration = 0.2f;
    private bool isDashAttacking;

    [Header("Combat")]
    public float attackCooldown = 0.5f;
    private bool canAttack = true;
    private bool isDead = false;

    [Header("Attack Hitbox")]
    public GameObject attackHitbox;

    [Header("UI")]
    public int monedas = 0;
    public int vidas = 10;

    [Header("Darkness")]
    public DarknessController darknessController;

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
    public int extraDamage = 1;
    [HideInInspector] public int baseExtraDamage;

    [Header("Text")]
    public TextMeshProUGUI textoTemporal;

    [Header("HUD")]
    public HUDController HUDController;

    // -----------------------------
    // AWAKE: SIEMPRE ANTES QUE Start()
    // -----------------------------
    void Awake()
    {
        baseJumpForce = jumpForce;
        baseSpeed = speed;
        baseExtraDamage = extraDamage;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (attackHitbox != null)
            attackHitbox.SetActive(false);

        // RESET TOTAL SOLO AL ENTRAR EN LA ESCENA DE INICIO
        if (SceneManager.GetActiveScene().name == "Map")
        {
            PlayerPrefs.DeleteAll();

            jumpForce = baseJumpForce;
            speed = baseSpeed;
            extraDamage = baseExtraDamage;

            vidas = 10;
            monedas = 0;
        }
    }


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
        CheckFallRespawn();

        HUDController?.ActualizarHUD();
    }

    // -----------------------------
    // MUERTE
    // -----------------------------
    public void TakeDamage()
    {
        if (isDead) return;

        PlaySound(damageSound);
        vidas--;

        HUDController.ActualizarHUD();
        anim.SetTrigger("Hurt");

        if (vidas <= 0)
            Die();
    }

    void Die()
    {
        PlaySound(deathSound);
        isDead = true;
        anim.SetTrigger("Death");

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        // Guardar escena exacta donde mueres
        PlayerPrefs.SetString("LastLevel", SceneManager.GetActiveScene().name);

        Invoke(nameof(LoadDeathScene), 2f);
    }


    void LoadDeathScene()
    {
        SceneManager.LoadScene("GameOver");
    }

    // -----------------------------
    // FLUJO DE ESCENAS
    // -----------------------------
    void LoadNextLevel()
    {
        string current = SceneManager.GetActiveScene().name;

        if (current == "Escena Level 1 Recuperada")
            SceneManager.LoadScene("Map 1-2");
        else if (current == "Map 1-2")
            SceneManager.LoadScene("Level2");
        else if (current == "Level2")
            SceneManager.LoadScene("Map 2-3");
        else if (current == "Map 2-3")
            SceneManager.LoadScene("FinalLevel");
        else if (current == "FinalLevel")
            SceneManager.LoadScene("Victory");
    }

    // -----------------------------
    // RESTO DE FUNCIONES (sin cambios)
    // -----------------------------
    void CheckFallRespawn()
    {
        if (SceneManager.GetActiveScene().name != "FinalLevel")
            return;

        if (transform.position.y < -12f)
            Respawn();
    }

    void Respawn()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = respawnPoint.position;

        vidas--;
        HUDController.ActualizarHUD();

        if (vidas <= 0)
            Die();
    }

    void CheckGround()
    {
        bool groundedNow = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );

        if (rb.linearVelocity.y > 0.1f)
            groundedNow = false;

        anim.SetBool("isGrounded", groundedNow);

        if (!isGrounded && groundedNow)
            jumpCount = 0;

        isGrounded = groundedNow;
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < 2)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            anim.SetTrigger(jumpCount == 0 ? "Jump" : "DoubleJump");

            jumpCount++;
        }
    }

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

    void Movement()
    {
        if (isDashing || isDashAttacking) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);

        anim.SetBool("velocidad", horizontalInput != 0);

        if (horizontalInput > 0 && !isFacingRight) Flip();
        if (horizontalInput < 0 && isFacingRight) Flip();
    }

    void DetectDashInput()
    {
        if (isDashAttacking) return;
        if (!isGrounded) return;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            float timeSinceLastTap = Time.time - lastTapTime;

            if (timeSinceLastTap <= doubleTapDelay)
                StartCoroutine(Dash());

            lastTapTime = Time.time;
        }
    }

    IEnumerator Dash()
    {
        PlaySound(dashSound);

        isDashing = true;
        anim.SetTrigger("Dash");

        float dir = isFacingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(dir * dashSpeed, 0);

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero;
        isDashing = false;
    }

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

        yield return new WaitForSeconds(0.1f);

        attackHitbox.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        attackHitbox.SetActive(false);

        rb.linearVelocity = Vector2.zero;
        isDashAttacking = false;
        canAttack = true;

        lastTapTime = 0;
    }

    void UpdateAnimations()
    {
        anim.SetFloat("verticalSpeed", rb.linearVelocity.y);
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
    void Attack()
    {
        if (Input.GetMouseButtonDown(0) && canAttack && isGrounded && !isDashAttacking)
        {
            StartCoroutine(AttackCoroutine());
            PlaySound(attackSound);
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Recoger monedas
        if (collision.CompareTag("PickUp"))
        {
            PlaySound(coinSound);
            monedas++;
            PlayerPrefs.SetInt("Monedas", monedas);
            PlayerPrefs.Save();

            darknessController?.UpdateScreenIcon(monedas);

            Destroy(collision.gameObject);
            return;
        }

        // Pinchos
        if (collision.CompareTag("Pincho"))
        {
            TakeDamage();
            return;
        }

        // Recoger llave
        if (collision.CompareTag("Key") && key.isVisible)
        {
            ShowText("You can now unlock the door.");
            keyCollected = true;
            Destroy(collision.gameObject);
            PlaySound(pickUpSound);
            return;
        }

        // Puerta cerrada
        if (collision.CompareTag("Door_closed"))
        {
            if (!keyCollected)
            {
                ShowText("You don't have enough coins to open the door.");
                PlaySound(doorSound);
            }
            else
            {
                doorController.OpenDoor();
                PlaySound(unlockSound);
            }
            return;
        }

        // Puerta abierta
        if (collision.CompareTag("Door_open"))
        {
            PlaySound(doorSound);
            Invoke(nameof(LoadNextLevel), 2f);
            return;
        }
    }

    void CheckCoins()
    {
        key.CheckCoins(monedas);

        if (!keyAppeared && key.isVisible)
        {
            keyAppeared = true;
            ShowText("The key has been summoned.");
            PlaySound(keySound);
        }
    }

    private void PlaySound(AudioClip sound)
    {
        if (audioSource != null && sound != null)
            audioSource.PlayOneShot(sound);
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
