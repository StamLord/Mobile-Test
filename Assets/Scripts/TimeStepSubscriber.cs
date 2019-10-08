using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeStepSubscriber : MonoBehaviour
{
    private void Awake()
    {
        TimeStep.onTimeStep += Action;
    }

    void Action()
    {
        Debug.Log("Step");
    }
}
