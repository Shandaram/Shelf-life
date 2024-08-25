using UnityEngine;

public class SpriteVisibility : MonoBehaviour
{
    public int startDay = 1; // The first day the sprite should be visible
    public int endDay = 1;   // The last day the sprite should be visible (inclusive)
    
    private SpriteRenderer spriteRenderer;
    private LevelManager levelManager;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        levelManager = FindObjectOfType<LevelManager>();

        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on this GameObject.");
            return;
        }

        UpdateSpriteVisibility();
    }

    void Update()
    {
        UpdateSpriteVisibility();
    }

    void UpdateSpriteVisibility()
    {
        int currentDay = levelManager.currentDay;

        // Check if the current day is within the specified range
        if (currentDay >= startDay && currentDay <= endDay)
        {
            spriteRenderer.enabled = true; // Make the sprite visible
        }
        else
        {
            spriteRenderer.enabled = false; // Hide the sprite
        }
    }
}
