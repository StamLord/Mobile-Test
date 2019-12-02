using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.UI;
public class UIStats : MonoBehaviour
{
    public UIView view;
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
        UpdateStrengthBars();
        UpdateAttentionBars();
    }

    public void UpdateViewingPet(int index)
    {
        if(index == -1)
            view.Hide();
        else
        {
            viewingPet = index;
            view.Show();
        }
    }

    void UpdateHungerBars()
    {   
        if(viewingPet > GameManager.instance.activePets.Count - 1 || GameManager.instance.activePets[viewingPet] == null)
        {
            for(int i = 0; i < hunger.Length; i++)
            {
                hunger[i].enabled = false;
            }
            return;
        }

        ActivePet pet = GameManager.instance.activePets[viewingPet];

        for(int i = 0; i < hunger.Length; i++)
        {
            hunger[i].enabled = i <= pet.hunger - 1;
        }
    }

    void UpdateStrengthBars()
    {   
        if(viewingPet > GameManager.instance.activePets.Count - 1 || GameManager.instance.activePets[viewingPet] == null)
        {
            for(int i = 0; i < strength.Length; i++)
            {
                strength[i].enabled = false;
            }
            return;
        }

        ActivePet pet = GameManager.instance.activePets[viewingPet];

        for(int i = 0; i < strength.Length; i++)
        {
            strength[i].enabled = i <= pet.strength - 1;
        }
    }

    void UpdateAttentionBars()
    {   
        if(viewingPet > GameManager.instance.activePets.Count - 1 || GameManager.instance.activePets[viewingPet] == null)
        {
            for(int i = 0; i < attention.Length; i++)
            {
                attention[i].enabled = false;
            }
            return;
        }

        ActivePet pet = GameManager.instance.activePets[viewingPet];

        for(int i = 0; i < attention.Length; i++)
        {
            attention[i].enabled = i <= pet.attention - 1;
        }
    }
}
