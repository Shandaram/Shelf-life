using UnityEngine;
using System.Collections;


public class ClickableObject : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Color hoverColor = Color.gray;  // The color to change to when hovered
    private Color originalColor;

    public float fadeDuration = 0.5f;  // Duration of the fade effect

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;  // Save the original color
    }

    void OnMouseEnter()
    {
        // Start fading to the hover color when the mouse enters the object
        StopAllCoroutines();  // Stop any ongoing fade effects
        StartCoroutine(FadeToColor(hoverColor));
    }

    void OnMouseExit()
    {
        // Start fading back to the original color when the mouse exits the object
        StopAllCoroutines();  // Stop any ongoing fade effects
        StartCoroutine(FadeToColor(originalColor));
    }

    private IEnumerator FadeToColor(Color targetColor)
    {
        Color currentColor = spriteRenderer.color;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(currentColor, targetColor, timer / fadeDuration);
            yield return null;
        }

        // Ensure the final color is set (in case the loop ends slightly before reaching the target color)
        spriteRenderer.color = targetColor;
    }
}

