using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 mid;
    public Vector3 up;

    public float moveSpeed;

    public enum CameraPlace {mid, up};
    public CameraPlace state;

    void Update()
    {
        switch(state)
        {
            case CameraPlace.mid:
                if(transform.position != mid)
                    transform.position = Vector3.Lerp(transform.position, mid, moveSpeed/Time.deltaTime);
                break;
            case CameraPlace.up:
                if(transform.position != up)
                    transform.position = Vector3.Lerp(transform.position, up, moveSpeed/Time.deltaTime);
                break;
        }
    }

    public void SetViewUp()
    {
        state = CameraPlace.up;
    }

    public void SetViewMid()
    {
        state = CameraPlace.mid;
    }
}
