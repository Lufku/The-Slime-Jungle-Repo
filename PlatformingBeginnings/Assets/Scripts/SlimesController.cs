using UnityEngine;
using System.Collections;

public class SlimesController : MonoBehaviour
{
    [Header("Size")]
    [Range(0.3f, 4f)]
    public float slimeSize = 1f;

    [Header("Stats")]
    public int baseVida = 3;
    public float baseSpeed = 2f;
    public int baseDamage = 1;

    private int vida;
    private float speed;
    private int damage;

    [Header("Detection")]
    public float baseDetectionRange = 6f;
    public float baseAttackRange = 1.5f;

    private float detectionRange;
    private float attackRange;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;

    private bool isDead = false;
    private bool isAttacking = false;
    private bool attackCooldown = false;

    [Header("Knockback")]
    public float baseKnockbackForce = 10f;
    public float baseKnockbackUpForce = 4f;

    private float knockbackForce;
    private float knockbackUpForce;

    [Header("Attack")]
    public float swallowDuration = 1f;
    public float attackDelay = 2f;

    [Header("Audio")]
    private AudioSource audioSource;
    public AudioClip killSound;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        transform.localScale = new Vector3(slimeSize, slimeSize, 1);

        vida = Mathf.RoundToInt(baseVida * slimeSize);
        damage = Mathf.RoundToInt(baseDamage * slimeSize);
        speed = baseSpeed / slimeSize;

        detectionRange = baseDetectionRange * slimeSize;
        attackRange = baseAttackRange * slimeSize;

        knockbackForce = baseKnockbackForce * slimeSize;
        knockbackUpForce = baseKnockbackUpForce * slimeSize;
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange && !isAttacking && !attackCooldown)
            StartCoroutine(SwallowAttack());
        else if (distance <= detectionRange)
            MoveTowardsPlayer();
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetBool("Move", false);
        }
    }

    void MoveTowardsPlayer()
    {
        anim.SetBool("Move", true);

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);

        transform.localScale = new Vector3(Mathf.Sign(direction.x) * slimeSize, slimeSize, 1);
    }

    IEnumerator SwallowAttack()
    {
        isAttacking = true;
        attackCooldown = true;

        anim.SetBool("Move", false);
        anim.SetTrigger("Swallow");

        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(swallowDuration);

        isAttacking = false;

        yield return new WaitForSeconds(attackDelay);
        attackCooldown = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead || !isAttacking) return;

        if (collision.CompareTag("Player"))
        {
            PlayerController p = collision.transform.root.GetComponent<PlayerController>();
            if (p != null)
            {
                for (int i = 0; i < damage; i++)
                    p.TakeDamage();
            }
        }
    }

    public void TakeHit(Transform attacker)
    {
        if (isDead) return;

        if (killSound != null)
            audioSource.PlayOneShot(killSound);

        int dmg = 1;

        PlayerController p = attacker.GetComponent<PlayerController>();
        if (p != null)
            dmg += p.extraDamage;

        vida -= dmg;

        anim.SetTrigger("Hit");
        ApplyKnockback(attacker);

        if (vida <= 0)
            Die();
    }

    void ApplyKnockback(Transform attacker)
    {
        Vector2 direction = (transform.position - attacker.position).normalized;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(direction.x * knockbackForce, knockbackUpForce), ForceMode2D.Impulse);
    }

    void Die()
    {
        Debug.Log("SLIME MUERTO");

        isDead = true;
        anim.SetTrigger("Death");

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        GivePowerUpToPlayer();

        FindObjectOfType<HUDController>().SumarSlime();

        Destroy(gameObject, 2f);
    }

    void GivePowerUpToPlayer()
    {
        Debug.Log("DANDO POWERUP AL PLAYER");

        if (player == null)
        {
            Debug.Log("PLAYER ES NULL");
            return;
        }

        PlayerPowerUps power = player.GetComponent<PlayerPowerUps>();
        if (power == null)
        {
            Debug.Log("PLAYER NO TIENE PlayerPowerUps");
            return;
        }

        power.GiveRandomPowerUp();
    }
}
