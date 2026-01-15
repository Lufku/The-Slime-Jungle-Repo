using UnityEngine;

public class MovimientoBala : MonoBehaviour
{
    [SerializeField] float speed;
    Rigidbody2D rbbala;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rbbala = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        rbbala.MovePosition(transform.position + transform.right * speed * Time.fixedDeltaTime);
    }
}
