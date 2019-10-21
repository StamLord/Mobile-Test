using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTraining : MonoBehaviour
{
    public Transform[] spots;
    public Transform player;
    public Animator playerAnimator;
    public Transform bag;
    
    public int position = 4;
    public int punchBag = -1;

    public int hits;
    public int misses;

    private bool isRunning;

    void Awake()
    {
        Timer.onTimerStart += StartTraining;
        Timer.onTimerEnd += StopTraining;

        InputListener.onButtonA += MoveLeft;
        InputListener.onButtonB += MoveRight;
        InputListener.onButtonC += Punch;
    }

    void MoveLeft()
    {
        if(isRunning == false)
            return;
        
        if(position > 0)
        {
            if(position - 1 == punchBag)
            {
                if(playerAnimator) 
                    playerAnimator.Play("Crash");
                Miss();
            }
            else
            {
                position--;
                UpdatePosition();
            }

            FlipPlayerLeft();
        }
    }

    void MoveRight()
    {
        if(isRunning == false)
            return;

        if(position < spots.Length - 1)
        {
            if(position+1 == punchBag)
            {
                if(playerAnimator) 
                    playerAnimator.Play("Crash");
                Miss();
            }
            else
            {
                position++;
                UpdatePosition();
            }

            FlipPlayerRight();
        }
    }

    void Punch()
    {
        if(isRunning == false)
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
            if(playerAnimator) 
                playerAnimator.Play("Miss");
            Miss();
        }
    }

    void Hit()
    {
        Debug.Log("Hit");
        hits++;
        if(playerAnimator) 
            playerAnimator.Play("Punch");
        GenerateBagPosition();
    }

    void Miss()
    {
        misses++;
        GenerateBagPosition();
    }

    void GenerateBagPosition()
    {
        int oldPosition = punchBag;
        punchBag = Random.Range(0, spots.Length);

        while(punchBag == position || punchBag == oldPosition)
            punchBag = Random.Range(0, spots.Length);

        bag.position = new Vector3(
            spots[punchBag].position.x, 
            bag.position.y, 
            bag.position.z);
    }

    void UpdatePosition()
    {
        player.position = new Vector3(
            spots[position].position.x, 
            player.position.y, 
            player.position.z);
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
        GenerateBagPosition();
    }

    void StopTraining()
    {
        isRunning = false;
    }

}
