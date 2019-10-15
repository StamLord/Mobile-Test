using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class User
{
    public string username;
    public PetSnapshot[] pets = new PetSnapshot[256];
}
