using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    public GameObject visual;
    public Rigidbody2D rigid2D;
    private bool isRunning;

    private Vector2 pausedVelocity;
    public SpeedTraining context;

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            rigid2D.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
            rigid2D.AddForce(Vector2.right * Random.Range(-2.5f, 2.5f), ForceMode2D.Impulse);
            context.Bounce();
        }
        else
        {   
            if(col.gameObject.name == "lowerEdge")
                DestroyBall();
            else
            {
                Vector3 direction = (col.transform.position - transform.position).normalized;
                rigid2D.AddForce(direction * 5f, ForceMode2D.Impulse);
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
