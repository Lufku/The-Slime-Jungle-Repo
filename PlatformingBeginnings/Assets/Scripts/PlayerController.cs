using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    private Rigidbody2D playerrb;
    private Animator anim;
    private float horizontalInput;

    [Header("Jump")]
    public float speed = 5f;
    public float jumpForce = 10f;
    private bool isFacingRight = true;
    [SerializeField] bool isGrounded;
    [SerializeField] GameObject groundCheck;
    [SerializeField] LayerMask groundLayer;

    [Header("Attack")]
    public float attackCooldown = 0.6f;
    private bool canAttack = true;
    private bool isAttacking = false;


    [Header("UI")]
    public int monedas = 0;
    public TextMeshProUGUI contadorMonedas;
    public int vidas = 3;
    public TextMeshProUGUI contadorVidas;

    void Start()
    {
        playerrb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        CheckGround();
        Movement();
        Jump();
        Attack();
        cambioescena();
        anim.SetFloat("verticalSpeed", playerrb.linearVelocity.y);

    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, 0.15f, groundLayer);
        Debug.Log("Grounded: " + isGrounded);
    }


    void Movement()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        playerrb.linearVelocity = new Vector2(horizontalInput * speed, playerrb.linearVelocity.y);

        anim.SetBool("velocidad", horizontalInput != 0);

        if (horizontalInput > 0 && !isFacingRight) Flip();
        if (horizontalInput < 0 && isFacingRight) Flip();
    }


    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Resetear velocidad vertical
            playerrb.linearVelocity = new Vector2(playerrb.linearVelocity.x, 0);

            // Aplicar fuerza de salto
            playerrb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // Animación
            anim.SetTrigger("Jump");
        }
    }


    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Q) && canAttack && !isAttacking)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    System.Collections.IEnumerator AttackCoroutine()
    {
        canAttack = false;
        isAttacking = true;

        // Detiene el movimiento mientras ataca
        playerrb.linearVelocity = new Vector2(0, playerrb.linearVelocity.y);

        anim.SetTrigger("Attack");

        // Espera a que termine la animaci�n
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        canAttack = true;
    }


    void Flip()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
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
            vidas--;
            contadorVidas.text = "Vidas: " + vidas;

            if (vidas <= 0)
            {
                SceneManager.LoadScene("Scene2");
            }
        }
    }
}
