using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;
public class UIStats : MonoBehaviour
{
    public UIImage[] hunger;
    public UIImage[] strength;
    public UIImage[] attention;

    public int activePet;

    void Update()
    {
        UpdateHungerBars();
    }

    void UpdateHungerBars()
    {   
        ActivePet pet = GameManager.instance.activePets[activePet];
        
        for(int i = 0; i < hunger.Length; i++)
        {
            if(i > pet.hunger - 1)
                hunger[i].enabled = false;
            else
                hunger[i].enabled = true;
        }
    }
}
