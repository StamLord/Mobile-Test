using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public EvolutionGarden evolutionGarden;
    public List<ActivePet> activePets = new List<ActivePet>();
    public User user = new User();
    public int maxActive = 2;

    #region Singleton

    public static GameManager instance;

    private void Awake()
    {
        TimeStep.onTimeStep += UpdatePets;

        if(instance == null)
            instance = this;
        else
            Debug.LogError("More than 1 instance of GameManager exists:" + instance);
    }

    #endregion

    public delegate void activePetsChangeDelegate();
    public static event activePetsChangeDelegate onActivePetsChange;

    public void SetUser(User user)
    {
        this.user = user;
        LoadPets();
    }

    void LoadPets()
    {
        for(int i = 0; i < maxActive && i < user.active.Length; i++)
        {
            foreach(PetSnapshot snapshot in user.pets)
            {
                if(snapshot._id == user.active[i])
                {
                    activePets.Add(PetFactory.CreateFromSnapshot(snapshot));
                }
            }
        }

        if(onActivePetsChange != null)
            onActivePetsChange();
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
        if(pet == null)
            return;

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
                    onActivePetsChange();
                }
            }
        }
    }

    public IEnumerator Login(string username, string password, bool remember)
    {
        Coroutine<User> routine = this.StartCoroutine<User>(DataService.Login(username, password));
        yield return routine.coroutine;

        if(routine.returnVal != null)
        {
            if(remember)
            {
                // Remembers credentials
                PlayerPrefs.SetString("username", username);
                PlayerPrefs.SetString("password", password);
            }

            SetUser(routine.returnVal);
        }
    }

    public bool PickupEgg(EvolutionTree tree)
    {
        if(activePets.Count >= maxActive)
            return false;

        ActivePet pet = PetFactory.CreateEgg(tree);
        if(pet != null)
            StartCoroutine(CreateEgg(pet));

        return true;
    }

    public IEnumerator CreateEgg(ActivePet pet)
    {
        Coroutine<string> routine = this.StartCoroutine<string>(DataService.CreatePet(pet, user.username));
        yield return routine.coroutine;

        if(routine.returnVal != null)
        {           
            pet.SetId(routine.returnVal);
            AddActive(pet);
        }
        else
        {
            Debug.Log("No _id from server: Aborting creation.");
        }
    }

    public void AddActive(ActivePet pet)
    {
        activePets.Add(pet);
            
        UpdateActive();

        onActivePetsChange();
    }

    public void RemoveActive(ActivePet pet)
    {
        activePets.Remove(pet);
            
        UpdateActive();

        onActivePetsChange();
    }

    public void UpdateActive()
    {
        string[] activeArray = new string[activePets.Count];
        for(int i = 0; i < activePets.Count; i++)
        {
            activeArray[i] = activePets[i]._id;
        }

        StartCoroutine(DataService.UpdateActive(user.username, activeArray));
    }

    public IEnumerator SaveSnapshot(ActivePet pet, PetSnapshot snapshot, PetSnapshot backup)
    {
        Coroutine<bool> routine = this.StartCoroutine<bool>(DataService.UpdatePet(snapshot));
        yield return routine.coroutine;

        if(routine.returnVal == false)
        {   
            Debug.Log("No response from server: Aborting save and restoring backup.");
            pet.SetSnapshot(backup);
        }
    }
}
