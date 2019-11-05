using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesheetAnimator : MonoBehaviour
{
    public float secondsPerFrame = 1;
    private float timer;
    private int frame = 0; // Current frame
    
    public string fallBackAnimation = "Idle";
    public string currentAnimation = "Idle";
    public bool looping = true;

    // Sprites
    public Spritesheet spritesheet;
    private SpriteRenderer rend;
    
    public GameObject teachIcon;

    public Sprite deadSprite;

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
            if (frame == spritesheet.idle.Length)
            {    
                frame = 0;
                if(looping == false)
                    PlayAnimation(fallBackAnimation, true);
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
            case "Dead":
                rend.sprite = deadSprite;
                break;
        }
    }

    public void PlayAnimation(string animation, bool loop)
    {
        currentAnimation = animation;
        looping = loop;
    }

    public void SetTeachIcon(bool visible)
    {
        teachIcon.SetActive(visible);
    }
}
