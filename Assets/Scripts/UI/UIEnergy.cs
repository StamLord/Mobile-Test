using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;
public class UIEnergy : MonoBehaviour
{
    public UIImage[] energy;

    public int viewingPet = 0;

    void Start()
    {
        GameManager.onSelectedPetUpdate += UpdateViewingPet;
    }

    void Update()
    {
        UpdateEnergyBars();
    }

    public void UpdateViewingPet(int index)
    {
        if(index < 0)
            index = 0;
        viewingPet = index;
    }

    void UpdateEnergyBars()
    {   
        if(GameManager.instance.activePets[viewingPet] == null)
            return;

        ActivePet pet = GameManager.instance.activePets[viewingPet];

        for(int i = 0; i < energy.Length; i++)
        {
            if(i > pet.energy - 1)
                energy[i].enabled = false;
            else
                energy[i].enabled = true;
        }
    }

    void OnDestroy()
    {
        GameManager.onSelectedPetUpdate -= UpdateViewingPet;
    }
}
