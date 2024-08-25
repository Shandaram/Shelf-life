using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;

public class CSVItemLoader : MonoBehaviour
{
    public TextAsset csvFile;  // Drag and drop your CSV file here in the Unity Inspector
 
    public List<Item> LoadItems()
    {
        List<Item> items = new List<Item>();
        string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++) // Start from 1 if the first row is a header
        {
            string[] fields = Regex.Split(lines[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            if (fields.Length < 5)
            {
                Debug.LogWarning($"Skipping row {i} due to insufficient columns: {fields.Length}");
                continue; // Ensure the row has enough columns
            }

            // Trim any extra quotes or spaces around the fields
            for (int j = 0; j < fields.Length; j++)
            {
                fields[j] = fields[j].Trim().Trim('"');
            }

            Item item = ScriptableObject.CreateInstance<Item>();

            item.itemName = fields[0];
            item.itemText = fields[1];
        
            
            if (int.TryParse(fields[2], out int itemDay))
            {
                if (itemDay == 0)
                {
                    // Generate a random number between 1 and 10
                    itemDay = Random.Range(1, 11);

                    // Save this value using PlayerPrefs so it remains consistent across game sessions
                    PlayerPrefs.SetInt($"Item_{item.itemName}_Day", itemDay);
                }
                item.itemDay = itemDay;
            }
            else
            {
                // If itemDay is not zero, check if we already have it saved
                if (PlayerPrefs.HasKey($"Item_{item.itemName}_Day"))
                {
                    item.itemDay = PlayerPrefs.GetInt($"Item_{item.itemName}_Day");
                }
                else
                {
                    item.itemDay = itemDay;
                }
            }

            // Load the Sprite from the Resources/Drawer folder using the image name
            string imageName = fields[3];
            item.itemImage = Resources.Load<Sprite>($"Drawer/{imageName}");

            if (item.itemImage == null)
            {
                Debug.LogWarning($"Image '{imageName}' not found in Resources/Drawer folder for item '{item.itemName}'");
            }

            item.itemEvent = fields[4];

            items.Add(item);

        }
                // Sort items by itemDay
        items.Sort((x, y) => x.itemDay.CompareTo(y.itemDay));

            Debug.Log($"Total items loaded: {items.Count}");
        return items;
    }
}
