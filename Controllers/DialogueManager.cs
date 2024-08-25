using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;
    public GameObject[] responseButtons;
    public GameObject newCardPanel;

    private string displayName = "...";
    public CardManager cardManager;
    public ScrollRect dialogueScrollRect;

    private DialogueNode currentNode;
    private Dictionary<string, DialogueNode> allNodes;
    private string currentNodeKey;
    private bool nameRevealed = false;
    private bool isTyping = false;
    private bool skipTyping = false;
    private Coroutine typingCoroutine;
    private AudioSource customerAudioSource;

    private HashSet<string> playedPassages = new HashSet<string>(); // Store played passages


    public float typingSpeed = 0.05f;
    public float buttonPadding = 20f;
    public float fadeDuration = 0.5f;
    private float fastTypingSpeed = 0f;


    public PopupManager popupManager;
    public DayManager dayManager;
    private StatsManager statsManager;

    private UIManager uiManager;

    private CameraManager cameraManager;

    private BookManager bookManager;

    private DialogueLoader dialogueLoader;

    void Start()
    {
        statsManager = FindObjectOfType<StatsManager>();
        popupManager = FindObjectOfType<PopupManager>();
        uiManager = FindObjectOfType<UIManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        bookManager = FindObjectOfType<BookManager>();
        dialogueLoader = FindObjectOfType<DialogueLoader>();

        if (statsManager == null)
        {
            Debug.LogError("StatsManager not found in the scene.");
        }
    }


    // Modified StartDialogue method to take a Customer object instead of CustomerManager
    public void StartDialogue(string dialogueKey, Customer customer, System.Action<int> onResponse, System.Action onEnd)
    {
        if (customer.dialogueJsonFile == null)
        {
            Debug.LogError("Customer's dialogue JSON file is not assigned.");
            return;
        }


        if (dialogueLoader != null)
        {
            dialogueLoader.LoadDialogueData(customer.dialogueJsonFile);
            allNodes = dialogueLoader.GetDialogueData().nodes;
        }
        else
        {
            Debug.LogError("DialogueLoader not found.");
            return;
        }

        if (allNodes == null || !allNodes.ContainsKey(dialogueKey))
        {
            Debug.LogError($"Dialogue with key {dialogueKey} not found.");
            return;
        }

        currentNodeKey = dialogueKey;
        currentNode = allNodes[dialogueKey];
        nameRevealed = false;

        // Open the dialogue panel
        dialoguePanel.SetActive(true);

        // Ensure the audio starts from the beginning
        customerAudioSource = customer.GetComponent<AudioSource>();
        if (customerAudioSource != null)
        {
            customerAudioSource.volume = 0;
            customerAudioSource.Play();
            StartCoroutine(FadeAudio(customerAudioSource, fadeDuration, PlayerPrefs.GetFloat("SFXVolume", 1f)));
        }
        ShowNextSentence(customer, onResponse, onEnd);

    }

    public void ShowNextSentence(Customer customer, System.Action<int> onResponse, System.Action onEnd)
    {
        if (currentNode != null)
        {
            string firstName = customer.firstName;
            string lastName = customer.lastName;
            string contentText = currentNode.content;

            if (!nameRevealed && (contentText.ToLower().Contains(firstName.ToLower()) || contentText.ToLower().Contains(lastName.ToLower())))
            {
                displayName = firstName;
                nameRevealed = true;
            }
            else if (nameRevealed)
            {
                displayName = firstName;
            }
            else if (!nameRevealed)
            {
                displayName = "...";

            }

            // Stop any previous typing coroutine
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }


            if (customerAudioSource != null)
            {
                // Reset volume to max (or original level) before playing
                customerAudioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);

                if (!customerAudioSource.isPlaying)
                {
                    customerAudioSource.Play();
                }
            }
            // Start typing the text with the display name and play audio
            typingCoroutine = StartCoroutine(TypeText($"{displayName}: {currentNode.content}"));

            DisplayChoices(customer, onResponse, onEnd);
        }
        else
        {
            Debug.LogError("Current node is null.");
        }
    }

    private void DisplayChoices(Customer customer, System.Action<int> onResponse, System.Action onEnd)
    {
        if (currentNode == null)
        {
            Debug.LogError("Current node is null.");
            return;
        }
        // Deselect all buttons before updating the choices
        EventSystem.current.SetSelectedGameObject(null);
        int i = 0;
        foreach (var choice in currentNode.choices)
        {
            if (i >= responseButtons.Length)
            {
                Debug.LogWarning("Not enough response buttons to display all choices.");
                break;
            }

            var responseButton = responseButtons[i];
            if (responseButton != null)
            {
                responseButton.SetActive(true);

                TextMeshProUGUI buttonText = responseButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = choice.Value.text;
                    ResizeButton(responseButton.GetComponent<RectTransform>(), buttonText);
                }
                else
                {
                    Debug.LogWarning($"TextMeshPro component not found in response button {responseButton.name}");
                }


                string choiceTag = choice.Value.tag;
                string nextNodeKey = choice.Value.next;
                int responsePoints = choice.Value.pep;       // Direct access, since pep defaults to 0 if not provided
                int responsePointsREP = choice.Value.rep;    // Direct access, since rep defaults to 0 if not provided


                responseButton.GetComponent<Button>().onClick.RemoveAllListeners();
                responseButton.GetComponent<Button>().onClick.AddListener(() => HandleChoice(customer, responsePoints, responsePointsREP, nextNodeKey, choiceTag, onResponse, onEnd));
            }
            else
            {
                Debug.LogWarning($"Response button at index {i} is not assigned in the Inspector.");
            }

            i++;
        }

        for (; i < responseButtons.Length; i++)
        {
            if (responseButtons[i] != null)
            {
                responseButtons[i].SetActive(false);
            }
        }
    }

    public void HandleChoice(Customer customer, int responsePoints, int responsePointsREP, string nextNodeKey, string choiceTag, System.Action<int> onResponse, System.Action onEnd)
    {
        // Update PEP and REP based on the response
        statsManager.UpdatePEP(responsePoints);
        statsManager.UpdateREP(responsePointsREP);

        playedPassages.Add(currentNodeKey);

        // Check if the next node corresponds to a popup
        if (popupManager.IsPopup(nextNodeKey))
        {
            popupManager.ShowPopup(nextNodeKey, (selectedNextNodeKey) =>
            {
                if (selectedNextNodeKey == "END")
                {
                    onResponse?.Invoke(responsePoints);
                    EndDialogue(onEnd);
                }
                else
                {
                    ContinueDialogue(customer, selectedNextNodeKey, onResponse, onEnd);
                }
            });
        }
        else
        {
            // Check if the current node's tag indicates that a new card should be created
            if (!string.IsNullOrEmpty(choiceTag) && choiceTag == "NEW_CARD")
            {
                // Show the new card panel using CardManager
                cardManager.ShowNewCardPanel(customer);

                // Start a coroutine to wait for the card creation process to finish
                StartCoroutine(WaitForCardCreation(() =>
                {
                    // Handle the end of the dialogue or continue based on the next node key
                    if (nextNodeKey == "END")
                    {
                        onResponse?.Invoke(responsePoints);
                        EndDialogue(onEnd);
                    }
                    else
                    {
                        onResponse?.Invoke(responsePoints);
                        ContinueDialogue(customer, nextNodeKey, onResponse, onEnd);
                    }
                }));
            }
            else if (!string.IsNullOrEmpty(choiceTag) && choiceTag == "COMPUTER")
            {
                // Redirect to the PC view using UIManager function
                cameraManager.SwitchToPCView();
                uiManager.ShowPCView();
                // Enable the "Lend Book" button by setting the flag
                bookManager.SetFromCustomer(true);

                // Use the book object to insert in the text passage
                if (customer == null) return;

                // Call the coroutine to wait for book lending
                StartCoroutine(WaitForBookLending(() =>
                {


                    // Continue the dialogue with the updated content
                    if (nextNodeKey == "END")
                    {
                        onResponse?.Invoke(responsePoints);
                        EndDialogue(onEnd);
                    }
                    else
                    {
                        onResponse?.Invoke(responsePoints);
                        ContinueDialogue(customer, nextNodeKey, onResponse, onEnd);
                    }
                }));
            }
            else if (!string.IsNullOrEmpty(choiceTag) && choiceTag == "NEW_CUSTOMER")
            {

                // Continue the dialogue with the updated content
                if (nextNodeKey == "END")
                {
                    onResponse?.Invoke(responsePoints);
                    EndDialogue(onEnd);
                }
                else
                {
                    onResponse?.Invoke(responsePoints);
                    ContinueDialogue(customer, nextNodeKey, onResponse, onEnd);
                }
                dayManager.SpawnSpecificCustomer("Yusuf");
            }
            else
            {
                // If the next node is the end, conclude the dialogue
                if (nextNodeKey == "END")
                {
                    onResponse?.Invoke(responsePoints);
                    EndDialogue(onEnd);
                }
                else
                {
                    // Otherwise, continue the dialogue
                    onResponse?.Invoke(responsePoints);
                    ContinueDialogue(customer, nextNodeKey, onResponse, onEnd);
                }
            }
        }
    }

    public bool HasPassagePlayed(string passageKey)
    {
        return playedPassages.Contains(passageKey);
    }

    private void ReplacePlaceholders(ref string content, Customer customer)
    {
        if (customer == null) return;

        content = content.Replace("[book name]", customer.bookTitle ?? "This Book");
        content = content.Replace("[book author]", customer.bookAuthor ?? "This Author");
        // content = content.Replace("[book published]", customer.bookPublished?.ToString() ?? "This Date");
    }
    private IEnumerator WaitForCardCreation(System.Action onCardCreated)
    {
        // Wait until the card creation panel is closed
        while (cardManager.IsCardCreationPanelOpen())
        {
            yield return null; // Wait for the next frame
        }
        onCardCreated?.Invoke(); // Invoke the callback once the card creation is done
    }

    private IEnumerator WaitForBookLending(System.Action onBookLent)
    {
        // Wait until the PC view panel is closed
        while (uiManager.IsPCViewPanelOpen())
        {
            yield return null; // Wait for the next frame
        }
        onBookLent?.Invoke(); // Invoke the callback once the panel is closed
    }



    public void ContinueDialogue(Customer customer, string nextNodeKey, System.Action<int> onResponse, System.Action onEnd)
    {
        if (allNodes.ContainsKey(nextNodeKey))
        {

            // Fetch the node data for the next key
            var nextNode = allNodes[nextNodeKey];

            // Fetch the content for the next node
            var nextNodeContent = nextNode.content;

            // Replace placeholders in the next node's content
            ReplacePlaceholders(ref nextNodeContent, customer);

            // Update the node content with replaced placeholders
            nextNode.content = nextNodeContent;

            // Set the current node to the next node
            currentNode = nextNode;
            ShowNextSentence(customer, onResponse, onEnd);
        }
        else
        {
            Debug.LogError($"Node with key {nextNodeKey} not found.");
        }
    }

    private void EndDialogue(System.Action onEnd)
    {
        dialoguePanel.SetActive(false);

        onEnd?.Invoke();
    }

    private IEnumerator TypeText(string message)
    {
        dialogueText.text = "";
        isTyping = true;
        skipTyping = false;
        float currentTypingSpeed = typingSpeed;

        foreach (char letter in message.ToCharArray())
        {
            if (skipTyping)
            {
                currentTypingSpeed = fastTypingSpeed;

            }

            dialogueText.text += letter;
            Canvas.ForceUpdateCanvases();
            yield return new WaitForSeconds(currentTypingSpeed);
        }

        isTyping = false;

        // Ensure the scroll rect stays at the top even if we skip typing
        Canvas.ForceUpdateCanvases();
        dialogueScrollRect.verticalNormalizedPosition = 0f;

        if (customerAudioSource != null)
        {
            StartCoroutine(FadeAudio(customerAudioSource, fadeDuration, 0f));
        }
    }

    private IEnumerator FadeAudio(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float startVolume = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            yield return null;
        }

        audioSource.volume = targetVolume;

        if (targetVolume == 0)
        {
            audioSource.Stop();
        }
    }

    private void ResizeButton(RectTransform buttonRectTransform, TextMeshProUGUI buttonText)
    {
        buttonText.ForceMeshUpdate();
        Vector2 textSize = buttonText.GetRenderedValues(false);
        buttonRectTransform.sizeDelta = new Vector2(buttonRectTransform.sizeDelta.x, textSize.y + buttonPadding);
    }

    public void OnDialoguePanelClick()
    {
        if (isTyping)
        {
            skipTyping = true;
        }
    }

    public bool IsDialogueActive()
    {
        return dialoguePanel.activeSelf;
    }
}











