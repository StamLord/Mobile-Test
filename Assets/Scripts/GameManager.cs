using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public EvolutionGarden evolutionGarden;
    ActivePet testPet;
    public ActivePet[] activePets = new ActivePet[1];
    public User user = new User();

    public bool light;

    public static GameManager instance;

    private void Awake()
    {
        TimeStep.onTimeStep += UpdatePets;

        if(instance == null)
            instance = this;
        else
            Debug.LogError("More than 1 instance of GameManager exists:" + instance);
    }

    void Start()
    {
        LoadPets();

        testPet = PetFactory.CreateEgg(evolutionGarden.GetTree("Egg1"));
        StartCoroutine(DataService.CreatePet(testPet, "testUser1"));
        activePets[0] = testPet;
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
        if(pet.hunger < 1) 
        {
            // Check if enough time passed for pet to starve
            double starveTime = pet.GetStarvingTime();            
            if(pet.isStarving == false && 
            starveTime > pet.starveAt)
            {
                pet.isStarving = true;
                Debug.Log("Pet is starving");
                pet.AddCareMistake();
            }
        }

        EvolutionCheck(pet);
        
    }

    public void EvolutionCheck(ActivePet pet)
    {
        EvolutionTree tree = evolutionGarden.GetTree(pet.treeName);

        if(tree == null) 
            Debug.LogError("Didn't find tree: " + pet.treeName +" of " + pet.species);
        else
        {
            if(tree.IsTimeToEvolve(pet))
            {
                Species evolveTo = tree.GetEvolution(pet);
                
                if(evolveTo)
                {
                    // Animate

                    PetFactory.Evolve(pet, evolveTo);
                }
            }
        }
    }

    void SavePet(ActivePet pet)
    {
        //StartCoroutine(DataService.PutRequest());
    }

    void LoadPets()
    {
        //StartCoroutine(DataService.Login("testUser1", "testPass1"));
    }
}
