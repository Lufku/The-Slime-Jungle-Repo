using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public void IrAlMenu()
    {
        SceneManager.LoadScene("Info");
    }

    public void Reintentar()
    {
        // Recuperar la escena donde murió el jugador
        string last = PlayerPrefs.GetString("LastLevel", "Escena Level 1 Recuperada");

        // Cargar exactamente esa escena
        SceneManager.LoadScene(last);
    }

    public void SalirDelJuego()
    {
        Application.Quit();
        Debug.Log("Estás saliendo del juego");
    }
}
