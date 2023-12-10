using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EventTracker : MonoBehaviour
{
    private List<EventData> eventDataList = new List<EventData>();
    private DateTime sceneLoadTime;
    private float timeSinceLastSave = 0f;
    private float saveInterval = 1f; // Set the save interval to 1 second

    void Start()
    {
        // Record the time when the scene is loaded
        sceneLoadTime = DateTime.Now;
    }

    void Update()
    {
        // Calculate the timestamp 
        float timestamp = (float)(DateTime.Now - sceneLoadTime).TotalSeconds - 1;

        // Accumulate the elapsed time
        timeSinceLastSave += Time.deltaTime;

        if (FlashlightController.batteryLife <= 0f) LogEvent("DeadBattery", timestamp);
        if (ClipboardPickupTracker.isInHand) LogEvent("ReadingClipboard", timestamp);
        if (MonsterInGazeTracker.isInGaze) LogEvent("MonsterInGaze", timestamp);
    }

    // Function to log an event
    void LogEvent(string eventName, float timestamp)
    {
        // Check if the save interval has passed
        if (timeSinceLastSave >= saveInterval)
        {
            // Save data every second
            eventDataList.Add(new EventData(timestamp, eventName));
            // Reset the timer
            timeSinceLastSave = 0f;
        }     
    }

    // Call this function when the level is finished to save data to CSV
    void SaveDataToCSV()
    {
        EventData[] eventDataArray = eventDataList.ToArray();

        // Define the directory path
        string directoryPath = Path.Combine(Application.dataPath, "Data", "HorrorEvents");

        // Create the directory if it doesn't exist
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Generate a unique filename using the scene load time
        string filename = $"eventData_{sceneLoadTime:yyyyMMdd_HHmmss}.csv";
        string filePath = Path.Combine(directoryPath, filename);

        // Use a CSV writing function to write the data to a CSV file
        WriteToCSV(filePath, eventDataArray);

        // Clear the data list after saving
        eventDataList.Clear();
    }

    // Function to write data to a CSV file
    void WriteToCSV(string filePath, EventData[] events)
    {
        // Create a new CSV file or overwrite if it already exists
        using (StreamWriter file = new StreamWriter(filePath))
        {
            // Write header
            file.WriteLine("Timestamp;Event");

            // Write event data
            foreach (EventData eventData in events)
            {
                file.WriteLine($"{eventData.timestamp.ToString().Split(',')[0]};{eventData.eventName}");
            }
        }
    }

    // Data structure to store event data, timestamp, and other relevant data
    private struct EventData
    {
        public float timestamp;
        public string eventName;

        public EventData(float timestamp, string eventName)
        {
            this.timestamp = timestamp;
            this.eventName = eventName;
        }
    }

    // OnDestroy is called when the GameObject is being destroyed
    void OnDisable()
    {
        // Call SaveDataToCSV when the GameObject is destroyed
        SaveDataToCSV();
    }
}
