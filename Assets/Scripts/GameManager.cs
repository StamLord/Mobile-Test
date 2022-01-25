using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public EvolutionGarden evolutionGarden;
    public Species[] species = new Species[0];

    public List<ActivePet> activePets = new List<ActivePet>();
    
    public User user = new User();
    [SerializeField] private int maxActive = 2;
    public int MaxActive { get{return maxActive;}}
    
    [SerializeField] private float feedingTime = 3f;

    [SerializeField] private float pettingTime = 3f;
    [SerializeField] private int pettingHappiness = 10;

    [SerializeField] private float sleepHour = 21;
    [SerializeField] private float wakeHour = 8;
    [SerializeField] private int napHours = 3;


    [SerializeField] private int selection = -1;
    public int SelectedPetIndex { get{ return selection; } }
    public ActivePet SelectedPet { get {return activePets[selection]; } }
    public Food selectedFood;
    
    [SerializeField] private DeadPopup deadPopup;

    private bool updatedVisual;

    #region Singleton

    public static GameManager instance;

    private void Awake()
    {
        TimeStep.onTimeStep += UpdateActivePets;
        SceneManager.sceneLoaded += OnSceneLoaded;

        if(instance == null)
        {    
            instance = this;
            DontDestroyOnLoad(instance.gameObject);
        }
        else
        {
            Debug.LogError("More than 1 instance of GameManager exists, destroying:" + instance);
            Destroy(gameObject);
        }
    }

    #endregion

    #region Events

    public delegate void activePetsChangeDelegate(List<ActivePet> activePets);
    public static event activePetsChangeDelegate onActivePetsChange;

    public delegate void selectedPetUpdateDelegate(int index);
    public static event selectedPetUpdateDelegate onSelectedPetUpdate;

    public delegate void midFeedingDelegate(float ratio);
    public static event midFeedingDelegate onMidFeeding;

    public delegate void finishedFeedingDelegate();
    public static event finishedFeedingDelegate onFinishedFeeding;

    public delegate void misbehavePetDelegate(int index);
    public static event misbehavePetDelegate onMisbehavePet;

    public delegate void evolutionDelegate(Sprite oldSprite, Sprite newSprite);
    public static event evolutionDelegate onEvolutionEvent;

    public delegate void sleepChangeDelegate(int index, bool sleeping);
    public static event sleepChangeDelegate onSleepChange;

    #endregion

    public void SetUser(User user)
    {
        this.user = user;
        LoadPets();
    }

    private void LoadPets()
    {
        Debug.Log(user.active);
        for(int i = 0; i < maxActive && i < user.active.Length; i++)
        {
            foreach(PetSnapshot snapshot in user.pets)
            {
                if(snapshot._id == user.active[i])
                {
                    bool inGraveyard = false;
                    
                    foreach(string grave in user.graveyard)
                    {
                        if(snapshot._id == grave)
                            inGraveyard = true;
                    }

                    if(inGraveyard == false)
                        activePets.Add(PetFactory.CreateFromSnapshot(snapshot));
                }
            }
        }

        if(onActivePetsChange != null)
            onActivePetsChange(activePets);
    }

    #region Scene Prepare

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded:" + scene.name + "Scene Mode: " +mode.ToString());
        updatedVisual = false;
    }

    void Update()
    {
        if(updatedVisual == false)
        {
            if(onActivePetsChange != null)
            {         
                onActivePetsChange(activePets);
            }
            updatedVisual = true;
        }
    }

    #endregion
    
    private void UpdateActivePets()
    {
        foreach (ActivePet pet in activePets)
            UpdatePet(pet);
    }

    private void UpdatePet(ActivePet pet)
    {   
        if(pet == null || SceneManager.GetActiveScene().name != "Main")
            return;

        // Care mistakes affect evolution so we check for them before evolving
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
                //Debug.Log("Evolving to " + evolveTo.name);
                if(evolveTo)
                {
                    Debug.Log("Evolving to " + evolveTo.name);
                    // Animate
                    if(onEvolutionEvent != null)
                        onEvolutionEvent(FindSheet(pet.species).idle[0], FindSheet(evolveTo.speciesName).idle[0]);

                    pet.EvolveTo(PetFactory.Evolve(pet, evolveTo));
                    
                    if(onActivePetsChange != null)
                        onActivePetsChange(this.activePets);
                }
            }
        }
    }

    public IEnumerator Login(string username, string password, bool remember)
    {
        UIManager.instance.SetLoading(true, "Syncing", true);

        Coroutine<User> routine = this.StartCoroutine<User>(DataService.Login(username, password));
        yield return routine.coroutine;

        UIManager.instance.SetLoading(false, "", true);

        if(routine.returnVal != null)
        {
            if(remember)
            {
                // Remembers credentials
                PlayerPrefs.SetString("username", username);
                PlayerPrefs.SetString("password", password);
            }

            SetUser(routine.returnVal);
            yield return true;
        }

        yield return false;
    }

    public IEnumerator Register(string username, string password, string email)
    {
        UIManager.instance.SetLoading(true, "Syncing", true);

        Coroutine<User> routine = this.StartCoroutine<User>(DataService.Register(username, password, email));
        yield return routine.coroutine;

        UIManager.instance.SetLoading(false, "", true);

        if(routine.returnVal != null)
        {
            Debug.Log(routine.returnVal);
            SetUser(routine.returnVal);
            yield return true;
        }

        yield return false;
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
            
        StartCoroutine(UpdateActive());

        if(onActivePetsChange != null)
            onActivePetsChange(this.activePets);
    }

    public void RemoveActive(ActivePet pet)
    {
        activePets.Remove(pet);
            
        StartCoroutine(UpdateActive());

        if(onActivePetsChange != null)
            onActivePetsChange(this.activePets);
    }


    public void MoveSelectedToGraveyard()
    {   
        deadPopup.Hide();

        Debug.Log("Moving selection " + selection + " to graveyard");

        if(selection < 0)
            return;
        
        user.graveyard.Add(activePets[selection]._id);

        StartCoroutine(UpdateGraveyard());
        RemoveActive(activePets[selection]);
    }

    public void SetSelection(int index)
    {
        if(index < 0) // If background selected (index -1)
            return;

        if(deadPopup.IsVisible()) // Currently not in use since death removed
            return;

        selection = index;
        Debug.Log("Selected: " + selection);

        if(onSelectedPetUpdate != null)
            onSelectedPetUpdate(index);
        
        if(selection >= 0)   
        {
            if(activePets[selection].isDead)
                deadPopup.Popup(activePets[selection]);
        }
    }

    /// <summary>
    /// Set the selection to be the first active pet or -1 if none
    /// </summary>
    public void ForceSelection()
    {
        if(activePets.Count > 0)
            SetSelection(0);
        else
            SetSelection(-1);
    }

    public void StartTraining(string stat)
    {
        int petIndex = selection;

        if(petIndex < 0)
        {
            UIManager.instance.ShowTrainingTip("Please select your pet!");
            return;
        }

        if(activePets[petIndex].isDead)
            return;

        if(activePets[petIndex].energy < 1)
        {
            UIManager.instance.ShowTrainingTip("Pet has no energy!");
            return;
        }

        if(DiscplineCheck(activePets[petIndex].discipline) == false)
        {
            if(onMisbehavePet != null)
                onMisbehavePet(petIndex);

            activePets[petIndex].Misbehave();
            UIManager.instance.ShowPopup("X");
            return;
        }
        
        switch(stat.ToLower())
        {
            case "atk":
                GameObject.Find("Graph Controller").
                GetComponent<Doozy.Engine.Nody.GraphController>().
                GoToNodeByName("Load ATK Training");
            //SceneManager.LoadSceneAsync("ATK Training");
                break; 
            case "spd":
                GameObject.Find("Graph Controller").
                GetComponent<Doozy.Engine.Nody.GraphController>().
                GoToNodeByName("Load SPD Training");
            // SceneManager.LoadSceneAsync("SPD Training");
                break; 
            case "def":
                GameObject.Find("Graph Controller").
                GetComponent<Doozy.Engine.Nody.GraphController>().
                GoToNodeByName("Load DEF Training");
            // SceneManager.LoadSceneAsync("DEF Training");
                break; 
        }
    }

    public bool DiscplineCheck(int discipline)
    {
        int check = Random.Range(0, 101);
        return (discipline >= check);
    }

    public void Praise()
    {
        if(activePets.Count == 1)
            SetSelection(0);
        
        if(selection > -1)
        {
            SelectedPet.Praise();
            UIManager.instance.ShowPopup("HAPPY", 10);
        }
    }

    public void Scold()
    {
        if(activePets.Count == 1)
            SetSelection(0);

        if(selection > -1)
        {
            SelectedPet.Scold();

        if(SelectedPet.CanDiscipline()) 
            UIManager.instance.ShowPopup("DISCIPLINE", 10);
        else
            UIManager.instance.ShowPopup("HAPPY", -10);
        }
    }

    public void Train(int activePet, string stat, int gain)
    {
        UIManager.instance.ShowPopup(stat, gain);

        activePets[activePet].TrainStat(stat, gain, 1);
        activePets[activePet].ReduceEnergy(1);
        
        int injury = InjuryCheck(activePets[activePet].energy, activePets[activePet].happiness);
        if(injury > 0)
            activePets[activePet].Injure(injury);
    }

    private int InjuryCheck(int energy, int happiness)
    {
        int injury = 0;

        float remap = (energy - 5) / (15 - 5); // Remaps the energy to a scale between 5 and 15
        if (remap < 0) remap = 0;
        float chance = 0.5f * (1 - remap);

        float rand = Random.Range(0f, 1f);

        if(rand < chance)
            injury = Random.Range(0, 6);

        return injury;
    }

    public bool Feed(int petIndex, float timeHovered)
    {
        if(timeHovered >= feedingTime && petIndex >= 0)
        {
            activePets[petIndex].Feed(
                selectedFood.hunger, 
                selectedFood.weight,
                selectedFood.happiness,
                selectedFood.discipline,
                selectedFood.energy);
            if(onFinishedFeeding != null)
                onFinishedFeeding();
            return true;
        }
        else
        {
            if(onMidFeeding != null)
                onMidFeeding(timeHovered / feedingTime);
            return false;
        }
    }

    public void Sleep()
    {
        if(selection < 0)
            return;
            
        SelectedPet.Sleep(napHours);
        if(onSleepChange != null)
            onSleepChange(selection, true);
    }

    public void Wake()
    {   
        if(selection < 0)
            return;

        SelectedPet.Wake();
        if(onSleepChange != null)
            onSleepChange(selection, false);
    }

    public bool Pet(int petIndex, float timeHovered)
    {
        if(timeHovered >= pettingTime && petIndex >= 0)
        {
            if(activePets[petIndex].happiness >= 100)
            {
                UIManager.instance.ShowPopup("MAX HAPPY");
                return false;
            }
            else
            {
                activePets[petIndex].UpdateHapiness(pettingHappiness);
                UIManager.instance.ShowPopup("HAPPY", pettingHappiness);
                return true;
            }
        }

        return false;
    }

    public IEnumerator SaveSnapshot(ActivePet pet, PetSnapshot snapshot, PetSnapshot backup)
    {
        UIManager.instance.SetLoading(true, "Syncing", true);

        Coroutine<bool> routine = this.StartCoroutine<bool>(DataService.UpdatePet(snapshot));
        yield return routine.coroutine;

        UIManager.instance.SetLoading(false, "", true);

        if(routine.returnVal == false)
        {   
            Debug.Log("No response from server: Aborting save and restoring backup.");
            pet.SetSnapshot(backup);
        }
    }

    public IEnumerator UpdateActive()
    {
        string[] activeArray = new string[activePets.Count];
        for(int i = 0; i < activePets.Count; i++)
        {
            activeArray[i] = activePets[i]._id;
        }

        UIManager.instance.SetLoading(true, "Syncing", true);

        Coroutine<bool> routine = this.StartCoroutine<bool>(DataService.UpdateActive(user.username, activeArray));
        yield return routine.coroutine;

        UIManager.instance.SetLoading(false, "", true);

        if(routine.returnVal == false)
        {
            Debug.Log("No response from server:");
        }
    }

    public IEnumerator UpdateGraveyard()
    {
        UIManager.instance.SetLoading(true, "Syncing", true);

        Coroutine<bool> routine = this.StartCoroutine<bool>(DataService.UpdateGraveyard(user.username, user.graveyard));
        yield return routine.coroutine;

        UIManager.instance.SetLoading(false, "", true);

        if(routine.returnVal == false)
        {
            Debug.Log("No response from server:");
        }
    }

    public Spritesheet FindSheet(string name)
    {
        foreach(Species s in species)
        {
            if(s.speciesName == name)
                return s.spritesheets[0];
        }

        Debug.Log("Species doesn't exist in collection!");
        return null;
    }
}
