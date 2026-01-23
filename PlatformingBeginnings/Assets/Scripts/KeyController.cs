using UnityEngine;

public class KeyController : MonoBehaviour
{
    private Rigidbody2D rb;
    public int coinsNeeded = 4;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // No gravity at start
    }

    public void CheckCoins(int currentCoins)
    {
        if (currentCoins >= coinsNeeded)
        {
            ActivateGravity();
        }
    }

    void ActivateGravity()
    {
        rb.gravityScale = 1f;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // asegúrate que tu suelo tiene tag "Ground"
        {
            rb.gravityScale = 0f;
        }
    }
}
