using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioDeNiveles : MonoBehaviour
{
	[SerializeField] float speed = 2f;        // Velocidad del icono
	[SerializeField] int startingPoint = 0;   // Punto inicial
	[SerializeField] Transform[] points;      // Puntos del mapa

	private int i;
	private bool sceneLoaded = false;

	void Start()
	{
		// Comprobaciones de seguridad
		if (points == null || points.Length == 0)
		{
			Debug.LogError("No hay puntos asignados en el mapa.");
			enabled = false;
			return;
		}

		if (startingPoint < 0 || startingPoint >= points.Length)
		{
			Debug.LogError("StartingPoint fuera de rango.");
			enabled = false;
			return;
		}

		transform.position = points[startingPoint].position;
		i = startingPoint;
	}

	void Update()
	{
		if (sceneLoaded) return;

		// Si llega al punto actual
		if (Vector3.Distance(transform.position, points[i].position) < 0.02f)
		{
			i++;

			// Si llega al último punto → cambiar escena
			if (i >= points.Length)
			{
				sceneLoaded = true;
				SceneManager.LoadScene("Escena Level 1 Recuperada");
				return;
			}
		}

		// Movimiento del icono del jugador
		transform.position = Vector3.MoveTowards(
			transform.position,
			points[i].position,
			speed * Time.deltaTime
		);
	}
}
