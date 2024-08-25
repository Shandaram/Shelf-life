using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;


[System.Serializable]
public class Poster : ScriptableObject
{

  public string posterName;
  public int posterDay;
  public Sprite posterImage;
}

public class CSVPosterLoader : MonoBehaviour
{
    public TextAsset csvFile;  // Drag and drop your CSV file here in the Unity Inspector
    
    public List<Poster> LoadPosters()
    {
        List<Poster> posters = new List<Poster>();
        string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++) // Start from 1 if the first row is a header
        {
            string[] fields = Regex.Split(lines[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            if (fields.Length < 2)
            {
                Debug.LogWarning($"Skipping row {i} due to insufficient columns: {fields.Length}");
                continue; // Ensure the row has enough columns
            }

            // Trim any extra quotes or spaces around the fields
            for (int j = 0; j < fields.Length; j++)
            {
                fields[j] = fields[j].Trim().Trim('"');
            }

            Poster poster = ScriptableObject.CreateInstance<Poster>();

            poster.posterName = fields[0];
        
            if (int.TryParse(fields[1], out int posterDay))
            {
                if (posterDay == 0)
                {
                    // Generate a random number between 1 and 10
                    posterDay = Random.Range(1, 11);

                    // Save this value using PlayerPrefs so it remains consistent across game sessions
                    PlayerPrefs.SetInt($"Poster_{poster.posterName}_Day", posterDay);
                }
                poster.posterDay = posterDay;
            }
            else
            {
                // If posterDay is not zero, check if we already have it saved
                if (PlayerPrefs.HasKey($"Poster_{poster.posterName}_Day"))
                {
                    poster.posterDay = PlayerPrefs.GetInt($"Poster_{poster.posterName}_Day");
                }
                else
                {
                    poster.posterDay = posterDay;
                }
            }

            // Load the Sprite from the Resources/Posters folder using the image name
            poster.posterImage = Resources.Load<Sprite>($"Posters/{poster.posterName}");

            if (poster.posterImage == null)
            {
                Debug.LogWarning($"Image '{poster.posterName}' not found in Resources/Posters folder");
                continue;
            }

            posters.Add(poster);

          
        }
                // Sort posters by posterDay
        posters.Sort((x, y) => x.posterDay.CompareTo(y.posterDay));
          Debug.Log($"Total posters loaded: {posters.Count}");

        return posters;
    }
}