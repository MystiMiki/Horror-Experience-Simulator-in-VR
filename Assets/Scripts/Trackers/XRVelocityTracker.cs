using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System.IO;

// Data structure to store velocity, timestamp, and other relevant data
public struct VelocityData
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

public class XRVelocityTracker : SaveToCSV<VelocityData>
{
    private List<VelocityData> _velocityDataList = new List<VelocityData>();
    private DateTime _sceneLoadTime;
    private Vector3 _lastHeadsetPosition;
    private Vector3 _lastLeftControllerPosition;
    private Vector3 _lastRightControllerPosition;
    private float _timeSinceLastSave = 0f;
    private const float _saveInterval = 1f; // Set the save interval to 1 second

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
        float timestamp = (float)(DateTime.Now - _sceneLoadTime).TotalSeconds - 1;

        // Calculate velocity for each axis
        Vector3 headsetVelocity = (headsetPosition - _lastHeadsetPosition) / Time.deltaTime;
        Vector3 leftControllerVelocity = (leftControllerPosition - _lastLeftControllerPosition) / Time.deltaTime;
        Vector3 rightControllerVelocity = (rightControllerPosition - _lastRightControllerPosition) / Time.deltaTime;

        // Accumulate the elapsed time
        _timeSinceLastSave += Time.deltaTime;

        // Check if the save interval has passed
        if (_timeSinceLastSave >= _saveInterval)
        {
            // Save data every second
            _velocityDataList.Add(new VelocityData(timestamp, headsetVelocity, leftControllerVelocity, rightControllerVelocity));
            // Reset the timer
            _timeSinceLastSave = 0f;
        }

        // Update last positions for the next frame
        _lastHeadsetPosition = headsetPosition;
        _lastLeftControllerPosition = leftControllerPosition;
        _lastRightControllerPosition = rightControllerPosition;

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
    protected override void WriteToCSV(string filePath, VelocityData[] velocities)
    {
        // Create a new CSV file or overwrite if it already exists
        using (StreamWriter file = new StreamWriter(filePath))
        {
            // Write header
            file.WriteLine("Timestamp;" +
                "HeadsetVelocityX;HeadsetVelocityY;HeadsetVelocityZ;" +
                "LeftControllerVelocityX;LeftControllerVelocityY;LeftControllerVelocityZ;" +
                "RightControllerVelocityX;RightControllerVelocityY;RightControllerVelocityZ");

            // Write velocity data
            foreach (VelocityData velocityData in velocities)
            {
                file.WriteLine($"{velocityData.timestamp.ToString().Split(',')[0]};" +
                    $"{velocityData.headsetVelocity.x};{velocityData.headsetVelocity.y};{velocityData.headsetVelocity.z};" +
                    $"{velocityData.leftControllerVelocity.x};{velocityData.leftControllerVelocity.y};{velocityData.leftControllerVelocity.z};" +
                    $"{velocityData.rightControllerVelocity.x};{velocityData.rightControllerVelocity.y};{velocityData.rightControllerVelocity.z}");
            }
        }
    }   

    // OnDestroy is called when the GameObject is being destroyed
    void OnDisable()
    {       
        // Call SaveDataToCSV when the GameObject is destroyed
        SaveDataToCSV("Velocity", _velocityDataList, _sceneLoadTime);
    }
}
