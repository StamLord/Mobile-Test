using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    Pet[] activePets = new Pet[0];
    public static User user = new User();

    private void Awake()
    {
        LoadPets();

        TimeStep.onTimeStep += UpdatePets;

    }

    void UpdatePets()
    {
        foreach (Pet pet in activePets)
        {
            UpdateStats(pet);
        }
    }

    void UpdateStats(Pet pet)
    {
        
    }

    void SavePet(Pet pet)
    {

    }

    void LoadPets()
    {
        StartCoroutine(DataService.GetRequest());
    }
}
