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
        string folderPath = "Assets/PlayerDocuments";

        // Get all .txt files in the specified folder
        string[] txtFiles = Directory.GetFiles(folderPath, "*.txt");

        // Read and process each file
        foreach (string filePath in txtFiles)
        {
            // Load the TextAsset directly
            TextAsset textAsset = Resources.Load<TextAsset>(filePath);

            if (textAsset != null)
            {
                // Add the TextAsset to the list
                textAssets.Add(textAsset);
            }
            else
            {
                Debug.LogError("TextAsset not found: " + filePath);
            }
        }
    }
}