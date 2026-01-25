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

        jumpLevel = PlayerPrefs.GetInt("JumpLevel", jumpLevel);
        speedLevel = PlayerPrefs.GetInt("SpeedLevel", speedLevel);
        damageLevel = PlayerPrefs.GetInt("DamageLevel", damageLevel);

        player.jumpForce += jumpLevel * jumpBonus;
        player.speed += speedLevel * speedBonus;
        player.extraDamage += damageLevel * damageBonus;

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
                player.jumpForce += jumpBonus;
                break;

            case 1:
                speedLevel++;
                player.speed += speedBonus;
                break;

            case 2:
                damageLevel++;
                player.extraDamage += damageBonus;
                break;
        }

        PlayerPrefs.SetInt("JumpLevel", jumpLevel);
        PlayerPrefs.SetInt("SpeedLevel", speedLevel);
        PlayerPrefs.SetInt("DamageLevel", damageLevel);
        PlayerPrefs.Save();

        var hud = FindFirstObjectByType<HUDController>();
        if (hud != null)
            hud.ActualizarHUD();
    }
}
