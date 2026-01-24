using UnityEngine;
using UnityEngine.SceneManagement;

public class InfoMenu : MonoBehaviour
{
	// Estos métodos se asignan a los botones desde el Inspector

	public void PlayGame()
	{
		SceneManager.LoadScene("Escena Level 1 Recuperada");
	}

	public void Controls()
	{
		SceneManager.LoadScene("Controls");
	}

	public void Menu()
	{
		SceneManager.LoadScene("Menu");
	}
}
