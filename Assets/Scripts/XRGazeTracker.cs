using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System.IO;

public class XRGazeTracker : MonoBehaviour
{
    private List<GazeData> _gazeDataList = new List<GazeData>();
    private DateTime _sceneLoadTime;
    private float _timeSinceLastSave = 0f;
    private const float _saveInterval = 1f; // Set the save interval to 1 second

    void Start()
    {
        // Record the time when the scene is loaded
        _sceneLoadTime = DateTime.Now;
    }

    void Update()
    {
        // Get the player's head position and forward direction
        Vector3 gazeDirection = GetGazeDirection();

        // Normalize the gaze direction vector
        Vector3 normalizedGazeDirection = gazeDirection.normalized;

        // Calculate the timestamp based on the time difference between now and when the scene was loaded
        float timestamp = (float)(DateTime.Now - _sceneLoadTime).TotalSeconds + 1;

        // Accumulate the elapsed time
        _timeSinceLastSave += Time.deltaTime;

        // Check if the save interval has passed
        if (_timeSinceLastSave >= _saveInterval)
        {
            // Save data every second
            _gazeDataList.Add(new GazeData(timestamp, normalizedGazeDirection));
            // Reset the timer
            _timeSinceLastSave = 0f;
        }

        // Your additional logic or checks here based on your game requirements
    }

    // Function to get the gaze direction of the player
    Vector3 GetGazeDirection()
    {
        // Get the player's head rotation
        InputDevices.GetDeviceAtXRNode(XRNode.Head).TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion headRotation);

        // Get the forward direction from the head rotation
        Vector3 gazeDirection = headRotation * Vector3.forward;

        return gazeDirection;
    }

    // Call this function when the level is finished to save data to CSV
    public void SaveDataToCSV()
    {
        GazeData[] gazeDataArray = _gazeDataList.ToArray();

        // Define the directory path
        string directoryPath = Path.Combine(Application.dataPath, "Data", "Gaze");

        // Create the directory if it doesn't exist
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Generate a unique filename using the scene load time
        string filename = $"gazeData_{_sceneLoadTime:yyyyMMdd_HHmmss}.csv";
        string filePath = Path.Combine(directoryPath, filename);

        // Use a CSV writing function to write the data to a CSV file
        WriteToCSV(filePath, gazeDataArray);

        // Clear the data list after saving
        _gazeDataList.Clear();
    }

    // Function to write data to a CSV file
    void WriteToCSV(string filePath, GazeData[] gazeData)
    {
        // Create a new CSV file or overwrite if it already exists
        using (StreamWriter file = new StreamWriter(filePath))
        {
            // Write header
            file.WriteLine("Timestamp;" +
                "NormalizedGazeDirectionX;" +
                "NormalizedGazeDirectionY;" +
                "NormalizedGazeDirectionZ");

            // Write gaze data
            foreach (GazeData gazeDataEntry in gazeData)
            {
                file.WriteLine($"{gazeDataEntry.timestamp.ToString().Split(',')[0]};" +
                    $"{gazeDataEntry.normalizedGazeDirection.x};" +
                    $"{gazeDataEntry.normalizedGazeDirection.y};" +
                    $"{gazeDataEntry.normalizedGazeDirection.z}");
            }
        }
    }

    // Data structure to store gaze direction, timestamp, and other relevant data
    private struct GazeData
    {
        public float timestamp;
        public Vector3 normalizedGazeDirection;

        public GazeData(float timestamp, Vector3 normalizedGazeDirection)
        {
            this.timestamp = timestamp;
            this.normalizedGazeDirection = normalizedGazeDirection;
        }
    }

    // OnDestroy is called when the GameObject is being destroyed
    void OnDisable()
    {
        // Call SaveDataToCSV when the GameObject is destroyed
        SaveDataToCSV();
    }
}
