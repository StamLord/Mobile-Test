using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.Touchy;

public class UIDraggable : MonoBehaviour
{
    private RectTransform rectTransform;
    private bool isDragging;
    private Vector3 startingPoint;
    private Vector3 dragStart;
    public int minimumMove = 5;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Touch Input
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Moved)
            {
                rectTransform.position = new Vector3(touch.deltaPosition.x, 0, 0) + rectTransform.position;
            }
        } 
        else 
        {
            if(Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                startingPoint = rectTransform.localPosition;
                dragStart = new Vector3(Input.mousePosition.x, 0, 0);
            }
            else if(Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }

            if(isDragging)
            {
                Vector3 deltaDrag = new Vector3(Input.mousePosition.x, 0, 0) - dragStart;
                Vector3 moveVector= new Vector3(
                    Mathf.FloorToInt(deltaDrag.x / minimumMove) * minimumMove,
                    Mathf.FloorToInt(deltaDrag.y / minimumMove) * minimumMove,
                    Mathf.FloorToInt(deltaDrag.z / minimumMove) * minimumMove
                    );
                rectTransform.localPosition = moveVector + startingPoint;
            }
        }
    }

    
}
