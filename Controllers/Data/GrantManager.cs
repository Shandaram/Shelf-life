using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class GrantManager : MonoBehaviour
{

    public int currentLaptops;
    private int currentBookCount;

    public Dictionary<string, int> bookGenres = new Dictionary<string, int>();
    public Dictionary<string, int> repairs = new Dictionary<string, int>();
    public Dictionary<string, int> upgrades = new Dictionary<string, int>();
    public Dictionary<string, int> authors = new Dictionary<string, int>();

    public CSVGrantLoader csvGrantLoader;
    public Transform grantListParent;
    public GameObject grantRowPrefab;
    public GameObject grantDetailsInstance;
    public GameObject popupPanel;      // The panel that contains the popup
    public TMP_Text popupText;         // The TextMeshPro text element in the popup


    public BookManager bookManager;
    public StatsManager statsManager;
     private Grant currentGrant;   


    private List<Grant> grants;


    private void Start()
    {
        CSVGrantLoader csvGrantLoader = GetComponent<CSVGrantLoader>();
        grants = csvGrantLoader.LoadGrants();
        Debug.Log(grants.Count);
        currentBookCount = bookManager.bookCount;
        PopulateGrantList();
         // Show details for the first grant if available
     grantDetailsInstance.SetActive(false);
        popupPanel.SetActive(false);
    }

    public void PopulateGrantList()
    {
        foreach (Grant grant in grants)
        {
            if (grant.location == "All" || grant.location == PlayerPrefs.GetString("SelectedTheme"))
            {
                GameObject grantRow = Instantiate(grantRowPrefab, grantListParent);
                grantRow.GetComponentInChildren<TMP_Text>().text = grant.grantName;

                Button button = grantRow.GetComponentInChildren<Button>();
                if (button == null)
                {
                    Debug.LogError("Button component not found in bookRowPrefab.");
                    continue;
                }
                button.onClick.AddListener(() => ShowGrantDetails(grant));
            }
        }
    }

    public void ShowGrantDetails(Grant grant)
    {
        grantDetailsInstance.SetActive(true);
        grantDetailsInstance.transform.Find("GrantName").GetComponent<TMP_Text>().text = grant.grantName;
        grantDetailsInstance.transform.Find("Body/GrantDescription").GetComponent<TMP_Text>().text = grant.description;
        grantDetailsInstance.transform.Find("Body/Objectives").GetComponent<TMP_Text>().text = grant.objective;
        grantDetailsInstance.transform.Find("HorInfo/Tier").GetComponent<TMP_Text>().text =  $"Type: {grant.tier}";
        if (grant.rewardType != "NA")
        {
            grantDetailsInstance.transform.Find("HorInfo/GrantReward").GetComponent<TMP_Text>().text = $"+{grant.rewardCount} {grant.rewardType}";
        }
        else
        {
            grantDetailsInstance.transform.Find("HorInfo/GrantReward").GetComponent<TMP_Text>().text = " ";

        }





        grantDetailsInstance.transform.Find("FundCont/Funded").GetComponent<TMP_Text>().text = $"Funded by {grant.funded}";
        grantDetailsInstance.transform.Find("HorInfo/Value").GetComponent<TMP_Text>().text = $"Reward: {grant.rewardCoins} coins ";



        Button applyButton = grantDetailsInstance.transform.Find("ApplyButton").GetComponent<Button>();
        applyButton.onClick.RemoveAllListeners();
           if (grant.received)
    {
        // Change the color to indicate the grant has been received
        grantDetailsInstance.GetComponent<Image>().color = Color.gray; // Change the detailed view color
        applyButton.interactable = false; // Disable the apply button
    }
    else
    {
        grantDetailsInstance.GetComponent<Image>().color = Color.white; // Default color
        applyButton.interactable = true; // Enable the apply button
        applyButton.onClick.AddListener(() => ApplyForGrant(grant));
    }
    
    currentGrant = grant;
        


    }

    public void ApplyForGrant(Grant grant)
    {
        if (CanApply(grant))
        {
            // Update the stats via StatsManager
            statsManager.UpdateCoins(grant.rewardCoins);

            if (grant.rewardType.ToLower() == "rep")
            {
                statsManager.UpdateREP(grant.rewardCount);
            }
            else if (grant.rewardType.ToLower() == "na")
            {
                Debug.Log("No extra rewards.");
            }
            else if (grant.rewardType.ToLower() == "laptop")
            {
                statsManager.UpdateLaptops(grant.rewardCount);
            }
            else
            {
                RewardBooks(grant.rewardType, grant.rewardCount);
            }

       
        // Mark the grant as received
        grant.received = true;

        // Optionally, display a confirmation message or popup
        ShowPopup("Grant applied successfully!");
        }
        else
        {
            ShowPopup("You do not meet the requirements to apply for this grant. Please, try again.");
        }
    }



    public void RewardBooks(string genre, int amount)
    {
        // Get the list of books with the matching genre
        var matchingBooks = bookManager.books.Where(book => book.genre == genre).ToList();

        // Handle the case where there are no matching books
        if (matchingBooks.Count == 0)
        {
            Debug.LogWarning($"No books found with the genre: {genre}");
            return;
        }

        int totalBooks = matchingBooks.Count;
        int index = 0;

        // Loop through the matching books and add +1 to their quantity
        while (amount > 0)
        {
            matchingBooks[index].quantity += 1;

            // Decrease the amount and move to the next book
            amount--;
            index++;

            // Loop back to the start if we reach the end of the list
            if (index >= totalBooks)
            {
                index = 0;
            }
        }

        // Update the CSV file after modifying the book quantities
        // CSVLoader csvLoader = bookManager.GetComponent<CSVLoader>();
        // csvLoader.SaveBooksToCSV(bookManager.books);

        // Recalculate the total book count in the BookManager
        bookManager.UpdateBookCount();

        // Optionally, refresh the book list in the UI
        bookManager.PopulateBookList();

        Debug.Log($"Updated the quantity of books with genre {genre}. {totalBooks} books were modified.");
    }

private bool CanApply(Grant grant)
{
    bool result = CheckBookGenres(grant) &&
                  CheckAuthors(grant) &&
                  CheckRepairs(grant) &&
                  CheckUpgrades(grant)&&
                  statsManager.PEP >= grant.pepRequired &&
                  statsManager.REP >= grant.repRequired &&
                  bookManager.bookCount >= grant.bookCountRequired;

    Debug.Log($"CanApply Result: {result}");
    Debug.Log($"PEP Check: {statsManager.PEP} >= {grant.pepRequired} = {statsManager.PEP >= grant.pepRequired}");
    Debug.Log($"REP Check: {statsManager.REP} >= {grant.repRequired} = {statsManager.REP >= grant.repRequired}");
    Debug.Log($"Book Count Check: {bookManager.bookCount} >= {grant.bookCountRequired} = {bookManager.bookCount >= grant.bookCountRequired}");

    return result;
}

private bool CheckBookGenres(Grant grant)
{
    // Check first genre requirement
    bool genre1Met = grant.bookGenreRequired == "NA" ||
                     CountBooksByGenre(grant.bookGenreRequired) >= grant.bookGenreCountRequired;
    Debug.Log($"Genre 1 Check: Genre '{grant.bookGenreRequired}' Count >= {grant.bookGenreCountRequired} = {genre1Met}");

    // Check second genre requirement
    bool genre2Met = grant.bookGenreRequired2 == "NA" ||
                     CountBooksByGenre(grant.bookGenreRequired2) >= grant.bookGenreCountRequired2;
    Debug.Log($"Genre 2 Check: Genre '{grant.bookGenreRequired2}' Count >= {grant.bookGenreCountRequired2} = {genre2Met}");

    // Return true only if both genre requirements are met
    return genre1Met && genre2Met;
}

private int CountBooksByGenre(string genre)
{
    var matchingBooks = bookManager.books.Where(book => book.genre == genre).ToList();
    int totalQuantity = matchingBooks.Sum(book => book.quantity);

    Debug.Log($"CountBooksByGenre: Genre '{genre}' Total Quantity = {totalQuantity}");

    return totalQuantity;
}

private int CountBooksByAuthor(string author)
{
    var matchingBooks = bookManager.books.Where(book => book.author == author).ToList();
    int totalQuantity = matchingBooks.Sum(book => book.quantity);

    Debug.Log($"CountBooksByAuthor: Author '{author}' Total Quantity = {totalQuantity}");

    return totalQuantity;
}

private bool CheckAuthors(Grant grant)
{
    bool authorMet = grant.bookGenreRequired != "NA" || 
                     CountBooksByAuthor(grant.authorNameRequired) >= grant.authorCountRequired;

    Debug.Log($"Author Check: Author '{grant.authorNameRequired}' Count >= {grant.authorCountRequired} = {authorMet}");

    return authorMet;
}

private bool CheckRepairs(Grant grant)
{
    bool result;
    switch (grant.repairTypeRequired)
    {
        case "Tech":
            result = statsManager.TechRepairs >= grant.repairCountRequired;
            Debug.Log($"Tech Repairs Check: TechRepairs ({statsManager.TechRepairs}) >= {grant.repairCountRequired} = {result}");
            break;

        case "Any":
            result = (statsManager.TechRepairs + statsManager.BuildingRepairs) >= grant.repairCountRequired;
            Debug.Log($"Any Repairs Check: TechRepairs ({statsManager.TechRepairs}) + BuildingRepairs ({statsManager.BuildingRepairs}) >= {grant.repairCountRequired} = {result}");
            break;

        case "None":
            result = true;
            Debug.Log("No Repairs Required: True");
            break;

        default:
            Debug.LogWarning("Unknown repair type: " + grant.repairTypeRequired);
            result = false;
            break;
    }

    return result;
}

private bool CheckUpgrades(Grant grant)
{
    bool result;
    switch (grant.upgradeTypeRequired)
    {
        case "Tech":
            result = statsManager.TechUpgrades >= grant.upgradeCountRequired;
            Debug.Log($"Tech Upgrades Check: TechUpgrades ({statsManager.TechUpgrades}) >= {grant.upgradeCountRequired} = {result}");
            break;

        case "Any":
            result = (statsManager.TechUpgrades + statsManager.EcoUpgrades) >= grant.upgradeCountRequired;
            Debug.Log($"Any Upgrades Check: TechUpgrades ({statsManager.TechUpgrades}) + EcoUpgrades ({statsManager.EcoUpgrades}) >= {grant.upgradeCountRequired} = {result}");
            break;

        case "Eco":
            result = statsManager.EcoUpgrades >= grant.upgradeCountRequired;
            Debug.Log($"Eco Upgrades Check: EcoUpgrades ({statsManager.EcoUpgrades}) >= {grant.upgradeCountRequired} = {result}");
            break;

        case "None":
            result = true;
            Debug.Log("No Upgrades Required: True");
            break;

        default:
            Debug.LogWarning("Unknown upgrade type: " + grant.upgradeTypeRequired);
            result = false;
            break;
    }

    return result;
}



private void UpdateGrantRowUI(Grant grant)
{
    // Find the corresponding grant row and update its color
    foreach (Transform grantRow in grantListParent)
    {
        TMP_Text grantText = grantRow.GetComponentInChildren<TMP_Text>();
        if (grantText.text == grant.grantName)
        {
            grantRow.GetComponent<Image>().color = Color.gray; // Change the color of the row
        }
    }
}
    public void ShowPopup(string message)
    {
        // Check if the popup panel is currently active in the hierarchy
        if (!popupPanel.activeInHierarchy)
        {
            // If the popup is inactive, set the text and activate the panel
            popupText.text = message;
            popupPanel.SetActive(true);
        }
       
    }
     public void ClosePopup()
    {
        popupPanel.SetActive(false);

    // Ensure that if a grant is successfully applied, it updates its status
    if (currentGrant != null && currentGrant.received)
    {
        UpdateGrantRowUI(currentGrant);
        ShowGrantDetails(currentGrant);
    }
        
    }
}

