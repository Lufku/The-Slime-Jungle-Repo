using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI monedasText;
    public TextMeshProUGUI slimesText;
    public TextMeshProUGUI statsText;

    [Header("Referencias Player")]
    public PlayerController player;
    public PlayerPowerUps powerUps;

    private int slimesDerrotados;

    void Start()
    {


    }

    public void SumarSlime()
    {
        slimesDerrotados++;
        PlayerPrefs.SetInt("Slimes", slimesDerrotados);
        PlayerPrefs.Save();
        ActualizarHUD();

    }

    public void ActualizarHUD()
    {
        if (monedasText != null)
            monedasText.text = "Monedas: " + player.monedas;

        if (slimesText != null)
            slimesText.text = "Slimes: " + slimesDerrotados;

        if (statsText != null)
        {
            statsText.text =
                "Fuerza: " + player.extraDamage + "\n" +
                "Salto: " + player.jumpForce + "\n" +
                "Velocidad: " + player.speed + "\n" +
                "Vidas: " + player.vidas;
        }
    }

}
