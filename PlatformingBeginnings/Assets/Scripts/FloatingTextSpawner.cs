using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    public GameObject floatingTextPrefab;

    public void SpawnText(string message, Color color)
    {
        if (floatingTextPrefab == null) return;

        GameObject obj = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);

        FloatingText ft = obj.GetComponentInChildren<FloatingText>();
        if (ft != null)
            ft.SetText(message, color);
    }
}
