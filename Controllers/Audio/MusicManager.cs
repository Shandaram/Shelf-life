using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    public GameObject musicPrefab;   // Assign your music prefab here
    private GameObject currentMusicObject;
    private AudioSource currentMusicSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeMusic();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeMusic()
    {
        if (currentMusicObject == null && musicPrefab != null)
        {
            Debug.Log("Initializing music with prefab: " + musicPrefab.name);
            currentMusicObject = Instantiate(musicPrefab);
            currentMusicSource = currentMusicObject.GetComponent<AudioSource>();
            if (currentMusicSource != null)
            {
                currentMusicSource.Play();
                currentMusicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            }
        }
    }


    public void ReplaceMusic(GameObject newMusicPrefab)
    {
        if (currentMusicObject != null)
        {
            StartCoroutine(FadeOutAndReplaceMusic(newMusicPrefab));
        }
        else
        {
            // Instantiate new music if there's no existing music
            currentMusicObject = Instantiate(newMusicPrefab);
            currentMusicSource = currentMusicObject.GetComponent<AudioSource>();
            if (currentMusicSource != null)
            {
                currentMusicSource.Play(); // Start playing immediately
                currentMusicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f); // Restore saved volume
            }
        }
    }


    private IEnumerator FadeOutAndReplaceMusic(GameObject newMusicPrefab)
    {
        if (currentMusicSource == null) yield break;
        Debug.Log("Fading out current music...");
        float fadeDuration = 1f;
        float startVolume = currentMusicSource.volume;

        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            currentMusicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }
        currentMusicSource.volume = 0;
        Destroy(currentMusicObject);
        Debug.Log("Replacing music with prefab: " + newMusicPrefab.name);
        // Instantiate new music object
        currentMusicObject = Instantiate(newMusicPrefab);
        currentMusicSource = currentMusicObject.GetComponent<AudioSource>();
        if (currentMusicSource != null)
        {
            currentMusicSource.volume = 0; // Ensure new music starts at zero volume
            currentMusicSource.Play(); // Start playing the new music immediately

            // Fade in
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                currentMusicSource.volume = Mathf.Lerp(0, 1f, t / fadeDuration);
                yield return null;
            }
            currentMusicSource.volume = 1f;
        }
    }
 public void UpdateMusicForTheme(GameObject newMusicPrefab, float fadeDuration)
    {
        if (currentMusicObject != null)
        {
            StartCoroutine(FadeOutAndReplaceMusic(newMusicPrefab));
        }
        else
        {
            currentMusicObject = Instantiate(newMusicPrefab);
            currentMusicSource = currentMusicObject.GetComponent<AudioSource>();
            StartCoroutine(FadeInMusic(fadeDuration));
        }
    }

    public void StartFadeInMusic(float fadeDuration)
    {
        if (currentMusicSource != null)
        {
            StartCoroutine(FadeInMusic(fadeDuration));
        }
    }

    IEnumerator FadeInMusic(float fadeDuration)
    {
        float targetVolume = 1f;
        currentMusicSource.volume = 0f; // Ensure the volume starts at 0
        currentMusicSource.Play(); // Start playing the music immediately

        float timer = 0f;

        while (timer <= fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            // Fade in the music smoothly
            currentMusicSource.volume = Mathf.Lerp(0f, targetVolume, t);

            yield return null;
        }

        // Ensure final volume
        currentMusicSource.volume = targetVolume;
    }

}
