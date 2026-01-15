using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerrb;
    private Animator anim;
    private float horizontalInput;

    public float speed;
    private bool isFacingRight = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerrb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Movimiento();        
    }

    void Movimiento()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        playerrb.linearVelocity = new Vector2(horizontalInput * speed, playerrb.linearVelocity.y);

        if (horizontalInput > 0)
        {
            anim.SetBool("velocidad", true);
            if (!isFacingRight)
            {
                Flipeo();
            }
        }
        else if (horizontalInput < 0)
        {
            anim.SetBool("velocidad", true);
            if (isFacingRight)
            {
                Flipeo();
            }
        }
        else if (horizontalInput == 0)
        {
            anim.SetBool("velocidad", false);
        }

    }

    void Flipeo()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
    }
}
