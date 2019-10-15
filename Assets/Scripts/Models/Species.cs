using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Species", menuName="Species")]
public class Species : ScriptableObject
{
    [SerializeField]
    Spritesheet[] spritesheets = new Spritesheet[1];
    public string speciesName;
    public double longetivityMin;
    public double longetivityMax;
    public int careMistakeCost;

    public int hungerRateMin;
    public int hungerRateMax;
    public int strengthRateMin;
    public int strengthRateMax;
    public int attentionRateMin;
    public int attentionRateMax;

    public int disciplineRateMin;
    public int disciplineRateMax;
    public int happinessRateMin;
    public int happinessRateMax;

    public int energyRecoveryRate;

    public int atk;
    public int spd;
    public int def;

}
