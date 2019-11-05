using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AIBrain : MonoBehaviour
{
    public enum AIState {ROAMING, EATING, PETTING, SLEEPING, DEAD}
    private AIState _state;
    public AIState state { 
        get{return this._state;}
        set{this._state = value; }
    }

    public SpritesheetAnimator animator {get; private set;}
    public ClickListener clickListener {get; private set;}

    public RoamBehaviour roaming;

    [SerializeField]
    private int petIndex = -1;
    [SerializeField]
    private float hoverFoodTimer = 0;
    [SerializeField]
    private float hoverPettingTimer = 0;

    void Awake()
    {
        animator = GetComponentInChildren<SpritesheetAnimator>();
        clickListener = GetComponent<ClickListener>();

        clickListener.onMouseHover += OnHover;
        clickListener.onMouseUnHover += OnUnHover;
        clickListener.onMouseDown += OnClick;
    }

    void Update()
    {
        if(petIndex == -1)
            return;
            
        switch(_state)
        {
            case AIState.ROAMING:
                roaming.Behaviour();
                break;
            case AIState.DEAD:
                break;
        }
    }

    void FixedUpdate()
    {
        if(petIndex == -1)
            return;

        if(GameManager.instance.activePets[petIndex].isDead)
        {
            state = AIState.DEAD;
            animator.PlayAnimation("Dead", true);
        }

        animator.SetTeachIcon(GameManager.instance.activePets[petIndex].canDiscipline);
    }

    public void SetIndex(int index)
    {
        petIndex = index;
    }

    public void SetSpriteSheet(Spritesheet spritesheet)
    {
         if(animator == null)
             animator = GetComponentInChildren<SpritesheetAnimator>();

        if(animator)
            animator.spritesheet = spritesheet;
    }

    public void Animate(string animation, bool loop)
    {
        animator.PlayAnimation(animation, loop);
    }

    public void OnClick()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        GameManager.instance.SetSelection(petIndex);
    }

    public void OnHover()
    {
        if(state == AIState.DEAD)
            return;

        if(GameManager.instance.selectedFood != null)
        {
            if(state != AIState.EATING)
                StartFeeding();

            hoverFoodTimer += Time.deltaTime;

            if(GameManager.instance.Feed(petIndex, hoverFoodTimer))
            {
                StopFeeding();
                animator.PlayAnimation("Happy", false);
            }
        }
        else if (EventSystem.current.IsPointerOverGameObject() == false)
        {
            if(state != AIState.PETTING)
                StartPetting();
                
            hoverPettingTimer += Time.deltaTime;
        }
    }

    void StartFeeding()
    {
        state = AIState.EATING;
        animator.PlayAnimation("Eating", true);
    }

    void StopFeeding()
    {
        if(GameManager.instance.selectedFood == null)
            hoverFoodTimer = 0f;
        if(state != AIState.ROAMING)
        {
            state = AIState.ROAMING;
            animator.PlayAnimation("Idle", true);
        }
    }

    void StartPetting()
    {
        state = AIState.PETTING;
        animator.PlayAnimation("Petting", true);
    }

    void StopPetting()
    {
        hoverPettingTimer = 0f;
        if(state != AIState.ROAMING)
        {
            state = AIState.ROAMING;
            animator.PlayAnimation("Idle", true);
        }
    }

    public void OnUnHover()
    {
        StopFeeding();
        StopPetting();
    }
    
}
