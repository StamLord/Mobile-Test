using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="NewTree", menuName="Evolution Tree")]
public class EvolutionTree : ScriptableObject
{
    public Species stage_0;
    public EvolutionPath[] evolutions;

    public double[] stagesEvolveAt = {60, 21600, 43200, 86400};

    public bool IsTimeToEvolve(ActivePet pet)
    {
        if(pet.stageTime >= stagesEvolveAt[pet.stage])
            return true;

        return false;
    }

    public Species GetEvolution(ActivePet pet)
    {
        if(pet.stage == 0)
            return GetEvolutionFromStage(pet, evolutions);
        else
            return GetEvolutionFromStage(pet, FindEvolutions(pet.species, evolutions));
    }

    private EvolutionPath[] FindEvolutions(string species, EvolutionPath[] paths)
    {
        EvolutionPath[] found;

        foreach(EvolutionPath e in paths)
        {
            if(e.species.name == species) // Found
                return e.evolutions;
            else
            {
                found = FindEvolutions(species, e.evolutions);
                if(found != null)
                    return found;
            }
        }

        return null;
    }
    
    private Species GetEvolutionFromStage(ActivePet pet, EvolutionPath[] paths)
    {
        // Return the species of the first path that meets all conditions or no conditions exist
        foreach(EvolutionPath path in paths)
        {
            if(path.CheckConditions(pet))
                return path.species;
        }
        // No Path met all conditions
        return null;
    }
}

[System.Serializable]
public class EvolutionPath
{
    public Species species;
    public EvolutionCondition[] conditions = new EvolutionCondition[1];
    [SerializeField] public EvolutionPath[] evolutions;

    public bool CheckConditions(ActivePet pet)
    {
        bool allConditionsMet = true;

        foreach(EvolutionCondition condition in conditions)
        {
            if(condition.Check(pet) == false)
                allConditionsMet = false;
        }

        return allConditionsMet;
    }

}

[System.Serializable]
public class EvolutionCondition
{
    public enum Property {hunger, strength, attention, atk, spd, def, happiness, discipline, careMistakes, UnderWeight, OverWeight, TimeOfDay, Weather, CustomValue};
    public enum Compare {BiggerThan, LessThan, Equal};

    public Property property1;
    public Compare compare;
    public Property property2;
    public int customValue;

    public bool Check(ActivePet pet){
        
        int prop1 = -1;
        int prop2 = -1;

        // Get 1st property from pet
        if(property1 == Property.Weather)
        {

        } 
        else if(property1 == Property.TimeOfDay)
        {

        } 
        else if(property1 == Property.CustomValue)
        {
            prop1 = customValue;
        }
        else 
        {
            string propName = property1.ToString("g");
            Debug.Log(propName);
            try 
            {
                prop1 = (int)pet.GetType().GetProperty(propName).GetValue(pet, null);
            } 
            catch(System.Exception error) 
            {
                Debug.Log(error.ToString());
            }
        }

        // Get 2nd property from pet
        if(property2 == Property.Weather)
        {

        } 
        else if(property2 == Property.TimeOfDay)
        {   
            
        } 
        else if(property2 == Property.CustomValue)
        {
            prop2 = customValue;
        }
        else 
        {
            string propName2 = property2.ToString("g");
            Debug.Log(propName2);
            try
            {
                prop2 = (int)pet.GetType().GetProperty(propName2).GetValue(pet, null);
            } 
            catch(System.Exception error) 
            {
                Debug.LogError(error.ToString());
            }
        }

        if(prop1 == -1 || prop2 == -1)
        {
            Debug.LogError("Error: One or more properties are -1 (Unassigned)");
            return false;
        }

        switch(compare)
        {
            case Compare.BiggerThan:
                return (prop1 > prop2);
            case Compare.Equal:
                return (prop1 == prop2);
            case Compare.LessThan:
                return (prop1 < prop2);
        }

        return false;
    }

}