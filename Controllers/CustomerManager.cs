using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public Customer customer;
    private DayManager dayManager;
    private LevelManager levelManger;

    private bool hasVisitedToday = false;
    private DialogueManager dialogueManager;
    private int currentDay = 1;

    private void Start()
    {

        // Find the DialogueManager in the scene
        dialogueManager = FindObjectOfType<DialogueManager>();
        Debug.Log("Customer dayVisit: " + customer.dayVisit);
        Debug.Log("Current day: " + currentDay);
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager not found.");
        }

    }
private void CheckDay()
{
    if (dayManager != null && customer.dayVisit < currentDay)
    {
        if (!hasVisitedToday)
        {
            Debug.Log("Customer " + customer.firstName + " leaving automatically because their assigned day has passed.");
            Leave(); // Automatically leave if the assigned day has passed
        }
    }
}
    public void UpdateDayNumber(int newDayNumber)
    {
        currentDay = newDayNumber;
        CheckDay();

    }

    public void SetDayManager(DayManager manager)
    {
        dayManager = manager;
    }

    private void OnMouseDown()
    {
        StartDialogue();
    }

    public void StartDialogue()
    {
        if (dialogueManager != null)
        {
            dayManager.customerDialogueFinished = false;
            // dayManager.specificCustomerSpawned;
            string initialKey = GetInitialDialogueKey();
            dialogueManager.StartDialogue(initialKey, customer, OnDialogueResponse, Leave);
        }
    }

    private string GetInitialDialogueKey()
    {
        // Return initial dialogue key if the customer is not Caryn
        if (customer.firstName != "Caryn")
        {
            return customer.initialDialogueKey;
        }
        else
        {
            // Define lists of positive and negative Mackayleigh passages
            string[] positiveMackayleighPassages = { "Mack19", "Mack22", "Mack24", "Mack32", "Mack33", "Mack35", "Mack36" };
            string[] negativeMackayleighPassages = { "Mack14", "Mack25", "Mack34", "Mack40" };

            // Check if any positive Mackayleigh passage has been played
            foreach (string passage in positiveMackayleighPassages)
            {
                if (dialogueManager.HasPassagePlayed(passage))
                {
                    return "Caryn1"; // Return Caryn1 for positive passages
                }
            }

            // Check if any negative Mackayleigh passage has been played
            foreach (string passage in negativeMackayleighPassages)
            {
                if (dialogueManager.HasPassagePlayed(passage))
                {
                    return "Caryn2"; // Return Caryn2 for negative passages
                }
            }

            if (dialogueManager.HasPassagePlayed("Mack39"))
            {
                return "Caryn3"; // Return Caryn3 for negative passages
            }

            // Default to Caryn1 if no specific passages were found
            return "Caryn1";
        }
    }


    // This method is now passed to the DialogueManager to handle dialogue responses.
    public void OnDialogueResponse(int responsePoints)
    {
        hasVisitedToday = true; // Mark as visited today
        customer.UpdateSatisfactionPoints(responsePoints);
    }

    // Method to handle customer leaving after dialogue ends.
    public void Leave()
    {
        if (dayManager != null)
        {
           

            dayManager.OnCustomerDialogueFinished();
        }

        Destroy(gameObject); // Remove customer after dialogue ends
    }

}




