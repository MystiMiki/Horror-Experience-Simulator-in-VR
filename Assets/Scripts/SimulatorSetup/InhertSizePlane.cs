using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InhertSizePlane : MonoBehaviour
{
    public GameObject sourcePlane; // Reference to the source plane
    public GameObject targetPlane; // Reference to the target plane

    void Start()
    {
        // Check if both planes are assigned
        if (sourcePlane != null && targetPlane != null)
        {
            // Inherit the size of the source plane
            InheritSize();
        }
        else
        {
            Debug.LogError("Please assign sourcePlane and targetPlane in the inspector.");
        }
    }

    void InheritSize()
    {
        // Get the scale of the source plane
        Vector3 sourceScale = sourcePlane.transform.localScale;

        // Apply the scale to the target plane
        targetPlane.transform.localScale = sourceScale;
    }
}
