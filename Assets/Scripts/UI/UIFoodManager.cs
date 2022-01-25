using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIFoodManager : MonoBehaviour
{
    public Food[] foods;
    public bool autoPopulate = false;

    public Transform foodParent;
    public GameObject foodButtonPrefab;

    public GameObject info;
    public TextMeshProUGUI hunger, weight, discipline, happiness, energy;

    [Tooltip("Values will be represented with this character. Leave empty if you want to display in numeric values.")]
    [SerializeField] private char bar_char = '█';

    void Start()
    {
        if(autoPopulate)
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

        UpdateText(hunger, "HUNGER", newFood.hunger);
        UpdateText(energy, "ENERGY", newFood.energy);
        weight.text = "WEIGHT " + newFood.weight + "G";

        if(newFood.discipline > 0)
            discipline.text = "DISCIPLINE    +" + newFood.discipline; 
        else if(newFood.discipline < 0)
            discipline.text = "DISCIPLINE    " + newFood.discipline;
        else
            discipline.text = "DISCIPLINE    N/A";

        if(newFood.happiness > 0)
            happiness.text = "HAPPINESS    +" + newFood.happiness; 
        else if(newFood.happiness < 0)
            happiness.text = "HAPPINESS    " + newFood.happiness;
        else
            happiness.text = "HAPPINESS    N/A";
    }

    private void UpdateText(TextMeshProUGUI textHolder, string attributeName, int value)
    {
        textHolder.text = attributeName + " ";
        if(bar_char != ' ')
        {
            for (var i = 0; i < Mathf.Abs(value); i++)
                textHolder.text += bar_char + " ";
        }
        else
        {
            if(value > 0) textHolder.text += "+";
            textHolder.text += value;
        }
    }
}
