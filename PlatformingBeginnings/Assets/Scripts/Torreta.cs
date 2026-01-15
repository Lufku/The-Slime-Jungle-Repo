using UnityEngine;

public class Torreta : MonoBehaviour
{
    [Header("Rango de Detección")]
    [SerializeField] float RangoVision;
    [SerializeField] Transform player;

    bool jugadorDetectado = false;
    bool jugadorenRango;

    [Header("Disparo de la Torreta")]
    public GameObject bala;
    public Transform spawn;

    void Update()
    {
        deteccion();
        Disparo();
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, RangoVision);
    }

    void deteccion()
    {
        float distancia = Vector2.Distance(transform.position, player.position);
        jugadorenRango = distancia <= RangoVision;

        if (distancia <= RangoVision && !jugadorDetectado)
        {
            jugadorDetectado = true;
            Debug.Log("Jugador Detectado");
        }
        else if (distancia > RangoVision && jugadorDetectado)
        {
            jugadorDetectado = false;
            Debug.Log("Salí del rango");
        }
    }

    void Disparo()
    {
        if (jugadorenRango && (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0)))
        {
            Instantiate(bala, spawn.position, spawn.rotation);
            Debug.Log("Bala Disparada");
        }
    }
}
