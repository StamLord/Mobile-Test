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

    void FixedUpdate()
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
        if(viewingPet > GameManager.instance.activePets.Count - 1 || GameManager.instance.activePets[viewingPet] == null)
        {
            for(int i = 0; i < energy.Length; i++)
            {
                energy[i].enabled = false;
            }
            
            return;
        }

        ActivePet pet = GameManager.instance.activePets[viewingPet];

        for(int i = 0; i < energy.Length; i++)
        {
            energy[i].enabled = i <= pet.energy - 1;
        }
        Debug.Log(pet.energy);
    }

    void OnDestroy()
    {
        GameManager.onSelectedPetUpdate -= UpdateViewingPet;
    }
}
