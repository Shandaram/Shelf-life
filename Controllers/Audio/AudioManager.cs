using UnityEngine;
using System.Linq;

public class AudioManager : MonoBehaviour

{
    public static AudioManager Instance { get; private set; } // Singleton instance


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
    }
    private void Start()
    {
        SetInitialVolume();
    }

    private void SetInitialVolume()
    {
        // Initialize volume settings from PlayerPrefs
        UpdateVolumeForTag("SFX", PlayerPrefs.GetFloat("SFXVolume", 1f));
        UpdateVolumeForTag("Music", PlayerPrefs.GetFloat("MusicVolume", 1f));
        UpdateVolumeForTag("Ambient", PlayerPrefs.GetFloat("AmbientVolume", 1f));
    }

    private void UpdateVolumeForTag(string tag, float volume)
    {
        // Find all audio sources with the specified tag
        AudioSource[] audioSources = GameObject.FindGameObjectsWithTag(tag)
            .Select(go => go.GetComponent<AudioSource>())
            .Where(source => source != null)
            .ToArray();

        foreach (var source in audioSources)
        {
            source.volume = volume;
        }
    }

    public void SetVolumeForTag(string tag, float volume)
    {
        UpdateVolumeForTag(tag, volume);
        PlayerPrefs.SetFloat(tag + "Volume", volume);
    }

      public void ResetVolumePreferences()
    {
        // Reset volume settings for all audio categories
        PlayerPrefs.SetFloat("MusicVolume", 1f);
        PlayerPrefs.SetFloat("SFXVolume", 1f);
        PlayerPrefs.SetFloat("AmbientVolume", 1f);
        PlayerPrefs.Save(); // Ensure changes are saved to PlayerPrefs

        // Update the AudioMixer directly
        SetInitialVolume();
    }
}



