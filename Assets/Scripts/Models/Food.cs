using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Food", menuName="Food")]
public class Food : ScriptableObject
{
    public string foodName;
    public Sprite[] sprite;

    [Range(0,5)]
    public int hunger;
    public int weight;
    public int happiness;
    public int discipline;
    [Range(0,20)]
    public int energy;
}
