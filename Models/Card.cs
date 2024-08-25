using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Card
{
    public string cardName;  // The name on the card
    public Customer customer;  // Reference to the associated customer
    public string libraryName;  // Name of the library

    // Constructor for existing active patrons
    public Card(Customer existingCustomer, string libraryName)
    {
        this.customer = existingCustomer;
        this.libraryName = libraryName;

        // Use the first and last name of the existing customer for the card name
        this.cardName = $"{existingCustomer.firstName} {existingCustomer.lastName}";
    }

    // Constructor for new patrons
    public Card(Customer newCustomer, string libraryName, string inputCardName)
    {
        this.customer = newCustomer;
        this.libraryName = libraryName;

        // Use the name provided by the player for the card name
        this.cardName = inputCardName;
    }
}
