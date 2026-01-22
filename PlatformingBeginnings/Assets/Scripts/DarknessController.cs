using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DarknessController : MonoBehaviour
{
    [Header("Screen Visibililty")]
    public GameObject smallScreen;
    public GameObject mediumScreen;
    public GameObject bigScreen;
    public GameObject giantScreen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdateScreenIcon(int monedas)
    {

        Debug.Log("UpdateScreenIcon llamado con monedas: " + monedas);

        if (SceneManager.GetActiveScene().name != "Level2") return;
        this.claeanScreen();

        if (monedas == 0) smallScreen?.SetActive(true);
        else if (monedas == 1) mediumScreen?.SetActive(true);
        else if (monedas == 2) bigScreen?.SetActive(true);
        else if (monedas == 3) giantScreen?.SetActive(true);
        else this.claeanScreen();
        
    }
    private void claeanScreen()
    {
        smallScreen?.SetActive(false);
        mediumScreen?.SetActive(false);
        bigScreen?.SetActive(false);
        giantScreen?.SetActive(false);
    }
}
