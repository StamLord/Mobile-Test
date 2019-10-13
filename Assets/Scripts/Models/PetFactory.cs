using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetFactory : MonoBehaviour
{
    public static PetBase CreatePet(Species species)
    {
        PetBase pet = new PetBase();

        pet.species = species.speciesName;
        pet.longetivity = Random.Range((float)species.longetivityRange[0], (float)species.longetivityRange[1] + 1);

        pet.careMistakeCost = species.careMistakeCost;

        pet.hungerRate = Random.Range(species.hungerRateMin, species.hungerRateMax + 1);
        pet.strengthRate = Random.Range(species.strengthRateMin, species.strengthRateMax + 1);
        pet.attentionRate = Random.Range(species.attentionRateMin, species.attentionRateMax + 1);

        pet.disciplineRate = Random.Range(species.disciplineRateMin, species.disciplineRateMax + 1);
        pet.happinessRate = Random.Range(species.happinessRateMin, species.happinessRateMax + 1);
        
        pet.s_atk = species.atk;
        pet.s_spd = species.spd;
        pet.s_def = species.def;

        pet.g_atk = Random.Range(0, 17);
        pet.g_spd = Random.Range(0, 17);
        pet.g_def = Random.Range(0, 17);

        return pet;
    }

    public static PetBase CreateEgg(EvolutionTree tree)
    {
        PetBase egg = CreatePet(tree.stage_0);
        egg.treeName = tree.name;
        egg.stage = 0;
        egg.birth = Timestamp.GetTimeStamp();

        return egg;
    }
}
