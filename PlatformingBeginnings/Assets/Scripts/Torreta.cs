using UnityEngine;

public class Torreta : MonoBehaviour
{
    [Header("Rango de Detección")]
    [SerializeField] float RangoVision;
    [SerializeField] Transform player;

    bool jugadorDetectado = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        deteccion();
        //Disparo();
    }


	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, RangoVision);
	}

    void deteccion()
    {
        float distancia = Vector2.Distance(transform.position, player.position);

        if(distancia <= RangoVision && !jugadorDetectado){
        jugadorDetectado = true;
        Debug.Log("Jugador Detectado");
        }
        else if(distancia > RangoVision && jugadorDetectado){
            jugadorDetectado = false;
            Debug.Log("Salí del rango");
        }
    }
}
