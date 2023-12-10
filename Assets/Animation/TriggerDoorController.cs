using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoorController : MonoBehaviour
{
    [SerializeField] private Animator door = null;

    private bool doorClosed = true;

    public void TriggerDoor()
    {
        Debug.Log("TriggerDoor");
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
}
