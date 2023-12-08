using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SimulatorSetup : MonoBehaviour
{
    private List<TextAsset> textAssets = new List<TextAsset>();

    void Start()
    {
        if (SimulatorSettings.documents)
        {
            LoadPlayerDocuments();
        }
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
}
