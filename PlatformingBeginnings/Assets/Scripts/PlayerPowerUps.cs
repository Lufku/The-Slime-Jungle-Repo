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
    private FloatingTextSpawner textSpawner;

    void Start()
    {
        player = GetComponent<PlayerController>();
        textSpawner = GetComponent<FloatingTextSpawner>();
    }

    public void GiveRandomPowerUp()
    {
        int random = Random.Range(0, 3);

        switch (random)
        {
            case 0:
                jumpLevel++;
                player.jumpForce += jumpBonus;
                textSpawner.SpawnText("+Salto", Color.yellow);
                break;

            case 1:
                speedLevel++;
                player.speed += speedBonus;
                textSpawner.SpawnText("+Velocidad", Color.green);
                break;

            case 2:
                damageLevel++;
                player.extraDamage += damageBonus;
                textSpawner.SpawnText("+Daño", Color.red);
                break;
        }
    }
}
