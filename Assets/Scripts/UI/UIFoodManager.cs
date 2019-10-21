using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIFoodManager : MonoBehaviour
{
    public Food[] foods;

    public Transform foodParent;
    public GameObject foodButtonPrefab;

    public GameObject info;
    public TextMeshProUGUI hunger, weight, discipline, happiness, energy;

    void Start()
    {
        InitializeFoods();
    }

    void InitializeFoods()
    {
        foreach(Food f in foods)
        {
            GameObject go = Instantiate(foodButtonPrefab, foodParent);
            UIFood uiFood = go.GetComponent<UIFood>();
            uiFood.SetFood(f);
            uiFood.manager = this;
        }
    }

    public void UpdateFoodInfo(Food newFood)
    {
        if(newFood == null)
        {
            info.SetActive(false);
            return;
        }

        info.SetActive(true);

        if(newFood.hunger > 0)
        {
            hunger.text = "HUNGER +" + newFood.hunger;
        } 
        else if(newFood.hunger < 0)
        {
            hunger.text = "HUNGER " + newFood.hunger;
        }
        else
        {
            hunger.text = "HUNGER ";
        }

        if(newFood.weight > 0)
        {
            weight.text = "WEIGHT +" + newFood.weight;
        } 
        else if(newFood.weight < 0)
        {
            weight.text = "WEIGHT " + newFood.weight;
        }
        else
        {
            weight.text = "WEIGHT ";
        }

        if(newFood.discipline > 0)
        {
            discipline.text = "DISCIPLINE +" + newFood.discipline;
        } 
        else if(newFood.discipline < 0)
        {
            discipline.text = "DISCIPLINE " + newFood.discipline;
        }
        else
        {
            discipline.text = "DISCIPLINE ";
        }

        if(newFood.happiness > 0)
        {
            happiness.text = "HAPPINESS +" + newFood.happiness;
        } 
        else if(newFood.happiness < 0)
        {
            happiness.text = "HAPPINESS " + newFood.happiness;
        }
        else
        {
            happiness.text = "HAPPINESS ";
        }

        if(newFood.energy > 0)
        {
            energy.text = "ENERGY +" + newFood.energy;
        } 
        else if(newFood.energy < 0)
        {
            energy.text = "ENERGY " + newFood.energy;
        }
        else
        {
            energy.text = "ENERGY ";
        }
    }
}
