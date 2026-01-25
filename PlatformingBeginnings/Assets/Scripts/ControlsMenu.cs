using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsMenu : MonoBehaviour
{
	// Botón Play Game
	public void PlayGame()
	{
		SceneManager.LoadScene("Map");
	}

	// Botón Menu
	public void GoToMenu()
	{
		SceneManager.LoadScene("info");
	}
}
