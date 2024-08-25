using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;


public class NewsManager : MonoBehaviour
{
    public GameObject newsPanel;
    public TextMeshProUGUI titleText;
    public CSVNewsLoader csvNewsLoader;
    private List<News> newses;

    private StatsManager statsManager;

    private int currentDay = 1;

    void Start()
    {
        newsPanel.SetActive(false);
        newses = csvNewsLoader.LoadNewses();
        statsManager = FindObjectOfType<StatsManager>();

        if (newses != null && newses.Count > 0)
        {
            PopulateNews();
        }


  
    }
 void Update()
    {
        // Detect if the user clicks anywhere within the newsPanel and close it
        if (Input.GetMouseButtonDown(0))
        {
            // Check if the mouse is over the newsPanel
            if (IsPointerOverUIObject(newsPanel))
            {
                CloseNewsPanel();
            }
        }
    }
    void PopulateNews()
    {
        foreach (News news in newses)
        {

            if (news.newsDay == currentDay)
            {

                if (statsManager.REP > 5)
                {
                    titleText.text = news.newsPositive;

                }
                else
                {
                    titleText.text = news.newsNegative;

                }

            }
        }
    }


    // Method to update the current day number and trigger relevant popups
    public void UpdateDayNumber(int newDayNumber)
    {
        currentDay = newDayNumber;
        PopulateNews();
    }


      // Method to close the news panel
    public void CloseNewsPanel()
    {
        newsPanel.SetActive(false);
    }

     // Helper method to check if the pointer is over a UI element
    private bool IsPointerOverUIObject(GameObject uiElement)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == uiElement)
            {
                return true;
            }
        }

        return false;
    }
}