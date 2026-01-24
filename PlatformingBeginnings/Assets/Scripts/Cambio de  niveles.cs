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
		// Si ya terminó el recorrido, NO mover más
		if (sceneLoaded) return;

		// Si ya no hay más puntos
		if (i >= points.Length)
		{
			sceneLoaded = true;
			return;
		}

		// Mover hacia el punto actual
		transform.position = Vector3.MoveTowards(
			transform.position,
			points[i].position,
			speed * Time.deltaTime
		);

		// ¿Ha llegado al punto?
		if (Vector3.Distance(transform.position, points[i].position) < 0.05f)
		{
			i++;

			// Si este era el último punto → parar y cambiar escena
			if (i >= points.Length)
			{
				sceneLoaded = true;
				SceneManager.LoadScene("Escena Level 1 Recuperada");
			}
		}
	}


}
