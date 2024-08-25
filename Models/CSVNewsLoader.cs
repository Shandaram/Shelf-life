using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;

[System.Serializable]
public class News : ScriptableObject
{
    public string newsNegative;
    public string newsPositive;
    public int newsDay;
}

public class CSVNewsLoader : MonoBehaviour
{
    public TextAsset csvFile;  // Drag and drop your CSV file here in the Unity Inspector

    public List<News> LoadNewses()
    {
        List<News> newses = new List<News>();
         string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++) // Start from 1 if the first row is a header
        {
            string[] fields = Regex.Split(lines[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            if (fields.Length < 3)
            {
                Debug.LogWarning($"Skipping row {i} due to insufficient columns: {fields.Length}");
                continue; // Ensure the row has enough columns
            }
            for (int j = 0; j < fields.Length; j++)
            {
                fields[j] = fields[j].Trim().Trim('"');
            }

            News news = ScriptableObject.CreateInstance<News>();

            news.newsPositive = fields[1];
            news.newsNegative = fields[1];
            news.newsDay = string.IsNullOrEmpty(fields[2]) ? 0 : int.Parse(fields[2]);
            newses.Add(news);

        }
        // Sort newses by newsDay
        newses.Sort((x, y) => x.newsDay.CompareTo(y.newsDay));

        Debug.Log($"Total newses loaded: {newses.Count}");
        return newses;
    }

 
}