using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AttackTraining : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreDisplay;
    [SerializeField] private TextMeshProUGUI comboDisplay;

    [SerializeField] private Transform[] spots;
    [SerializeField] private Transform player;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private SpritesheetAnimator spritesheet;
    [SerializeField] private Transform bag;
    [SerializeField] private Animator bagAnimator;
    [SerializeField] private StatPopup popup;
    
    [SerializeField] private int position = 4;
    [SerializeField] private int punchBag = -1;

    [SerializeField] private int hits;
    [SerializeField] private int misses;
    [SerializeField] private int score;
    [SerializeField] private int combo;
    [SerializeField] private int highestCombo;

    [SerializeField] private int[] scoreLevels = {5000, 10000, 15000, 20000};

    private bool isRunning;
    private bool canPunch;

    void Awake()
    {
        Timer.onTimerStart += StartTraining;
        Timer.onTimerEnd += StopTraining;

        InputListener.onButtonA += MoveLeft;
        InputListener.onButtonB += MoveRight;
        InputListener.onButtonC += Punch;
    }

    void Start()
    {
        scoreDisplay = UIManager.instance.atkScore;
        comboDisplay = UIManager.instance.atkCombo;
        
        spritesheet.spritesheet = GameManager.instance.FindSheet(GameManager.instance.SelectedPet.species);

        UpdateScore(0);
        UpdateComboDisplay();
    }

    void MoveLeft()
    {   
        if(isRunning == false || canPunch == false)
            return;
        
        if(position > 0)
        {
            if(position - 1 == punchBag)
                Crash();
            else
            {
                position--;
                UpdatePosition();
            }

            FlipPlayerLeft();
        }

        spritesheet.PlayAnimation("Idle", true);
    }

    void MoveRight()
    {  
        if(isRunning == false || canPunch == false)
            return;

        if(position < spots.Length - 1)
        {
            if(position + 1 == punchBag)
                Crash();
            else
            {
                position++;
                UpdatePosition();
            }

            FlipPlayerRight();
        }

        spritesheet.PlayAnimation("Idle", true);
    }

    void Punch()
    {   
        if(isRunning == false || canPunch == false)
            return;
            
        if(position < spots.Length -1  && punchBag == position + 1)
        {
            // Hit
            FlipPlayerRight();
            Hit();
            
        }
        else if(position > 0 && punchBag == position - 1)
        {
            // Hit
            FlipPlayerLeft();
            Hit();
        }
        else
        {
            // Miss
            Miss();
        }
    }

    void Hit()
    {
        canPunch = false;
        hits++;
        
        IncrementCombo();
        int addToScore = 100;

        if(combo > 16)
            addToScore *= 8;
        else if(combo > 8)
            addToScore *= 4;
        else if(combo > 4)
            addToScore *= 2;

        UpdateScore(addToScore);
        Popup("+" + addToScore);


        // if(playerAnimator) 
        //     playerAnimator.Play("Punch");
        
        spritesheet.PlayAnimation("Punch", false, 1, 1);

        StartCoroutine(HitBagAnimation());
    }

    void Miss()
    {
        canPunch = false;
        misses++;
        ResetCombo();
        StartCoroutine(MissAnimation());
        Popup("MISS");
    }

    void Crash()
    {
        canPunch = false;
        misses++;
        ResetCombo();
        StartCoroutine(CrashAnimation());
        Popup("OOF");
    }

    private void Popup(string str)
    {
        // Move popup to current player location
        popup.transform.position = new Vector2(spots[position].position.x, popup.transform.position.y);
        popup.Popup(str);
    }

    IEnumerator HitBagAnimation()
    {
        if(bagAnimator)
        {   
            bagAnimator.Play("Hit");
            yield return new WaitForSeconds(bagAnimator.GetCurrentAnimatorStateInfo(0).length);
        }

        GenerateBagPosition();
    }

    IEnumerator MissAnimation()
    {
        if(playerAnimator)
        {   
            playerAnimator.Play("Miss");
            yield return new WaitForSeconds(.5f);
        }

        GenerateBagPosition();
    }

    IEnumerator CrashAnimation()
    {
        if(playerAnimator)
        {   
            playerAnimator.Play("Crash");
            yield return new WaitForSeconds(.5f);
        }

        GenerateBagPosition();
    }
    
    void GenerateBagPosition()
    {
        int oldPosition = punchBag;
        punchBag = Random.Range(0, spots.Length);

        while(punchBag == position || punchBag == oldPosition)
            punchBag = Random.Range(0, spots.Length);

        if(punchBag < position)
            bag.localScale = new Vector3(-1,1,1);
        else
            bag.localScale = new Vector3(1,1,1);

        bag.position = new Vector3(
            spots[punchBag].position.x, 
            bag.position.y, 
            bag.position.z);

        canPunch = true;
    }

    void UpdatePosition()
    {
        player.position = new Vector3(
            spots[position].position.x, 
            player.position.y, 
            player.position.z);
    }

    void UpdateScore(int change)
    {
        score += change;
        if(scoreDisplay)
            scoreDisplay.text = "SCORE: " + score;
    }

    void IncrementCombo()
    {
        combo++;
        UpdateComboDisplay();
    }

    void ResetCombo()
    {
        combo = 0;
        UpdateComboDisplay();
    }

    void UpdateComboDisplay()
    {
        if(comboDisplay)
            comboDisplay.text = "COMBO: " + combo;
    }

    void FlipPlayerLeft()
    {
        player.localScale = new Vector3(1, 1, 1);
    }

    void FlipPlayerRight()
    {
        player.localScale = new Vector3(-1, 1, 1);
    }

    void StartTraining()
    {
        isRunning = true;
        canPunch = true;
        GenerateBagPosition();
    }

    void StopTraining()
    {
        isRunning = false;

        int stat = GetStatGain(score);
        GameManager.instance.Train(GameManager.instance.SelectedPetIndex, "atk", stat);

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
        Timer.onTimerEnd -= StopTraining;

        InputListener.onButtonA -= MoveLeft;
        InputListener.onButtonB -= MoveRight;
        InputListener.onButtonC -= Punch;
    }

}
