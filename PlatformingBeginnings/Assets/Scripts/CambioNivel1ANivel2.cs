using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioNivel1ANivel2 : MonoBehaviour
{
	[SerializeField] float speed = 2f;
	[SerializeField] Transform puntoNivel1;
	[SerializeField] Transform puntoNivel2;

	private bool moviendo = false;

	void Start()
	{
		// Empieza en el nivel 1
		transform.position = puntoNivel1.position;
		moviendo = true;
	}

	void Update()
	{
		if (!moviendo) return;

		transform.position = Vector3.MoveTowards(
			transform.position,
			puntoNivel2.position,
			speed * Time.deltaTime
		);

		if (Vector3.Distance(transform.position, puntoNivel2.position) < 0.05f)
		{
			moviendo = false;
			SceneManager.LoadScene("Level2");
		}
	}
}
