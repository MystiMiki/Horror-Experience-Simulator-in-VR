using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoorController : MonoBehaviour
{
    [SerializeField] private Animator door = null;

    private bool doorClosed = true;
    private bool isLocked = false;

    public void ToggleLock()
    {
        isLocked = !isLocked;        
    }

    public void TriggerDoor()
    {
        if (!isLocked)
        {
            if (doorClosed)
            {
                door.Play("DoorOpen", 0, 0.0f);
                doorClosed = false;
            }
            else
            {
                door.Play("DoorClose", 0, 0.0f);
                doorClosed = true;
            }
        }
        else
        {
            door.Play("DoorShake", 0, 0.0f);
        } 
    }
}
