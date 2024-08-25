using UnityEngine;

public class ObjectPopupInteraction : MonoBehaviour
{
    public string popupName;  // Name of the popup to be triggered
    private PopupManager popupManager;

    private void Start()
    {
        popupManager = FindObjectOfType<PopupManager>();
    }

    private void OnMouseDown()
    {
        // Trigger the specific popup when the object is clicked
        popupManager.OpenSpecificPopup(popupName, this.gameObject);
    }
}
