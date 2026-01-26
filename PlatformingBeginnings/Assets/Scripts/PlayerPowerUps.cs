using UnityEngine;

public class PlayerPowerUps : MonoBehaviour
{
    [Header("PowerUp Levels")]
    public int jumpLevel = 0;
    public int speedLevel = 0;
    public int damageLevel = 0;

    [Header("Bonus por nivel")]
    public float jumpBonus = 1f;
    public float speedBonus = 0.5f;
    public int damageBonus = 1;

    private PlayerController player;

    void Start()
    {
        player = GetComponent<PlayerController>();

        // Cargar niveles guardados y evitar negativos
        jumpLevel = Mathf.Max(0, PlayerPrefs.GetInt("JumpLevel", 0));
        speedLevel = Mathf.Max(0, PlayerPrefs.GetInt("SpeedLevel", 0));
        damageLevel = Mathf.Max(0, PlayerPrefs.GetInt("DamageLevel", 0));

        // Aplicar stats desde los valores base
        player.jumpForce = player.baseJumpForce + jumpLevel * jumpBonus;
        player.speed = player.baseSpeed + speedLevel * speedBonus;
        player.extraDamage = player.baseExtraDamage + damageLevel * damageBonus;

        var hud = FindFirstObjectByType<HUDController>();
        if (hud != null)
            hud.ActualizarHUD();
    }

    public void GiveRandomPowerUp()
    {
        int random = Random.Range(0, 3);

        switch (random)
        {
            case 0:
                jumpLevel++;
                player.jumpForce = player.baseJumpForce + jumpLevel * jumpBonus;
                break;

            case 1:
                speedLevel++;
                player.speed = player.baseSpeed + speedLevel * speedBonus;
                break;

            case 2:
                damageLevel++;
                player.extraDamage = player.baseExtraDamage + damageLevel * damageBonus;
                break;
        }

        // Guardar niveles
        PlayerPrefs.SetInt("JumpLevel", jumpLevel);
        PlayerPrefs.SetInt("SpeedLevel", speedLevel);
        PlayerPrefs.SetInt("DamageLevel", damageLevel);
        PlayerPrefs.Save();

        var hud = FindFirstObjectByType<HUDController>();
        if (hud != null)
            hud.ActualizarHUD();
    }
}
