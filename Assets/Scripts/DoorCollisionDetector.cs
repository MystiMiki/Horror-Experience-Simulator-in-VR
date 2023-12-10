using UnityEngine;

public class DoorCollisionDetector : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Door collided with: " + collision.gameObject.name);
        // You can add additional logic here based on the collided object
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log("Door stopped colliding with: " + collision.gameObject.name);
        // Additional logic for when collision ends
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Door triggered with: " + other.gameObject.name);
        // You can add additional logic here based on the triggered object
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Door stopped triggering with: " + other.gameObject.name);
        // Additional logic for when trigger exit
    }
}
