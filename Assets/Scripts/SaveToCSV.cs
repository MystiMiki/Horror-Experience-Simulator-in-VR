using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public abstract class SaveToCSV<T> : MonoBehaviour where T : struct
{
    // Call this function when the level is finished to save data to CSV
    protected void SaveDataToCSV(string nameOfData, List<T> dataList, DateTime sceneLoadTime)
    {
        T[] dataArray = dataList.ToArray();

        // Define the directory path
        string directoryPath = Path.Combine(Application.dataPath, "Data", nameOfData);

        // Create the directory if it doesn't exist
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Generate a unique filename using the scene load time
        string filename = $"{nameOfData}Data_{sceneLoadTime:yyyyMMdd_HHmmss}.csv";
        string filePath = Path.Combine(directoryPath, filename);

        // Use a CSV writing function to write the data to a CSV file
        WriteToCSV(filePath, dataArray);

        // Clear the data list after saving
        dataList.Clear();
    }

    protected abstract void WriteToCSV(string filePath, T[] data);
}
