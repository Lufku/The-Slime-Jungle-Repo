using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Slime"))
        {
            SlimesController slime = collision.GetComponent<SlimesController>();
            if (slime == null)
            {
                // Por si el collider está en un hijo
                slime = collision.GetComponentInParent<SlimesController>();
            }

            if (slime != null)
            {
                // Pasamos el player como atacante para el knockback
                slime.TakeHit(transform.root);
            }
        }
    }
}
