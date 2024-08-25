using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public SpriteRenderer pictureToFade; // The picture whose opacity will change
    public SpriteRenderer squareSprite;  // The sprite whose color will change

    public Color originalColor = Color.white; // The original color for the sprite
    public Color middleColor = Color.yellow;  // The middle color for the sprite
    public Color targetColor = Color.red;     // The target color for the sprite
    public float changeEndPercentage = 0.8f;  // Percentage of time after which changes stop

    // Assign these in the Inspector
    public Sprite cityImage;
    public Sprite cottageImage;
    public Sprite coastImage;

    private float dayDuration;
    private float elapsedTime = 0f;
    private bool isRunning = false;
    private bool isOriginalColorSet = false;  // Track if the original color has been set
    private string currentTheme; // Variable to store the current theme

    private void Start()
    {
        // Initialize theme from PlayerPrefs or another source
        currentTheme = PlayerPrefs.GetString("SelectedTheme", "Citytown");
        ApplyTheme(currentTheme);
    }

    public void StartTimer()
    {
        DayManager dayManager = FindObjectOfType<DayManager>();
        dayDuration = dayManager.dayDuration;
        elapsedTime = 0f; // Reset elapsed time at the start of each day
        isRunning = true;

        // Set initial alpha of the picture to 0
        Color pictureColor = pictureToFade.color;
        pictureColor.a = 0f;
        pictureToFade.color = pictureColor;

        // Set the sprite color to the original color at the start
        if (!isOriginalColorSet)
        {
            originalColor = squareSprite.color;
            isOriginalColorSet = true;
        }
        squareSprite.color = originalColor; // Reset to original color
    }


     public void ExtendDay(float additionalTimeInSeconds)
    {
        if (additionalTimeInSeconds > 0)
        {
            dayDuration += additionalTimeInSeconds;
            elapsedTime = Mathf.Clamp(elapsedTime, 0, dayDuration); // Adjust elapsed time if it exceeds the new duration
            isRunning = true;
            Debug.Log("Day extended by " + additionalTimeInSeconds + " seconds.");
        }
    }

    private void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            float remainingTime = dayDuration - elapsedTime;
            UpdateTimerText(remainingTime);

            // Calculate the percentage of the duration completed
            float t = Mathf.Clamp01(elapsedTime / (dayDuration * changeEndPercentage));

            // Fade in the picture
            Color pictureColor = pictureToFade.color;
            pictureColor.a = t; // Alpha goes from 0 to 1 over the duration
            pictureToFade.color = pictureColor;

            if (t < 0.5f)
            {
                // Interpolate from the original color to the middle color
                squareSprite.color = Color.Lerp(originalColor, middleColor, t * 2f);
            }
            else
            {
                // Interpolate from the middle color to the target color
                squareSprite.color = Color.Lerp(middleColor, targetColor, (t - 0.5f) * 2f);
            }

            if (remainingTime <= 0)
            {
                isRunning = false;
                // Notify the DayManager that the day has ended
                FindObjectOfType<DayManager>().EndDay();
            }
        }
    }

    private void UpdateTimerText(float remainingTime)
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Method to apply the selected theme
    private void ApplyTheme(string theme)
    {
        switch (theme)
        {
            case "Citytown":
                pictureToFade.sprite = cityImage;
                Debug.Log("Theme applied: City");
                break;
            case "Valley":
                pictureToFade.sprite = cottageImage;
                Debug.Log("Theme applied: Cottage");
                break;
            case "Capecat":
                pictureToFade.sprite = coastImage;
                Debug.Log("Theme applied: Coast");
                break;
            default:
                Debug.LogWarning("Theme not recognized, using default image.");
                pictureToFade.sprite = null; // Or assign a default image
                break;
        }

        if (pictureToFade.sprite == null)
        {
            Debug.LogError("No sprite assigned to pictureToFade.");
        }
        else
        {
            Debug.Log("Sprite assigned: " + pictureToFade.sprite.name);
        }
    }
}





