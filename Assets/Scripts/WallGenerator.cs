using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public GameObject wallPrefab; // The prefab for the walls
    public float wallHeight = 10f; // Height of the walls
    public float wallWidth = 0.2f;

    private int correctScaling = 10;

    void Start()
    {
        //GenerateWalls();
    }

    void GenerateWalls()
    {
        if (wallPrefab == null)
        {
            Debug.LogError("Wall prefab not assigned!");
            return;
        }

        // Get the size of the plane
        Vector3 planeSize = correctScaling * transform.localScale;

        // Calculate half extents to position the walls correctly
        float halfWidth = planeSize.x / 2f;
        float halfLength = planeSize.z / 2f;

        // Generate walls as children of the plane
        CreateWall(new Vector3(0f, wallHeight, halfLength), Quaternion.identity, "Top"); 
        CreateWall(new Vector3(0f, wallHeight, -halfLength), Quaternion.identity, "Bottom");
        CreateWall(new Vector3(halfWidth, wallHeight, 0f), Quaternion.Euler(0f, 90f, 0f), "Right");
        CreateWall(new Vector3(-halfWidth, wallHeight, 0f), Quaternion.Euler(0f, 90f, 0f), "Left");
    }

    void CreateWall(Vector3 localPosition, Quaternion rotation, string name)
    {
        GameObject wall = Instantiate(wallPrefab, transform);
        wall.transform.localPosition = localPosition;
        wall.transform.localRotation = rotation;
        wall.name = name;
    }
}
