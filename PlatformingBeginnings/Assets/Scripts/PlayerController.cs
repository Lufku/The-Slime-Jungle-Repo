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
        if (isDead) return;

        CheckGround();
        Movement();
        Jump();
        Attack();
        UpdateAnimations();
        cambioescena();
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    void Movement()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);

        anim.SetBool("velocidad", horizontalInput != 0);

        if (horizontalInput > 0 && !isFacingRight) Flip();
        if (horizontalInput < 0 && isFacingRight) Flip();
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            anim.SetTrigger("Jump");
        }
    }

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

    void UpdateAnimations()
    {
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("verticalSpeed", rb.linearVelocity.y);
    }

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
        isDead = true;
        anim.SetTrigger("Death");
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

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
