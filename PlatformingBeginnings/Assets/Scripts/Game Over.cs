using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
	// Volver al menú principal
	public void IrAlMenu()
	{
		SceneManager.LoadScene("Menu");
	}

	// Reintentar el nivel actual
	public void Reintentar()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
	}

	// Salir del juego
	public void SalirDelJuego()
	{
		Application.Quit();
		Debug.Log("Estás saliendo del juego"); // Para probar en el editor
	}
}
