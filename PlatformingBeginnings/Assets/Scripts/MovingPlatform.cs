using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
	[SerializeField] float speed;              // Velocidad de la plataforma
	[SerializeField] int startingPoint;        // Índice inicial
	[SerializeField] Transform[] points;       // Puntos de destino

	private int i;

	void Start()
	{
		// Coloca la plataforma en el punto inicial
		transform.position = points[startingPoint].position;
		i = startingPoint;
	}

	void Update()
	{
		// Si está cerca del punto actual
		if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
		{
			i++; // Siguiente punto

			if (i == points.Length)
			{
				i = 0; // Vuelve al inicio
			}
		}

		// Movimiento de la plataforma
		transform.position = Vector2.MoveTowards(
			transform.position,
			points[i].position,
			speed * Time.deltaTime
		);
	}
}
