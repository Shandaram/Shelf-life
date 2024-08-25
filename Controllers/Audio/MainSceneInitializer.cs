using UnityEngine;
using System.Linq;

public class MainSceneInitializer : MonoBehaviour
{
    public AudioClip cityClip;     // Assign these in the Inspector
    public AudioClip cottageClip;
    public AudioClip coastClip;

    private void Start()
    {
        string selectedTheme = PlayerPrefs.GetString("SelectedTheme");
        Debug.Log(selectedTheme);
        if (!string.IsNullOrEmpty(selectedTheme))
        {
            ApplyTheme(selectedTheme);
        }
    }

    void ApplyTheme(string theme)
    {
        GameObject[] allThemeObjects = GameObject.FindGameObjectsWithTag("Citytown");
        allThemeObjects = allThemeObjects.Concat(GameObject.FindGameObjectsWithTag("Valley")).ToArray();
        allThemeObjects = allThemeObjects.Concat(GameObject.FindGameObjectsWithTag("Capecat")).ToArray();

        foreach (GameObject obj in allThemeObjects)
        {
            if (obj.CompareTag(theme))
            {
                obj.SetActive(true);
            }
            else
            {
                obj.SetActive(false);
            }
        }

        // Play the corresponding theme music
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        AudioSource audioSource = audioManager.GetComponent<AudioSource>();
        AudioClip themeMusic = FindClipForTheme(theme); // Find the correct AudioClip

        if (themeMusic != null && audioSource.clip != themeMusic)
        {
            audioSource.clip = themeMusic;
            audioSource.Play();
        }
    }

    AudioClip FindClipForTheme(string theme)
    {
        switch (theme)
        {
            case "Citytown":
                return cityClip;
            case "Valley":
                return cottageClip;
            case "Capecat":
                return coastClip;
            default:
                Debug.LogWarning("Theme not recognized, no audio clip found.");
                return null;
        }
    }
}
