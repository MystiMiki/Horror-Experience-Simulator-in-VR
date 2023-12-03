using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class roomGenerator : MonoBehaviour
{      
    [SerializeField] GameObject nodePrefab;
    [SerializeField] List<GameObject> roomPrefabs;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject doorPrefab;
    [SerializeField] GameObject ceilingPrefab;

    private int numberOfRooms;
    private int _correctScaling = 10; // Corresponds to the size of the plane
    private int _gridDimension;
    private string _prevDoor;
    private bool _flashlight = false;
    private GameObject[,] _grid;
    private List<Vector2Int> _path;
    private List<Vector3> _wallsCoordination = new List<Vector3>();
    private List<GameObject> nonmoveableObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    { 
        numberOfRooms = SimulatorSettings.numberOfRooms;
        Initialize_grid();
        Generate_path();
        Activate_path();
    }

    void Initialize_grid()
    {
        // Initialize the grid with nodes 
        _gridDimension = Mathf.CeilToInt(Mathf.Sqrt(numberOfRooms));
        if (_gridDimension * _gridDimension == numberOfRooms) _gridDimension += 1;
        _grid = new GameObject[_gridDimension, _gridDimension];

        for (int i = 0; i < _gridDimension; i++)
        {
            for (int j = 0; j < _gridDimension; j++)
            {
                // Add scaling to the position of the node
                float xPos = i * _correctScaling;
                float yPos = j * _correctScaling;

                // Create a non-active plane with roomPrefab, assign the plane to the grid
                GameObject node = Instantiate(nodePrefab, new Vector3(xPos, 0, yPos), Quaternion.identity);
                node.SetActive(false);
                _grid[i, j] = node;
            }
        }
    }

    void Generate_path()
    {
        // Use the backtracking algorithm to find a path
        _path = Find_path(0, 0, new List<Vector2Int> { new Vector2Int(0, 0) });
    }

    List<Vector2Int> Find_path(int x, int y, List<Vector2Int> path)
    {
        // If all nodes are visited, return the path
        if (path.Count == _gridDimension * _gridDimension)
        {
            return path;
        }


        // Possible moves: up, down, left, right
        Vector2Int[] moves = { 
            new Vector2Int(0, 1), 
            new Vector2Int(0, -1), 
            new Vector2Int(1, 0), 
            new Vector2Int(-1, 0) 
        };

        // Shuffle the moves randomly, so it is possible to create a random path 
        Shuffle(moves);

        foreach (var move in moves)
        {
            int newX = x + move.x;
            int newY = y + move.y;

            if (IsValidMove(newX, newY, path))
            {
                // Make the move
                path.Add(new Vector2Int(newX, newY));

                // Recursively find the _path from the new position
                List<Vector2Int> result = Find_path(newX, newY, path);

                // If a valid _path is found, return it
                if (result != null)
                {
                    return result;
                }

                // Backtrack if the current path does not lead to a solution
                path.RemoveAt(path.Count - 1);
            }
        }

        // No valid _path found
        return null;
    }

    void Shuffle<T>(T[] array) 
    {
        int n = array.Length;
        for (int i = 0; i < n; i++)
        {
            int r = i + Random.Range(0, n - i);
            T temp = array[i];
            array[i] = array[r];
            array[r] = temp;
        }
    }


    bool IsValidMove(int x, int y, List<Vector2Int> path)
    {
        // Check if the move is within the _grid and if the node has not been visited before
        return x >= 0 && x < _gridDimension && y >= 0 && y < _gridDimension && !path.Contains(new Vector2Int(x, y));
    }

    void Activate_path()
    {
        // Replace the nodes with the rooms and activate them along the _path
        if (_path != null)
        {
            // Shorten the _path according to the number of rooms
            if (_path.Count > numberOfRooms)
            {
                _path.RemoveRange(numberOfRooms + 1, _path.Count - numberOfRooms - 1);
            }

            string _pathString = "Path: ";
            List<GameObject> availableRooms = new List<GameObject>();
            foreach (var node in _path)
            {
                int x = node.x;
                int y = node.y;

                if (_path.IndexOf(node) != _path.Count - 1)
                {
                    // Store the position and scale of the existing node
                    Vector3 currentPosition = _grid[x, y].transform.position;
                    Vector3 currentScale = _grid[x, y].transform.localScale;

                    // Destroy the existing node
                    Destroy(_grid[x, y]);

                    if (availableRooms.Count == 0)
                    {
                        // If the list is empty, refill it with all room prefabs                        
                        availableRooms = roomPrefabs;
                    }

                    // Randomly select a room prefab from the list
                    int randomIndex = Random.Range(0, availableRooms.Count);
                    GameObject selectedRoomPrefab = availableRooms[randomIndex];

                    // Remove the selected room prefab from the list
                    availableRooms.RemoveAt(randomIndex);

                    // Instantiate a new prefab at the same position and with the same scale
                    GameObject room = Instantiate(selectedRoomPrefab, currentPosition, Quaternion.identity);
                    room.transform.localScale = currentScale;
                    room.name = x + "," + y;

                    // Assign the new node to the _grid
                    _grid[x, y] = room;

                    // Append coordinates to the _path string
                    _pathString += $"({x},{y}) ";
                }
            }

            // Generate walls along the _path
            GenerateWalls();

            // Print the final _path to the console
            Debug.Log(_pathString);
        }
        else
        {
            Debug.Log("No valid path found.");
        }
    }

    void GenerateWalls()
    {
        // Generate walls around the plane with remembering the door in the previous room
        if (wallPrefab == null || doorPrefab == null)
        {
            Debug.LogError("Wall prefab not assigned!");
            return;
        }

        // Get the size of the plane
        Vector3 planeSize = _correctScaling * transform.localScale;

        // Calculate half extents to position walls correctly
        float halfWidth = planeSize.x / 2f;
        float halfLength = planeSize.z / 2f;

        // Set which side the door was on in the previous room
        _prevDoor = "";

        for (int i = 0; i < _path.Count - 1; i++)
        {
            int currentX = _path[i].x;
            int currentY = _path[i].y;

            int nextX = _path[i + 1].x;
            int nextY = _path[i + 1].y;

            // Calculate the direction of the _path
            int dx = nextX - currentX;
            int dy = nextY - currentY;

            // Create walls as children of the plane
            if (dx == 1) // Move to the right
            {
                CreateWall(_grid[currentX, currentY], wallPrefab, new Vector3(0f, 0f, halfLength), Quaternion.identity, "Top");
                CreateWall(_grid[currentX, currentY], wallPrefab, new Vector3(0f, 0f, -halfLength), Quaternion.identity, "Bottom");
                CreateWall(_grid[currentX, currentY], wallPrefab, new Vector3(-halfWidth, 0f, 0f), Quaternion.Euler(0f, 90f, 0f), "Left");
                CreateWall(_grid[currentX, currentY], doorPrefab, new Vector3(halfWidth, 0f, 0f), Quaternion.Euler(0f, -90f, 0f), "Right");
                _prevDoor = "Left";
            }
            else if (dx == -1) // Move to the left
            {
                CreateWall(_grid[currentX, currentY], wallPrefab, new Vector3(0f, 0f, halfLength), Quaternion.identity, "Top");
                CreateWall(_grid[currentX, currentY], wallPrefab, new Vector3(0f, 0f, -halfLength), Quaternion.identity, "Bottom");
                CreateWall(_grid[currentX, currentY], doorPrefab, new Vector3(-halfWidth, 0f, 0f), Quaternion.Euler(0f, -90f, 0f), "Left");
                CreateWall(_grid[currentX, currentY], wallPrefab, new Vector3(halfWidth, 0f, 0f), Quaternion.Euler(0f, 90f, 0f), "Right");
                _prevDoor = "Right";
            }
            else if (dy == 1) // Move upward
            {

                CreateWall(_grid[currentX, currentY], doorPrefab, new Vector3(0f, 0f, halfLength), Quaternion.Euler(0f, -180f, 0f), "Top");
                CreateWall(_grid[currentX, currentY], wallPrefab, new Vector3(0f, 0f, -halfLength), Quaternion.identity, "Bottom");
                CreateWall(_grid[currentX, currentY], wallPrefab, new Vector3(halfWidth, 0f, 0f), Quaternion.Euler(0f, 90f, 0f), "Right");
                CreateWall(_grid[currentX, currentY], wallPrefab, new Vector3(-halfWidth, 0f, 0f), Quaternion.Euler(0f, 90f, 0f), "Left");
                _prevDoor = "Bottom";
            }
            else if (dy == -1) // Move downward
            {

                CreateWall(_grid[currentX, currentY], wallPrefab, new Vector3( 0f, 0f, halfLength), Quaternion.identity, "Top");
                CreateWall(_grid[currentX, currentY], doorPrefab, new Vector3( 0f, 0f, -halfLength), Quaternion.Euler(0f, -180f, 0f), "Bottom");
                CreateWall(_grid[currentX, currentY], wallPrefab, new Vector3( halfWidth, 0f, 0f), Quaternion.Euler(0f, 90f, 0f), "Right");
                CreateWall(_grid[currentX, currentY], wallPrefab, new Vector3( -halfWidth, 0f, 0f), Quaternion.Euler(0f, 90f, 0f), "Left");
                _prevDoor = "Top";
            }
            // Create the ceiling
            CreateWall(_grid[currentX, currentY], ceilingPrefab, new Vector3(0f, 4f, 0f), Quaternion.identity, "Ceiling");
            SpawnFlashlightOrBattery();


            PlaceEvent(_grid[currentX, currentY]);
        }

        // Mark the last doors for further processing
        Transform door = _grid[_path[_path.Count - 2].x, _path[_path.Count - 2].y].transform.Find("Door");
        door.name = "LastDoor";
    }

    void CreateWall(GameObject node, GameObject prefab, Vector3 localPosition, Quaternion rotation, string name)
    {     
        // Separate method to create one wall according to the given attributes
        if (name != _prevDoor)
        {
            GameObject wall = Instantiate(prefab, transform);
            wall.transform.SetParent(node.transform);
            wall.transform.localPosition = localPosition;
            wall.transform.localRotation = rotation;
            if (prefab == doorPrefab) 
            {
                wall.name = "Door";
                Transform doorWing = wall.transform.Find("Cube/jj_door_3_white_weathered/doorWing");
            }
            else
                wall.name = name;
        }
    }

    void PlaceEvent(GameObject node)
    {
        // Generate a random boolean value
        bool placeDocument = Random.Range(0, 2) == 0;
        bool placeJumpscare = Random.Range(0, 2) == 0;
        bool placeKey = Random.Range(0, 2) == 0;
        
        //TODO
    }

    void SpawnFlashlightOrBattery()
    {
        // Find all GameObjects with the tag "FlashlightOrBattery" in the entire hierarchy
        GameObject[] flashlightOrBatteryObjects = GameObject.FindGameObjectsWithTag("FlashlightOrBattery");
        Shuffle(flashlightOrBatteryObjects);

        // Loop through each found object
        foreach (GameObject obj in flashlightOrBatteryObjects)
        {
            // Set all to nonactive
            if (!SimulatorSettings.limitedResources)
            {
                GameObject battery = obj.transform.Find("Battery").gameObject;
                battery.SetActive(false);
            }

            // Get the parent's name
            string parentName = obj.transform.parent.name;

            // Check if the parent's name is "0,0" and activate the flashlight
            if (parentName == "0,0" && !_flashlight)
            {
                _flashlight = true;

                // Assuming there's a child named "Flashlight" under the found object
                GameObject flashlight = obj.transform.Find("Flashlight").gameObject;              
                flashlight.SetActive(true);

                GameObject battery = obj.transform.Find("Battery").gameObject;
                battery.SetActive(false);
            }            
        }
    }
}
