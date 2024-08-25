using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CustomiserUIManager : MonoBehaviour
{
    public GameObject InstrutionsView;
    public Image displayImage;              // The UI Image element where the picture will be displayed
    public TextMeshProUGUI displayText;    // The TextMeshPro element where the name will be displayed
    public Button leftArrow;              // Button for the left arrow
    public Button rightArrow;             // Button for the right arrow
    public Button saveAndContinueButton;  // Button for saving and continuing to the main scene

    public TMP_InputField playerNameInput;  // Input field for player's name
    public TMP_InputField libraryNameInput; // Input field for library name

    public Sprite[] images;          // Array to hold the images
    public string[] imageNames;      // Array to hold the image names

    private int currentIndex = 0;    // Current image index
    private ThemeManager themeManager;

    void Start()
    {
        themeManager = FindObjectOfType<ThemeManager>();

        if (images.Length != imageNames.Length)
        {
            Debug.LogError("The number of images and image names must be the same.");
            return;
        }

        // Add listeners to the arrow buttons and the save button
        leftArrow.onClick.AddListener(ShowPreviousImage);
        rightArrow.onClick.AddListener(ShowNextImage);

        // Initialize button state
        UpdateButtonInteractable();



        // Add listeners to input fields to check their values when changed
        playerNameInput.onValueChanged.AddListener(delegate { UpdateButtonInteractable(); });
        libraryNameInput.onValueChanged.AddListener(delegate { UpdateButtonInteractable(); });

        // Add listener to the button to call SaveAndContinue method
        saveAndContinueButton.onClick.AddListener(SaveAndContinue);

        // Display the first image and update the theme
        UpdateGallery();
    }

    void ShowPreviousImage()
    {
        currentIndex = (currentIndex - 1 + images.Length) % images.Length;
        UpdateGallery();
    }

    void ShowNextImage()
    {
        currentIndex = (currentIndex + 1) % images.Length;
        UpdateGallery();
    }

    void UpdateGallery()
    {
        if (images.Length > 0 && imageNames.Length > 0)
        {
            displayImage.sprite = images[currentIndex];
            displayText.text = imageNames[currentIndex];

            // Notify the ThemeManager of the new selected theme
            themeManager.SetSelectedTheme(imageNames[currentIndex]);
        }
    }

    void UpdateButtonInteractable()
    {
        bool isPlayerNameValid = !string.IsNullOrEmpty(playerNameInput.text);
        bool isLibraryNameValid = !string.IsNullOrEmpty(libraryNameInput.text);

        // Log the status of input fields and button
        Debug.Log($"Player Name Valid: {isPlayerNameValid}, Library Name Valid: {isLibraryNameValid}");
        Debug.Log($"Button Interactable: {isPlayerNameValid && isLibraryNameValid}");

        // Enable or disable the button based on input field values
        saveAndContinueButton.interactable = isPlayerNameValid && isLibraryNameValid;
    }

    void SaveAndContinue()
    {
        // Save the player's name and library name
        string playerName = playerNameInput.text;
        string libraryName = libraryNameInput.text;

        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.SetString("LibraryName", libraryName);

        // Load the main scene (replace "MainLibrary" with your scene name)
        SceneManager.LoadScene("MainLibrary");
    }

    public void OpenInstructions()
    {
        InstrutionsView.SetActive(true);
    }

    public void CloseInstructions()
    {
        InstrutionsView.SetActive(false);
    }
}
