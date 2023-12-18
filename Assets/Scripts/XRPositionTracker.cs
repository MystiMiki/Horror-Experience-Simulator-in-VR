using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System.IO;

// Data structure to store position, timestamp, and other relevant data
public struct PositionData
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

public class XRPositionTracker : SaveToCSV<PositionData>
{
    private List<PositionData> _positionDataList = new List<PositionData>();
    private DateTime _sceneLoadTime;
    private float _timeSinceLastSave = 0f;
    private float _saveInterval = 1f; // Set the save interval to 1 second

    void Start()
    {
        // Record the time when the scene is loaded
        _sceneLoadTime = DateTime.Now;
    }

    void Update()
    {
        Vector3 headsetPosition = GetDevicePosition(XRNode.Head);
        Vector3 leftControllerPosition = GetDevicePosition(XRNode.LeftHand);
        Vector3 rightControllerPosition = GetDevicePosition(XRNode.RightHand);

        // Calculate the timestamp based on the time difference between now and when the scene was loaded
        float timestamp = (float)(DateTime.Now - _sceneLoadTime).TotalSeconds;

        // Accumulate the elapsed time
        _timeSinceLastSave += Time.deltaTime;

        // Check if the save interval has passed
        if (_timeSinceLastSave >= _saveInterval)
        {
            // Save data every second
            _positionDataList.Add(new PositionData(timestamp, headsetPosition, leftControllerPosition, rightControllerPosition));
            // Reset the timer
            _timeSinceLastSave = 0f;
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
    
    // Function to write data to a CSV file
    protected override void WriteToCSV(string filePath, PositionData[] positions)
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

    // OnDestroy is called when the GameObject is being destroyed
    void OnDisable()
    {
        // Call SaveDataToCSV when the GameObject is destroyed
        SaveDataToCSV("Position", _positionDataList, _sceneLoadTime);
    }
}
