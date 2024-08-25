using UnityEngine;
using UnityEngine.UI;


public class VolumeControlByTag : MonoBehaviour
{
    public Slider volumeSlider; // Reference to the slider
    public string audioSourceTag; // Tag used to identify audio sources

    private AudioManager audioManager;

    void Start()
    {
        // Find the AudioManager instance
        audioManager = AudioManager.Instance;
        if (audioManager == null)
        {
            Debug.LogError("AudioManager instance not found.");
            return;
        }

        // Initialize the slider's value to reflect the current volume
        InitializeSlider();

        if (volumeSlider != null)
        {
            // Add a listener to handle changes to the slider's value
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    void InitializeSlider()
    {
        if (audioManager == null || volumeSlider == null) return;

        // Load the current volume from PlayerPrefs
        float currentVolume = PlayerPrefs.GetFloat(audioSourceTag + "Volume", 1f);

        // Set the slider's value to match the current volume
        volumeSlider.value = currentVolume;
    }

    void SetVolume(float volume)
    {
        if (audioManager == null) return;

        // Update volume in AudioManager
        switch (audioSourceTag)
        {
            case "Music":
                audioManager.SetVolumeForTag("Music", volume);
                break;
            case "SFX":
                audioManager.SetVolumeForTag("SFX", volume);
                break;
            case "Ambient":
                audioManager.SetVolumeForTag("Ambient", volume);
                break;
            default:
                Debug.LogWarning("Unknown audioSourceTag: " + audioSourceTag);
                break;
        }

        // Save the volume setting via PlayerPrefs
        PlayerPrefs.SetFloat(audioSourceTag + "Volume", volume);
    }
}





