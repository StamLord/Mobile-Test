using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActivePet: PetBase
{
    private PetSnapshot snapshot;
    
    #region Properties

    public new string species { get{ return snapshot.species; } }

    public new string treeName { get{ return snapshot.treeName; } }

    public new int stage { get{ return snapshot.stage; } }
    public double stageTime { get{ return CalculateStageTime(); } }

    public new double birth { get{ return snapshot.birth; } }
    public int age { get { return CalculateAge(); } }

    public new string nickname { get{ return snapshot.nickname; } }

    public new bool isDead
    {
        get {return IsDead();}
    }

    //public new bool isStarving;

    public new int careMistakes { get{ return snapshot.careMistakes; } }
    public new int careMistakeCost { get{ return snapshot.careMistakeCost; } }

    public new int weight { get{ return snapshot.weight; } }

    public new double starveAt { get{ return snapshot.starveAt; } }

    public new int hunger 
    {
        get { return CalculateHunger(); }
    }

    public new int strength 
    {
        get { return CalculateStrength(); }
    }

    public new int attention 
    {
        get { return CalculateAttention(); }
    }

    public new int happiness { get{ return CalculateHappiness(); }}
    public new int discipline { get{ return CalculateDiscipline(); }}

    public new int energy { get{ return CalculateEnergy(); }}

    public new int atk { get{ return snapshot.atk; } }
    public new int spd { get{ return snapshot.spd; } }
    public new int def { get{ return snapshot.def; } }

    #endregion

    public void SetSnapshot(PetSnapshot snapshot)
    {
        this.snapshot = snapshot;
    }

    public int CalculateAge()
    {
        double timeSinceBirth = Timestamp.GetSecondsSince(birth);
        return Mathf.FloorToInt((float)timeSinceBirth / 60 / 24);
    }

    public double CalculateStageTime()
    {
        return Timestamp.GetSecondsSince(snapshot.stageStamp);
    }

    public double GetTimeSinceFed()
    {
        return Timestamp.GetSecondsSince(snapshot.hungerStamp);
    }

    private int CalculateHunger()
    {
        // Update hunger based on time since snapshot
        double timeSinceFed = Timestamp.GetSecondsSince(snapshot.hungerStamp);
        int calculatedHunger = snapshot.hunger - Mathf.FloorToInt((float)timeSinceFed / (float)snapshot.hungerRate);
        calculatedHunger = Mathf.Clamp(calculatedHunger, 0, 5);

        Debug.Log("Seconds passed: " + timeSinceFed + "calculatedHunger: " + calculatedHunger + "snapshot hunger: " +snapshot.hunger + " hunger rate:" + snapshot.hungerRate);

        return calculatedHunger;
    }

    private int CalculateStrength()
    {
        // Update strength based on time since snapshot
        double timeSinceStrength = Timestamp.GetSecondsSince(snapshot.strengthStamp);
        int calculatedStrength = snapshot.strength - Mathf.FloorToInt((float)timeSinceStrength / (float)snapshot.strengthRate);
        calculatedStrength = Mathf.Clamp(calculatedStrength, 0, 5);

        return calculatedStrength;
    }

    private int CalculateAttention()
    {
        // Update attention based on time since snapshot
        double timeSinceAttention = Timestamp.GetSecondsSince(snapshot.attentionStamp);
        int calculatedAttention = snapshot.attention - Mathf.FloorToInt((float)timeSinceAttention / (float)snapshot.attentionRate);
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
        calculatedDiscipline = Mathf.Clamp(calculatedDiscipline, 0, 5);

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
        double starveTime = timeSinceFed - (snapshot.hunger * snapshot.hungerRate);
        return starveTime;
    }
    
    public bool IsDead()
    {
        double stageTime = Timestamp.GetSecondsSince(snapshot.stageStamp);
        if(stageTime >= snapshot.longetivity)
        {
            // Kill Pet :<
            return true;
        }
        return false;
    }

    public void Feed(int hungerChange, int weightChange)
    {   
        snapshot.hunger = hunger + hungerChange;
        Debug.Log("Feeding" + snapshot.hunger);
        snapshot.hungerStamp = Timestamp.GetTimeStamp();
        snapshot.weight += weightChange;
        snapshot.isStarving = false;

        // Send snapshot to server
    }

    public void AddCareMistake()
    {
        snapshot.careMistakes++;
        snapshot.longetivity -= careMistakeCost;

        // Send snapshot to server
    }

    public void SetStarving(bool starving)
    {
        snapshot.isStarving = starving;

        // Send snapshot to server
    }

    public void TrainStat(string stat, int change)
    {
        switch(stat)
        {
            case "atk":
                snapshot.t_atk += change;
                break;
            case "spd":
                snapshot.t_spd += change;
                break;
            case "def":
                snapshot.t_def += change;
                break;
        }
        
        // Send snapshot to server
    }

    public void ReduceEnergy(int amount)
    {
        snapshot.energy = energy - amount;
        snapshot.energyStamp = Timestamp.GetTimeStamp();

        // Send snapshot to server
    }

    public bool SendSnapshot(PetSnapshot newSnapshot)
    {
        return true;
    }

    public void SetBirth(double timestamp)
    {
        snapshot.birth = timestamp;
    }

    public PetSnapshot GetSnapshotCopy()
    {
        PetSnapshot copy = new PetSnapshot();

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

        copy.hunger = snapshot.hunger;
        copy.hungerRate = snapshot.hungerRate;
        copy.hungerStamp = snapshot.hungerStamp;

        copy.strength = snapshot.strength;
        copy.strengthRate = snapshot.strengthRate;
        copy.strengthStamp = snapshot.strengthStamp;

        copy.attention = snapshot.attention;
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
