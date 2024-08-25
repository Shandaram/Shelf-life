using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;


[System.Serializable]
public class BookData : ScriptableObject
{
    public string title;
    public string author;
    public int published;
    public string blurb;
    public int quantity;
    public bool controversial;
    public string genre;
}
public class CSVLoader : MonoBehaviour
{
    public TextAsset defaultCsvFile; // Reference to the default TextAsset in Resources

    // private string persistentFilePath;

    // public string PersistentFilePath => persistentFilePath;

    // void Start()
    // {
    //     // Initialize the file path
    //     persistentFilePath = Path.Combine(Application.persistentDataPath, "BookDB.csv");
        
    //     // Initialize the database file
    //     InitializeDatabase();
    // }

    // Initialize the database by checking if the file exists and copying the default if necessary
    // public void InitializeDatabase()
    // {
    //     if (!File.Exists(persistentFilePath))
    //     {
    //         Debug.Log("Database file not found. Copying default database.");
    //         CopyDefaultDatabase();
    //     }
    //     else
    //     {
    //         Debug.Log("Database file found.");
    //     }

    //     // Load the books from the CSV file after initialization
    //     List<BookData> books = LoadBooksFromCSV(persistentFilePath);
    //     foreach (var book in books)
    //     {
    //         book.quantity = 0;
    //     }

    // }

    // Copy the default CSV file from Resources to persistent data path
    // private void CopyDefaultDatabase()
    // {
    //     if (defaultCsvFile == null)
    //     {
    //         Debug.LogError("Default CSV file is missing or not assigned.");
    //         return;
    //     }

    //     // Write the default CSV file to the persistent file path
    //     File.WriteAllBytes(persistentFilePath, defaultCsvFile.bytes);
    //     Debug.Log($"Copied default database to {persistentFilePath}");

    //     // Load the copied CSV and reset quantities to 0
    //     List<BookData> books = LoadBooksFromCSV(persistentFilePath);
    //     foreach (var book in books)
    //     {
    //         book.quantity = 0;
    //     }

    //     // Save the updated books back to the persistent file
    //     SaveBooksToCSV(books);
    //     Debug.Log("Reset all book quantities to 0 for debugging purposes.");
    // }

    // Load books from the CSV file located at the specified path
    public List<BookData> LoadBooksFromCSV()
    {
        List<BookData> books = new List<BookData>();

        // if (!File.Exists(filePath))
        // {
        //     Debug.LogError("CSV file not found at path: " + filePath);
        //     return books;
        // }

        // string[] lines = File.ReadAllLines(filePath);
           string[] lines = defaultCsvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++) // Start at 1 to skip header
        {
               string[] fields = Regex.Split(lines[i], ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            if (fields.Length < 7) // To handle incomplete rows
                continue;

            BookData book = ScriptableObject.CreateInstance<BookData>();

            book.title = fields[0];
            book.author = fields[1];

            // Safe parsing of Published year
            if (int.TryParse(fields[2], out int publishedYear))
            {
                book.published = publishedYear;
            }
            else
            {
                book.published = 0; // Default or error value
            }

            book.genre = fields[3];

            // Safe parsing of Quantity
            if (int.TryParse(fields[4], out int quantity))
            {
                book.quantity = quantity;
            }
            else
            {
                book.quantity = 0; // Default or error value
            }

            book.blurb = fields[5];

            // Parse Controversial (yes/no)
            book.controversial = fields[6].Trim().ToLower() == "yes";

            books.Add(book);
        }

        return books;
    }

    // Save the books back to the CSV file
    // public void SaveBooksToCSV(List<BookData> books)
    // {
    //     using (StreamWriter writer = new StreamWriter(persistentFilePath))
    //     {
    //         // Write the header
    //         writer.WriteLine("Title,Author,Published,Genre,Quantity,Blurb,Controversial");

    //         // Write the data rows
    //         foreach (BookData book in books)
    //         {
    //             // Ensure that any field with commas is enclosed in double quotes
    //             string line = $"{EscapeCSVField(book.title)},{EscapeCSVField(book.author)},{book.published},{EscapeCSVField(book.genre)},{book.quantity},{EscapeCSVField(book.blurb)},{(book.controversial ? "yes" : "no")}";
    //             writer.WriteLine(line);
    //         }
    //     }

    //     // Debug.Log($"Books saved to {persistentFilePath}");
    // }

    // Method to escape fields with commas for CSV formatting
    // private string EscapeCSVField(string field)
    // {
    //     if (field.Contains(","))
    //     {
    //         return $"\"{field}\"";
    //     }
    //     return field;
    // }
}




