using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryUI : MonoBehaviour
{
	// Volver a jugar (recarga el nivel anterior o principal)
	public void PlayAgain()
	{
		// Cambia "Level1" por el nombre de tu nivel si quieres
		SceneManager.LoadScene("Escena Level 1 Recuperada");
	}

	// Salir del juego
	public void SalirDelJuego()
	{
		Application.Quit();
		Debug.Log("Estás saliendo del juego"); // Para probar en el editor
	}
}