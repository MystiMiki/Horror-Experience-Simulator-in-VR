using UnityEngine;
using UnityEngine.XR;

public class MonsterInGazeTracker : MonoBehaviour
{ 
    public static bool isInGaze = false; // Track whether the player has monsters in gaze
    private Camera playerCamera;

    private void Start()
    {
        playerCamera = FindPlayerCamera(transform); // Find the player camera recursively
    }

    void Update()
    {
        // Check if the monster is in the player's gaze
        if (IsMonsterInGaze())
        {
            isInGaze = true;
        }
        else
        {
            isInGaze = false;   
        }
    }

    Camera FindPlayerCamera(Transform parent)
    {
        // Recursively search for the Camera component in the hierarchy
        foreach (Transform child in parent)
        {
            Camera foundCamera = child.GetComponent<Camera>();
            if (foundCamera != null)
            {
                return foundCamera;
            }

            // Continue searching in children
            Camera foundInChildren = FindPlayerCamera(child);
            if (foundInChildren != null)
            {
                return foundInChildren;
            }
        }

        // If no camera is found, return null or handle it as needed
        return null;
    }


    bool IsMonsterInGaze()
    {
        // Set up a ray from the player's head position in the direction they are looking
        if (playerCamera != null)
        {
            Ray gazeRay = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            // Set the maximum distance for the raycast
            float maxGazeDistance = 8f; // Adjust the distance as needed

            // Number of segments to simulate thickness
            int numSegments = 10;

            // Calculate offset to simulate thickness
            Vector3 offset = playerCamera.transform.forward * 0.01f;

            // Perform the raycast for each segment
            for (int i = 0; i < numSegments; i++)
            {
                Vector3 start = gazeRay.origin + offset * i;
                Vector3 end = start + gazeRay.direction * maxGazeDistance;

                // Draw the ray segment
                Debug.DrawRay(start, end - start, Color.red);
            }

            // Perform the raycast for the final hit detection
            if (Physics.Raycast(gazeRay, out hit, maxGazeDistance))
            {
                // Check if the hit object has the "monster" tag
                if (hit.collider.CompareTag("Monster"))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
