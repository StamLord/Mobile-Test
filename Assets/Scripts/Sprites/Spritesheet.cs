using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Spritesheet", menuName="Spritesheet")]
public class Spritesheet : ScriptableObject
{
    public Sprite[] idle;
    public Sprite[] pet;
    public Sprite[] happy;
    public Sprite[] sad;
    public Sprite[] eating;
    public Sprite[] sleeping;
}
