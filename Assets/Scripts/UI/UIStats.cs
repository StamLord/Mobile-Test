using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;
public class UIStats : MonoBehaviour
{
    public UIImage[] hunger;
    public UIImage[] strength;
    public UIImage[] attention;

    public int viewingPet = 0;

    void Start()
    {
        GameManager.onSelectedPetUpdate += UpdateViewingPet;
    }

    void Update()
    {
        UpdateHungerBars();
    }

    public void UpdateViewingPet(int index)
    {
        if(index < 0)
            index = 0;
        viewingPet = index;
    }

    void UpdateHungerBars()
    {   
        if(GameManager.instance.activePets[viewingPet] == null)
            return;

        ActivePet pet = GameManager.instance.activePets[viewingPet];

        for(int i = 0; i < hunger.Length; i++)
        {
            if(i > pet.hunger - 1)
                hunger[i].enabled = false;
            else
                hunger[i].enabled = true;
        }
    }
}
