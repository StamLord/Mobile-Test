using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActivePet
{
    [SerializeField] private PetSnapshot snapshot;
    
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

    public int weight { get{ return snapshot.Weight; } }
    public int minWeight {get{ return snapshot.minWeight; } }
    public int maxWeight {get{ return snapshot.maxWeight; } }

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
        calculatedHappiness = Mathf.Clamp(calculatedHappiness, 0, 100);

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

    /// <summary>
    /// Get energy based on time slept
    /// </summary>
    /// <returns>Amount of Energy</returns>
    private int CalculateEnergy()
    {
        if(snapshot.sleepStamp < snapshot.energyStamp) // If did not sleep after last energy reduction, no change in
            return snapshot.energy;

        double totalSleepTime = SleepTime();
        
        int calculatedEnergy = snapshot.energy + Mathf.FloorToInt((float)totalSleepTime / (float)snapshot.energyRecoveryRate);

        calculatedEnergy = Mathf.Clamp(calculatedEnergy, 0, 10);
        Debug.Log("Total SleepTime: " + totalSleepTime);
        Debug.Log("Energy: " + calculatedEnergy);
        return calculatedEnergy;
    }

    /// <summary>
    /// Get energy based on time since snapshot taking into account sleeping time. Currently does not work properly
    /// </summary>
    /// <returns>Amount of Energy</returns>
    private int CalculateEnergyComplex()
    {
        double timeSinceTrained = Timestamp.GetSecondsSince(snapshot.energyStamp);
        
        // Find out how much of this time was spent sleeping
        double timeSlept = 0.0;

        if(snapshot.sleepStamp > snapshot.energyStamp) // Compares stamps to see if slept after training
            timeSlept = (IsSleeping())? Timestamp.GetSecondsSince(snapshot.sleepStamp) : snapshot.sleepHours * 60 * 60; // <-- Need to add wakeupTime to snapshot to calculate sleep to wake time

        Debug.Log("Sleep hours: " + snapshot.sleepHours);
        Debug.Log("Time slept: " + timeSlept);
        
        if(timeSlept < 0) timeSlept = 0;
        double timeAwake = timeSinceTrained - timeSlept;
        
        Debug.Log("Awake time: " + timeAwake);

        double totalTime = timeAwake + timeSlept * 2; // When sleeping energy regenerates twice as fast
        Debug.Log("Total time: " + totalTime);
        int calculatedEnergy = snapshot.energy + Mathf.FloorToInt((float)totalTime / (float)snapshot.energyRecoveryRate);

        calculatedEnergy = Mathf.Clamp(calculatedEnergy, 0, 10);
        Debug.Log("Energy: " + calculatedEnergy);
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

    public void Sleep(int hours)
    {
        if(IsSleeping())
            return;
        
        // Create backup in case server doesn't respond
        PetSnapshot backup = GetSnapshotCopy(); 
        snapshot.sleepHours = hours;
        snapshot.sleepStamp = Timestamp.GetTimeStamp();
        
        // Send snapshot to server
        GameManager.instance.StartCoroutine(GameManager.instance.SaveSnapshot(this, snapshot, backup));
    }

    public void Wake()
    {
        if(IsSleeping() == false)
            return;

        // Create backup in case server doesn't respond
        PetSnapshot backup = GetSnapshotCopy(); 
        snapshot.wakeStamp = Timestamp.GetTimeStamp();
        snapshot.energy = CalculateEnergy(); // We "save" the energy replenished until this point

        // Send snapshot to server
        GameManager.instance.StartCoroutine(GameManager.instance.SaveSnapshot(this, snapshot, backup));
    }

    public bool IsSleeping()
    {
        return (snapshot.wakeStamp < snapshot.sleepStamp);
    }

    public double SleepTime()
    {
        if(IsSleeping())
            return Timestamp.GetSecondsSince(snapshot.sleepStamp);
        
        double timeSinceSleepStart = Timestamp.GetSecondsSince(snapshot.sleepStamp);
        double timeSinceWake = Timestamp.GetSecondsSince(snapshot.wakeStamp);
        return timeSinceSleepStart - timeSinceWake;
    }

    private bool IsDead()
    {
        // Remove death for testing
        return false;
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
        snapshot.Weight += weightChange;
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

    public bool CanDiscipline()
    {
        double timeSinceMisbehaved = Timestamp.GetSecondsSince(snapshot.misbehaveStamp);
        return (timeSinceMisbehaved <= 600);
    }

    public void Praise()
    {
        // Create backup in case server doesn't respond
        PetSnapshot backup = GetSnapshotCopy(); 
        snapshot.happiness += 10;
        snapshot.happinessStamp = Timestamp.GetTimeStamp();

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
            // Remove misbehave timestamp
            snapshot.misbehaveStamp = 0;
        }
        snapshot.happiness -= 10;
        snapshot.happinessStamp = Timestamp.GetTimeStamp();

        // Send snapshot to server
        GameManager.instance.StartCoroutine(GameManager.instance.SaveSnapshot(this, snapshot, backup));
    }

    public void UpdateHapiness(int amount)
    {
        // Create backup in case server doesn't respond
        PetSnapshot backup = GetSnapshotCopy(); 
        snapshot.happiness += amount;
        snapshot.happinessStamp = Timestamp.GetTimeStamp();

        // Send snapshot to server
        GameManager.instance.StartCoroutine(GameManager.instance.SaveSnapshot(this, snapshot, backup));
    }

    public void SetStarving(bool starving)
    {
        snapshot.isStarving = starving;

        // Send snapshot to server
    }

    public int GetWeightStatus()
    {   
        float precentage = (float)(weight - minWeight) / (maxWeight - minWeight);
        Debug.Log(precentage);
        if(precentage < 0.3)
            return -1; // Underweight
        else if (precentage > 0.7)
            return 1; // Overweight
        else
            return 0; // Normal
    }

    public void TrainStat(string stat, int statChange, int strengthChange)
    {       
        PetSnapshot backup;
        switch(stat)
        {
            case "atk":
                backup = GetSnapshotCopy(); 
                snapshot.t_atk += statChange;
                snapshot.Strength += strengthChange;
                snapshot.strengthStamp = Timestamp.GetTimeStamp();
                GameManager.instance.StartCoroutine(GameManager.instance.SaveSnapshot(this, snapshot, backup));
                break;
            case "spd":
                backup = GetSnapshotCopy(); 
                snapshot.t_spd += statChange;
                snapshot.Strength += strengthChange;
                snapshot.strengthStamp = Timestamp.GetTimeStamp();
                GameManager.instance.StartCoroutine(GameManager.instance.SaveSnapshot(this, snapshot, backup));
                break;
            case "def":
                backup = GetSnapshotCopy(); 
                snapshot.t_def += statChange;
                snapshot.Strength += strengthChange;
                snapshot.strengthStamp = Timestamp.GetTimeStamp();
                GameManager.instance.StartCoroutine(GameManager.instance.SaveSnapshot(this, snapshot, backup));
                break;
        }
    }

    public void ReduceEnergy(int amount)
    {
        // Create backup in case server doesn't respond
        PetSnapshot backup = GetSnapshotCopy(); 
        snapshot.energy = energy - amount;
        snapshot.energyStamp = Timestamp.GetTimeStamp();Debug.Log(snapshot.energy);

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

        copy.Weight = snapshot.Weight;
        copy.minWeight = snapshot.minWeight;
        copy.maxWeight = snapshot.maxWeight;

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

        copy.sleepStamp = snapshot.sleepStamp;
        copy.sleepHours = snapshot.sleepHours;

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
