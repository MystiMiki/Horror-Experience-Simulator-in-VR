using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SimulatorSettingsTracker : MonoBehaviour
{
    private List<string> lines = new List<string>();
    private DateTime sceneLoadTime;

    void Start()
    {
        // Record the time when the scene is loaded
        sceneLoadTime = DateTime.Now;
    }

    // Save simulation settings to CSV file
    private void SaveSettingsToCSV()
    {
        // Define the directory path
        string directoryPath = Path.Combine(Application.dataPath, "Data", "SimulatorSettings");

        // Create the directory if it doesn't exist
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Generate a unique filename using the scene load time
        string filename = $"settings_{sceneLoadTime:yyyyMMdd_HHmmss}.csv";
        string filePath = Path.Combine(directoryPath, filename);

        // Add header
        lines.Add("numberOfRooms,soundIndex,limitedResources,documents,keys,spiders,scavenger,mutatedInsect");

        string sound = (SimulatorSettings.soundIndex == 0) ? "ambient" : "silence";

        // Add values from SimulatorSettings
        lines.Add($"{SimulatorSettings.numberOfRooms},{sound}," +
                  $"{SimulatorSettings.limitedResources},{SimulatorSettings.documents}," +
                  $"{SimulatorSettings.keys},{SimulatorSettings.spiders}," +
                  $"{SimulatorSettings.scavenger},{SimulatorSettings.mutatedInsect}");

        // Write to file
        File.WriteAllLines(filePath, lines);

        // Clear the data list after saving
        lines.Clear();
    }

    // OnDestroy is called when the GameObject is being destroyed
    void OnDisable()
    {
        // Call SaveSettingsToCSV when the GameObject is destroyed
        SaveSettingsToCSV();
    }
}
