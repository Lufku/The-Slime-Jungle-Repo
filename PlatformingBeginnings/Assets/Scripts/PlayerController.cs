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
	public float speed;
	public float jumpForce;
	private bool isFacingRight = true;
	[SerializeField] bool isGrounded;
	[SerializeField] GameObject groundCheck;
	[SerializeField] LayerMask groundLayer;
	private int saltosRestantes = 1;

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
		isGrounded = Physics2D.OverlapCircle(
			groundCheck.transform.position,
			0.1f,
			groundLayer
		);

		Movement();
		Jump();
		cambioescena();
	}

	void Movement()
	{
		horizontalInput = Input.GetAxis("Horizontal");
		playerrb.linearVelocity = new Vector2(
			horizontalInput * speed,
			playerrb.linearVelocity.y
		);

		if (horizontalInput > 0)
		{
			anim.SetBool("velocidad", true);
			if (!isFacingRight)
				Flip();
		}
		else if (horizontalInput < 0)
		{
			anim.SetBool("velocidad", true);
			if (isFacingRight)
				Flip();
		}
		else
		{
			anim.SetBool("velocidad", false);
		}
	}

	void Jump()
	{
		anim.SetBool("Jump", !isGrounded);

		if (isGrounded)
		{
			saltosRestantes = 1;
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (isGrounded)
			{
				playerrb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
			}
			else if (saltosRestantes > 0)
			{
				playerrb.linearVelocity = new Vector2(playerrb.linearVelocity.x, 0);
				playerrb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
				saltosRestantes--;
			}
		}
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
			vidas--;
			contadorVidas.text = "Vidas: " + vidas;

			if (vidas <= 0)
			{
				SceneManager.LoadScene("Scene2");
			}
		}
	}
}
