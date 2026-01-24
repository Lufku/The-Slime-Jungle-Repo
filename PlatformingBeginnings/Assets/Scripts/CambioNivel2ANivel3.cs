using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioNivelCon3Puntos : MonoBehaviour
{
	[SerializeField] float speed = 2f;

	[SerializeField] Transform puntoInicio;
	[SerializeField] Transform puntoIntermedio;
	[SerializeField] Transform puntoFinal;

	private Transform[] puntos;
	private int indice = 0;
	private bool moviendo = true;

	void Start()
	{
		// Guardamos los puntos en orden
		puntos = new Transform[] { puntoInicio, puntoIntermedio, puntoFinal };

		// Empezar en el primer punto
		transform.position = puntos[0].position;
	}

	void Update()
	{
		if (!moviendo) return;

		// Mover hacia el punto actual
		transform.position = Vector3.MoveTowards(
			transform.position,
			puntos[indice].position,
			speed * Time.deltaTime
		);

		// ¿Ha llegado?
		if (Vector3.Distance(transform.position, puntos[indice].position) < 0.05f)
		{
			indice++;

			// Si ya pasó por todos los puntos
			if (indice >= puntos.Length)
			{
				moviendo = false;
				SceneManager.LoadScene("FinalLevel");
			}
		}
	}
}
