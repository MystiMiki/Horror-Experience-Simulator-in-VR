using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatorSettings : MonoBehaviour
{
    // The private static variable you want to set
    public static int numberOfRooms = 5;
    public static int soundIndex = 0;
    public static bool limitedResources = true;
    public static bool documents = true;
    public static bool keys = true;
    public static bool jumpScares = true;


    public void CheckAndSaveNumber(string number)
    {
        if (int.TryParse(number, out int value) && value > 0)
        {
            if (value < 10)
            {
                numberOfRooms = value;
                Debug.Log("Number of rooms is valid: " + value + ".");
            }
            Debug.Log("Number is too high.");
        }
        else
        {
            // Handle the case where the input cannot be parsed as an integer
            Debug.LogWarning("Invalid input for number of rooms.");
        }
    }

    public void Sound(int index)
    {
        soundIndex = index;
        Debug.Log("Sound index is " + soundIndex +".");
    }

    public void LimitedResources(bool check)
    {
        limitedResources = check;
        Debug.Log("Limited resources are set to " + check + ".");
    }

    public void Documents(bool check)
    {
        documents = check;
        Debug.Log("Documents are set to " + documents + ".");
    }

    public void Keys(bool check)
    {
        keys = check;
        Debug.Log("Keys are set to " + keys + ".");
    }

    public void JumpScares(bool check)
    {
        jumpScares = check;
        Debug.Log("Jump scares are set to " + jumpScares + ".");
    }

}
