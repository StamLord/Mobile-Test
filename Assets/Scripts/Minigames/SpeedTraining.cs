using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Doozy.Engine.UI;
using TMPro;

public class SpeedTraining : MonoBehaviour
{
    public bool isRunning;
    public GameObject player;
    public GameObject hoop;
    public float speed = .1f;

    public GameObject ballPrefab;

    public int hoopScore = 1000;
    public float hoopInterval = 10f;
    private float nextHoop;
    private bool hoopGenerated;

    public Transform poolParent;
    public List<Bouncer> pooledBalls;
    public Transform spawnedParent;
    public List<Bouncer> spawnedBalls;

    public int bounces;
    public int score;
    public TextMeshProUGUI bounceDisplay;
    public TextMeshProUGUI scoreDisplay;
    public UIImage[] indicator;

    private Camera cam;
    private Vector2 screenHorizontalEdges;

    public Timer timer;
    public float[] ballTimes = {0f, .5f, .75f};
    private int currentBall;

    public int[] scoreLevels = {7500, 15000, 20000, 25000, 32000};
    
    private bool holdingLeft;
    private bool holdingRight;

    void Awake()
    {
        Timer.onTimerStart += StartTraining;
        Timer.onTimerEnd += EndTraining;

        InputListener.onButtonA += LeftStart;
        InputListener.onButtonAUp += LeftEnd;
        InputListener.onButtonB += RightStart;
        InputListener.onButtonBUp += RightEnd;

        cam = Camera.main;
    }

    void Start()
    {
        GenerateCollidersAcrossScreen();

        bounceDisplay = UIManager.instance.spdBounces;
        scoreDisplay = UIManager.instance.spdScore;
        indicator = UIManager.instance.spdIndicators;
    }

    void Update()
    {
        if(isRunning == false)
            return;
            
        if(holdingLeft)
            MovePlayer(-1);
        else if(holdingRight)
            MovePlayer(1);

        if(currentBall < ballTimes.Length && timer.GetPrecentage() > ballTimes[currentBall])
        {
            SpawnBall();
            currentBall++;
        }

        DisplayIndicators();

        if(Time.time >= nextHoop && hoopGenerated == false)
        {
            GenerateHoop();
        }
    }

    void GenerateHoop()
    {
        hoop.transform.position = new Vector3(
            Random.Range(screenHorizontalEdges.x, screenHorizontalEdges.y),
            hoop.transform.position.y,
            hoop.transform.position.z);

        nextHoop = Time.time + hoopInterval;
        hoopGenerated = true;
    }

    void MovePlayer(int direction)
    {
        player.transform.position += new Vector3(speed * direction, 0f, 0f);
        player.transform.localScale = new Vector3(direction * -1, 1f, 1f);
        ClampPlayerPosition();
    }

    void LeftStart()
    {
        if(isRunning == false)
            return;

        holdingLeft = true;            
    }

    void LeftEnd()
    {
        holdingLeft = false;
    }

    void RightStart()
    {
        if(isRunning == false)
            return;
        holdingRight = true;
    }

    void RightEnd()
    {
        holdingRight = false;
    }

    void ClampPlayerPosition()
    {
        player.transform.position = new Vector3(
            Mathf.Clamp(player.transform.position.x, screenHorizontalEdges.x, screenHorizontalEdges.y),
            player.transform.position.y,
            player.transform.position.z
        );
    }

    void DisplayIndicators()
    {
        for(int i = 0; i < indicator.Length; i++)
        {
            if(i < spawnedBalls.Count)
            {
                Vector3 screenPos = cam.WorldToScreenPoint(spawnedBalls[i].transform.position);
                if(screenPos.y > 500)
                {
                    indicator[i].rectTransform.position = new Vector3(
                    screenPos.x,
                    indicator[i].rectTransform.position.y,
                    indicator[i].rectTransform.position.z
                    );    
                }
                else
                {
                    indicator[i].rectTransform.position = new Vector3(
                    -99f,
                    indicator[i].rectTransform.position.y,
                    indicator[i].rectTransform.position.z
                    );
                }
            }
            else
            {
                indicator[i].rectTransform.position = new Vector3(
                -99f,
                indicator[i].rectTransform.position.y,
                indicator[i].rectTransform.position.z
                );
            }
        }

        // for(int i = 0; i < spawnedBalls.Count; i++)
        // {
        //     Vector3 screenPos = cam.WorldToScreenPoint(spawnedBalls[i].transform.position);
        //     if(screenPos.y > 500)
        //     {
        //         indicator[i].rectTransform.position = new Vector3(
        //         screenPos.x,
        //         indicator[i].rectTransform.position.y,
        //         indicator[i].rectTransform.position.z
        //         );    
        //     }
        //     else
        //     {
        //         indicator[i].rectTransform.position = new Vector3(
        //         -99f,
        //         indicator[i].rectTransform.position.y,
        //         indicator[i].rectTransform.position.z
        //         );
        //     }
        // }
    }

