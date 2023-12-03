using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectingController : MonoBehaviour
{
    public void CollectKey(GameObject key)
    {
        //TODO
        Destroy(key);
    }

    public void UseBattery(GameObject battery)
    {
        FlashlightController.batteryLife += 100.0f;
        Destroy(battery);
    }
}
