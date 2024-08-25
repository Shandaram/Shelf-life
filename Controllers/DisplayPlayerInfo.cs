using UnityEngine;
using TMPro;

public class DisplayPlayerInfo : MonoBehaviour
{
    public TextMeshProUGUI playerNameText;   // Reference to the TextMeshProUGUI component for the player name sentence
    public TextMeshProUGUI libraryNameText;  // Reference to the TextMeshProUGUI component for the library name sentence

    void Start()
    {
        // Retrieve the player's name and library name from PlayerPrefs
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");  // Default to "Player" if not found
        string libraryName = PlayerPrefs.GetString("LibraryName", "Library");  // Default to "Library" if not found

        // Create sentences that include the player's name and library name
        string playerSentence = $"Hang in there, {playerName}...";
     
        // Set the sentences to the TextMeshPro elements
        playerNameText.text = playerSentence;
     libraryNameText.text = libraryName;
    }
}

