using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompleteSimulationController : MonoBehaviour
{
    public void CompleteSimulation()
    {
        if (gameObject.name == "LastDoor")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
