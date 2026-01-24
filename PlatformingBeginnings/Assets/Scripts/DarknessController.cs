using UnityEngine;
using UnityEngine.SceneManagement;

public class DarknessController : MonoBehaviour
{
    [Header("Screen Visibility")]
    public GameObject smallScreen;
    public GameObject mediumScreen;
    public GameObject bigScreen;
    public GameObject giantScreen;

    [Header("Player Reference")]
    public PlayerController player; // Arrastrar el Player en el inspector

    void Start()
    {
        CleanScreen();

        // Activar pantalla correcta según las monedas del Player
        if (player != null)
            UpdateScreenIcon(player.monedas);
    }

    /// <summary>
    /// Actualiza la pantalla según la cantidad de monedas.
    /// </summary>
    /// <param name="monedas">Cantidad de monedas actuales del Player</param>
    public void UpdateScreenIcon(int monedas)
    {

        // Solo se activa en Level2
        if (SceneManager.GetActiveScene().name != "Level2") return;

        CleanScreen();

        if (monedas <= 0)
            smallScreen?.SetActive(true);
        else if (monedas == 1)
            mediumScreen?.SetActive(true);
        else if (monedas == 2)
            bigScreen?.SetActive(true);
        else if (monedas == 3)
            giantScreen?.SetActive(true);

       // else // 4 o más monedas
         //   CleanScreen?.SetActive(true);
    }

    /// <summary>
    /// Apaga todas las pantallas
    /// </summary>
    private void CleanScreen()
    {
        smallScreen?.SetActive(false);
        mediumScreen?.SetActive(false);
        bigScreen?.SetActive(false);
        giantScreen?.SetActive(false);
    }
}
