using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroscopeController : MonoBehaviour
{
    float x, y;

    void Start()
    {
        Input.gyro.enabled = true;
    }

    void Update()
    {
        x = -Input.gyro.rotationRateUnbiased.x / 2;
        y = -Input.gyro.rotationRateUnbiased.y / 2;

        x = Mathf.Clamp(x, -15f, 15f);
        y = Mathf.Clamp(y, -20f, 20f);

        transform.Rotate(x, y, 0);
    }
}
