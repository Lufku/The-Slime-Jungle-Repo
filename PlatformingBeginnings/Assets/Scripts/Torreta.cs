using UnityEngine;

public class Torreta : MonoBehaviour
{

	[Header("Rango de Detección")]
	[SerializeField] float RangoVision;
	[SerializeField] Transform player;

    bool jugadorDetectado = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

   void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, RangoVision);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
