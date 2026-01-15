using UnityEngine;

public class MovimientoBala : MonoBehaviour
{
    [SerializeField] float speed;
    Rigidbody2D rbbala;

    void Start()
    {
        rbbala = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 3f);
    }

    void FixedUpdate()
    {
        rbbala.MovePosition(transform.position + transform.right * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.tag == ("Player"))
        {
            Destroy(gameObject);
            Debug.Log("He sido golpeado por una bala");
        }
    }
}
