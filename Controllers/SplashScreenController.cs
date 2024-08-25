using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class SplashScreenEffect : MonoBehaviour
{
    public Image logoImage;               // The logo image to scale and fade in
    public Image whiteOverlay;            // The white overlay image for the exposure effect
    public Image movingPanel;             // The panel that should move up
    public Image fadingPanel;
    public Image secondWhiteOverlay;      // The second white overlay panel for opacity increase
    public TMP_Text typewriterText;       // The TextMeshPro text to appear in typewriter style
    public TMP_Text secondText;           // The second TMP_Text element that fades in after typewriter effect
    private AudioManager audioManager;
    public float logoFadeInTime = 2f;     // Time for logo to fade in and scale up
    public float exposureTime = 2f;       // Time for the exposure effect to intensify
    public float holdTime = 3f;           // Time to hold the fully white screen
    public float panelMoveTime = 2f;      // Time for the panel to move up
    public float secondOverlayFadeTime = 2f; // Time for the second overlay to fade in
    public float secondOverlayDelay = 1f; // Delay before the second overlay starts fading in
    public float typewriterSpeed = 0.05f; // Speed of the typewriter effect
    public float secondTextFadeTime = 1f; // Time for the second text to fade in
    public float fadingPanelFadeTime = 2f;
    public string mainMenuScene = "StartingScene"; // Name of the scene to load after the splash

    private Vector3 panelStartPos;        // Starting position of the moving panel
    private Vector3 panelEndPos;          // Ending position of the moving panel

    void Start()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
        panelStartPos = movingPanel.transform.localPosition;
        panelEndPos = new Vector3(panelStartPos.x, panelStartPos.y + 300f, panelStartPos.z); // Adjust 300f to how far you want the panel to move up
        typewriterText.text = ""; // Clear the text initially
        secondText.color = new Color(secondText.color.r, secondText.color.g, secondText.color.b, 0f); // Set the second text to be invisible initially
        fadingPanel.color = new Color(fadingPanel.color.r, fadingPanel.color.g, fadingPanel.color.b, 1f); // Set the fading panel to be fully visible initially

        audioManager.ResetVolumePreferences();
        // Ensure the music source object is managed by MusicManager
        if (MusicManager.Instance != null)
        {
            // Start fading in the music immediately
            MusicManager.Instance.StartFadeInMusic(2f); // Adjust the fade duration as needed
        }
        else
        {
            Debug.LogWarning("MusicManager is not available.");
        }

        // Start the animation sequence
        StartCoroutine(PlaySplashScreen());
    }

    IEnumerator PlaySplashScreen()
    {
        // Step 0: Fade out the fading panel
        yield return StartCoroutine(FadeOutFadingPanel());

        // Step 1: Fade in and scale up the logo
        yield return StartCoroutine(FadeInAndScaleUpLogo());

        // Step 2: Intensify the exposure (increase white overlay)
        yield return StartCoroutine(IntensifyExposure());

        // Step 3: Start moving the panel up and the typewriter effect simultaneously
        StartCoroutine(MovePanelUp());
        yield return StartCoroutine(TypewriterEffect());

        // Step 4: Fade in the second text after the typewriter effect
        yield return StartCoroutine(FadeInSecondText());

        // Step 5: Wait for the specified delay before fading in the second white overlay
        yield return new WaitForSeconds(secondOverlayDelay);

        // Step 6: Fade in the second white overlay
        yield return StartCoroutine(FadeInSecondOverlay());

        // Step 7: Hold the screen for a moment
        yield return new WaitForSeconds(holdTime);

        // Step 8: Transition to the main menu scene
        SceneManager.LoadScene(mainMenuScene);
    }

    IEnumerator FadeOutFadingPanel()
    {
        float timer = 0f;
        Color panelColor = fadingPanel.color;

        while (timer <= fadingPanelFadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadingPanelFadeTime;

            // Decrease the opacity of the fading panel
            fadingPanel.color = new Color(panelColor.r, panelColor.g, panelColor.b, 1f - t);

            yield return null;
        }

        // Ensure final opacity
        fadingPanel.color = new Color(panelColor.r, panelColor.g, panelColor.b, 0f);
    }

    IEnumerator FadeInAndScaleUpLogo()
    {
        float timer = 0f;
        Color logoColor = logoImage.color;
        Vector3 initialScale = logoImage.transform.localScale;

        while (timer <= logoFadeInTime)
        {
            timer += Time.deltaTime;
            float t = timer / logoFadeInTime;

            // Scale and fade in the logo
            logoImage.transform.localScale = Vector3.Lerp(Vector3.zero, initialScale, t);
            logoImage.color = new Color(logoColor.r, logoColor.g, logoColor.b, t);

            yield return null;
        }

        // Ensure final values
        logoImage.transform.localScale = initialScale;
        logoImage.color = new Color(logoColor.r, logoColor.g, logoColor.b, 1f);
    }


    IEnumerator IntensifyExposure()
    {
        float timer = 0f;
        Color overlayColor = whiteOverlay.color;

        while (timer <= exposureTime)
        {
            timer += Time.deltaTime;
            float t = timer / exposureTime;

            // Increase the opacity of the white overlay
            whiteOverlay.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, t);

            yield return null;
        }

        // Ensure final values
        whiteOverlay.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, 1f);
    }


    IEnumerator MovePanelUp()
    {
        float timer = 0f;
        while (timer <= panelMoveTime)
        {
            timer += Time.deltaTime;
            float t = timer / panelMoveTime;

            // Move the panel up
            movingPanel.transform.localPosition = Vector3.Lerp(panelStartPos, panelEndPos, t);

            yield return null;
        }

        // Ensure final position
        movingPanel.transform.localPosition = panelEndPos;
    }

    IEnumerator FadeInSecondOverlay()
    {
        float timer = 0f;
        Color overlayColor = secondWhiteOverlay.color;

        while (timer <= secondOverlayFadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / secondOverlayFadeTime;

            // Increase the opacity of the second white overlay
            secondWhiteOverlay.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, t);

            yield return null;
        }

        // Ensure final opacity
        secondWhiteOverlay.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, 1f);
    }

    IEnumerator TypewriterEffect()
    {
        string fullText = "bad librarian studios"; // Replace with your actual text

        foreach (char c in fullText)
        {
            typewriterText.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }
    }

    IEnumerator FadeInSecondText()
    {
        float timer = 0f;
        Color textColor = secondText.color;

        while (timer <= secondTextFadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / secondTextFadeTime;

            // Fade in the second text
            secondText.color = new Color(textColor.r, textColor.g, textColor.b, t);

            yield return null;
        }

        // Ensure final opacity
        secondText.color = new Color(textColor.r, textColor.g, textColor.b, 1f);
    }
}




