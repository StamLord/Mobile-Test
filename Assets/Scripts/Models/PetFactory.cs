﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetFactory : MonoBehaviour
{
    public static ActivePet CreateFromSnapshot(PetSnapshot snapshot)
    {
        ActivePet pet = new ActivePet();
        pet.SetSnapshot(snapshot);
        return pet;
    }

    public static ActivePet CreatePet(Species species, int stage, string treeName)
    {
        if(species == null)
            return null;
            
        double timestamp = Timestamp.GetTimeStamp();

        PetSnapshot petSnapshot = new PetSnapshot();

        petSnapshot.nickname = "";
        petSnapshot.species = species.speciesName;
        petSnapshot.birth = timestamp;
        petSnapshot.longetivity = Random.Range((int)species.longetivityMin, (int)species.longetivityMax + 1);
        
        petSnapshot.treeName = treeName;
        petSnapshot.stage = stage;
        petSnapshot.stageStamp = timestamp;

        petSnapshot.minWeight = species.minWeight;
        petSnapshot.maxWeight = species.maxWeight;
        petSnapshot.Weight = species.baseWeight;

        petSnapshot.careMistakeCost = species.careMistakeCost;

        petSnapshot.hungerRate = Random.Range(species.hungerRateMin, species.hungerRateMax + 1);
        petSnapshot.strengthRate = Random.Range(species.strengthRateMin, species.strengthRateMax + 1);
        petSnapshot.attentionRate = Random.Range(species.attentionRateMin, species.attentionRateMax + 1);

        petSnapshot.hungerStamp = timestamp;
        petSnapshot.strengthStamp = timestamp;
        petSnapshot.attentionStamp = timestamp;

        petSnapshot.happiness = 50;
        petSnapshot.happinessRate = Random.Range(species.happinessRateMin, species.happinessRateMax + 1);
        petSnapshot.happinessStamp = timestamp;
        
        petSnapshot.discipline = 50;
        petSnapshot.disciplineRate = Random.Range(species.disciplineRateMin, species.disciplineRateMax + 1);
        petSnapshot.disciplineStamp = timestamp;

        petSnapshot.energy = 20;
        petSnapshot.energyRecoveryRate = species.energyRecoveryRate;
        petSnapshot.energyStamp = timestamp;
        
        petSnapshot.s_atk = species.atk;
        petSnapshot.s_spd = species.spd;
        petSnapshot.s_def = species.def;

        petSnapshot.g_atk = Random.Range(0, 17);
        petSnapshot.g_spd = Random.Range(0, 17);
        petSnapshot.g_def = Random.Range(0, 17);

        ActivePet pet = new ActivePet();
        
        pet.SetSnapshot(petSnapshot);
        
        return pet;
    }

    public static ActivePet CreateEgg(EvolutionTree tree)
    {
        if(tree == null)
            return null;
        return CreatePet(tree.stage_0, 0, tree.name);
    }

    public static PetSnapshot Evolve(ActivePet from, Species to)
    {
        double timestamp = Timestamp.GetTimeStamp();

        PetSnapshot snapshot = from.GetSnapshotCopy(); 

        snapshot.species = to.speciesName;
        snapshot.longetivity = Random.Range((int)to.longetivityMin, (int)to.longetivityMax + 1);
        snapshot.stage = from.stage + 1;
        snapshot.stageStamp = timestamp;

        snapshot.minWeight = to.minWeight;
        snapshot.maxWeight = to.maxWeight;
        if(from.maxWeight - from.minWeight == 0) 
            snapshot.Weight = to.baseWeight;
        else 
            snapshot.Weight = Mathf.FloorToInt(((from.weight - from.minWeight ) / (from.maxWeight - from.minWeight)) * (to.maxWeight - to.minWeight) + to.minWeight);

        snapshot.careMistakeCost = to.careMistakeCost;

        snapshot.hungerRate = Random.Range(to.hungerRateMin, to.hungerRateMax + 1);
        snapshot.strengthRate = Random.Range(to.strengthRateMin, to.strengthRateMax + 1);
        snapshot.attentionRate = Random.Range(to.attentionRateMin, to.attentionRateMax + 1);

        snapshot.happinessRate = Random.Range(to.happinessRateMin, to.happinessRateMax + 1);
        snapshot.disciplineRate = Random.Range(to.disciplineRateMin, to.disciplineRateMax + 1);

        snapshot.energy = 20;
        snapshot.energyRecoveryRate = to.energyRecoveryRate;
        snapshot.energyStamp = timestamp;

        snapshot.s_atk = to.atk;
        snapshot.s_spd = to.spd;
        snapshot.s_def = to.def;

        return snapshot;
    }
}
