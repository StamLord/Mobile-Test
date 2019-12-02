using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamBehaviour : MonoBehaviour
{
    public float speed = .05f;
    public float speedVariation = .2f;
    public float wait = 2f;
    public float waitVariation = 1.5f;

    public float minDistance = 5f;
    public float maxDistance = 15f;

    public float xBoundaryLeft = -5f;
    public float xBoundaryRight = 5f;
    public float zBoundaryLeft = 0f;
    public float zBoundaryRight = 0f;
    private float yPlaneHeight = 0f;

    private bool isWalking;
    private Vector3 destination;
    private float currentSpeed;
    private float startWait;
    private float currentWait;

    void Start()
    {
        yPlaneHeight = transform.position.y;
    }
    
    public void Behaviour()
    {
        if(isWalking == false)
        {
            if(Time.time >= startWait + currentWait)
            {
                destination = GenerateDestination(minDistance, maxDistance);
                currentSpeed = GenerateVariation(speed, speedVariation);
                isWalking = true;
            }
        } 
        else 
            RoamTo(destination, currentSpeed);
    }

    Vector3 GenerateDestination(/*float xRange, float zRange*/ float minDistance, float maxDistance)
    {
        // return new Vector3(
        //     Random.Range(-xRange, xRange),
        //     0,
        //     Random.Range(-zRange,zRange)
        // );

        Vector2 randomDir = Random.insideUnitCircle;
        float distance = Random.Range(minDistance, maxDistance);
        randomDir *= distance;

        randomDir.x = Mathf.Clamp(randomDir.x, xBoundaryLeft, xBoundaryRight);
        randomDir.y = Mathf.Clamp(randomDir.y, zBoundaryLeft, zBoundaryRight);

        return new Vector3(randomDir.x, yPlaneHeight, randomDir.y);

    }

    float GenerateVariation(float baseNumber, float variation)
    {
        return baseNumber * Random.Range(-variation, variation) + baseNumber;
    }

    void RoamTo(Vector3 position, float speed)
    {
        if(position.x > transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);

        Vector3 direction = (position - transform.position).normalized;
        transform.position += direction * speed;

        if(Vector3.Distance(transform.position, position) < 0.1f)
        {
            StopWalking();
        }
    }

    public void StopWalking()
    {
        startWait = Time.time;
        currentWait = GenerateVariation(wait, waitVariation);
        isWalking = false;
    }


}
