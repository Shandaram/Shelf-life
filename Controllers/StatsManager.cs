using UnityEngine;
using TMPro;

public class StatsManager : MonoBehaviour
{
    // UI Text references (only those that need to be displayed)
    public TextMeshProUGUI pepText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI repText;
    public TextMeshProUGUI bookText;

    // Stat variables
    public int PEP { get; private set; } = 0;       // Initial PEP value
    public int Coins { get; private set; } = 5500;  // Initial Coins value
    public int REP { get; private set; } = 10;       // Initial REP value
    public int bookReturns { get; private set; } = 0; // New BookReturns variable

    // Added stat variables
    public int Laptops { get; private set; } = 0;
    public int BuildingRepairs { get; private set; } = 0;
    public int TechRepairs { get; private set; } = 0;
    public int EcoUpgrades { get; private set; } = 0;
    public int TechUpgrades { get; private set; } = 0;
    private BookManager bookManager;

    // Public methods to update stats
    public void UpdatePEP(int points)
    {
        PEP += points;
        UpdatePEPText();
    }

    public void UpdateCoins(int amount)
    {
        Coins = Mathf.Max(Coins + amount, 0); // Ensure coins don't go negative
        UpdateCoinsText();
    }

    public void UpdateREP(int points)
    {
        REP += points;
        UpdateREPText();
    }

    public void UpdateLaptops(int count)
    {
        Laptops = Mathf.Max(Laptops + count, 0);
    }

    public void UpdateBuildingRepairs(int count)
    {
        BuildingRepairs = Mathf.Max(BuildingRepairs + count, 0);
    }

    public void UpdateTechRepairs(int count)
    {
        TechRepairs = Mathf.Max(TechRepairs + count, 0);
    }

    public void UpdateEcoUpgrades(int count)
    {
        EcoUpgrades = Mathf.Max(EcoUpgrades + count, 0);
    }

    public void UpdateTechUpgrades(int count)
    {
        TechUpgrades = Mathf.Max(TechUpgrades + count, 0);
    }

    // Function to check if enough coins are available
    public bool HasEnoughCoins(int amount)
    {
        return Coins >= amount;
    }

    // Internal methods to update UI text fields
    public void UpdatePEPText()
    {
        pepText.text = "PEP: " + PEP.ToString();
    }

    private void UpdateCoinsText()
    {
        coinsText.text = "Funds: " + Coins.ToString();
    }

    private void UpdateREPText()
    {
        repText.text = "REP: " + REP.ToString();
    }

      public void UpdateBookText(string bookcount)
    {
        bookText.text = "Books: " + bookcount;
    }

        public void UpdateBookReturns(int amount)
    {
        bookReturns = Mathf.Max(bookReturns + amount, 0); // Ensure bookReturns don't go negative
    }

    public void ResetBookReturns()
    {
        bookReturns = 0;
    }


    // Call this method to set up initial values if needed
    void Start()
    {

        bookManager = FindObjectOfType<BookManager>();
        

        UpdatePEPText();
        UpdateCoinsText();
        UpdateREPText();
        UpdateBookText(bookManager.bookCount.ToString());

    }
}



