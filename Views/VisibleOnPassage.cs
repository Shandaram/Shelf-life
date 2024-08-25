using UnityEngine;

public class VisibleOnPassage : MonoBehaviour
{
    public SpriteRenderer spriteToControl; // The SpriteRenderer you want to show/hide
    public DialogueManager dialogueManager; // Reference to the DialogueManager
    public string passageKey; // Passage name to track

    void Update()
    {
        // Check if the passage has been played
        if (dialogueManager != null && dialogueManager.HasPassagePlayed(passageKey))
        {
            // Make the sprite visible
            if (spriteToControl != null)
            {
                spriteToControl.enabled = true; // Enable the SpriteRenderer
            }
        }
        else
        {
            // Optionally, you can disable the sprite if the passage hasn't been played
            if (spriteToControl != null)
            {
                spriteToControl.enabled = false; // Disable the SpriteRenderer
            }
        }
    }
}

