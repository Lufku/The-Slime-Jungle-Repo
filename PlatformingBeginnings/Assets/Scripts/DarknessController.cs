using UnityEngine;
using UnityEngine.SceneManagement;

public class DarknessController : MonoBehaviour
{
    [Header("Screen Visibility")]
    public GameObject smallScreen;
    public GameObject mediumScreen;
    public GameObject bigScreen;
    public GameObject giantScreen;

    void Start()
    {
        CleanScreen();
    }

    public void UpdateScreenIcon(int monedas)
    {
        Debug.Log("UpdateScreenIcon llamado con monedas: " + monedas);

      
        if (SceneManager.GetActiveScene().name != "Level2") return;

        CleanScreen();

        if (monedas == 0)
            smallScreen?.SetActive(true);
        else if (monedas == 1)
            mediumScreen?.SetActive(true);
        else if (monedas == 2)
            bigScreen?.SetActive(true);
        else if (monedas >= 3)
            giantScreen?.SetActive(true);
    }

    private void CleanScreen()
    {
        smallScreen?.SetActive(false);
        mediumScreen?.SetActive(false);
        bigScreen?.SetActive(false);
        giantScreen?.SetActive(false);
    }
}