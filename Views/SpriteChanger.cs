using UnityEngine;
using System.Collections;
using System.Reflection;

public class SpriteChanger : MonoBehaviour
{
    public string targetPropertyName; // The name of the property (e.g., "bookCount")
    public float[] breakpoints; // Breakpoints to determine the phases
    public Sprite[] phaseSprites; // Sprites for each phase (must have between 2 and 4 sprites)
    public float fadeDuration = 1f; // Duration of the fade transition

    private SpriteRenderer spriteRenderer;
    private int currentPhase = -1;
    public MonoBehaviour targetObject; // Reference to the object with the target property

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (targetObject == null)
        {
            Debug.LogError("Target object with property not found!");
            return;
        }

        // Ensure that the number of breakpoints is valid relative to the number of sprites
        if (breakpoints.Length != phaseSprites.Length - 1)
        {
            Debug.LogError("Number of breakpoints must be one less than the number of phaseSprites!");
            return;
        }

        // Initial update
        UpdateSprite();
    }

    void Update()
    {
        UpdateSprite();
    }

    void UpdateSprite()
    {
        // Use reflection to get the value of the property
        PropertyInfo propertyInfo = targetObject.GetType().GetProperty(targetPropertyName);
        if (propertyInfo == null)
        {
            Debug.LogError($"Property {targetPropertyName} not found!");
            return;
        }

        float propertyValue = System.Convert.ToSingle(propertyInfo.GetValue(targetObject));

        // Determine the phase based on breakpoints
        int newPhase = GetPhase(propertyValue);

        // If the phase has changed, initiate a fade transition
        if (newPhase != currentPhase && newPhase >= 0 && newPhase < phaseSprites.Length)
        {
            StartCoroutine(FadeToSprite(phaseSprites[newPhase]));
            currentPhase = newPhase;
        }
    }

    int GetPhase(float value)
    {
        // Loop through breakpoints to determine which phase we're in
        for (int i = 0; i < breakpoints.Length; i++)
        {
            if (value < breakpoints[i])
            {
                return i;
            }
        }
        // If value exceeds all breakpoints, return the last phase
        return breakpoints.Length;
    }

    IEnumerator FadeToSprite(Sprite newSprite)
    {
        if (spriteRenderer == null)
            yield break;

        // Fade out the current sprite
        float timer = 0f;
        while (timer <= fadeDuration)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, timer / fadeDuration));
            timer += Time.deltaTime;
            yield return null;
        }

        // Switch the sprite
        spriteRenderer.sprite = newSprite;

        // If the new sprite is not null, fade it in
        if (newSprite != null)
        {
            timer = 0f;
            while (timer <= fadeDuration)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, Mathf.Lerp(0f, 1f, timer / fadeDuration));
                timer += Time.deltaTime;
                yield return null;
            }

            spriteRenderer.color = Color.white; // Ensure it's fully visible at the end
        }
        else
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0f); // Ensure it's fully transparent at the end
        }
    }
}


