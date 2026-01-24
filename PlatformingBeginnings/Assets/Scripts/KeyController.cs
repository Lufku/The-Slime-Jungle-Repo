using UnityEngine;

public class KeyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    public int coinsNeeded = 4;
    public bool isVisible = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0f;
        spriteRenderer.enabled = false;
    }

    public void CheckCoins(int currentCoins)
    {
        if (!isVisible && currentCoins >= coinsNeeded)
        {
            ShowKey();
        }
    }

    void ShowKey()
    {
        spriteRenderer.enabled = true;
        isVisible = true;
    }
    public void HideKey()
    {
        if (isVisible)
        {
            isVisible = false;
        }
    }

    }
