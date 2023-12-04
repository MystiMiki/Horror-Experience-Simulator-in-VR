using UnityEngine;
using System.Collections;

public class MonsterMovement : MonoBehaviour
{
    public enum MovementType
    {
        FastChaotic,    // Fast and chaotic movement
        TwitchyStalker, // Twitchy movement, can teleport and disappear
    }

    [Tooltip("Type of the movement.")]
    public MovementType movementType;
    [Tooltip("Speed of the monster. Default values: 5/2")]
    public float movementSpeed;
    [Tooltip("Rotation speed of the monster. Default values: 50/10")]
    public float rotationSpeed;
    [Tooltip("Distance to teleport for TwitchyStalker monster. Default values: 5")]
    public float teleportDistance; 
    [Tooltip("Time duration for TwitchyStalker monster to disappear. Default values: 5")]
    public float disappearTime;    
    [Tooltip("Time duration for TwitchyStalker monster to reappear. Default values: 1")]
    public float appearTime;       

    private Vector3 targetDirection;
    private float smoothRotationFactor;

    private float directionChangeTimer;
    private float directionChangeInterval;

    // Variables specific to TwitchyStalker monster
    private bool isJumping;
    private float jumpCooldown; // Cooldown before TwitchyStalker monster can teleport again
    private float disappearTimer; // Timer for TwitchyStalker monster to disappear
    private bool isTwitchyStalkerVisible; // Flag to track if TwitchyStalker is currently visible

    private GameObject firstChildMonster; // Reference to the first child monster object

    void Start()
    {
        SetInitialValues();
    }

    void Update()
    {
        MoveMonster();
    }

    void SetInitialValues()
    {
        switch (movementType)
        {
            case MovementType.FastChaotic:
                // Fast and chaotic movement settings
                movementSpeed = 5f;
                rotationSpeed = 50f;
                directionChangeInterval = 0.5f;
                smoothRotationFactor = 5f;
                break;

            case MovementType.TwitchyStalker:
                // TwitchyStalker settings
                movementSpeed = 2f;
                rotationSpeed = 10f;
                directionChangeInterval = 3f;
                smoothRotationFactor = 5f;
                teleportDistance = 5f;
                disappearTime = 5f;
                appearTime = 1f; 
                disappearTimer = 0f;
                isTwitchyStalkerVisible = true;

                // Find the first child of the TwitchyStalker
                Transform[] children = GetComponentsInChildren<Transform>();
                if (children.Length > 1) // Ensure there is at least one child
                {
                    firstChildMonster = children[1].gameObject;
                }
                else
                {
                    Debug.LogError("TwitchyStalker has no child monsters!");
                }
                break;
        }

        // Set the initial target direction
        SetRandomTargetDirection();
    }

    void MoveMonster()
    {
        // Change direction at intervals
        directionChangeTimer += Time.deltaTime;
        if (directionChangeTimer >= directionChangeInterval)
        {
            SetRandomTargetDirection();
            directionChangeTimer = 0f; // Reset the timer
        }

        // Move in the current direction
        transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);

        // Smoothly rotate towards the target direction
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothRotationFactor);

        // Handle TwitchyStalker-specific actions
        if (movementType == MovementType.TwitchyStalker)
        {
            RandomAction();
        }
    }

    void SetRandomTargetDirection()
    {
        // Set the initial target direction using Quaternion to rotate Vector3.forward(Unity's shorthand for the positive Z-axis)
        targetDirection = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) * Vector3.forward;
    }

    void RandomAction()
    {
        // Randomly decide which action to perform
        float randomValue = Random.value;

        if (randomValue < 0.5f)
        {
            JumpAction();
        }
        else
        {
            DisappearAction();
        }
    }

    void JumpAction()
    {
        // Implement jumping (teleport) logic for TwitchyStalker monster
        if (!isJumping && jumpCooldown <= 0f)
        {            
            // Calculate a random teleport position within the specified distance
            Vector3 teleportPosition = transform.position + Random.onUnitSphere * teleportDistance;
            teleportPosition.y = transform.position.y; // Keep the same height

            // Check if the destination is clear of obstacles
            if (!IsObstacleInPath(transform.position, teleportPosition))
            {
                // Teleport the monster to the calculated position
                transform.position = teleportPosition;

                // Set jump cooldown to prevent continuous teleportation
                jumpCooldown = 5f; // You can adjust the cooldown time
                isJumping = true;
            }
        }

        // Decrease jump cooldown
        jumpCooldown -= Time.deltaTime;

        // Reset isJumping after cooldown
        if (isJumping && jumpCooldown <= 0f)
        {
            isJumping = false;
        }
    }

    bool IsObstacleInPath(Vector3 start, Vector3 end)
    {
        // Check if there is an obstacle between the start and end positions
        RaycastHit hit;
        if (Physics.Raycast(start, end - start, out hit, Vector3.Distance(start, end)))
        {
            // Adjust this condition based on your specific needs
            if (hit.collider.gameObject.tag == "Obstacle")
            {
                return true; // There is an obstacle in the path
            }
        }

        return false; // Path is clear
    }


    void DisappearAction()
    {
        // Implement disappearing and reappearing logic for TwitchyStalker monster
        disappearTimer += Time.deltaTime;

        if (isTwitchyStalkerVisible && disappearTimer >= disappearTime)
        {
            // TwitchyStalker should disappear
            isTwitchyStalkerVisible = false;
            disappearTimer = 0f;

            // Implement the logic to make the TwitchyStalker disappear
            if (firstChildMonster != null)
            {
                firstChildMonster.SetActive(false); // Placeholder, you may need a different mechanism
            }
            else
            {
                Debug.LogError("TwitchyStalker has no child monsters!");
            }
        }
        else if (!isTwitchyStalkerVisible && disappearTimer >= appearTime)
        {
            // TwitchyStalker should reappear
            isTwitchyStalkerVisible = true;
            disappearTimer = 0f;

            // Implement the logic to make the TwitchyStalker reappear
            if (firstChildMonster != null)
            {
                firstChildMonster.SetActive(true); // Placeholder, you may need a different mechanism
            }
            else
            {
                Debug.LogError("TwitchyStalker has no child monsters!");
            }
        }
    }
}
