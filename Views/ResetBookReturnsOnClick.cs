using UnityEngine;

public class ResetBookReturnsOnClick : MonoBehaviour
{
    private StatsManager statsManager;

    private void Start()
    {
        statsManager = FindObjectOfType<StatsManager>(); // Initialize StatsManager reference
    }

    private void OnMouseDown()
    {
        statsManager.ResetBookReturns(); // Reset bookReturns when the sprite is clicked
    }
}
