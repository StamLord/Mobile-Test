using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private DefenseTraining context;

    public float secondsToImpact = 3f;
    public AnimationCurve zPosition;
    public AnimationCurve xPosition;

    public float lifeTimer;

    public string direction = "left"; // Left or right (Landscape). Down (Portrait)

    public void Initialize(float secondsToImpact, string direction, DefenseTraining context)
    {
        this.secondsToImpact = secondsToImpact;
        this.direction = direction;
        this.context = context;
    }

    void Update()
    {
        lifeTimer += Time.deltaTime;
        Vector3 newPos;

        // Vertical trajectory
        if(direction == "down")
        {
            newPos = new Vector3(
            transform.position.x,
            xPosition.Evaluate(lifeTimer / secondsToImpact), // Use xPosition curve on Y axis
            zPosition.Evaluate(lifeTimer / secondsToImpact)
            ); 
        }
        else // Horizontal trajectory
        {
            newPos = new Vector3(
            xPosition.Evaluate(lifeTimer / secondsToImpact),
            transform.position.y,
            zPosition.Evaluate(lifeTimer / secondsToImpact)
            ); 

            if( direction == "left")
            newPos.x *= -1;
        }

        transform.position = newPos;

        // Reached player
        if(lifeTimer >= secondsToImpact * 0.98)
        {
            context.Collision(direction);
            Destroy(gameObject);
        }
    }

}
