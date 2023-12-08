using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System.IO;

public class XRVelocityTracker : MonoBehaviour
{
    private List<VelocityData> velocityDataList = new List<VelocityData>();
    private DateTime sceneLoadTime;
    private Vector3 lastHeadsetPosition;
    private Vector3 lastLeftControllerPosition;
    private Vector3 lastRightControllerPosition;
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

        // Calculate velocity for each axis
        Vector3 headsetVelocity = (headsetPosition - lastHeadsetPosition) / Time.deltaTime;
        Vector3 leftControllerVelocity = (leftControllerPosition - lastLeftControllerPosition) / Time.deltaTime;
        Vector3 rightControllerVelocity = (rightControllerPosition - lastRightControllerPosition) / Time.deltaTime;

        // Accumulate the elapsed time
        timeSinceLastSave += Time.deltaTime;

        // Check if the save interval has passed
        if (timeSinceLastSave >= saveInterval)
        {
            // Save data every second
            velocityDataList.Add(new VelocityData(timestamp, headsetVelocity, leftControllerVelocity, rightControllerVelocity));
            // Reset the timer
            timeSinceLastSave = 0f;
        }

        // Update last positions for the next frame
        lastHeadsetPosition = headsetPosition;
        lastLeftControllerPosition = leftControllerPosition;
        lastRightControllerPosition = rightControllerPosition;

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
        VelocityData[] velocityDataArray = velocityDataList.ToArray();

        // Define the directory path
        string directoryPath = Path.Combine(Application.dataPath, "Data", "Dynamic");

        // Create the directory if it doesn't exist
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Generate a unique filename using the scene load time
        string filename = $"velocityData_{sceneLoadTime:yyyyMMdd_HHmmss}.csv";
        string filePath = Path.Combine(directoryPath, filename);

        // Use a CSV writing function to write the data to a CSV file
        WriteToCSV(filePath, velocityDataArray);

        // Clear the data list after saving
        velocityDataList.Clear();
    }

    // Function to write data to a CSV file
    void WriteToCSV(string filePath, VelocityData[] velocities)
    {
        // Create a new CSV file or overwrite if it already exists
        using (StreamWriter file = new StreamWriter(filePath))
        {
            // Write header
            file.WriteLine("Timestamp,HeadsetVelocityX,HeadsetVelocityY,HeadsetVelocityZ,LeftControllerVelocityX,LeftControllerVelocityY,LeftControllerVelocityZ,RightControllerVelocityX,RightControllerVelocityY,RightControllerVelocityZ");

            // Write velocity data
            foreach (VelocityData velocityData in velocities)
            {
                file.WriteLine($"{velocityData.timestamp.ToString().Split(',')[0]},{velocityData.headsetVelocity.x},{velocityData.headsetVelocity.y},{velocityData.headsetVelocity.z},{velocityData.leftControllerVelocity.x},{velocityData.leftControllerVelocity.y},{velocityData.leftControllerVelocity.z},{velocityData.rightControllerVelocity.x},{velocityData.rightControllerVelocity.y},{velocityData.rightControllerVelocity.z}");
            }
        }
    }

    // Data structure to store velocity, timestamp, and other relevant data
    private struct VelocityData
    {
        public float timestamp;
        public Vector3 headsetVelocity;
        public Vector3 leftControllerVelocity;
        public Vector3 rightControllerVelocity;

        public VelocityData(float timestamp, Vector3 headsetVelocity, Vector3 leftControllerVelocity, Vector3 rightControllerVelocity)
        {
            this.timestamp = timestamp;
            this.headsetVelocity = headsetVelocity;
            this.leftControllerVelocity = leftControllerVelocity;
            this.rightControllerVelocity = rightControllerVelocity;
        }
    }

    // OnDestroy is called when the GameObject is being destroyed
    void OnDestroy()
    {
        // Call SaveDataToCSV when the GameObject is destroyed
        SaveDataToCSV();
    }
}