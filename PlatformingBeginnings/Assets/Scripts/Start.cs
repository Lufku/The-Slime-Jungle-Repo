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
        // Reset total al empezar partida nueva
        PlayerPrefs.DeleteAll();

        // Cargar el primer mapa
        SceneManager.LoadScene("Map");
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
