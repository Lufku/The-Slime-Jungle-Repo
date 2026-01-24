using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    public GameObject floatingTextPrefab;

    public void SpawnText(string message, Color color)
    {
        if (floatingTextPrefab == null) return;

        GameObject obj = Instantiate(floatingTextPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
        FloatingText ft = obj.GetComponent<FloatingText>();
        ft.SetText(message, color);
    }
}
