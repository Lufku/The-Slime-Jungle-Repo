using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 50f;
    public float lifetime = 1f;

    private TextMeshProUGUI textMesh;
    private RectTransform rectTransform;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (rectTransform != null)
            rectTransform.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;
    }

    public void SetText(string text, Color color)
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshProUGUI>();

        textMesh.text = text;
        textMesh.color = color;
    }
}
