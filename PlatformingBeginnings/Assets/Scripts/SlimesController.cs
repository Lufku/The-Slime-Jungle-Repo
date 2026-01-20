using UnityEngine;
using System.Collections;

public class SlimesController : MonoBehaviour
{
    [Header("Stats")]
    public int vida = 3;
    public float speed = 2f;

    [Header("Detection")]
    public float detectionRange = 6f;
    public float attackRange = 1.5f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;

    private bool isDead = false;
    private bool isAttacking = false;
    private bool attackCooldown = false; // cooldown entre ataques

    [Header("Knockback")]
    public float knockbackForce = 10f;  // Aumentado
    public float knockbackUpForce = 4f; // Aumentado

    [Header("Attack")]
    public float swallowDuration = 1f;   // duración de la animación
    public float attackDelay = 2f;       // cooldown entre ataques en segundos

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Swallow si está cerca y no atacando y no en cooldown
        if (distance <= attackRange && !isAttacking && !attackCooldown)
            StartCoroutine(SwallowAttack());
        // Moverse hacia el player si está en rango de detección
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

        // Girar sprite
        transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
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

        // Espera antes de poder atacar otra vez
        yield return new WaitForSeconds(attackDelay);
        attackCooldown = false;
    }

    // ---------- DAÑO AL PLAYER ----------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead || !isAttacking) return;

        if (collision.CompareTag("Player"))
        {
            PlayerController playerScript = collision.transform.root.GetComponent<PlayerController>();
            if (playerScript != null)
                playerScript.TakeDamage();
        }
    }

    // ---------- DAÑO AL SLIME ----------
    public void TakeHit(Transform attacker)
    {
        if (isDead) return;

        vida--;
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
        isDead = true;
        anim.SetTrigger("Death");

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        Destroy(gameObject, 2f);
    }

    // ---------- GIZMOS ----------
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
