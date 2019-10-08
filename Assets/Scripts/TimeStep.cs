using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStep : MonoBehaviour
{
    public delegate void timeStepDelegate();
    public static event timeStepDelegate onTimeStep;

    [Tooltip("Time between each step in seconds")]
    public float interval = 1;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= interval)
        {
            if (onTimeStep != null)
            {
                onTimeStep();
            }

            timer = 0;
        }
    }
}
