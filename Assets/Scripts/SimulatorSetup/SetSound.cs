using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSound : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(SimulatorSettings.soundIndex == 1)
            gameObject.SetActive(false);
    }
}
