using UnityEngine;

public class RotacionCañon : MonoBehaviour
{
    [SerializeField] Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Rotacion();
    }

    void Rotacion()
    {
        Vector2 direccion = player.position - transform.position;
        transform.right = direccion;
    }
}
