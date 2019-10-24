using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Doozy.Engine.UI;
using TMPro;

public class DefenseTraining : MonoBehaviour
{   
    public Timer mainTimer;
    public Transform player;
    public Animator playerAnimator;
    public GameObject defenseVisual;
    
    public UIImage defendeMeterDisplay;
    public TextMeshProUGUI blockedDisplay;

    public int blocks;
    public int[] scoreLevels = {10, 15, 20, 25, 35};

    public bool isRunning;
    public bool isDefendingLeft;
    public bool isDefendingRight;
    public float defendMeter = 1f;
    public float depleteRate = .01f;
    public float recoveryRate = .005f;

    public Transform asteroidParent;
    public GameObject asteroidPrefab;
    public float asteroidSecondsToImpactStart = 3f;
    public float asteroidSecondsToImpactEnd = 1f;
    public float secondsToImpactVariation = .2f;
    public float asteroidWaitStart = 3f;
    public float asteroidWaitEnd = .5f;
    public float waitVariation = .2f;
    private float nextAsteroid = 1f;
    private float asteroidTimer;

    void Awake()
    {
        Timer.onTimerStart += StartTraining;
        Timer.onTimerEnd += EndTraining;

        InputListener.onButtonA += DefendLeftStart;
        InputListener.onButtonB += DefendRightStart;
        InputListener.onButtonAUp += DefendLeftEnd;
        InputListener.onButtonBUp += DefendRightEnd;
    }

    void Update()
    {
        asteroidTimer += Time.deltaTime;
        if(asteroidTimer >= nextAsteroid)
            GenerateAsteroid();
    }

    void GenerateAsteroid()
    {
        if(isRunning == false)
            return;

        // Get precentage to minigame completion
        float timerPrecentage = mainTimer.GetPrecentage();
        
        // Randomize Direction
        int randDir = Random.Range(0,2);
        string direction = (randDir == 0)? "left" : "right";

        // Randomize speed
        float secondsToImpact = Mathf.Lerp(asteroidSecondsToImpactStart, asteroidSecondsToImpactEnd, timerPrecentage);
        secondsToImpact += (Random.Range(-1f, 1f) * secondsToImpactVariation) * secondsToImpact;

        // Instantiate
        GameObject asteroid = Instantiate(asteroidPrefab, asteroidParent);
        asteroid.GetComponent<Asteroid>().Initialize(secondsToImpact, direction, this);

        // Decide next asteroid
        float wait = Mathf.Lerp(asteroidWaitStart, asteroidWaitEnd, timerPrecentage);
        wait += (Random.Range(-1f,1f) * waitVariation) * wait;
        nextAsteroid = asteroidTimer + wait;
    }

    void FixedUpdate()
    {
        DefendMeterUpdate();
    }

    void DefendMeterUpdate()
    {
        if(isDefendingLeft || isDefendingRight)
        {    
            defendMeter -= depleteRate;
            if(defendMeter <= 0)
            {
                DefendLeftEnd();
                DefendRightEnd();
            }
        }
        else
            defendMeter += recoveryRate;

        defendMeter = Mathf.Clamp(defendMeter, 0f, 1f);

        defendeMeterDisplay.fillAmount = defendMeter;
    }

    void DefendLeftStart()
    {
        if(isRunning == false)
            return;
        
        player.localScale = new Vector3(1,1,1);

        isDefendingLeft = true;
        UpdateVisualDefense();
    }   

    void DefendRightStart()
    {
        if(isRunning == false)
            return;

        player.localScale = new Vector3(-1,1,1);

        isDefendingRight = true;
        UpdateVisualDefense();
    } 

    void DefendRightEnd()
    {
        isDefendingRight = false;
        UpdateVisualDefense();
    }

    void DefendLeftEnd()
    {
        isDefendingLeft = false;
        UpdateVisualDefense();
    }

    void UpdateVisualDefense()
    {
        defenseVisual.SetActive(isDefendingRight || isDefendingLeft);
    }
    
    public void Collision(string direction)
    {
        switch(direction)
        {
            case "left":
                if(isDefendingLeft && defendMeter > 0)
                {    
                    Success();
                    return;
                }
                break;
            case "right":
                if(isDefendingRight && defendMeter > 0)
                {    
                    Success();
                    return;
                }
                break;
        }
        Fail();
    }

    void Success()
    {
        blocks++;
        blockedDisplay.text = string.Format("BLOCKS: {0}", blocks);
    }

    void Fail()
    {
        if(playerAnimator)
            playerAnimator.Play("Hit");
    }

    void StartTraining()
    {
        isRunning = true;
        isDefendingLeft = isDefendingRight = false;
    }

    void EndTraining()
    {
        isRunning = false;

        int stat = GetStatGain(blocks);
        GameManager.instance.Train(0,"def", stat);

        StartCoroutine(ExitTraining(3));
    }

    int GetStatGain(int score)
    {
        int gain = 0;
        for(int i = 0; i < scoreLevels.Length; i++)
        {
            if (score >= scoreLevels[i])
                gain = i + 1;
            else
                break;
        }
        return gain;
    }

    IEnumerator ExitTraining(float secondsToExit)
    {
        yield return new WaitForSeconds(secondsToExit);
        SceneManager.LoadSceneAsync("Main");
    }
}
