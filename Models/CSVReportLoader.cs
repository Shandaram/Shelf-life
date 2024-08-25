using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;

public class CSVReportLoader : MonoBehaviour
{
    public TextAsset csvFile;  // Drag and drop your CSV file here in the Unity Inspector
  
    public List<Report> LoadReports()
    {
        List<Report> reports = new List<Report>();
        string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++) // Start from 1 if the first row is a header
        {
            string[] fields = Regex.Split(lines[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
       

            if (fields.Length < 6)
            {
                Debug.LogWarning($"Skipping row {i} due to insufficient columns: {fields.Length}");
                continue; // Ensure the row has enough columns
            }

            // Trim any extra quotes or spaces around the fields
            for (int j = 0; j < fields.Length; j++)
            {
                fields[j] = fields[j].Trim().Trim('"');
            }

            Report report = ScriptableObject.CreateInstance<Report>();

            report.type = fields[1];
            report.description = fields[2];
            report.followup = fields[3];
            report.date = fields[4];
            report.patron = fields[5];
            
            reports.Add(report);

     
        }
  
       Debug.Log($"Total reports loaded: {reports.Count}");
        return reports;
    }
}
