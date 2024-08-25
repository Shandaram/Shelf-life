using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject MainView;
    public GameObject CustomerView;
    public GameObject DeskView;
    public GameObject CustomerActiveView;
    public GameObject PCView;
    public GameObject BookBaseView;
    public GameObject GrantsView;
    public GameObject EmailsView;

    public GameObject ReportsView;

    public GameObject NewCardView;

    public GameObject CardsView;
    public GameObject JunkView;
    public GameObject MainMenu;
    public GameObject EndDayWarning;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI pepText;
    public TextMeshProUGUI repText;
    public TextMeshProUGUI fundsText;
    public TextMeshProUGUI bookText;

    // Reference to the panel you want to move
    public RectTransform movablePanel;

    // Movement distance (change this to adjust the movement)
    private float moveDistance = 200f;

    // Track if the panel is moved down or not
    private bool isPanelMovedDown = true;

    private BookManager bookManager;

    void Start()
    {
        ShowLibraryMainView();
    }


    public void ShowLibraryMainView()
    {
        MainView.SetActive(true);
        CustomerView.SetActive(false);
        DeskView.SetActive(false);
        CustomerActiveView.SetActive(false);
        PCView.SetActive(false);
        NewCardView.SetActive(false);
        CardsView.SetActive(false);
        JunkView.SetActive(false);
        MainMenu.SetActive(false);
        FindObjectOfType<LevelManager>().endOfDayPanel.SetActive(false);
        EndDayWarning.SetActive(false);

    }

    public void ShowCustomerView()
    {
        MainView.SetActive(false);
        CustomerView.SetActive(true);
        DeskView.SetActive(false);
        PCView.SetActive(false);
    }

    public void ShowCustomerActiveView()
    {
        MainView.SetActive(false);
        CustomerView.SetActive(true);
        DeskView.SetActive(false);
        PCView.SetActive(false);
        CustomerActiveView.SetActive(true);
    }

    public void ShowDeskView()
    {
        MainView.SetActive(false);
        CustomerView.SetActive(false);
        DeskView.SetActive(true);
        PCView.SetActive(false);
    }

    public void ShowPCView()
    {
        MainView.SetActive(false);
        CustomerView.SetActive(false);
        DeskView.SetActive(false);
        PCView.SetActive(true);
        BookBaseView.SetActive(false);
        GrantsView.SetActive(false);
        EmailsView.SetActive(false);
        ReportsView.SetActive(false);
        CustomerActiveView.SetActive(false);
    }

    public bool IsPCViewPanelOpen()
    {
        return PCView.activeSelf;
    }

    public void OpenGrants()
    {
        GrantsView.SetActive(true);
    }
    public void OpenBookBase()
    {
        BookBaseView.SetActive(true);
    }

    public void OpenEmails()
    {
        EmailsView.SetActive(true);
    }

    public void OpenReports()
    {
        ReportsView.SetActive(true);
    }
    public void CloseEmails()
    {
        EmailsView.SetActive(false);
    }

    public void CloseBookBase()
    {
        BookBaseView.SetActive(false);
    }

    public void CloseGrants()
    {
        GrantsView.SetActive(false);
    }


    public void CloseReports()
    {
        ReportsView.SetActive(false);
    }


    public void ClosePatronCards()
    {
        CardsView.SetActive(false);

    }

    public void CloseEndOfDayWarning()
    {
        EndDayWarning.SetActive(false);

    }


    public void CloseJunkDrawer()
    {
        JunkView.SetActive(false);

    }
    public void ToggleMenu()
    {
        if (MainMenu.activeSelf) // Check if the menu is currently active
        {
            Debug.Log("main-menu-active");
            MainMenu.SetActive(false); // Deactivate the menu
        }
        else
        {
            Debug.Log("main-menu-off");
            MainMenu.SetActive(true); // Activate the menu
        }
    }



    // Method to toggle the panel's position
    public void TogglePanelPosition()
    {
        // Get the current position of the panel
        Vector3 currentPosition = movablePanel.anchoredPosition;

        if (isPanelMovedDown)
        {
            // Move the panel back up
            movablePanel.anchoredPosition = new Vector3(currentPosition.x, currentPosition.y + moveDistance, currentPosition.z);
        }
        else
        {
            // Move the panel down
            movablePanel.anchoredPosition = new Vector3(currentPosition.x, currentPosition.y - moveDistance, currentPosition.z);
        }

        // Toggle the boolean value
        isPanelMovedDown = !isPanelMovedDown;
    }


    public void OnNextDayButtonClicked()
    {
        FindObjectOfType<LevelManager>().endOfDayPanel.SetActive(false);
        FindObjectOfType<LevelManager>().StartNextDay(); // Start the next day
    }

    public void DisplayEndOfDayStats(StatsManager statsManager)
    {
        // Update the text fields with stats from the StatsManager

        bookManager = FindObjectOfType<BookManager>();
        pepText.text = "patron engagement points: " + statsManager.PEP.ToString();
        fundsText.text = "current Funds: " + statsManager.Coins.ToString();
        repText.text = "Library reputation: " + statsManager.REP.ToString();
        bookText.text = "books: " + bookManager.bookCount.ToString();

    }
}

