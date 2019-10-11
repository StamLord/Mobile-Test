using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PetBase
{
    public string species;
    public string nickname;

    public double longetivity;

    public int careMistakes;
    public double starveAt = 20;
    public bool starving;
    private int _hunger;
    public int hunger 
    {
        get { return _hunger; } 
        set{ _hunger = Mathf.Clamp(value, 0, 5); }
    }
    public double hungerRate = 10;
    public int strength;
    public double strengthRate = 10;
    public int attention;
    public double attentionRate = 10;

    public int happiness;
    public int happinessRate = 10;

    public int discipline;
    public int disciplineRate = 10;
}