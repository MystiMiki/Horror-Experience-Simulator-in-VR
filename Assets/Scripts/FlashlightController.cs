using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public static float batteryLife = 100.0f; // Initial battery life in seconds = 100.0f; // Initial battery life in seconds

    private Light _light;
    private bool _isDead;
    private float _drainRate = 0f; // Battery drain rate in units per second
    private float _originalIntensity = 0.7f;

    void Awake()
    {
        if (SimulatorSettings.limitedResources)
        {
            _drainRate = 1.0f;
        }

        _light = GetComponentInChildren<Light>();

        if (_light == null)
        {
            Debug.LogWarning("Light component not found during Awake.");
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        _isDead = false;
    }

    void Update()
    {
        if (batteryLife <= 0)
        {
            _isDead = true;
        }

        if (!_isDead && _light.intensity > 0)
        {
            if (_drainRate > 0)
            {
                batteryLife -= _drainRate * Time.deltaTime;
                Debug.Log("Battery: " + batteryLife);
            }
        }
        else if (_isDead && _light.intensity > 0)
        {
            LightOnAndOff();
        }

    }

    public void LightOnAndOff()
    {
        if (_light != null)
        {
            _light.intensity = (_light.intensity > 0) ? 0 : _originalIntensity; // Toggle between 0 and the original intensity
        }
    }
}
