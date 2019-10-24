using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public int duration;
    [SerializeField]
    private float timer;

    public TextMeshProUGUI timerDisplay;

    private bool isRunning;

    public delegate void  timerStartDelegate();
    public static event timerStartDelegate onTimerStart;

    public delegate void  timerEndDelegate();
    public static event timerEndDelegate onTimerEnd;
    
    void Start()
    {   
        if(UIManager.instance)
            timerDisplay = UIManager.instance.atkTimer;
        StartTimer();
    }

    public void StartTimer()
    {
        timer = 0;
        isRunning = true;
        
        if(onTimerStart != null)
            onTimerStart();
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    void Update()
    {
        UpdateTimer();
    }

    void UpdateTimer()
    {   
        if(isRunning == false)
            return;
            
        timer += Time.deltaTime;

        if(timer >= duration)
        {
            timer = duration;
            isRunning = false;
            if(onTimerEnd != null)
                onTimerEnd();
        }

        if(timerDisplay)
            timerDisplay.text = Mathf.FloorToInt(timer).ToString();
    }

    public float GetPrecentage()
    {
        return timer / duration;
    }


}
