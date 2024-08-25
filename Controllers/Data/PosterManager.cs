using System.Collections.Generic;
using UnityEngine;

public class PosterManager : MonoBehaviour
{
    public List<Vector3> startingPositions = new List<Vector3>(); // List of 5 starting positions
    public Vector2 randomOffsetRange = new Vector2(-0.1f, 0.1f); // Define the random offset range
    public GameObject posterPrefab; // The prefab for the posters
    private List<GameObject> activePosters = new List<GameObject>(); // To track currently active posters

    private CSVPosterLoader csvPosterLoader;
    private List<Poster> allPosters;

    private int curDay;

    void Start()
    {
        csvPosterLoader = GetComponent<CSVPosterLoader>();
        allPosters = csvPosterLoader.LoadPosters();
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        curDay = levelManager.currentDay;
        DisplayPostersForDay(curDay); // Example: Display posters for day 1 at start
    }

    public void DisplayPostersForDay(int day)
    {
        // ClearActivePosters(); // Clear old posters

        List<Poster> postersForToday = allPosters.FindAll(p => p.posterDay <= day);

        for (int i = 0; i < postersForToday.Count && i < startingPositions.Count; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(randomOffsetRange.x, randomOffsetRange.y),
                Random.Range(randomOffsetRange.x, randomOffsetRange.y),
                0f
            );

            Vector3 finalPosition = startingPositions[i] + randomOffset;

            GameObject posterObject = Instantiate(posterPrefab, finalPosition, Quaternion.identity);
            posterObject.GetComponent<SpriteRenderer>().sprite = postersForToday[i].posterImage;
            posterObject.GetComponent<SpriteRenderer>().sortingOrder = 5; // Ensures the layer order is at least 5

            activePosters.Add(posterObject);
        }
    }

    private void ClearActivePosters()
    {
        foreach (GameObject poster in activePosters)
        {
            Destroy(poster);
        }
        activePosters.Clear();
    }

       public void UpdateDayNumber(int newDayNumber)
    {
        curDay = newDayNumber;
        DisplayPostersForDay(curDay); // Refresh the inventory based on the new day
        Debug.Log("Posters updated for day: " + curDay);
    }
}





