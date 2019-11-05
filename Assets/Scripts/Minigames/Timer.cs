using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        if(UIManager.instance != null)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            switch(sceneName)
            {
                case "ATK Training":
                    timerDisplay = UIManager.instance.atkTimer;
                    break;
                case "SPD Training":
                    timerDisplay = UIManager.instance.spdTimer;
                    break;
                case "DEF Training":
                    timerDisplay = UIManager.instance.defTimer;
                    break;
            }
        }
        StartTimer();
    }

    public void StartTimer()
    {
        timer = duration;
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
            
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            timer = 0;
            isRunning = false;
            if(onTimerEnd != null)
                onTimerEnd();
        }

        if(timerDisplay)
            timerDisplay.text = Mathf.FloorToInt(timer).ToString();
    }

    public float GetPrecentage()
    {
        return (duration - timer) / duration;
    }


}
