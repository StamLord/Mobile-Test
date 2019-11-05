using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActivePet
{
    private PetSnapshot snapshot;
    
    #region Properties

    public string _id {get {return snapshot._id; } }

    public string species { get{ return snapshot.species; } }

    public string treeName { get{ return snapshot.treeName; } }

    public int stage { get{ return snapshot.stage; } }
    public double stageTime { get{ return CalculateStageTime(); } }

    public double birth { get{ return snapshot.birth; } }
    public int age { get { return CalculateAge(); } }

    public string nickname { get{ return snapshot.nickname; } }

    public bool isInjured
    {
        get {return IsInjured();}
    }

    public bool isDead
    {
        get {return IsDead();}
    }

    //public new bool isStarving;

    public int careMistakes { get{ return snapshot.careMistakes; } }
    public int careMistakeCost { get{ return snapshot.careMistakeCost; } }

    public int weight { get{ return snapshot.weight; } }

    public double starveAt { get{ return snapshot.starveAt; } }
    public bool isStarving {get {return snapshot.isStarving; } set{ snapshot.isStarving = value;} }

    public int hunger 
    {
        get { return CalculateHunger(); }
    }

    public int strength 
    {
        get { return CalculateStrength(); }
    }

    public int attention 
    {
        get { return CalculateAttention(); }
    }

    public int happiness { get{ return CalculateHappiness(); }}
    public int discipline { get{ return CalculateDiscipline(); }}

    public bool canDiscipline 
    {
        get{ return CanDiscipline();}
    }
    public int energy { get{ return CalculateEnergy(); }}

    public int atk { get{ return snapshot.atk; } }
    public int spd { get{ return snapshot.spd; } }
    public int def { get{ return snapshot.def; } }

    #endregion

    public void SetSnapshot(PetSnapshot snapshot)
    {
        this.snapshot = snapshot;
    }

    public void SetId(string id)
    {
        this.snapshot._id = id;
    }

    private int CalculateAge()
    {
        double timeSinceBirth = Timestamp.GetSecondsSince(birth);
        int age = Mathf.FloorToInt((float)timeSinceBirth / 60 /60 / 24);
        if(isDead)
        {
            double secondsSinceDeath = Timestamp.GetSecondsSince(snapshot.stageStamp + snapshot.longetivity);
            age -= Mathf.FloorToInt((float)secondsSinceDeath / 60 / 60 / 24);
        }
        
        return age;
    }

    private double CalculateStageTime()
    {
        return Timestamp.GetSecondsSince(snapshot.stageStamp);
    }

    private double GetTimeSinceFed()
    {
        return Timestamp.GetSecondsSince(snapshot.hungerStamp);
    }

    private int CalculateHunger()
    {
        // Update hunger based on time since snapshot
        double timeSinceFed = Timestamp.GetSecondsSince(snapshot.hungerStamp);
        int calculatedHunger = snapshot.Hunger - Mathf.FloorToInt((float)timeSinceFed / (float)snapshot.hungerRate);
        calculatedHunger = Mathf.Clamp(calculatedHunger, 0, 5);

        //Debug.Log("Seconds passed: " + timeSinceFed + "calculatedHunger: " + calculatedHunger + "snapshot hunger: " +snapshot.hunger + " hunger rate:" + snapshot.hungerRate);

        return calculatedHunger;
    }

    private int CalculateStrength()
    {
        // Update strength based on time since snapshot
        double timeSinceStrength = Timestamp.GetSecondsSince(snapshot.strengthStamp);
        int calculatedStrength = snapshot.Strength - Mathf.FloorToInt((float)timeSinceStrength / (float)snapshot.strengthRate);
        calculatedStrength = Mathf.Clamp(calculatedStrength, 0, 5);

        return calculatedStrength;
    }

    private int CalculateAttention()
    {
        // Update attention based on time since snapshot
        double timeSinceAttention = Timestamp.GetSecondsSince(snapshot.attentionStamp);
        int calculatedAttention = snapshot.Attention - Mathf.FloorToInt((float)timeSinceAttention / (float)snapshot.attentionRate);
        calculatedAttention = Mathf.Clamp(calculatedAttention, 0, 5);

        return calculatedAttention;
    }

    private int CalculateHappiness()
    {
        // Update happiness based on time since snapshot
        double timeSinceHappiness = Timestamp.GetSecondsSince(snapshot.happinessStamp);
        int calculatedHappiness = snapshot.happiness - Mathf.FloorToInt((float)timeSinceHappiness / (float)snapshot.happinessRate);
        calculatedHappiness = Mathf.Clamp(calculatedHappiness, 0, 5);

        return calculatedHappiness;
    }

    private int CalculateDiscipline()
    {
        // Update discipline based on time since snapshot
        double timeSinceDiscipline = Timestamp.GetSecondsSince(snapshot.disciplineStamp);
        int calculatedDiscipline = snapshot.discipline - Mathf.FloorToInt((float)timeSinceDiscipline / (float)snapshot.disciplineRate);
        calculatedDiscipline = Mathf.Clamp(calculatedDiscipline, 0, 100);

        return calculatedDiscipline;
    }

    private int CalculateEnergy()
    {
        // Update discipline based on time since snapshot
        double timeSinceTrained = Timestamp.GetSecondsSince(snapshot.energyStamp);
        int calculatedEnergy = snapshot.energy + Mathf.FloorToInt((float)timeSinceTrained / (float)snapshot.energyRecoveryRate);
        calculatedEnergy = Mathf.Clamp(calculatedEnergy, 0, 20);

        return calculatedEnergy;
    }

    public double GetStarvingTime()
    {
        double timeSinceFed = GetTimeSinceFed();
        double starveTime = timeSinceFed - (snapshot.Hunger * snapshot.hungerRate);
        return starveTime;
    }

    public void Injure(int amount)
    {
        if(IsInjured())
            return;

        // Create backup in case server doesn't respond
        PetSnapshot backup = GetSnapshotCopy(); 
        snapshot.injury = amount;
        snapshot.injuryStamp = Timestamp.GetTimeStamp();
        
        // Send snapshot to server
        GameManager.instance.StartCoroutine(GameManager.instance.SaveSnapshot(this, snapshot, backup));
    }
    
    private bool IsInjured()
    {
        double timeSinceInjured = Timestamp.GetSecondsSince(snapshot.injuryStamp);
        int currentInjury = snapshot.injury - (Mathf.FloorToInt((float)timeSinceInjured / snapshot.injuryRecoveryRate));

        return (currentInjury > 0);
    }

    public bool IsDead()
    {
        if(stage == 0) // Eggs can't die
            return false;

        double stageTime = Timestamp.GetSecondsSince(snapshot.stageStamp);
        if(stageTime >= snapshot.longetivity)
        {
            // Kill Pet :<
            return true;
        }
        return false;
    }

    public void Feed(int hungerChange, int weightChange, int happinessChange, int disciplineChange, int energyChange)
    {   
        double stamp = Timestamp.GetTimeStamp();
        // Create backup in case server doesn't respond
        PetSnapshot backup = GetSnapshotCopy(); 
        snapshot.Hunger = hunger + hungerChange;
        snapshot.hungerStamp = stamp;
        snapshot.weight += weightChange;
        snapshot.happiness += happinessChange;
        snapshot.discipline += disciplineChange;
        snapshot.disciplineStamp = stamp;
        snapshot.energy += energyChange;
        snapshot.isStarving = false;
        
        // Send snapshot to server
        GameManager.instance.StartCoroutine(GameManager.instance.SaveSnapshot(this, snapshot, backup));

    }

    public void AddCareMistake()
    {
        // Create backup in case server doesn't respond
        PetSnapshot backup = GetSnapshotCopy(); 
        snapshot.careMistakes++;
        snapshot.longetivity -= careMistakeCost;

        // Send snapshot to server
        GameManager.instance.StartCoroutine(GameManager.instance.SaveSnapshot(this, snapshot, backup));
    }

    public void Misbehave()
    {
        // Create backup in case server doesn't respond
        PetSnapshot backup = GetSnapshotCopy(); 
        snapshot.misbehaveStamp = Timestamp.GetTimeStamp();

        // Send snapshot to server
        GameManager.instance.StartCoroutine(GameManager.instance.SaveSnapshot(this, snapshot, backup));
    }

    private bool CanDiscipline()
    {
        double timeSinceMisbehaved = Timestamp.GetSecondsSince(snapshot.misbehaveStamp);
        return (timeSinceMisbehaved <= 600);
    }

    public void Praise()
    {
        // Create backup in case server doesn't respond
        PetSnapshot backup = GetSnapshotCopy(); 
        snapshot.happiness += 10;

        // Send snapshot to server
        GameManager.instance.StartCoroutine(GameManager.instance.SaveSnapshot(this, snapshot, backup));
    }

    public void Scold()
    {
        // Create backup in case server doesn't respond
        PetSnapshot backup = GetSnapshotCopy(); 
        if(CanDiscipline())
        {
            snapshot.discipline = discipline + 10;
            snapshot.disciplineStamp = Timestamp.GetTimeStamp();
        }
        snapshot.happiness -= 10;

        // Send snapshot to server
        GameManager.instance.StartCoroutine(GameManager.instance.SaveSnapshot(this, snapshot, backup));
    }

    public void SetStarving(bool starving)
    {
        snapshot.isStarving = starving;

        // Send snapshot to server
    }

    public void TrainStat(string stat, int change)
    {       
        PetSnapshot backup;
        switch(stat)
        {
            case "atk":
                backup = GetSnapshotCopy(); 
                snapshot.t_atk += change;
                GameManager.instance.StartCoroutine(GameManager.instance.SaveSnapshot(this, snapshot, backup));
                break;
            case "spd":
                backup = GetSnapshotCopy(); 
                snapshot.t_spd += change;
                GameManager.instance.StartCoroutine(GameManager.instance.SaveSnapshot(this, snapshot, backup));
                break;
            case "def":
                backup = GetSnapshotCopy(); 
                snapshot.t_def += change;
                GameManager.instance.StartCoroutine(GameManager.instance.SaveSnapshot(this, snapshot, backup));
                break;
        }

        Debug.Log(snapshot.energy);
    }

    public void ReduceEnergy(int amount)
    {
        // Create backup in case server doesn't respond
        PetSnapshot backup = GetSnapshotCopy(); 
        snapshot.energy = energy - amount;
        snapshot.energyStamp = Timestamp.GetTimeStamp();

        // Send snapshot to server
        GameManager.instance.StartCoroutine(GameManager.instance.SaveSnapshot(this, snapshot, backup));
    }

    public void EvolveTo(PetSnapshot newSnapshot)
    {
        // Create backup in case server doesn't respond
        PetSnapshot backup = GetSnapshotCopy(); 
        SetSnapshot(newSnapshot);

        // Send snapshot to server
        GameManager.instance.StartCoroutine(GameManager.instance.SaveSnapshot(this, snapshot, backup));
    }

    public PetSnapshot GetSnapshotCopy()
    {
        PetSnapshot copy = new PetSnapshot();

        copy._id = snapshot._id;

        copy.species = snapshot.species;
        copy.treeName = snapshot.treeName;

        copy.stage = snapshot.stage;
        copy.stageStamp = snapshot.stageStamp;

        copy.birth = snapshot.birth;

        copy.nickname = snapshot.nickname;
        
        copy.careMistakes = snapshot.careMistakes;
        copy.careMistakeCost = snapshot.careMistakeCost;

        copy.weight = snapshot.weight;
        copy.starveAt = snapshot.starveAt;

        copy.Hunger = snapshot.Hunger;
        copy.hungerRate = snapshot.hungerRate;
        copy.hungerStamp = snapshot.hungerStamp;

        copy.Strength = snapshot.Strength;
        copy.strengthRate = snapshot.strengthRate;
        copy.strengthStamp = snapshot.strengthStamp;

        copy.Attention = snapshot.Attention;
        copy.attentionRate = snapshot.attentionRate;
        copy.attentionStamp = snapshot.attentionStamp;

        copy.happiness = snapshot.happiness;
        copy.happinessRate = snapshot.happinessRate;
        copy.happinessStamp = snapshot.happinessStamp;

        copy.discipline = snapshot.discipline;
        copy.disciplineRate = snapshot.disciplineRate;
        copy.disciplineStamp = snapshot.disciplineStamp;

        copy.energy = snapshot.energy;
        copy.energyStamp = snapshot.energyStamp;

        copy.s_atk = snapshot.s_atk;
        copy.s_spd = snapshot.s_spd;
        copy.s_def = snapshot.s_def;

        copy.g_atk = snapshot.g_atk;
        copy.g_spd = snapshot.g_spd;
        copy.g_def = snapshot.g_def;

        copy.t_atk = snapshot.t_atk;
        copy.t_spd = snapshot.t_spd;
        copy.t_def = snapshot.t_def;

        

        return copy;
    }
}
