using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    ActivePet testPet = new ActivePet();
    ActivePet[] activePets = new ActivePet[0];
    public static User user = new User();

    private void Awake()
    {
        testPet.snapshot = new PetSnapshot();
        testPet.snapshot.hunger = 5;
        testPet.snapshot.hungerStamp = Timestamp.GetTimeStamp();
        testPet.snapshot.hungerRate = 10;
        testPet.hunger = 0;

        LoadPets();

        TimeStep.onTimeStep += UpdatePets;
    }

    void UpdatePets()
    {
    //    foreach (Pet pet in activePets)
    //    {
    //        UpdateStats(pet);
    //    }
        UpdateStats(testPet);
    }

    void UpdateStats(ActivePet pet)
    {
        #region Hunger

        // Update hunger based on time since snapshot
        double timeSinceFed = Timestamp.GetSecondsSince(pet.snapshot.hungerStamp);
        pet.hunger = pet.snapshot.hunger - Mathf.FloorToInt((float)timeSinceFed / (float)pet.snapshot.hungerRate);
        pet.hunger = Mathf.Clamp(pet.hunger, 0, 5);
        Debug.Log(pet.hunger);

        if(pet.hunger < 1) 
        {
            // Check if enough time passed for pet to starve
            double starveTime = timeSinceFed - (pet.snapshot.hunger * pet.snapshot.hungerRate);
            if(pet.starving == false && 
            starveTime > pet.snapshot.starveAt)
            {
                pet.starving = true;
                Debug.Log("Pet is starving");
            }
        }

        #endregion


    }

    void SavePet(ActivePet pet)
    {
        //StartCoroutine(DataService.PutRequest());
    }

    void LoadPets()
    {
        StartCoroutine(DataService.GetRequest());
    }
}
