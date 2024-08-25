using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

using System.IO;


public class CSVGrantLoader : MonoBehaviour
{
    public TextAsset csvFile;  // Drag and drop your CSV file here in the Unity Inspector
   
    public List<Grant> LoadGrants()
    {
        List<Grant> grants = new List<Grant>();
        string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++) // Start from 1 if the first fields is a header
        {

            string[] fields = Regex.Split(lines[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            // Debug log to check fields length and content
       

            if (fields.Length < 22)
            {
                Debug.LogWarning($"Skipping fields {i} due to insufficient columns: {fields.Length}");
                continue; // Ensure the fields has enough columns
            }

            // Trim any extra quotes or spaces around the fields
            for (int j = 0; j < fields.Length; j++)
            {
                fields[j] = fields[j].Trim().Trim('"');
            }
            Grant grant = ScriptableObject.CreateInstance<Grant>();

            grant.grantName = fields[0];
            grant.objective = fields[1];
            grant.description = fields[2];
            grant.tier = fields[3];
            grant.location = fields[4];

            if (int.TryParse(fields[5], out int pepRequired))
            {
                grant.pepRequired = pepRequired;
            }
            else
            {
                grant.pepRequired = 0; // Default or error value
            }

            if (int.TryParse(fields[6], out int repRequired))
            {
                grant.repRequired = repRequired;
            }
            else
            {
                grant.repRequired = 0; // Default or error value
            }

            //deleted//

            if (int.TryParse(fields[7], out int bookCountRequired))
            {
                grant.bookCountRequired = bookCountRequired;
            }
            else
            {
                grant.bookCountRequired = 0; // Default or error value
            }

            grant.bookGenreRequired = fields[8];

            if (int.TryParse(fields[9], out int bookGenreCountRequired))
            {
                grant.bookGenreCountRequired = bookGenreCountRequired;
            }
            else
            {
                grant.bookGenreCountRequired = 0; // Default or error value
            }

            grant.bookGenreRequired2 = fields[10];

            if (int.TryParse(fields[11], out int bookGenreCountRequired2))
            {
                grant.bookGenreCountRequired2 = bookGenreCountRequired2;
            }
            else
            {
                grant.bookGenreCountRequired2 = 0; // Default or error value
            }

            grant.repairTypeRequired = fields[12];

            if (int.TryParse(fields[13], out int repairCountRequired))
            {
                grant.repairCountRequired = repairCountRequired;
            }
            else
            {
                grant.repairCountRequired = 0; // Default or error value
            }

            grant.upgradeTypeRequired = fields[14];

            if (int.TryParse(fields[15], out int upgradeCountRequired))
            {
                grant.upgradeCountRequired = upgradeCountRequired;
            }
            else
            {
                grant.upgradeCountRequired = 0; // Default or error value
            }

            grant.authorNameRequired = fields[16];

            if (int.TryParse(fields[17], out int authorCountRequired))
            {
                grant.authorCountRequired = authorCountRequired;
            }
            else
            {
                grant.authorCountRequired = 0; // Default or error value
            }

            if (int.TryParse(fields[18], out int rewardCoins))
            {
                grant.rewardCoins = rewardCoins;
            }
            else
            {
                grant.rewardCoins = 0; // Default or error value
            }

            grant.rewardType = fields[19];

            if (int.TryParse(fields[20], out int rewardCount))
            {
                grant.rewardCount = rewardCount;
            }
            else
            {
                grant.rewardCount = 0; // Default or error value
            }

            grant.funded = fields[21];


            grants.Add(grant);

        
        }
    Debug.Log($"Total grants loaded: {grants.Count}");
        return grants;
    }
}





