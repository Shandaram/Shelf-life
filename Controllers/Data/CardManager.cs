using UnityEngine;
using System.Collections.Generic;


public class CardManager : MonoBehaviour
{
    // UI References
    public GameObject newCardPanel; // The panel for creating a new card
    public GameObject existingCardsPanel; // The panel showing all existing cards
    public Transform cardsContainer; // The container where all cards are added (inside scrollable list)
    public GameObject cardPrefab;  // Assign this to your Card prefab in the Inspector

    // Data References
    public List<Customer> allCustomers; // All customers in the game
    private List<Card> activePatrons = new List<Card>(); // List of all active patrons' cards

    private Customer currentCustomer; // Reference to the customer currently creating a card

    // This method runs when the game starts
    void Start()
    {
        //   Debug.Log("Total customers: " + allCustomers.Count);
        // Create cards for all active patrons
        foreach (Customer customer in allCustomers)
        {
            if (customer.isActivePatron)
            {

                CreateCardForExistingPatron(customer);
            }
        }


    }

    // Method to create a card for an existing patron
    private void CreateCardForExistingPatron(Customer customer)
    {
        // Create a card using the existing patron's first and last name
        Card card = new Card(customer, PlayerPrefs.GetString("LibraryName", "Library"));

        // Add the card to the list of active patrons
        activePatrons.Add(card);

        // Add the card to the existing cards panel
        AddCardToExistingCardsPanel(card);
    }

    // Method to show the new card panel for a customer
    public void ShowNewCardPanel(Customer customer)
    {
        currentCustomer = customer;
        newCardPanel.SetActive(true);
        // Ensure the card UI is set up correctly inside the panel
        SetupCardCreationUI();
    }

    private void SetupCardCreationUI()
    {
        // Instantiate the card prefab inside the new card panel
        GameObject cardUIObject = Instantiate(cardPrefab, newCardPanel.transform);
        CardUI cardUI = cardUIObject.GetComponent<CardUI>();
        if (cardUI != null)
        {
            // Set up the card UI for a new patron
            cardUI.SetCardInfo(new Card(currentCustomer, PlayerPrefs.GetString("LibraryName", "Library")), isNewPatron: true);
        }
    }


    // Method to save a new card created by the player
    public void SaveCard()
    {
        // Use the player input for the card name
        string inputCardName = FindObjectOfType<CardUI>().GetEnteredCardDetails(); // Get input from CardUI
                                                                                   // Check if the input matches the customer's name
        bool isNameCorrect = ValidateCardName(inputCardName, currentCustomer);

        // Create a card using the new patron's name provided by the player
        Card newCard = new Card(currentCustomer, PlayerPrefs.GetString("LibraryName", "Library"), inputCardName);

        // Add the new card to the list of active patrons
        activePatrons.Add(newCard);

        // Set the customer as an active patron
        currentCustomer.isActivePatron = true;

        // Add the card to the existing cards panel
        AddCardToExistingCardsPanel(newCard);
        // Update the customer's PEP based on the name validation
        if (!isNameCorrect)
        {
            currentCustomer.UpdateSatisfactionPoints(-5); // Deduct 5 PEP if the name is incorrect
                Debug.Log("have added satisfaction points from card!");
        }
        // Hide the new card panel
        newCardPanel.SetActive(false);
    }

    private void AddCardToExistingCardsPanel(Card card)
    {
        GameObject cardUIObject = Instantiate(cardPrefab, cardsContainer);
        CardUI cardUI = cardUIObject.GetComponent<CardUI>();
        if (cardUI != null)
        {
            cardUI.SetCardInfo(card, isNewPatron: false);
        }
    }
    private bool ValidateCardName(string input, Customer customer)
    {
        // Normalize the input and customer's name for comparison
        string[] inputParts = input.Trim().Split(' ');
        string firstName = customer.firstName.Trim().ToLower();
        string lastName = customer.lastName.Trim().ToLower();
        
        

        if (inputParts.Length < 2)
        {
            // If the input does not contain at least two parts (first and last name), it's incorrect
            return false;
        }

        string inputFirstName = inputParts[0].ToLower();
        string inputLastName = inputParts[1].ToLower();

        // Check if both parts of the name match
        Debug.Log("real name:" + firstName + lastName + "input name:" + inputFirstName + inputLastName );

        return inputFirstName == firstName && inputLastName == lastName;

    }
    public bool IsCardCreationPanelOpen()
    {
        return newCardPanel.activeSelf;
    }


}
