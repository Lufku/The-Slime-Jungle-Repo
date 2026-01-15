using UnityEngine;

[System.Diagnostics.DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class Torreta : MonoBehaviour
{
	[Header("Rango de Detección")]
	[SerializeField] float RangoVision;
	[SerializeField] Transform player;

	bool jugadorDetectado = false;
	bool jugadorenRango;

	[Header("Disparo de la Torreta")]
	public GameObject[] balas;   // Array de balas
	public Transform spawn;

	bool disparoAutomatico;

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

	void BalaAutomatica()
	{
		if (jugadorDetectado && !disparoAutomatico)
		{
			InvokeRepeating(nameof(DisparoRandom), 0.5f, 0.5f);
			disparoAutomatico = true;
		}
		else if (!jugadorDetectado && disparoAutomatico)
		{
			CancelInvoke(nameof(DisparoRandom));
			disparoAutomatico = false;
		}
	}

	void DisparoRandom()
	{
		int tipoBala = Random.Range(0, balas.Length);

		Instantiate(balas[tipoBala], spawn.position, spawn.rotation);

		switch (tipoBala)
		{
			case 0:
				Debug.Log("Bala Ligera Disparada (5 de daño)");
				break;
			case 1:
				Debug.Log("Bala Normal Disparada (10 de daño)");
				break;
			case 2:
				Debug.Log("Bala Pesada Disparada (20 de daño)");
				break;
		}
	}

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}