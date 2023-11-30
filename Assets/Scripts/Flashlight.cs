using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{

    private Light _light;
    private bool _isDead;

    private float _batteryLife = 100.0f; // Initial battery life in seconds
    private float _drainRate = 1.0f; // Battery drain rate in units per second



    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponentInChildren<Light>();
        _isDead = false;
    }

    void Update()
    {
        if (_light != null)
        {
            if (!_isDead)
            {
                _batteryLife -= _drainRate * Time.deltaTime;
                Debug.Log("Battery: " + _batteryLife);
                LightOnAndOf();
            }
            else if (_isDead && _light.enabled)
            {
                LightOnAndOf();
            }
        }
        else
        {
            Debug.LogWarning("Light component not found.");
        }        
    }

    public void LightOnAndOf()
    {
        _light.enabled = !_light.enabled;        
    }
}
