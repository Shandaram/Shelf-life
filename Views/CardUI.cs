using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    public Image ageBasedImage;  // The image that changes based on the customer's age
    public Image customerSpriteImage;  // The image representing the customer's sprite
    public TMP_Text libraryNameText;  // The TextMeshPro text for the library name
    public TMP_Text cardName;  // The TextMeshPro text for the card details (used for existing patrons)
    public TMP_InputField cardNameInputField;  // The TextMeshPro input field for entering card details (used for new patrons)
    public Button saveButton;
    // Sprites for different age ranges (assign these in the Unity Inspector)
    public Sprite ageBasedSprite1;  // For ages < 18
    public Sprite ageBasedSprite2;  // For ages 18-59
    public Sprite ageBasedSprite3;  // For ages 60+
    private CardManager cardManager;

       private void Start()
    {
        // Assign the CardManager instance if not already set (you could also pass this in from the manager)
        cardManager = FindObjectOfType<CardManager>();
        if (cardManager != null)
        {
            saveButton.onClick.AddListener(OnSaveButtonClicked);
        }
    }

    // Method to set up the card UI with customer details
    public void SetCardInfo(Card card, bool isNewPatron)
    {
        // Set the age-based image based on the customer's age
        ageBasedImage.sprite = GetAgeBasedImage(card.customer.age);

        // Set the customer's sprite image
        customerSpriteImage.sprite = card.customer.customerSprite;

        // Set the library name on the card
        libraryNameText.text = card.libraryName;

        if (isNewPatron)
        {
            // For new patrons, use the input field for the player to enter details
            cardNameInputField.gameObject.SetActive(true);
            cardName.gameObject.SetActive(false);
             saveButton.gameObject.SetActive(true);
        }
        else
        {
            // For existing patrons, display their details using a text field
            cardName.text = card.cardName;
            cardNameInputField.gameObject.SetActive(false);
            cardName.gameObject.SetActive(true);
             saveButton.gameObject.SetActive(false);
        }
       
    }

    // Returns the appropriate sprite based on the customer's age
    private Sprite GetAgeBasedImage(int age)
    {
        if (age < 18)
        {
            return ageBasedSprite1;
        }
        else if (age < 60)
        {
            return ageBasedSprite2;
        }
        else
        {
            return ageBasedSprite3;
        }
    }
  private void OnSaveButtonClicked()
    {
        if (cardManager != null)
        {
            cardManager.SaveCard();
        }
    }
    // Method to retrieve the card details entered by the player (for new patrons)
    public string GetEnteredCardDetails()
    {
        return cardNameInputField.text;
    }
}


