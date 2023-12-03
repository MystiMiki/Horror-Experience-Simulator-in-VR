using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadPlayerDocuments : MonoBehaviour
{
    // Static list to store TextAssets
    public static List<TextAsset> textAssets = new List<TextAsset>();

    // Start is called before the first frame update
    void Start()
    {
        // Specify the folder path where your .txt files are located
        string folderPath = "PlayerDocuments";

        // Get all .txt files in the specified folder
        string[] txtFiles = Directory.GetFiles(Path.Combine("Assets", "Resources", folderPath), "*.txt");

        // Read and process each file
        foreach (string filePath in txtFiles)
        {
            // Load the TextAsset directly (without extension)
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            TextAsset textAsset = Resources.Load<TextAsset>(Path.Combine(folderPath, fileName));

            if (textAsset != null)
            {
                // Add the TextAsset to the list
                textAssets.Add(textAsset);
            }
            else
            {
                Debug.LogError("TextAsset not found: " + fileName);
            }
        }
    }
}
