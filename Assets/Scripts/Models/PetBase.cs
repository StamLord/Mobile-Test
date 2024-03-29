﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PetBase
{   
    public string _id;

    public string species;
    public string treeName;
    public int stage;
    public double birth;
    public string nickname;

    public bool isDead;
    public double longetivity;

    public int careMistakes;
    public int careMistakeCost;

    [SerializeField]
    private int weight;
    public int Weight 
    { 
        get { return weight; }
        set { weight = Mathf.Clamp(value, minWeight, maxWeight); }
    }

    public int minWeight;
    public int maxWeight;

    public double starveAt = 20;
    public bool isStarving;
    
    [SerializeField]
    private int hunger;
    public int Hunger 
    {
        get { return hunger; } 
        set { hunger = Mathf.Clamp(value, 0, 5); }
    }
    public double hungerRate = 10;
    
    [SerializeField]
    private int strength;
    public int Strength
    {
        get { return strength;}
        set { strength = Mathf.Clamp(value, 0, 5); }
    }
    public double strengthRate = 10;
    private int attention;
    public int Attention
    {
        get { return attention; }
        set {attention = Mathf.Clamp(value, 0, 5); }
    }
    public double attentionRate = 10;

    public int happiness;
    public int happinessRate = 10;

    public int discipline;
    public int disciplineRate = 10;

    public int energy;
    public int energyRecoveryRate = 60;

    public int injury;
    public int injuryRecoveryRate = 3600;

    public int s_atk;
    public int s_spd;
    public int s_def;

    public int g_atk;
    public int g_spd;
    public int g_def;

    public int t_atk;
    public int t_spd;
    public int t_def;

    public int atk
    {
        get {return s_atk + g_atk + t_atk;}
    }
    public int spd
    {
        get {return s_spd + g_spd + t_spd;}
    }
    public int def
    {
        get {return s_def + g_def + t_def;}
    }


}