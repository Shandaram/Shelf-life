using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject instructionsPanel;
    public GameObject creditsPanel;
    public SpriteRenderer fadingSprite;
    public float fadingSpriteFadeTime = 2f;


    void Start()
    {
        // Ensure the sprite starts fully visible
        if (fadingSprite != null)
        {
            Color spriteColor = fadingSprite.color;
            fadingSprite.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, 1f); // Set alpha to 1
        }
        StartCoroutine(FadeOutFadingSprite());
        ShowMainMenu();
        // Start the animation sequence

    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        instructionsPanel.SetActive(false);

    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("CustomizeLibrary");
    }

    public void LoadSavedGame()
    {
        // Implement your logic to load the saved game, for now, load the GameScene
        SceneManager.LoadScene("MainLibrary");
    }

    public void ShowSettingsPanel()
    {
        if (settingsPanel.activeSelf)
        {
            // If the settings panel is already active, deactivate it
            settingsPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
        else
        {
            // If the settings panel is not active, activate it and deactivate the credits panel
            settingsPanel.SetActive(true);
            creditsPanel.SetActive(false);
            mainMenuPanel.SetActive(false);
        }
    }


    public void ShowCreditsPanel()
    {
        if (creditsPanel.activeSelf)
        {
            // If the credit panel is already active, deactivate it
            creditsPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
        else
        {
            // If the credit panel is not active, activate it 
            creditsPanel.SetActive(true);
            settingsPanel.SetActive(false);
            mainMenuPanel.SetActive(false);
        }


    }
    public void CloseSettingsPanel()
    {

        settingsPanel.SetActive(false);

    }

    public void CloseCreditsPanel()
    {

        creditsPanel.SetActive(false);

    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true); // Show the settings panel
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false); // Hide the settings panel
    }

    public void OpenInstr()
    {
        instructionsPanel.SetActive(true); // Show the settings panel
    }

    public void CloseInstr()
    {
        instructionsPanel.SetActive(false); // Hide the settings panel
    }

    public void SaveGame()
    {
        // Implement your save logic here
        Debug.Log("Game Saved!");
    }

    public void ExitToMainMenu()
    {
        // Implement any cleanup logic here if needed
        SceneManager.LoadScene("StartingScreen"); // Replace with your main menu scene name
    }

    IEnumerator FadeOutFadingSprite()
    {
        float timer = 0f;
        Color spriteColor = fadingSprite.color;

        while (timer <= fadingSpriteFadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadingSpriteFadeTime;

            // Decrease the opacity of the fading sprite
            fadingSprite.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, 1f - t);

            yield return null;
        }

        // Ensure final opacity
        fadingSprite.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, 0f);
    }

    // public void QuitGame()
    // {
    //     Application.Quit();
    // }
}

