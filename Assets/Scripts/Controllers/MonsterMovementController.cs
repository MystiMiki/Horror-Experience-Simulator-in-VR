using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMovementController : MonoBehaviour
{    
    private NavMeshAgent _navMeshAgent;
    private GameObject firstChildMonster; // Reference to the first child monster object

    private Vector3 _roomSize = new Vector3(50f, 0f, 50f);   // Initial room size assumed to be larger than the actual room size. 
    private bool isDisappeared = false;
    private float disappearTimer = 0f;

    private const float disappearTime = 4f;
    private const float appearTime = 1f;
    

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

        if (_navMeshAgent != null && _navMeshAgent.isOnNavMesh)
        {
            // Enable the NavMeshAgent and set the initial destination
            _navMeshAgent.enabled = true;
            SetRandomDestination();
        }


        if (gameObject.name == "Scavenger")
        {
            // Find the first child of the Scavenger
            Transform[] children = GetComponentsInChildren<Transform>();
            if (children.Length > 1) // Ensure there is at least one child
            {
                firstChildMonster = children[1].gameObject;
            }
            else
            {
                Debug.LogError("Scavenger has no child monsters!");
            }
        }
    }

    void Update()
    {
        if (_navMeshAgent != null && _navMeshAgent.isOnNavMesh)
        {
            if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance < 0.1f)
            {
                // For monsters, simply move to a random destination
                SetRandomDestination();
            }

            if (gameObject.name == "Scavenger")
            {
                float randomValue = Random.value;
                if (randomValue < 0.5f) DisappearReappear();
            }           
        }
    }

    void SetRandomDestination()
    {
        // Calculate a random position within the room
        float randomX = Random.Range(-_roomSize.x / 2f, _roomSize.x / 2f);
        float randomZ = Random.Range(-_roomSize.z / 2f, _roomSize.z / 2f);
        Vector3 randomPosition = new Vector3(randomX, 0f, randomZ);

        // Set the random position as the destination
        _navMeshAgent.SetDestination(randomPosition);
    }
    void DisappearReappear()
    {
        disappearTimer += Time.deltaTime;

        if(!isDisappeared && disappearTimer >= disappearTime)
        {
            isDisappeared = true;
            disappearTimer = 0f;
            firstChildMonster.SetActive(false);
        }
        else if(isDisappeared && disappearTimer >= appearTime)
        {
            isDisappeared = false;
            disappearTimer = 0f;
            firstChildMonster.SetActive(true);
        }
    }

}
