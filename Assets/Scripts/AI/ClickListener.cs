using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickListener : MonoBehaviour
{
    public delegate void mouseDownDelegate();
    public event mouseDownDelegate onMouseDown;

    public delegate void mouseUpDelegate();
    public event mouseUpDelegate onMouseUp;

    public delegate void mouseHoverDelegate();
    public event mouseHoverDelegate onMouseHover;

    public delegate void mouseUnHoverDelegate();
    public event mouseUnHoverDelegate onMouseUnHover;

    public delegate void mouseDragDelegate();
    public event mouseDragDelegate onMouseDrag;

    private Camera cam;
    // [SerializeField] private Vector3 screenPosition;
    // [SerializeField] private Vector2 screenColliderRange = new Vector2(100, 100);
    // [SerializeField] private Vector2 colliderOffset = new Vector2(50, 50);
    
    public new BoxCollider2D collider;
    public Vector2 lLeft, lRight, uLeft, uRight;

    private bool clickDown;
    private bool hovering;
    private float hoverFoodTimer;

    void Awake()
    {
        cam = Camera.main;
    }
    
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        
        if(Input.GetMouseButtonDown(0))
        {
            if(IsOnPet(mousePos))
            {
                MouseDown();
            }
        }
        else if(Input.GetMouseButtonUp(0) && clickDown)
        {
            MouseUp();
        }

        if(IsOnPet(mousePos))
        {
            MouseHover();
        }
        else if(hovering)
        {
            MouseUnHover();
        }
    }

    void LateUpdate()
    {
        // screenPosition = cam.WorldToScreenPoint(transform.position);
        lLeft = cam.WorldToScreenPoint(new Vector2(collider.bounds.min.x, collider.bounds.min.y));
        lRight = cam.WorldToScreenPoint(new Vector2(collider.bounds.max.x, collider.bounds.min.y));
        uLeft = cam.WorldToScreenPoint(new Vector2(collider.bounds.min.x, collider.bounds.max.y));
        uRight = cam.WorldToScreenPoint(new Vector2(collider.bounds.max.x, collider.bounds.max.y));
    }

    bool IsOnPet(Vector3 position)
    {
        /*
       return (position.x > -screenColliderRange.x + screenPosition.x + colliderOffset.x &&
               position.x < screenColliderRange.x + screenPosition.x + colliderOffset.x &&
               position.y > -screenColliderRange.y + screenPosition.y + colliderOffset.y &&
               position.y < screenColliderRange.y + screenPosition.y + colliderOffset.y);*/

        return (position.x > lLeft.x &&
                position.x < lRight.x &&
                position.y > lLeft.y &&
                position.y < uLeft.y);
    }
    
    void MouseDown()
    {       
        clickDown = true;
        if(onMouseDown != null)
            onMouseDown();
    }

    void MouseUp()
    {
        clickDown = false;
        if(onMouseUp != null)
            onMouseUp();
    }

    void MouseHover()
    {
        hovering = true;
        if(onMouseHover != null)
            onMouseHover();
    }

    void MouseUnHover()
    {
        hovering = false;
        if(onMouseUnHover != null)
            onMouseUnHover();
    }
    
    void MouseDrag()
    {
        if(onMouseDrag != null)
            onMouseDrag();
    }

}
