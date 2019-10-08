using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesheetAnimator : MonoBehaviour
{
    public float secondsPerFrame = 1;
    private float timer;
    private int frame = 0; // Current frame

    public enum State { Idle, Happy, Eat};
    public State state = State.Idle;
    // Sprites

    [SerializeField]
    Sprite[] idle = new Sprite[2];

    [SerializeField]
    Sprite[] happy = new Sprite[2];

    [SerializeField]
    Sprite[] eat = new Sprite[2];

    SpriteRenderer rend;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = idle[frame];
    }

    void Update()
    {
        if(timer >= secondsPerFrame)
        {
            frame++;
            if (frame == idle.Length)
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
                rend.sprite = idle[frame];
                break;
            case State.Happy:
                rend.sprite = happy[frame];
                break;
            case State.Eat:
                rend.sprite = eat[frame];
                break;
        }
    }
}
