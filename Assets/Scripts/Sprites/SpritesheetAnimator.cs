using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesheetAnimator : MonoBehaviour
{
    [SerializeField] private float secondsPerFrame = 1;
    
    [SerializeField] private string fallBackAnimation = "Idle";
    [SerializeField] private string currentAnimation = "Idle";
    [SerializeField] private bool looping = true;
    [SerializeField] private int cycles;

    // Sprites
    public Spritesheet spritesheet;
    [SerializeField] private SpriteRenderer rend;
    
    [SerializeField] private GameObject teachIcon;

    [SerializeField] private Sprite deadSprite;

    private float timer;
    private int frame = 0; // Current frame
    
    private void Awake()
    {   
        rend = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(spritesheet == null)
        {
            rend.sprite = null;
            return;
        }
        
        if(timer >= secondsPerFrame)
        {
            frame++;
            if (frame == spritesheet.idle.Length) // Reached end of cycle
            {    
                frame = 0;
                if(looping == false)
                {
                    if(cycles == 1)
                        PlayAnimation(fallBackAnimation, true);
                    else
                        cycles--;
                }
            }

            SetSprite(frame);

            timer -= secondsPerFrame;
        }

        timer += Time.deltaTime;
    }

    void SetSprite(int frame)
    {
        switch (currentAnimation)
        {
            case "Idle":
                rend.sprite = spritesheet.idle[frame];
                break;
            case "Happy":
                rend.sprite = spritesheet.happy[frame];
                break;
            case "Sad":
                rend.sprite = spritesheet.sad[frame];
                break;
            case "Eating":
                rend.sprite = spritesheet.eating[frame];
                break;
            case "Petting":
                rend.sprite = spritesheet.pet[frame];
                break;
            case "Sleeping":
                rend.sprite = spritesheet.sleeping[frame];
                break;
            case "Punch":
                rend.sprite = spritesheet.punch[frame];
                break;
            case "Defend":
                rend.sprite = spritesheet.defend[frame];
                break;
            case "Hit":
                rend.sprite = spritesheet.hit[frame];
                break;
            case "Dead":
                rend.sprite = deadSprite;
                break;
        }
    }

    public void PlayAnimation(string animation, bool loop)
    {
        PlayAnimation(animation, loop, 0, 1);
    }

    public void PlayAnimation(string animation, bool loop, int startFrame, int times)
    {
        currentAnimation = animation;
        looping = loop;
        frame = startFrame;
        cycles = times;
        timer = 0;
        SetSprite(frame);
    }

    public void SetTeachIcon(bool visible)
    {
        teachIcon.SetActive(visible);
    }
}
