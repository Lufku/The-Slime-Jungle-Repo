using UnityEngine;
using UnityEngine.SceneManagement;


public class CambioEscena : MonoBehaviour
{
    public void Cambioescena()
    {
        SceneManager.LoadScene("CopySampleScene");
    }

    public void Salida()
    {
        Debug.Log("Estas saliendo del juego");
        Application.Quit();
    }
}
