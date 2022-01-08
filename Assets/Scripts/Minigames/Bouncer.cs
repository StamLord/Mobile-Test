using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    [SerializeField] private SpeedTraining context;

    [SerializeField] private GameObject visual;
    [SerializeField] private Rigidbody2D rigid2D;

    [SerializeField] private float playerBounceForce = 10f;
    [SerializeField] private float wallBounceForce = 5f;

    private bool isRunning;
    private Vector2 pausedVelocity;

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            rigid2D.AddForce(Vector2.up * playerBounceForce, ForceMode2D.Impulse);
            rigid2D.AddForce(Vector2.right * Random.Range(-2.5f, 2.5f), ForceMode2D.Impulse);
            context.Bounce();
        }
        else
        {   
            switch(col.gameObject.name)
            {
                case "lowerEdge":
                    DestroyBall();
                    break;
                case "leftEdge":
                    rigid2D.AddForce(Vector2.right * wallBounceForce, ForceMode2D.Impulse);
                    break;
                case "rightEdge":
                    rigid2D.AddForce(Vector2.left * wallBounceForce, ForceMode2D.Impulse);
                    break;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Hoop"))
        {
            context.HoopScore();
        }
    }
    
    public void SetContext(SpeedTraining context)
    {
        this.context = context;
    }

    void DestroyBall()
    {
        context.FallBall(this);
    }

    public void StartBall()
    {
        rigid2D.velocity = Vector2.zero;
        rigid2D.simulated = true;
    }
    

    public void StopBall()
    {
        pausedVelocity = rigid2D.velocity;
        rigid2D.simulated = false;
    }
}
