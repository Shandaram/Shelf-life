using UnityEngine;

public class ClickToShowPanel : MonoBehaviour
{
    // Reference to the UI panel that should appear when the object is clicked
    public GameObject panelToShow;

    // Optional: Should the panel toggle visibility on click? 
    // If false, it will always show the panel when clicked.
    public bool togglePanel = true;

    void OnMouseDown()
    {
        if (panelToShow != null)
        {
            if (togglePanel)
            {
                // Toggle the panel's active state
                panelToShow.SetActive(!panelToShow.activeSelf);
            }
            else
            {
                // Ensure the panel is active (visible)
                panelToShow.SetActive(true);
            }
        }
        else
        {
            Debug.LogWarning("No panel assigned to the ClickToShowPanel script on " + gameObject.name);
        }
    }
}
