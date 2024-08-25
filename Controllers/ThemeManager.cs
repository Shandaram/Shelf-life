using System.Linq;
using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance { get; private set; }

    // public AudioClip[] audioClips;  // Array to hold the audio clips for each theme
    public GameObject[] themeMusicPrefabs; // Array to hold the music prefabs for each theme
    private string selectedTheme;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        string selectedTheme = PlayerPrefs.GetString("SelectedTheme");
        Debug.Log($"Awake called, selected theme: {selectedTheme}");
        if (!string.IsNullOrEmpty(selectedTheme))
        {
            SetSelectedTheme(selectedTheme);
        }
        // // Optionally, set the initial theme
        // SetSelectedTheme(PlayerPrefs.GetString("SelectedTheme", "Citytown"));
    }

    public void SetSelectedTheme(string themeName)
    {
        selectedTheme = themeName;
        PlayerPrefs.SetString("SelectedTheme", selectedTheme);
        ApplySelectedTheme(selectedTheme);
        UpdateThemeMusic(selectedTheme);
    }

    public void ApplySelectedTheme(string selectedTheme)
    {
        // Apply the selected theme by enabling/disabling the correct sprites
        GameObject[] allThemeObjects = GameObject.FindGameObjectsWithTag("Citytown")
            .Concat(GameObject.FindGameObjectsWithTag("Valley"))
            .Concat(GameObject.FindGameObjectsWithTag("Capecat"))
            .ToArray();

        foreach (GameObject obj in allThemeObjects)
        {
            obj.SetActive(obj.CompareTag(selectedTheme));
        }
    }

     private void UpdateThemeMusic(string themeName)
    {
        int index = GetThemeIndex(themeName);
        Debug.Log("index:" + index);
        
        if (index >= 0 && index < themeMusicPrefabs.Length)
        {
             GameObject newMusicPrefab = themeMusicPrefabs[index];
        
        // Use FindObjectOfType and check if MusicManager.Instance is available
        MusicManager musicManager = FindObjectOfType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.UpdateMusicForTheme(newMusicPrefab, 1f); // Adjust fade duration as needed
        }
        else
        {
            Debug.LogError("MusicManager instance is not available.");
        }
        }
        else
        {
            Debug.LogWarning("No valid theme index found for theme: " + themeName);
        }
    }

    private int GetThemeIndex(string themeName)
    {
        return System.Array.FindIndex(themeMusicPrefabs, prefab => prefab.name == themeName);
    }
}












