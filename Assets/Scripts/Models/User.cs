using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class User
{
    public string username;
    public Pet[] collection = new Pet[256];
}
