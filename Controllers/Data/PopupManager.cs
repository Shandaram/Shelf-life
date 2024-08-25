using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PopupManager : MonoBehaviour
{
    public GameObject popupPanel;
    public GameObject vandalism;
    public TextMeshProUGUI popupText;
    public Image popupImage;
    public Button choiceAButton;
    public Button choiceBButton;
    public TextMeshProUGUI choiceAText;
    public TextMeshProUGUI choiceBText;

    public CSVPopupLoader popupLoader;

    private Dictionary<string, Popup> allPopups;
    private StatsManager statsManager;
    private DialogueManager dialogueManager;
    private GrantManager grantManager;
    private int currentDay = 1;

    private DayManager dayManager;

    void Start()
    {
        popupPanel.SetActive(false);
        InitializePopups(popupLoader);
        statsManager = FindObjectOfType<StatsManager>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        grantManager = FindObjectOfType<GrantManager>();
        dayManager = FindObjectOfType<DayManager>();
    }

    public void InitializePopups(CSVPopupLoader popupLoader)
    {
        List<Popup> popups = popupLoader.LoadPopups();
        allPopups = new Dictionary<string, Popup>();

        foreach (Popup popup in popups)
        {
            allPopups[popup.popupName] = popup;
        }
    }

    public bool IsPopup(string nodeKey)
    {
        return allPopups.ContainsKey(nodeKey);
    }

    public void ShowPopup(string popupName, System.Action<string> onChoiceSelected)
    {
        if (!allPopups.ContainsKey(popupName))
        {
            Debug.LogError($"Popup with name {popupName} not found.");
            return;
        }

        Popup popup = allPopups[popupName];

        // Handle donation logic if the popup is of type "Donation"
        HandleDonation(popup);

        // Display popup text and image
        popupText.text = popup.popupText;
        popupImage.sprite = popup.popupImage;

        // Set up the first button with the first choice and check coin requirements
        SetupChoiceButton(popup.ChoiceA, popup.Coins_A, choiceAButton, choiceAText,
            () => OnPopupChoiceSelected(popup.popupName, true, popup.REP_A, popup.PEP_A, popup.Coins_A, popup.Next_A, onChoiceSelected, popup.popupType));

        // Check if the second choice exists
        if (!string.IsNullOrEmpty(popup.ChoiceB))
        {
            // If ChoiceB exists, set up the second button
            choiceBButton.gameObject.SetActive(true); // Ensure the button is active
            SetupChoiceButton(popup.ChoiceB, popup.Coins_B, choiceBButton, choiceBText,
                () => OnPopupChoiceSelected(popup.popupName, false, popup.REP_B, popup.PEP_B, popup.Coins_B, popup.Next_B, onChoiceSelected, popup.popupType));
        }
        else
        {
            // If ChoiceB doesn't exist, hide the second button
            choiceBButton.gameObject.SetActive(false);
        }

        // Show the popup panel
        popupPanel.SetActive(true);
    }

    private void SetupChoiceButton(string choiceText, int coinCost, Button button, TextMeshProUGUI buttonText, UnityEngine.Events.UnityAction callback)
    {
        buttonText.text = choiceText;

        if (statsManager.HasEnoughCoins(-coinCost)) // Check if player has enough coins (remember coins are presented as negative numbers)
        {
            button.interactable = true;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(callback);
        }
        else
        {
            button.interactable = false; // Disable the button if not enough coins
        }
    }

    private void OnPopupChoiceSelected(string popupName, bool IsChoiceA, int repPoints, int pepPoints, int coins, string nextNodeKey, System.Action<string> onChoiceSelected, string popupType)
    {

        // Update stats based on general points and coins
        UpdateStats(repPoints, pepPoints, coins);
        // Determine the type of the popup and update corresponding stats
        if (IsChoiceA)
        {
            switch (popupType)
            {
                case "Laptops":
                    statsManager.UpdateLaptops(2);  // Assuming pressing the button adds 1 Laptop
                    break;
                case "BuildingRepair":
                    statsManager.UpdateBuildingRepairs(1);  // Increment BuildingRepair stat
                    break;
                case "TechRepair":
                    statsManager.UpdateTechRepairs(1);  // Increment TechRepair stat
                    break;
                case "EcoUpgrade":
                    statsManager.UpdateEcoUpgrades(1);  // Increment EcoUpgrade stat
                    break;
                case "TechUpgrade":
                    statsManager.UpdateTechUpgrades(1);  // Increment TechUpgrade stat
                    break;
                default:
                    Debug.LogWarning("Popup type not recognized for stat update.");
                    break;
            }
        }
        Debug.Log("Next node" + nextNodeKey);


        // Close the popup panel
        popupPanel.SetActive(false);
        //vandalism
        if (popupName == "PostVandalism" && IsChoiceA)
        {
            vandalism.SetActive(false);
        }
        //spawn customer
        if (nextNodeKey == "Police0")
        {
            dayManager.SpawnSpecificCustomer("Police");

        }
        else if (nextNodeKey == "Yusuf0")
        {
            dayManager.SpawnSpecificCustomer("Yusuf");
        }
        else
        {
            // Invoke the callback with the next node key
            onChoiceSelected?.Invoke(nextNodeKey);
        }

    }



    private void HandleDonation(Popup popup)
    {
        if (popup.popupType == "Donation")
        {
            grantManager.RewardBooks(popup.Genre1, popup.Genre1Qty);
            if (popup.Genre2 != "")
            {
                grantManager.RewardBooks(popup.Genre2, popup.Genre2Qty);
            }
        }
        else
        {
            return;
        }
    }

    // Method to update the current day number and trigger relevant popups
    public void UpdateDayNumber(int newDayNumber)
    {
        currentDay = newDayNumber;
        CheckAndTriggerEventPopups();
        Debug.Log("Day updated to: " + currentDay);
    }

    // Method to check and trigger any popups that should occur on the current day
    private void CheckAndTriggerEventPopups()
    {
        foreach (var popup in allPopups.Values)
        {
            if (popup.triggerDay == currentDay)
            {
                ScheduleEventPopup(popup.popupName, 5f);
            }
        }
    }

    // Method to schedule and trigger a popup immediately with optional delay
    public void ScheduleEventPopup(string popupName, float delay = 5f)
    {
        StartCoroutine(TriggerEventPopup(popupName, delay));
    }

    private IEnumerator TriggerEventPopup(string popupName, float delay)
    {
        // Apply optional delay
        yield return new WaitForSeconds(delay);

        ShowPopup(popupName, (selectedNextNodeKey) =>
        {
            if (!string.IsNullOrEmpty(selectedNextNodeKey))
            {
                // Replace the previous method call with the correct signature
                var customerManager = FindObjectOfType<CustomerManager>();
                if (customerManager != null && customerManager.customer != null)
                {
                    dialogueManager.StartDialogue(selectedNextNodeKey, customerManager.customer, null, null);
                }
                else
                {
                    Debug.LogError("CustomerManager or Customer not found.");
                }
            }
        });
    }

    public void OpenSpecificPopup(string popupName, GameObject objectToDisappear)
    {
        if (!allPopups.ContainsKey(popupName))
        {
            Debug.LogError($"Popup with name {popupName} not found.");
            return;
        }

        Popup popup = allPopups[popupName];

        // Display popup text and image
        popupText.text = popup.popupText;
        popupImage.sprite = popup.popupImage;

        // Set up the first button with the first choice
        SetupChoiceButton(popup.ChoiceA, popup.Coins_A, choiceAButton, choiceAText,
            () => OnPopupChoiceSelectedForObject(true, popup.REP_A, popup.PEP_A, popup.Coins_A, popup.Next_A, objectToDisappear, true, popup.popupType));

        // Set up the second button with the second choice
        SetupChoiceButton(popup.ChoiceB, popup.Coins_B, choiceBButton, choiceBText,
            () => OnPopupChoiceSelectedForObject(false, popup.REP_B, popup.PEP_B, popup.Coins_B, popup.Next_B, objectToDisappear, false, popup.popupType));

        // Show the popup panel
        popupPanel.SetActive(true);
    }

    private void OnPopupChoiceSelectedForObject(bool IsChoiceA, int repPoints, int pepPoints, int coins, string nextNodeKey, GameObject objectToDisappear, bool shouldDisappear, string popupType)
    {
        // Update stats
        UpdateStats(repPoints, pepPoints, coins);
        if (IsChoiceA)
        {
            switch (popupType)
            {
                case "Laptops":
                    statsManager.UpdateLaptops(2);  // Assuming pressing the button adds 1 Laptop
                    break;
                case "BuildingRepair":
                    statsManager.UpdateBuildingRepairs(1);  // Increment BuildingRepair stat
                    break;
                case "TechRepair":
                    statsManager.UpdateTechRepairs(1);  // Increment TechRepair stat
                    break;
                case "EcoUpgrade":
                    statsManager.UpdateEcoUpgrades(1);  // Increment EcoUpgrade stat
                    break;
                case "TechUpgrade":
                    statsManager.UpdateTechUpgrades(1);  // Increment TechUpgrade stat
                    break;
                default:
                    Debug.LogWarning("Popup type not recognized for stat update.");
                    break;
            }
        }
        Debug.Log("New stats: TR:" + statsManager.TechRepairs + "TU:" + statsManager.TechUpgrades);

        popupPanel.SetActive(false);

        // Handle the disappearance of the object if choice A is selected
        if (shouldDisappear && objectToDisappear != null)
        {
            Destroy(objectToDisappear);
        }


    }

    private void UpdateStats(int repPoints, int pepPoints, int coins)
    {
        // Assuming you have a StatsManager class to handle these updates
        if (statsManager != null)
        {
            statsManager.UpdateREP(repPoints);
            statsManager.UpdatePEP(pepPoints);
            statsManager.UpdateCoins(coins);
        }
    }
}



