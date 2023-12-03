using UnityEngine;

public class MonsterMoveController : MonoBehaviour
{
    // Battery drain rate in units per second
    private float _drainRate = 1.0f; // Adjust this value as needed

    // Update is called once per frame
    void Update()
    {
        // Move the monster in the negative x-axis direction based on drain rate
        transform.position -= new Vector3(0f, 0f, -_drainRate * Time.deltaTime * 2);
    }
}
