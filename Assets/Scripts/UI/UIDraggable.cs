using UnityEngine;
using UnityEngine.EventSystems;

public class UIDraggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum DragDirection {Vertical, Horizontal, Both};
    public DragDirection direction;

    private RectTransform rectTransform;
    
    public bool canDrag = true;
    public bool isDragging;
    public Vector3 startingPoint;
    private Vector3 dragStart;
    public int minimumMove = 10;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if(canDrag == false)
            return;

        isDragging = true;
        startingPoint = rectTransform.position;
        dragStart = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    void Update()
    {
        if(isDragging)
        {
            Vector3 deltaDrag = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0) - dragStart;
            Vector3 moveVector;

            if(minimumMove == 0)
            {
                moveVector = deltaDrag;
            }
            else
            {
                moveVector= new Vector3(
                    Mathf.FloorToInt(deltaDrag.x / minimumMove) * minimumMove,
                    Mathf.FloorToInt(deltaDrag.y / minimumMove) * minimumMove,
                    Mathf.FloorToInt(deltaDrag.z / minimumMove) * minimumMove
                    );
            }

            switch(direction)
            {
                case DragDirection.Horizontal:
                    moveVector.y = 0;
                    break;
                case DragDirection.Vertical:
                    moveVector.x = 0;
                    break;
            }
            
            rectTransform.position = moveVector + startingPoint;
        }
    }
    
}
