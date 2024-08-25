using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class AutoScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public TextMeshProUGUI dialogueText;

    private void Update()
    {
        // Automatically scroll to the bottom if text is still being typed
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }
}
