using UnityEngine;
using System.Collections.Generic;

public class Customer : MonoBehaviour
{
    public string firstName;
    public string lastName;
    public int age;
    public AudioClip talkingSound;
    public Sprite customerSprite; 
    public int initialPEP;
    public string initialDialogueKey;
    public int dayVisit;
    public bool isActivePatron;
    
    public TextAsset dialogueJsonFile; // Assign the dialogue JSON file in the Inspector

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    public int currentPEP { get; private set; }
    private Dictionary<string, DialogueNode> dialogues;
    public string[] desiredGenres;
    public string bookTitle;
    public string bookAuthor;
    public int bookPublished;

    private StatsManager statsManager;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        statsManager = FindAnyObjectByType<StatsManager>();
    }

    private void Start()
    {
        audioSource.clip = talkingSound;
        currentPEP = initialPEP;
        UpdateCustomerSprite();

        if (dialogueJsonFile != null)
        {
            LoadDialogueData(dialogueJsonFile);
        }
        else
        {
            Debug.LogError("Dialogue JSON file is not assigned.");
        }
    }

    private void UpdateCustomerSprite()
    {
        if (spriteRenderer != null && customerSprite != null)
        {
            spriteRenderer.sprite = customerSprite;
        }
    }

    private void OnValidate()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        UpdateCustomerSprite();
    }

    public void PlayTalkingSound()
    {
        if (audioSource != null && talkingSound != null)
        {
            audioSource.Play();
        }
    }

    public void UpdateSatisfactionPoints(int points)
    {
        currentPEP += points;
        statsManager.UpdatePEP(points);
    }

 
    
    // Load dialogue data from the provided TextAsset
    public void LoadDialogueData(TextAsset dialogueJson)
    {
        DialogueLoader dialogueLoader = FindObjectOfType<DialogueLoader>();
        if (dialogueLoader != null)
        {
            dialogueLoader.LoadDialogueData(dialogueJson);
            dialogues = dialogueLoader.GetDialogueData().nodes;
        }
        else
        {
            Debug.LogError("DialogueLoader not found.");
        }
    }

    public DialogueNode GetDialogue(string key)
    {
        if (dialogues.TryGetValue(key, out var dialogue))
        {
            return dialogue;
        }
        return null;
    }
}

