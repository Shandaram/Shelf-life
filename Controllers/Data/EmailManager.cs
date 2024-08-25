using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EmailManager : MonoBehaviour
{
    public Transform contentParent; // The parent of the grid emails (a GridLayoutGroup)
    public GameObject gridEmailPrefab;     // The prefab for the grid emails
    public GameObject detailViewPrefab;
    public GameObject passwordPanel; // Reference to the password panel
    public TMP_InputField passwordInputField; // Reference to the password input field
    public TMP_Text warningText; // Reference to the warning text
    public Button closeWarningButton; // Reference to the close button for the warning message

    private List<Email> emails;
    private Email currentEmail;
    public CSVEmailLoader csvLoader;

    private int currentDay = 1;
    private bool passwordMatched = false;
    private string correctPassword = "CS7043"; // Set your desired password here

    void Start()
    {
        emails = csvLoader.LoadEmails();
        detailViewPrefab.SetActive(false);
        passwordPanel.SetActive(!passwordMatched); // Show password panel if not matched
        warningText.gameObject.SetActive(false); // Initially hide the warning text
        PopulateEmails();

        // Add listener to the password submit button
        passwordPanel.transform.Find("Panel/SubmitButton").GetComponent<Button>().onClick.AddListener(CheckPassword);

        // Add listener to the close button for the warning message
        closeWarningButton.onClick.AddListener(HideWarning);
    }


    void PopulateEmails()
    {
        if (!passwordMatched) return; // Do not populate inventory if password isn't matched

        foreach (Email email in emails)
        {
            if (email.type == "Existing" || (email.type == "Incoming" && email.emailDay <= currentDay))
            {
                GameObject gridEmail = Instantiate(gridEmailPrefab, contentParent);

                TMP_Text senderText = gridEmail.transform.Find("EmailSender").GetComponent<TMP_Text>();
                TMP_Text subjectText = gridEmail.transform.Find("Subject").GetComponent<TMP_Text>();

                senderText.text = email.sender;
                subjectText.text = email.subject;

                // Apply bold and color to unread incoming emails
                if (email.type == "Incoming" && !email.isRead)
                {
                    senderText.fontStyle = FontStyles.Bold;
                    subjectText.fontStyle = FontStyles.Bold;

                    senderText.color = Color.blue; // Customize this color as needed
                    subjectText.color = Color.blue;
                }

                Button emailButton = gridEmail.GetComponent<Button>();
                if (emailButton == null)
                {
                    Debug.LogError("Button component not found in emailgridprefab.");
                    continue;
                }

                emailButton.onClick.AddListener(() => ShowEmailDetails(email));
            }
        }
    }

    // Show details of the selected email
    public void ShowEmailDetails(Email email)
    {
        if (!passwordMatched) return; // Do not show details if password isn't matched

        currentEmail = email;
        detailViewPrefab.SetActive(true);

        detailViewPrefab.transform.Find("HorInfo/EmailSender").GetComponent<TMP_Text>().text = email.sender;
        detailViewPrefab.transform.Find("HorInfo/Email").GetComponent<TMP_Text>().text = email.email;
        detailViewPrefab.transform.Find("Heading/Subject").GetComponent<TMP_Text>().text = email.subject;
        detailViewPrefab.transform.Find("Scroll View/Viewport/Content/Body").GetComponent<TMP_Text>().text = email.emailText;

        // Mark the email as read and update its type
        if (!email.isRead)
        {
            email.isRead = true;
            email.type = "Existing";

            // Optionally, you can re-populate the inventory to reflect the change
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
            PopulateEmails();

            // Add a listener to the close button
            Button closeButton = detailViewPrefab.transform.Find("CloseButton").GetComponent<Button>();
            closeButton.onClick.RemoveAllListeners(); // Clear any existing listeners
            closeButton.onClick.AddListener(CloseDetailView);
        }
    }

    private void CloseDetailView()
    {
        detailViewPrefab.SetActive(false);
        currentEmail = null;
    }

    // Check the entered password
    private void CheckPassword()
    {
        if (passwordInputField.text == correctPassword)
        {
            passwordMatched = true;
            passwordPanel.SetActive(false);
            PopulateEmails(); // Now populate inventory as the password is correct
        }
        else
        {
            warningText.gameObject.SetActive(true); // Show warning if password is wrong
        }
    }

    // Hide the warning message when the close button is clicked
    private void HideWarning()
    {
        warningText.gameObject.SetActive(false);
    }

     // Method to update the current day number and trigger relevant popups
    public void UpdateDayNumber(int newDayNumber)
    {
        currentDay = newDayNumber;
        PopulateEmails();
    }
}



