using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Doozy.Engine.UI;
using TMPro;

public class DefenseTraining : MonoBehaviour
{   
    [Header("UI")]
    [SerializeField] private Timer mainTimer;
    [SerializeField] private UIImage defendeMeterDisplay;
    [SerializeField] private TextMeshProUGUI blockedDisplay;
    [SerializeField] private bool landscape = true;

    [Header("Objects Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private SpritesheetAnimator spritesheet;
    [SerializeField] private GameObject defenseVisual;

    [Header("Score Settings")]
    [SerializeField] private int blocks;
    [SerializeField] private int[] scoreLevels = {10, 15, 20, 25, 35};

    [SerializeField] private bool isRunning;
    [SerializeField] private bool isDefendingLeft;
    [SerializeField] private bool isDefendingRight;
    [SerializeField] private float defendMeter = 1f;
    [SerializeField] private float depleteRate = .01f;
    [SerializeField] private float recoveryRate = .005f;

    [Header("Asteroid Settings")]
    [SerializeField] private Transform asteroidParent;
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private float asteroidSecondsToImpactStart = 3f;
    [SerializeField] private float asteroidSecondsToImpactEnd = 1f;
    [SerializeField] private float secondsToImpactVariation = .2f;
    [SerializeField] private float asteroidWaitStart = 3f;
    [SerializeField] private float asteroidWaitEnd = .5f;
    [SerializeField] private float waitVariation = .2f;
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
    
    void Start()
    {
        defendeMeterDisplay = UIManager.instance.defMeter;
        blockedDisplay = UIManager.instance.defBlocks;

        spritesheet.spritesheet = GameManager.instance.FindSheet(GameManager.instance.SelectedPet.species);

        UpdateBlocksDisplay();
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
        
        string direction = "down";

        // Randomize Direction if in landscape mode
        if(landscape)
        {
            int randDir = Random.Range(0,2);
            direction = (randDir == 0)? "left" : "right";
        }

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
        //defenseVisual.SetActive(isDefendingRight || isDefendingLeft);
        if(isDefendingLeft || isDefendingRight)
            spritesheet.PlayAnimation("Defend", true);
        else
            spritesheet.PlayAnimation("Idle", true);
        
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
            case "down":
                if(isDefendingRight || isDefendingLeft && defendMeter > 0)
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
        UpdateBlocksDisplay();
    }

    void UpdateBlocksDisplay()
    {
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
        GameManager.instance.Train(GameManager.instance.SelectedPetIndex, "def", stat);

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

    void OnDestroy()
    {
        Timer.onTimerStart -= StartTraining;
        Timer.onTimerEnd -= EndTraining;

        InputListener.onButtonA -= DefendLeftStart;
        InputListener.onButtonB -= DefendRightStart;
        InputListener.onButtonAUp -= DefendLeftEnd;
        InputListener.onButtonBUp -= DefendRightEnd;
    }
}
