using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class UIFood : UIDraggable
{
    public Food food;
    public Image image;
    public UIFoodManager manager;

    public void SetFood(Food food)
    {
        this.food = food;
        image.sprite = food.sprite[0];
        image.SetNativeSize();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        PickFood();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        ReturnFood();
    }

    public void ReturnFood()
    {
        isDragging = false;
        StartCoroutine(ReturnToOrigin(1f));
        CancelFood();
    }

    void BiteFood(float ratio)
    {
        float percent = 1f / food.sprite.Length;
        int i = Mathf.FloorToInt(ratio / percent);
        image.sprite = food.sprite[i];
    }

    void AteFood()
    {
        image.enabled = false;
        ReturnFood();
    }

    public void PickFood()
    {
        if(manager)
            manager.UpdateFoodInfo(food);
        GameManager.instance.selectedFood = food;
        GameManager.onMidFeeding += BiteFood;
        GameManager.onFinishedFeeding += AteFood;
    }

    public void CancelFood()
    {
        GameManager.instance.selectedFood = null;
         GameManager.onMidFeeding -= BiteFood;
        GameManager.onFinishedFeeding -= AteFood;
    }

    IEnumerator ReturnToOrigin(float time)
    {
        canDrag = false;
        float timer = 0;

        while(timer < time)
        {
            transform.position = Vector3.Lerp(transform.position, startingPoint, timer / time);
            yield return null;

            if(Vector3.Distance(transform.position, startingPoint) < 2f)
                timer = time;

            timer += Time.deltaTime;
        }

        canDrag = true;
        image.enabled = true;
        image.sprite = food.sprite[0];
    }
}
