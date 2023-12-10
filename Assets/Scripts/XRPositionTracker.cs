using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System.IO;

public class XRPositionTracker : MonoBehaviour
{
    private List<PositionData> positionDataList = new List<PositionData>();
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
        Vector3 headsetPosition = GetDevicePosition(XRNode.Head);
        Vector3 leftControllerPosition = GetDevicePosition(XRNode.LeftHand);
        Vector3 rightControllerPosition = GetDevicePosition(XRNode.RightHand);

        // Calculate the timestamp based on the time difference between now and when the scene was loaded
        float timestamp = (float)(DateTime.Now - sceneLoadTime).TotalSeconds - 1;

        // Accumulate the elapsed time
        timeSinceLastSave += Time.deltaTime;

        // Check if the save interval has passed
        if (timeSinceLastSave >= saveInterval)
        {
            // Save data every second
            positionDataList.Add(new PositionData(timestamp, headsetPosition, leftControllerPosition, rightControllerPosition));
            // Reset the timer
            timeSinceLastSave = 0f;
        }

        // Your additional logic or checks here based on your game requirements
    }

    // Function to get the position of a specific XR device
    Vector3 GetDevicePosition(XRNode node)
    {
        Vector3 devicePosition = Vector3.zero;

        InputDevice device = InputDevices.GetDeviceAtXRNode(node);
        if (device.isValid)
        {
            device.TryGetFeatureValue(CommonUsages.devicePosition, out devicePosition);
        }

        return devicePosition;
    }

    // Call this function when the level is finished to save data to CSV
    public void SaveDataToCSV()
    {
        PositionData[] positionDataArray = positionDataList.ToArray();

        // Define the directory path
        string directoryPath = Path.Combine(Application.dataPath, "Data", "Kinematic");

        // Create the directory if it doesn't exist
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Generate a unique filename using the scene load time
        string filename = $"positionData_{sceneLoadTime:yyyyMMdd_HHmmss}.csv";
        string filePath = Path.Combine(directoryPath, filename);

        // Use a CSV writing function to write the data to a CSV file
        WriteToCSV(filePath, positionDataArray);

        // Clear the data list after saving
        positionDataList.Clear();
    }

    // Function to write data to a CSV file
    void WriteToCSV(string filePath, PositionData[] positions)
    {
        // Create a new CSV file or overwrite if it already exists
        using (StreamWriter file = new StreamWriter(filePath))
        {
            // Write header
            file.WriteLine("Timestamp;" +
                "HeadsetPositionX;HeadsetPositionY;HeadsetPositionZ;" +
                "LeftControllerPositionX;LeftControllerPositionY;LeftControllerPositionZ;" +
                "RightControllerPositionX;RightControllerPositionY;RightControllerPositionZ");

            // Write position data
            foreach (PositionData positionData in positions)
            {
                file.WriteLine($"{positionData.timestamp.ToString().Split(',')[0]};" +
                    $"{positionData.headsetPosition.x};{positionData.headsetPosition.y};{positionData.headsetPosition.z};" +
                    $"{positionData.leftControllerPosition.x};{positionData.leftControllerPosition.y};{positionData.leftControllerPosition.z};" +
                    $"{positionData.rightControllerPosition.x};{positionData.rightControllerPosition.y};{positionData.rightControllerPosition.z}");
            }
        }
    }

    // Data structure to store position, timestamp, and other relevant data
    private struct PositionData
    {
        public float timestamp;
        public Vector3 headsetPosition;
        public Vector3 leftControllerPosition;
        public Vector3 rightControllerPosition;

        public PositionData(float timestamp, Vector3 headsetPosition, Vector3 leftControllerPosition, Vector3 rightControllerPosition)
        {
            this.timestamp = timestamp;
            this.headsetPosition = headsetPosition;
            this.leftControllerPosition = leftControllerPosition;
            this.rightControllerPosition = rightControllerPosition;
        }
    }

    // OnDestroy is called when the GameObject is being destroyed
    void OnDisable()
    {
        // Call SaveDataToCSV when the GameObject is destroyed
        SaveDataToCSV();
    }
}
