using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
public class DayManager : MonoBehaviour
{
    public float dayDuration;
    public float minSpawnDelay = 1f;
    public float maxSpawnDelay = 5f;
    public int maxCustomersPerDay;
    public float minCustomerInterval; // Minimum interval between customers
    public float maxCustomerInterval; // Maximum interval between customers
    public Transform customerSpawnPoint;
    public GameObject dialoguePanel; // Reference to the dialogue panel
  public AudioSource spawnSoundSource; // Reference to the AudioSource component
   public GameObject endDayPopup; // Assign this in the Inspector, it should be the popup UI
    public Button okButton; // Assign this in the Inspector, it should be the OK button in the popup
    // public Button extendDayButton; 


    private LevelManager levelManager;
        private UIManager uiManager;
    private StatsManager statsManager; // Reference to the StatsManager
    private List<GameObject> availableCustomers;
    private List<GameObject> excludedCustomers;
    private float elapsedTime = 0f;
    private int customersServed = 0;
    public bool customerDialogueFinished  = true;
    public bool customerPresent = false;
    private bool dayEnded = false;
    private Coroutine checkBookReturnsCoroutine;


    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
         uiManager = FindObjectOfType<UIManager>();
        dialoguePanel.SetActive(false); // Ensure the dialogue panel is initially hidden
         endDayPopup.SetActive(false);
    }



    public void SetupDay(int dayNumber, int customersPerDay, List<GameObject> todaysCustomers, List<GameObject> passedExcludedCustomers)
    {
        availableCustomers = new List<GameObject>(todaysCustomers);
        excludedCustomers = new List<GameObject>(passedExcludedCustomers); // Use a different name for the parameter to avoid confusion
        Debug.Log("DayManager: excludedCustomers count after assignment: " + excludedCustomers.Count);

        statsManager = FindObjectOfType<StatsManager>();
        dayEnded = false;

        StartCoroutine(RunDay());
        StartCoroutine(RandomlyIncreaseBookReturns()); // Start the coroutine
        checkBookReturnsCoroutine = StartCoroutine(CheckBookReturns()); // Start checking bookReturns
    }
 private IEnumerator SpawnCustomerWithRandomDelay()
    {

   
        // Generate a random delay between minSpawnDelay and maxSpawnDelay
        float randomDelay = Random.Range(minSpawnDelay, maxSpawnDelay);

        // Wait for the random delay duration
        yield return new WaitForSeconds(randomDelay);

        // Call the SpawnCustomer method after the delay
        SpawnCustomer();
    }
    private IEnumerator RunDay()
    {
        elapsedTime = 0f;
        customersServed = 0;
        StartTimer();
        yield return new WaitForSeconds(20f);
        while (elapsedTime < dayDuration && !dayEnded)
        {
            if (!customerPresent && customersServed < maxCustomersPerDay && availableCustomers.Count > 0 && customerDialogueFinished)
            {
                Debug.Log("CustomerPresent: " + customerPresent);
                StartCoroutine(SpawnCustomerWithRandomDelay());
                customersServed++;
                customerPresent = true;

                // Set a random interval for the next customer spawn
                float customerInterval = Random.Range(minCustomerInterval, maxCustomerInterval);
                yield return new WaitForSeconds(customerInterval);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        EndDay();
    }

    private void SpawnCustomer()
    {
       
        int randomIndex = Random.Range(0, availableCustomers.Count);
        GameObject customerObject = Instantiate(availableCustomers[randomIndex], customerSpawnPoint.position, Quaternion.identity);
        customerObject.SetActive(true); // Enable the customer GameObject when spawned
        availableCustomers.RemoveAt(randomIndex);

        Debug.Log("Spawned Customer: " + customerObject.name);

        CustomerManager customerManager = customerObject.GetComponent<CustomerManager>();
        customerManager.SetDayManager(this);
        Debug.Log("CustomerPresent: " + customerPresent);
        // Play the spawn sound
        if (spawnSoundSource != null)
        {
            spawnSoundSource.Play();
        }
        
        else
        {
            Debug.LogWarning("Spawn sound source is not assigned.");
        }
    }

    public void SpawnSpecificCustomer(string customerName)
    {

          customerPresent = true; 
        // Check if excludedCustomers list is initialized
        if (excludedCustomers == null || excludedCustomers.Count == 0)
        {
            Debug.LogError("excludedCustomers list is either null or empty.");
            return;
        }

        // Find the customer by firstName in the excludedCustomers list
        GameObject specificCustomer = excludedCustomers.Find(customerPrefab =>
        {
            Customer customer = customerPrefab.GetComponent<Customer>();
            return customer != null && customer.firstName.Equals(customerName, System.StringComparison.OrdinalIgnoreCase);
        });

        if (specificCustomer != null)
        {

            // Spawn the customer at the spawn point
            GameObject customerObject = Instantiate(specificCustomer, customerSpawnPoint.position, Quaternion.identity);
            customerObject.SetActive(true); // Activate the customer GameObject when spawned
  // Set the flag to true to prevent random customer spawning
          
            Debug.Log("Spawned Specific Customer: " + customerObject.name);
            Debug.Log("CustomerPresent: " + customerPresent);

            // Assign the DayManager to the CustomerManager component of the spawned customer
            CustomerManager customerManager = customerObject.GetComponent<CustomerManager>();
            if (customerManager != null)
            {
                customerManager.SetDayManager(this);
            }
            else
            {
                Debug.LogError("CustomerManager component is missing on " + customerObject.name);
            }

            // Switch camera to focus on the customer
            CameraManager cameraManager = FindAnyObjectByType<CameraManager>();
            if (cameraManager != null)
            {
                cameraManager.SwitchToCustomerView();
                uiManager.ShowCustomerView();
            }
            else
            {
                Debug.LogError("CameraManager could not be found in the scene.");
            }

         
        }
        else
        {
            Debug.LogWarning("Customer with name " + customerName + " not found in excludedCustomers list.");
        }
    }




    public void OnCustomerDialogueFinished()
    {

        Debug.Log("not talking: " + customerDialogueFinished + "customerpresent: " + customerPresent);
        customerDialogueFinished = true;
        customerPresent = false;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    private void StartTimer()
    {
        GameTimer gameTimer = FindObjectOfType<GameTimer>();
        gameTimer.StartTimer();
    }

   public void EndDay()
{
    // Check if the day has already ended to prevent duplicate end day logic
    if (!dayEnded)
    {
        // Set the day ended flag to prevent further customer spawns
        dayEnded = true;

        // Start a coroutine to wait for customer dialogue to finish before ending the day
        StartCoroutine(WaitForDialogueToFinishAndEndDay());
    }
}

    private void OnOkButtonClicked()
    {
        // Hide the popup
        endDayPopup.SetActive(false);

        // Proceed with ending the day
        levelManager.EndDay(); // Show the end of day panel or proceed to the next level
    }

    // private void OnExtendDayButtonClicked()
    // {
    //     // Hide the popup
    //     endDayPopup.SetActive(false);

    //     // Extend the day by 2 minutes
    //     FindObjectOfType<GameTimer>().ExtendDay(2 * 60); // Add 2 minutes in seconds
    //      Debug.Log("Day extended!");
    //     // Reset the dayEnded flag so the day can continue
    //     dayEnded = false;
    // }
private IEnumerator WaitForDialogueToFinishAndEndDay()
{
    // Wait until the customer dialogue is finished
    while (!customerDialogueFinished)
    {
        yield return null; // Wait for the next frame before checking again
    }

    // Once the dialogue is finished, proceed with the end day logic
    // Show the popup when the day is supposed to end
    endDayPopup.SetActive(true);

    // Disable all active customers
    foreach (var customer in availableCustomers)
    {
        customer.SetActive(false);
    }

    // Add listeners to the buttons in the popup
    okButton.onClick.RemoveAllListeners(); // Remove old listeners
    okButton.onClick.AddListener(OnOkButtonClicked);

    // Optionally, reset specificCustomerSpawned flag here if needed
}

    private IEnumerator RandomlyIncreaseBookReturns()
    {
        while (!dayEnded)
        {
            yield return new WaitForSeconds(Random.Range(20f, 60f)); // Wait for a random time between 5 and 30 seconds
            statsManager.UpdateBookReturns(Random.Range(1, 2)); // Increment bookReturns by a random amount between 1 and 3
        }
    }

    private IEnumerator CheckBookReturns()
    {
        while (!dayEnded)
        {
            if (statsManager.bookReturns > 10)
            {
                float timeAboveThreshold = 0f;

                while (statsManager.bookReturns > 10 && timeAboveThreshold < 60f)
                {
                    timeAboveThreshold += Time.deltaTime;
                    yield return null;
                }

                if (timeAboveThreshold >= 60f && statsManager.bookReturns > 10)
                {
                    statsManager.UpdateREP(- 1); // Deduct 1 REP
                }
            }
            yield return new WaitForSeconds(2f); // Check every 2 seconds
        }
    }
}