    public void FallBall(Bouncer ball)
    {
        Pool(ball);
        if(spawnedBalls.Count < 1)
        {
            bounces = 0;
            UpdateBounceDisplay();
            SpawnBall();
        }
    }

    void Pool(Bouncer ball)
    {
        int index = spawnedBalls.IndexOf(ball);
        spawnedBalls[index].StopBall();
        spawnedBalls[index].transform.SetParent(poolParent);
        spawnedBalls[index].transform.localPosition = Vector3.zero;
        pooledBalls.Add(spawnedBalls[index]);
        spawnedBalls.RemoveAt(index);
    }

    void SpawnBall()
    {
        if(isRunning == false)
            return;

        pooledBalls[0].transform.SetParent(spawnedParent);
        pooledBalls[0].transform.localPosition = Vector3.zero;
        pooledBalls[0].StartBall();
        spawnedBalls.Add(pooledBalls[0]);
        pooledBalls.RemoveAt(0);
    }

    public void Bounce()
    {
        bounces++;
        score += 100 * Mathf.Clamp(bounces, 0, 10);

        UpdateBounceDisplay();
        UpdateScoreDisplay();
    }

    public void HoopScore()
    {
        score += hoopScore;
        hoop.transform.position = new Vector3(
            999f, 
            hoop.transform.position.y, 
            hoop.transform.position.z);
        hoopGenerated = false;
        UpdateScoreDisplay();
    }

    void StartTraining()
    {
        isRunning = true;
        nextHoop = Time.time + hoopInterval;
        holdingLeft = holdingRight = false;
    }

    void EndTraining()
    {
        isRunning = false;

        int stat = GetStatGain(score);
        GameManager.instance.Train(0, "spd", stat);

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

    void UpdateBounceDisplay()
    {
        bounceDisplay.text = "BOUNCES: " + bounces;
    }

    void UpdateScoreDisplay()
    {
        scoreDisplay.text = "SCORE: " + score;
    }

    void GenerateCollidersAcrossScreen()
    {
        Vector2 lDCorner = cam.ViewportToWorldPoint(new Vector3(0, 0f, 10f));
        Vector2 rUCorner = cam.ViewportToWorldPoint(new Vector3(1f, 1f, 10f));
        Vector2[] colliderpoints;

        // EdgeCollider2D upperEdge = new GameObject("upperEdge").AddComponent<EdgeCollider2D>();
        // colliderpoints = upperEdge.points;
        // colliderpoints[0] = new Vector2(lDCorner.x, rUCorner.y);
        // colliderpoints[1] = new Vector2(rUCorner.x, rUCorner.y);
        // upperEdge.points = colliderpoints;

        EdgeCollider2D lowerEdge = new GameObject("lowerEdge").AddComponent<EdgeCollider2D>();
        colliderpoints = lowerEdge.points;
        colliderpoints[0] = new Vector2(lDCorner.x, lDCorner.y);
        colliderpoints[1] = new Vector2(rUCorner.x, lDCorner.y);
        lowerEdge.points = colliderpoints;

        EdgeCollider2D leftEdge = new GameObject("leftEdge").AddComponent<EdgeCollider2D>();
        colliderpoints = leftEdge.points;
        colliderpoints[0] = new Vector2(lDCorner.x, lDCorner.y);
        colliderpoints[1] = new Vector2(lDCorner.x, rUCorner.y);
        leftEdge.points = colliderpoints;

        EdgeCollider2D rightEdge = new GameObject("rightEdge").AddComponent<EdgeCollider2D>();

        colliderpoints = rightEdge.points;
        colliderpoints[0] = new Vector2(rUCorner.x, rUCorner.y);
        colliderpoints[1] = new Vector2(rUCorner.x, lDCorner.y);
        rightEdge.points = colliderpoints;

        screenHorizontalEdges = new Vector2(lDCorner.x, rUCorner.x);
    }

    void OnDestroy()
    {
        Timer.onTimerStart -= StartTraining;
        Timer.onTimerEnd -= EndTraining;

        InputListener.onButtonA -= LeftStart;
        InputListener.onButtonAUp -= LeftEnd;
        InputListener.onButtonB -= RightStart;
        InputListener.onButtonBUp -= RightEnd;
    }

}
