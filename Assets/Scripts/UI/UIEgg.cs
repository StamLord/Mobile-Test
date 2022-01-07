using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Doozy.Engine.UI;

public class UIEgg : MonoBehaviour
{   
    private UIView view;
    EvolutionTree currentChoice;
    [SerializeField] private TextMeshProUGUI pickUpMessage;

    [SerializeField] private GameObject yes;
    [SerializeField] private GameObject no;
    
    public void Awake()
    {
        view = GetComponent<UIView>();
    }

    public void ChooseEgg(string treeName)
    {
        currentChoice = GameManager.instance.evolutionGarden.GetTree(treeName);
        UpdateText();
    }

    public void PickEgg()
    {
        if(GameManager.instance.PickupEgg(currentChoice))
        {    
            ScrapChoice();
            view.Hide();
        }
        else
        {
            ScrapChoice();
            pickUpMessage.text = string.Format("You are already training {0} pets", GameManager.instance.MaxActive).ToUpper();
        }        
    }

    public void ScrapChoice()
    {
        currentChoice = null;
        UpdateText();
    }

    public void UpdateText()
    {
        if(currentChoice)
        {
            pickUpMessage.text = string.Format("PICK UP {0} EGG?", currentChoice.name.ToUpper());
            yes.SetActive(true);
            no.SetActive(true);
        }
        else
        {
            pickUpMessage.text = "";
            yes.SetActive(false);
            no.SetActive(false);
        }
    }
}
