using UnityEngine;

public class Torreta : MonoBehaviour
{

	[Header("Rango de Detección")]
	[SerializeField] float RangoVision;
	[SerializeField] Transform player;

    bool jugadorDetectado = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, RangoVision);
	}

    void deteccion()
    {
        float distancia = Vector2.Distance(transform.position, player.position);

        if(distancia )
    }
}
