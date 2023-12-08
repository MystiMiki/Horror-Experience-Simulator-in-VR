using UnityEngine;
using UnityEngine.AI;

public class MonsterMovementController : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

        if(_navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent is not attached to " + gameObject.name);
        }
        else
        {
            // Check if navMeshAgent is not null before trying to access its properties/methods
            _navMeshAgent.enabled = true;
            SetRandomDestination();
        } 
    }

    void Update()
    {
        // Check if the monster has reached the current destination
        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance < 0.1f)
        {
            SetRandomDestination();
        }
    }

    void SetRandomDestination()
    {
        // Calculate a random position within the room
        float randomX = Random.Range(-roomSize.x / 2f, roomSize.x / 2f);
        float randomZ = Random.Range(-roomSize.z / 2f, roomSize.z / 2f);
        Vector3 randomPosition = new Vector3(randomX, 0f, randomZ);

        // Set the random position as the destination
        // Check if navMeshAgent is not null before calling SetDestination
        if (_navMeshAgent.isOnNavMesh)
        {
            _navMeshAgent.SetDestination(randomPosition);
        }
        else
        {
            Debug.LogError("NavMeshAgent is null or not on the NavMesh.");
        }
    }

    // Replace roomSize with the actual size of your room
    private Vector3 roomSize = new Vector3(50f, 0f, 50f);
}
