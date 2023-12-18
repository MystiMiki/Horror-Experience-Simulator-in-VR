using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectingController : MonoBehaviour
{
    public void CollectKey(GameObject key)
    {       
        Transform parentTransform = transform.parent;
        if (parentTransform != null)
        {
            // Find an object by tag
            foreach (Transform child in parentTransform)
            {
                if (child.gameObject.CompareTag("Door"))
                {
                    //TODO animation enabled
                }
            }            
        }
        else
        {
            Debug.LogWarning("No parent found for the current GameObject.");
        }

        Destroy(key);
    }

    public void UseBattery(GameObject battery)
    {
        FlashlightController.batteryLife += 100.0f;
        Destroy(battery);
    }
}
