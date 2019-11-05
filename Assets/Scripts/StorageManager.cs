using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    public float planeHeight = .5f;

    public GameObject petVisualPrefab;
    public PetSnapshot[] storagePets;
    public List<SpritesheetAnimator> sprites = new List<SpritesheetAnimator>();
    public Species[] species = new Species[0];

    void Start()
    {
        //SetupPets(GameManager.instance.user.pets);

        PetSnapshot[] mocksnapshots = {
            new PetSnapshot(),
            new PetSnapshot(),
            new PetSnapshot(),
            new PetSnapshot()
        };

        mocksnapshots[0].species = "Chamiri";
        mocksnapshots[1].species = "Chamiri";
        mocksnapshots[2].species = "Harvest Egg";
        mocksnapshots[3].species = "Chamiri";

        SetupPets(mocksnapshots);
    }

    void SetupPets(PetSnapshot[] pets)
    {
        storagePets = pets;
        foreach(PetSnapshot pet in storagePets)
        {
            GameObject p = Instantiate(petVisualPrefab);
            
            p.AddComponent<Billboard>();
            p.AddComponent<RoamBehaviour>();

            p.transform.position = new Vector3(
                Random.Range(-7f, 7f),
                planeHeight,
                Random.Range(-7f, 7f));

            sprites.Add(p.GetComponent<SpritesheetAnimator>());
        }

        SpritesUpdate();
    }

    void SpritesUpdate()
    {
        for(int i = 0; i < sprites.Count; i++)
        {
            Spritesheet sheet = FindSheet(storagePets[i].species);
            sprites[i].spritesheet = sheet;
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
