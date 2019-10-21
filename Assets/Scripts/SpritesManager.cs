using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesManager : MonoBehaviour
{
    public List<SpritesheetAnimator> sprites = new List<SpritesheetAnimator>();
    public Species[] species = new Species[0];

    void Start()
    {
        GameManager.onActivePetsChange += UpdateSpriteSheets;
    }

    void UpdateSpriteSheets()
    {
        for (int i = 0; i < sprites.Count; i++)
        {   
            if(i < GameManager.instance.activePets.Count)
            {
                ActivePet pet = GameManager.instance.activePets[i];
                Spritesheet sheet = FindSheet(pet.species);
                if(sheet != null)
                    sprites[i].spritesheet = sheet;
                else
                    sprites[i].spritesheet = null;
            }
            else
                sprites[i].spritesheet = null;
            
        }
    }

    Spritesheet FindSheet(string name)
    {
        foreach(Species s in species)
        {
            if(s.speciesName == name)
                return s.spritesheets[0];
        }
        return null;
    }
}
