using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
	public void Info()
	{
		SceneManager.LoadScene("Info");
	}

	public void PlayGame()
	{
		SceneManager.LoadScene("Escena Level 1 Recuperada");
	}

	public void Controls()
	{
		SceneManager.LoadScene("Controls");
	}

	public void ExitGame()
	{
		Application.Quit();
		Debug.Log("Juego cerrado"); // Solo se ve en el editor
	}
}
