using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;


[System.Serializable]
public class Email : ScriptableObject
{
    public string subject;
    public string emailText;
    public string type;
    public string email;
    public string sender;
    public string trigger;
    public int emailDay;
    public bool isRead;
}
public class CSVEmailLoader : MonoBehaviour
{
    public TextAsset csvFile;  // Drag and drop your CSV file here in the Unity Inspector

    public List<Email> LoadEmails()
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "New Librarian");  // Default to "Player" if not found
        string libraryName = PlayerPrefs.GetString("LibraryName", "Library");

        List<Email> emails = new List<Email>();
        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) // Start from 1 if the first row is a header
        {
            string[] fields = Regex.Split(lines[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");


            if (fields.Length < 8)
            {
                Debug.LogWarning($"Skipping row {i} due to insufficient columns: {fields.Length}");
                continue; // Ensure the row has enough columns
            }

            // Trim any extra quotes or spaces around the fields
            for (int j = 0; j < fields.Length; j++)
            {
                fields[j] = fields[j].Trim().Trim('"');
            }

            Email email = ScriptableObject.CreateInstance<Email>();

            email.subject = fields[1];
            email.email = fields[2];
            email.sender = fields[3];
            email.emailText = fields[4].Replace("\\n", "\n").Replace("[PLAYER NAME]", playerName).Replace("[LIBRARY NAME]", libraryName);    
            email.type = fields[5];
            email.trigger = fields[6];
            email.isRead = false; 

            if (int.TryParse(fields[7], out int emailDay))
            {
                if (emailDay == 0)
                {
                    // Generate a random number between 1 and 10
                    emailDay = Random.Range(1, 11);

                    // Save this value using PlayerPrefs so it remains consistent across game sessions
                    PlayerPrefs.SetInt($"Email_{email.subject}_Day", emailDay);
                }
                email.emailDay = emailDay;
            }
            else
            {
                email.emailDay = emailDay;
            }
            emails.Add(email);
        }
        // Sort emails by emailDay
        emails.Sort((x, y) => x.emailDay.CompareTo(y.emailDay));
        Debug.Log($"Total emails loaded: {emails.Count}");

        return emails;
    }
}
