using UnityEngine;

public class SpriteColorRestorer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        // Começa preto
        spriteRenderer.color = Color.black;
    }

    public void SetColorProgress(float progress)
    {
        progress = Mathf.Clamp01(progress); // entre 0 e 1
        spriteRenderer.color = Color.Lerp(Color.black, originalColor, progress);
    }
}
