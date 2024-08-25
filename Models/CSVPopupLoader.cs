using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;


[System.Serializable]
public class Popup : ScriptableObject
{
    public string popupName;
    public string popupType;
    public string popupTrigger;
    public string popupText;
    public string ChoiceA;
    public int REP_A;
    public int PEP_A;
    public int Coins_A;
    public string Next_A;
    public string ChoiceB;
    public int REP_B;
    public int PEP_B;
    public int Coins_B;
    public string Next_B;
    public string Genre1;
    public int Genre1Qty;
    public string Genre2;
    public int Genre2Qty;
    public Sprite popupImage;

    public int triggerDay;
}

public class CSVPopupLoader : MonoBehaviour
{
    public TextAsset csvFile;  // Drag and drop your CSV file here in the Unity Inspector
 
    public List<Popup> LoadPopups()
    {
        List<Popup> popups = new List<Popup>();
        string[] lines = csvFile.text.Split('\n');
        System.Random random = new System.Random(); // Declare and initialize the random object here


        for (int i = 1; i < lines.Length; i++) // Start from 1 to skip the header
        {
            string[] fields = Regex.Split(lines[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            if (fields.Length < 20)
            {
                Debug.LogWarning($"Skipping row {i} due to insufficient columns: {fields.Length}");
                continue; // Ensure the row has enough columns
            }

            // Trim any extra quotes or spaces around the fields
            for (int j = 0; j < fields.Length; j++)
            {
                fields[j] = fields[j].Trim().Trim('"');
            }

            // Create a new Popup instance
            Popup popup = ScriptableObject.CreateInstance<Popup>();

            // Assign values to the Popup fields
            popup.popupName = fields[0];
            popup.popupType = fields[1];
            popup.popupTrigger = fields[2];
            popup.popupText = fields[3];
            popup.ChoiceA = fields[4];

            // Implementing IsNullOrEmpty check for int values
            popup.REP_A = string.IsNullOrEmpty(fields[5]) ? 0 : int.Parse(fields[5]);
            popup.PEP_A = string.IsNullOrEmpty(fields[6]) ? 0 : int.Parse(fields[6]);
            popup.Coins_A = string.IsNullOrEmpty(fields[7]) ? 0 : int.Parse(fields[7]);
            popup.Next_A = fields[8];
            popup.ChoiceB = fields[9];
            popup.REP_B = string.IsNullOrEmpty(fields[10]) ? 0 : int.Parse(fields[10]);
            popup.PEP_B = string.IsNullOrEmpty(fields[11]) ? 0 : int.Parse(fields[11]);
            popup.Coins_B = string.IsNullOrEmpty(fields[12]) ? 0 : int.Parse(fields[12]);
            popup.Next_B = fields[13];
            popup.Genre1 = fields[14];
            popup.Genre1Qty = string.IsNullOrEmpty(fields[15]) ? 0 : int.Parse(fields[15]);
            popup.Genre2 = fields[16];
            popup.Genre2Qty = string.IsNullOrEmpty(fields[17]) ? 0 : int.Parse(fields[17]);


            // Load the Sprite from the Resources folder using the image name
            string imageName = fields[18];
            popup.popupImage = Resources.Load<Sprite>($"Popup/{imageName}");

            if (popup.popupImage == null)
            {
                Debug.LogWarning($"Image '{imageName}' not found in Resources/Popups folder for event '{popup.popupName}'");
            }

        
                // For other popup types, assign the specified day
                popup.triggerDay = string.IsNullOrEmpty(fields[19]) ? 0 : int.Parse(fields[19]);
            

            // Add the Popup to the list
            popups.Add(popup);
        }

        Debug.Log($"Total popups loaded: {popups.Count}");
        return popups;
    }
}







