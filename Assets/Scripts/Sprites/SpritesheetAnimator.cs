using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesheetAnimator : MonoBehaviour
{
    public float secondsPerFrame = 1;
    private float timer;
    private int frame = 0; // Current frame

    public enum State { Idle, Happy, Eating};
    public State state = State.Idle;
    // Sprites

    [SerializeField]
    public Spritesheet spritesheet;

    SpriteRenderer rend;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = spritesheet.idle[frame];
    }

    void Update()
    {
        if(timer >= secondsPerFrame)
        {
            frame++;
            if (frame == spritesheet.idle.Length)
                frame = 0;

            SetSprite(frame);

            timer = 0;
        }

        timer += Time.deltaTime;
    }

    void SetSprite(int frame)
    {
        switch (state)
        {
            case State.Idle:
                rend.sprite = spritesheet.idle[frame];
                break;
            case State.Happy:
                rend.sprite = spritesheet.happy[frame];
                break;
            case State.Eating:
                rend.sprite = spritesheet.eating[frame];
                break;
        }
    }
}
