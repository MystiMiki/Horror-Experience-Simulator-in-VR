using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SimulatorSetup : MonoBehaviour
{
    private List<TextAsset> textAssets = new List<TextAsset>();
    private List<GameObject> documents = new List<GameObject>();
    private List<GameObject> jumpScares = new List<GameObject>();

    void Start()
    {
        if (SimulatorSettings.documents)
        {
            LoadPlayerDocuments();
        }

        if (SimulatorSettings.jumpScares)
        {
            // Random count between 1 and numberOfRooms - 1
            GenerateObjects(jumpScares, "JumpScare", SimulatorSettings.numberOfRooms);
        }
    }

    void Update()
    {
        // Your update logic
    }

    void LoadPlayerDocuments()
    {
        string folderPath = "PlayerDocuments";
        TextAsset[] txtFiles = Resources.LoadAll<TextAsset>(folderPath);

        foreach (TextAsset loadedTextAsset in txtFiles)
        {
            textAssets.Add(loadedTextAsset);
        }
    }


    void GenerateObjects(List<GameObject> objectList, string objectName, int count)
    {
        int numberOfObjects = Random.Range(1, count);

        for (int i = 0; i < numberOfObjects; i++)
        {
            GameObject obj = new GameObject(objectName);
            obj.transform.parent = transform;
            obj.SetActive(false);
            objectList.Add(obj);
            
        }
    }
}
