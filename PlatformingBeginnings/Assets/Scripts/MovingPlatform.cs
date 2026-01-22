using UnityEngine;
using UnityEngine.SceneManagement;

public class MovingPlatform : MonoBehaviour
{
	[SerializeField] float speed;        // Velocidad de la plataforma
	[SerializeField] int startingPoint;  // Índice inicial
	[SerializeField] Transform[] points; // Puntos de destino

	private int i;
	private bool sceneLoaded = false;

	void Start()
	{
		transform.position = points[startingPoint].position;
		i = startingPoint;
	}

	void Update()
	{
		// Evita que cargue la escena varias veces
		if (sceneLoaded)
			return;

		// Si está cerca del punto actual, pasar al siguiente
		if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
		{
			i++;

			// ⬇️ CUANDO LLEGA AL FINAL
			if (i >= points.Length)
			{
				sceneLoaded = true;
				SceneManager.LoadScene("Escena Level 1 Recuperada");
				return;
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
