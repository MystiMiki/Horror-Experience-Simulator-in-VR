using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {

        if (SimulatorSettings.numberOfRooms != 0 || SimulatorSettings.events != null) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
            Debug.Log("Not all requested values where filled. ");
    }

    public void Exit()
    {
        Application.Quit();
        Debug.Log("Player has quit the game.");
    }

}
