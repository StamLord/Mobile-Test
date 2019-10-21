using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFood : MonoBehaviour
{
    public Food food;
    public Image image;
    public UIFoodManager manager;

    public void SetFood(Food food)
    {
        this.food = food;
        image.sprite = food.sprite;
        image.SetNativeSize();
    }

    public void PickFood()
    {
        if(manager)
            manager.UpdateFoodInfo(food);
    }
}
