using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public EvolutionGarden evolutionGarden;
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
        testPet.strength = 1;
        testPet.stage = 0;
        testPet.treeName = "Egg1";
        testPet.s_atk = 1;
        testPet.g_atk = 2;
        testPet.t_atk = 1;

        testPet.s_spd = 1;
        testPet.g_spd = 1;
        testPet.t_spd = 1;

        Debug.Log(evolutionGarden.GetTree(testPet.treeName).GetEvolution(testPet));

        LoadPets();

        TimeStep.onTimeStep += UpdatePets;
    }

    void UpdatePets()
    {
       foreach (ActivePet pet in activePets)
       {
           UpdateStats(pet);
       }
    }

    void UpdateStats(ActivePet pet)
    {
        if(pet.isDead) return;

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

        #region Death

        double stageTime = Timestamp.GetSecondsSince(pet.snapshot.stageStamp);
        if(stageTime >= pet.snapshot.longetivity)
        {
            // Kill Pet :<
            pet.isDead = true;
        }
        
        #endregion
    }

    void SavePet(ActivePet pet)
    {
        //StartCoroutine(DataService.PutRequest());
    }

    void LoadPets()
    {
        StartCoroutine(DataService.Login("testUser1", "testPass1"));
    }
}
