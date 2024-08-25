using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class BookManager : MonoBehaviour
{
    public GameObject bookRowPrefab;      // Prefab for each row in the book list
    public GameObject bookDetailInstance; // Reference to the instance of bookDetailPrefab already in the scene
    public Transform bookListParent;      // Parent object for the book list

    public List<BookData> books;         // List of books loaded from the CSV
    private BookData currentBook;         // Currently displayed book

    public TMP_Dropdown buyDropdownField;
 private Button lendButton;
    private Button updateQuantityButton;  // Button to update quantity
    private Dictionary<string, Sprite> genreImageMap; // Map of genre to images

    public int bookCount { get; private set; }
    public event System.Action<int> OnBookCountChanged;
    public StatsManager statsManager;

    public TMP_Dropdown genreDropdown;   // Dropdown to select genre
    public TMP_InputField searchInput;   // Input field to search by title
    private Button removeButton; // Button to remove book
    public GameObject notEnoughBudgetPanel; // Attach your popup panel here in the Inspector

    public bool fromCustomer { get; private set; } = false;
    public UIManager uiManager;

    public CameraManager cameraManager;
    private string currentGenreFilter = "All Genres";
private string currentSearchQuery = "";


    void Start()
    {
        // Load books from CSV
        CSVLoader csvLoader = GetComponent<CSVLoader>();
        books = csvLoader.LoadBooksFromCSV();
        cameraManager = FindObjectOfType<CameraManager>();
        uiManager = FindObjectOfType<UIManager>();
        // Set quantity first to 0, then too 50 random !!
        //
        SetRandomBooksQuantityToOne(50);

        // // Save the updated list back to the CSV
        // csvLoader.SaveBooksToCSV(books);

        // Populate the UI with book rows
        PopulateBookList();

        InitializeGenreImageMap();

        // Show details for the first book by default
        bookDetailInstance.SetActive(false);
        // Find the input field and button in the detail view
        updateQuantityButton = bookDetailInstance.transform.Find("Hor-group2/Buy")?.GetComponent<Button>();

        if (updateQuantityButton != null)
        {
            updateQuantityButton.onClick.AddListener(BuyBooks);
        }

        removeButton = bookDetailInstance.transform.Find("Hor-group2/Remove")?.GetComponent<Button>();

        if (removeButton != null)
        {
            removeButton.onClick.AddListener(RemoveBook);
        }

        PopulateGenreDropdown();

        // Add listeners for filtering and searching
        genreDropdown.onValueChanged.AddListener(delegate { FilterAndSearchBooks(); });
        searchInput.onValueChanged.AddListener(delegate { FilterAndSearchBooks(); });
        FilterAndSearchBooks();  // Initial population with no filters
                                 // Ensure the panel is initially hidden
        if (notEnoughBudgetPanel != null)
        {
            notEnoughBudgetPanel.SetActive(false);

            // Find and attach the close button's functionality
            Button closeButton = notEnoughBudgetPanel.transform.Find("Popup/CloseButton")?.GetComponent<Button>();
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(CloseNotEnoughBudgetPopup);
            }
        }
    }

    void PopulateGenreDropdown()
    {
        List<string> genres = books.Select(book => book.genre).Distinct().ToList();
        genres.Insert(0, "All Genres"); // Add an option to show all books

        genreDropdown.ClearOptions();
        genreDropdown.AddOptions(genres);
    }



    // void FilterAndSearchBooks()
    // {
    //     string selectedGenre = genreDropdown.options[genreDropdown.value].text;
    //     string searchQuery = searchInput.text.ToLower();

    //     var filteredBooks = books.Where(book =>
    //     {
    //         bool matchesGenre = selectedGenre == "All Genres" || book.genre == selectedGenre;
    //         bool matchesTitle = string.IsNullOrEmpty(searchQuery) || book.title.ToLower().Contains(searchQuery);
    //         return matchesGenre && matchesTitle;
    //     }).ToList();

    //     PopulateBookList(filteredBooks);
    // }

    void FilterAndSearchBooks()
{
    currentGenreFilter = genreDropdown.options[genreDropdown.value].text;
    currentSearchQuery = searchInput.text.ToLower();

    var filteredBooks = books.Where(book =>
    {
        bool matchesGenre = currentGenreFilter == "All Genres" || book.genre == currentGenreFilter;
        bool matchesTitle = string.IsNullOrEmpty(currentSearchQuery) || book.title.ToLower().Contains(currentSearchQuery);
        return matchesGenre && matchesTitle;
    }).ToList();

    PopulateBookList(filteredBooks);
}


    void InitializeGenreImageMap()
    {
        // Initialize the genre image map (case-insensitive)
        genreImageMap = new Dictionary<string, Sprite>(System.StringComparer.OrdinalIgnoreCase);

        // Load your genre images from Resources or any other folder
        // For example, assume the genre images are named after the genres and stored in Resources/GenreImages
        Sprite[] genreSprites = Resources.LoadAll<Sprite>("GenreImages");
        foreach (Sprite sprite in genreSprites)
        {
            genreImageMap[sprite.name.ToLower()] = sprite;
        }
    }
    string GetFormattedGenreKey(string genre)
    {
        // Replace spaces with underscores, remove special characters, and convert to lowercase
        string formattedKey = genre.ToLower()
                                    .Replace(" ", "_")      // Replace spaces with underscores
                                    .Replace("-", "_");     // Replace hyphens with underscores

        // Remove any other special characters if needed
        return formattedKey;
    }

    // Method to set fromCustomer
    public void SetFromCustomer(bool value)
    {
        fromCustomer = value;
    }
    public void PopulateBookList(List<BookData> filteredBooks = null)
    {
        foreach (Transform child in bookListParent)
        {
            Destroy(child.gameObject); // Clear existing rows
        }

        var booksToDisplay = filteredBooks ?? books;


        foreach (var book in booksToDisplay)
        {
            GameObject bookRow = Instantiate(bookRowPrefab, bookListParent);

            TMP_Text titleText = bookRow.transform.Find("TitleText")?.GetComponent<TMP_Text>();
            TMP_Text authorText = bookRow.transform.Find("AuthorText")?.GetComponent<TMP_Text>();
            TMP_Text publishedText = bookRow.transform.Find("PublishedText")?.GetComponent<TMP_Text>();
            TMP_Text genreText = bookRow.transform.Find("GenreText")?.GetComponent<TMP_Text>();
            TMP_Text quantityText = bookRow.transform.Find("QuantityText")?.GetComponent<TMP_Text>();

            Button button = bookRow.GetComponentInChildren<Button>();
            if (button == null)
            {
                Debug.LogError("Button component not found in bookRowPrefab.");
                continue;
            }

            titleText.text = book.title;
            authorText.text = book.author;
            genreText.text = book.genre;
            publishedText.text = book.published.ToString();
            quantityText.text = book.quantity.ToString();

            button.onClick.AddListener(() => ShowBookDetails(book));
        }
    }

    void ShowBookDetails(BookData book)
    {
        // Enable the bookDetailInstance if it's disabled
        if (bookDetailInstance != null)
        {
            bookDetailInstance.SetActive(true);

            TMP_Text titleText = bookDetailInstance.transform.Find("TitleText")?.GetComponent<TMP_Text>();
            TMP_Text authorText = bookDetailInstance.transform.Find("Hor-group/Ver-group/AuthorText")?.GetComponent<TMP_Text>();
            TMP_Text publishedText = bookDetailInstance.transform.Find("Hor-group/Ver-group/PublishedText")?.GetComponent<TMP_Text>();
            TMP_Text descriptionText = bookDetailInstance.transform.Find("DescriptionText")?.GetComponent<TMP_Text>();
            TMP_Text genreText = bookDetailInstance.transform.Find("Hor-group/Ver-group/GenreText")?.GetComponent<TMP_Text>();
            TMP_Text quantityText = bookDetailInstance.transform.Find("Hor-group2/QuantityText")?.GetComponent<TMP_Text>();
            Image genreImage = bookDetailInstance.transform.Find("Hor-group/GenreImage")?.GetComponent<Image>();

            if (titleText != null) titleText.text = book.title;
            if (authorText != null) authorText.text = book.author;
            if (publishedText != null) publishedText.text = book.published.ToString();
            if (descriptionText != null) descriptionText.text = book.blurb;
            if (genreText != null) genreText.text = book.genre;
            if (quantityText != null) quantityText.text = book.quantity.ToString();


            // Set the current book
            currentBook = book;

            // Format the genre to match the dictionary key
            string formattedGenreKey = GetFormattedGenreKey(book.genre);

            // Set the genre image using the formatted genre key
            if (genreImageMap.TryGetValue(formattedGenreKey, out Sprite genreSprite))
            {
                genreImage.sprite = genreSprite;
            }
            else
            {
                Debug.LogWarning($"No image found for genre: {book.genre}");
            }

            lendButton = bookDetailInstance.transform.Find("LendOut").GetComponent<Button>();
            lendButton.onClick.RemoveAllListeners();
            lendButton.onClick.AddListener(() => LendBook());


            if (book.quantity > 0)
            {
                lendButton.interactable = fromCustomer;

            }
            else
            {
                lendButton.interactable = false;
            }


        }
        else
        {
            Debug.LogError("bookDetailInstance is not assigned.");
        }
    }

    void LendBook()
    {
        // Reduce the quantity of the book by 1
        currentBook.quantity--;

        // Check for genre match and award PEP if it matches
        CustomerManager customerManager = FindObjectOfType<CustomerManager>();
        if (customerManager != null && customerManager.customer.desiredGenres.Contains(currentBook.genre))
        {
             customerManager.customer.UpdateSatisfactionPoints(5);
        }

        // Assign book details to the customer
        customerManager.customer.bookTitle = currentBook.title;
        customerManager.customer.bookAuthor = currentBook.author;
        customerManager.customer.bookPublished = currentBook.published;

        // Save the changes and update the UI
        // CSVLoader csvLoader = GetComponent<CSVLoader>();
        // csvLoader.SaveBooksToCSV(books);

        PopulateBookList();
        ShowBookDetails(currentBook);

        cameraManager.SwitchToCustomerView();
        uiManager.ShowCustomerActiveView();
        Debug.Log($"Lent book '{currentBook.title}' is {customerManager.customer.bookTitle}to the {customerManager.customer.firstName}.");

    }


    void SetRandomBooksQuantityToOne(int numberOfBooks)
    {
        if (books.Count <= 0) return;

        List<BookData> shuffledBooks = books.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < Mathf.Min(numberOfBooks, shuffledBooks.Count); i++)
        {
            shuffledBooks[i].quantity = 1;
        }

        // Recalculate the total book count after updating quantities
        UpdateBookCount();
    }


    public void UpdateBookCount()
    {
        bookCount = books.Sum(book => book.quantity);
        Debug.Log($"Total Book Count: {bookCount}");

        // Notify other objects about the change
        NotifyBookCountChange();
        statsManager.UpdateBookText(bookCount.ToString());
    }

    // void BuyBooks()
    // {
    //     if (currentBook != null && buyDropdownField != null)
    //     {
    //         int quantity;
    //         if (int.TryParse(buyDropdownField.options[buyDropdownField.value].text, out quantity))
    //         {
    //             int cost = quantity * 25;

    //             if (statsManager.HasEnoughCoins(cost))
    //             {
    //                 // Update the book quantity
    //                 currentBook.quantity += quantity;

    //                 // Deduct the coins
    //                 statsManager.UpdateCoins(-cost);

    //                 // Save the updated book list back to the CSV
    //                 CSVLoader csvLoader = GetComponent<CSVLoader>();
    //                 csvLoader.SaveBooksToCSV(books);

    //                 // Refresh the book list or other relevant UI
    //                 PopulateBookList();
    //                 ShowBookDetails(currentBook);
    //                 // Update the quantity display in the detailed view
    //                 TMP_Text quantityText = bookDetailInstance.transform.Find("QuantityText")?.GetComponent<TMP_Text>();
    //                 if (quantityText != null)
    //                 {
    //                     quantityText.text = currentBook.quantity.ToString();
    //                 }
    //             }
    //             else
    //             {
    //                 // Trigger not enough budget popup
    //                 ShowNotEnoughBudgetPopup();
    //             }
    //         }
    //         else
    //         {
    //             Debug.LogError("Invalid quantity selected.");
    //         }
    //     }
    // }

