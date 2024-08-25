using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int currentDay = 1;
    public int totalDays;
    private int customersPerDay;
    public GameObject loadingScreen;
    public GameObject endOfDayPanel;
    public DayManager dayManager;
    public CameraManager cameraManager;
    public UIManager uiManager;
    public List<GameObject> customerPrefabs;
    private List<GameObject> remainingCustomers;
    private List<GameObject> excludedCustomers;
    public StatsManager statsManager; // Reference to PEPStatsManager

    private BookManager bookManager;

    public int startPEP { get; private set; }

  private void Start()
    {

        
        remainingCustomers = new List<GameObject>(customerPrefabs);
        bookManager = FindObjectOfType<BookManager>();
        startPEP = CalculateInitialPEP();
        statsManager.UpdatePEP(startPEP); // Initialize PEP display
        customersPerDay = dayManager.maxCustomersPerDay;

        // Initialize the excludedCustomers list
        excludedCustomers = new List<GameObject>();

        // Assign customers to their respective days
        List<GameObject> todaysCustomers;
        AssignRandomDaysToCustomers(out todaysCustomers, excludedCustomers);

        // Start the day with assigned customers
        StartCoroutine(StartDay(todaysCustomers, excludedCustomers));
    }

    private void AssignRandomDaysToCustomers(out List<GameObject> todaysCustomers, List<GameObject> excludedCustomers)
    {
        todaysCustomers = new List<GameObject>();

        // Maximum number of characters allowed per day
        Dictionary<int, int> dayCharacterCount = new Dictionary<int, int>();

        // Initialize the dictionary with all possible days
        for (int day = 1; day <= totalDays; day++)
        {
            dayCharacterCount[day] = 0;
        }

        // Assign customers based on dayVisit
        foreach (GameObject customerPrefab in remainingCustomers)
        {
            Customer customer = customerPrefab.GetComponent<Customer>();
            if (customer != null)
            {
                if (customer.dayVisit == currentDay)
                {
                    todaysCustomers.Add(customerPrefab);
                    dayCharacterCount[currentDay]++;
                }
                else if (customer.dayVisit == 99)
                {
                    excludedCustomers.Add(customerPrefab);
                }
                else if (customer.dayVisit == 0)
                {
                    // Handle random day assignment
                    List<int> availableDays = new List<int>();

                    // Find all days that are still available for assignment
                    for (int day = 1; day <= totalDays; day++)
                    {
                        if (dayCharacterCount[day] < customersPerDay)
                        {
                            availableDays.Add(day);
                        }
                    }

                    if (availableDays.Count > 0)
                    {
                        int randomIndex = Random.Range(0, availableDays.Count);
                        int selectedDay = availableDays[randomIndex];
                        customer.dayVisit = selectedDay;
                        dayCharacterCount[selectedDay]++;

                        if (selectedDay == currentDay)
                        {
                            todaysCustomers.Add(customerPrefab);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("No available days left to assign for customer: " + customer.firstName);
                    }
                }
            }
        }

        Debug.Log("Today's Customers Count: " + todaysCustomers.Count);
        Debug.Log("Excluded Customers Count: " + excludedCustomers.Count);
    }

    private IEnumerator StartDay(List<GameObject> todaysCustomers, List<GameObject> excludedCustomers)
    {
        if (currentDay <= totalDays)
        {
            ShowLoadingScreen();
            yield return new WaitForSeconds(2); // Simulate loading time
            HideLoadingScreen();

            // Pass the customer lists to the DayManager
            if (dayManager != null)
            {
                dayManager.SetupDay(currentDay, customersPerDay, todaysCustomers, excludedCustomers);
            }
        }
        else
        {
            EndLevel();
        }
    }

    // Method to update the day number
    private void UpdateDayNumber(int newDayNumber)
    {
        currentDay = newDayNumber;
        // Notify the Managers of the day change
        ItemManager itemManager = FindObjectOfType<ItemManager>();
        PosterManager posterManager = FindObjectOfType<PosterManager>();
        PopupManager popupManager = FindObjectOfType<PopupManager>();
        NewsManager newsManager = FindObjectOfType<NewsManager>();
        EmailManager emailManager = FindObjectOfType<EmailManager>();
      CustomerManager customerManager = FindObjectOfType<CustomerManager>();
        itemManager.UpdateDayNumber(currentDay);
        emailManager.UpdateDayNumber(currentDay);
        posterManager.UpdateDayNumber(currentDay);
        popupManager.UpdateDayNumber(currentDay);
        newsManager.UpdateDayNumber(currentDay);
        if (customerManager != null){
      customerManager.UpdateDayNumber(currentDay);
        }
  

    }


    public void EndDay()
    {

        ShowEndOfDayPanel(); // Show the end of day panel when the day ends

    }


    private void ShowLoadingScreen()
    {
        loadingScreen.SetActive(true);
    }

    private void HideLoadingScreen()
    {
        cameraManager.SwitchToMainCamera();
        loadingScreen.SetActive(false);
        uiManager.ShowLibraryMainView();
    }


    private void ShowEndOfDayPanel()
    {
        endOfDayPanel.SetActive(true);

        bookManager.CheckControversialBooks();
        uiManager.DisplayEndOfDayStats(statsManager); // Display stats on the panel
    }

  public void StartNextDay()
{
    currentDay++;
    if (currentDay <= totalDays)
    {
        UpdateDayNumber(currentDay);

        // Re-assign customers for the new day
        List<GameObject> todaysCustomers;
        List<GameObject> excludedCustomers = new List<GameObject>(); // Initialize the excluded list

        AssignRandomDaysToCustomers(out todaysCustomers, excludedCustomers);

        // Pass the customers lists to the StartDay method
        StartCoroutine(StartDay(todaysCustomers, excludedCustomers));
    }
    else
    {
        EndLevel();
    }
}


    private List<GameObject> GetTodaysCustomers()
    {
        List<GameObject> todaysCustomers = new List<GameObject>();
        foreach (GameObject customerPrefab in remainingCustomers)
        {
            Customer customer = customerPrefab.GetComponent<Customer>();
            if (customer != null && customer.dayVisit == currentDay)
            {
                todaysCustomers.Add(customerPrefab);
            }
        }

        return todaysCustomers;
    }

    private void EndLevel()
    {
        
        SceneManager.LoadScene("EndScene");
    }

    private int CalculateInitialPEP()
    {
        int initialPEP = 0;
        foreach (GameObject customerPrefab in customerPrefabs)
        {
            Customer customer = customerPrefab.GetComponent<Customer>();
            if (customer != null)
            {
                initialPEP += customer.initialPEP;
            }
        }
        return initialPEP;
    }
}