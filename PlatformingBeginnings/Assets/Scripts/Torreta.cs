using UnityEngine;

public class Torreta : MonoBehaviour
{
    [Header("Rango de Detección")]
    [SerializeField] float RangoVision;
    [SerializeField] Transform player;

    bool jugadorDetectado = false;
    bool jugadorenRango;
    bool disparoAutomatico;


    [Header("Disparo de la Torreta")]
    public GameObject bala;
    public Transform spawn;

    void Update()
    {
        deteccion();
        BalaAutomatica();
        
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
        Instantiate(bala, spawn.position, spawn.rotation);
        Debug.Log("Bala Disparada");
    }

    void BalaAutomatica()
    {      
       if (jugadorDetectado && !disparoAutomatico) 
       {
          InvokeRepeating("Disparo", 0.5f, 0.5f);
          disparoAutomatico = true;
       }

        else if (!jugadorenRango && disparoAutomatico)

        {
            CancelInvoke("Disparo");
            disparoAutomatico = false;
        }
    }
}