void BuyBooks()
{
    if (currentBook != null && buyDropdownField != null)
    {
        int quantity;
        if (int.TryParse(buyDropdownField.options[buyDropdownField.value].text, out quantity))
        {
            int cost = quantity * 25;

            if (statsManager.HasEnoughCoins(cost))
            {
                // Update the book quantity
                currentBook.quantity += quantity;

                // Deduct the coins
                statsManager.UpdateCoins(-cost);

                // Save the updated book list back to the CSV
                // CSVLoader csvLoader = GetComponent<CSVLoader>();
                // csvLoader.SaveBooksToCSV(books);

                // Refresh the book list or other relevant UI
                FilterAndSearchBooks(); // Apply filters before updating UI
                ShowBookDetails(currentBook);

                // Update the quantity display in the detailed view
                TMP_Text quantityText = bookDetailInstance.transform.Find("QuantityText")?.GetComponent<TMP_Text>();
                if (quantityText != null)
                {
                    quantityText.text = currentBook.quantity.ToString();
                }
            }
            else
            {
                // Trigger not enough budget popup
                ShowNotEnoughBudgetPopup();
            }
        }
        else
        {
            Debug.LogError("Invalid quantity selected.");
        }
         // Recalculate and update the total book count
        UpdateBookCount();
    }
}

    void ShowNotEnoughBudgetPopup()
    {
        notEnoughBudgetPanel.SetActive(true);
    }

    void CloseNotEnoughBudgetPopup()
    {
        notEnoughBudgetPanel.SetActive(false);

    }


    void NotifyBookCountChange()
    {
        OnBookCountChanged?.Invoke(bookCount);
    }

    // void RemoveBook()
    // {
    //     if (currentBook != null)
    //     {
    //         // Set the quantity of the current book to 0
    //         currentBook.quantity = 0;

    //         // Save the updated book list back to the CSV
    //         CSVLoader csvLoader = GetComponent<CSVLoader>();
    //         csvLoader.SaveBooksToCSV(books);

    //         // Refresh the book list to reflect changes
    //         PopulateBookList();
    //         ShowBookDetails(currentBook);
    //         // Update the quantity display in the detailed view
    //         TMP_Text quantityText = bookDetailInstance.transform.Find("Hor-group2/QuantityText")?.GetComponent<TMP_Text>();
    //         if (quantityText != null)
    //         {
    //             quantityText.text = currentBook.quantity.ToString();
    //         }

    //         // Recalculate and update the total book count
    //         UpdateBookCount();
    //     }
    // }

    void RemoveBook()
{
    if (currentBook != null)
    {
        // Set the quantity of the current book to 0
        currentBook.quantity = 0;

        // Save the updated book list back to the CSV
        // CSVLoader csvLoader = GetComponent<CSVLoader>();
        // csvLoader.SaveBooksToCSV(books);

        // Refresh the book list to reflect changes
        FilterAndSearchBooks(); // Apply filters before updating UI
        ShowBookDetails(currentBook);

        // Update the quantity display in the detailed view
        TMP_Text quantityText = bookDetailInstance.transform.Find("Hor-group2/QuantityText")?.GetComponent<TMP_Text>();
        if (quantityText != null)
        {
            quantityText.text = currentBook.quantity.ToString();
        }

        // Recalculate and update the total book count
        UpdateBookCount();
    }
}
public void CheckControversialBooks()
{
    int controversialBookCount = books
        .Where(book => book.controversial == true)
        .Sum(book => book.quantity);
   Debug.Log(controversialBookCount);
    if (controversialBookCount >= 7)
    {
        // Deduct REP points (assuming you have a method in StatsManager for this)
        statsManager.UpdateREP(-1);
        Debug.Log(controversialBookCount);
    }
}


}




