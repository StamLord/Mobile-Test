using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeVisual : MonoBehaviour
{
    public EvolutionTree tree;
    public GameObject speciesPrefab;
    public float branchHeight;
    public float branchLength;

    // Start is called before the first frame update
    void Start()
    {
        BuildTree(tree.evolutions, 0, 0);
    }

    void BuildTree(EvolutionPath[] paths, int level, float x)
    {
        if(paths == null || paths.Length == 0)
            return;

        int i = 0;
        foreach(EvolutionPath p in paths)
        {
            float offsetX = branchLength / paths.Length;
            if(level == 0) offsetX = 0;
            float posX = i * branchLength - offsetX + x;
            Debug.Log(offsetX);
            Vector3 pos = new Vector3(posX, level * branchHeight);
            GameObject species = Instantiate(speciesPrefab, transform) as GameObject;
            if(p.species)
            {
                species.name = p.species.name;
                //species.GetComponent<SpriteRenderer>().sprite =
                species.transform.position = pos;
                // Add visual component
            }
            BuildTree(p.evolutions, level + 1, posX);

            i++;
        }
    }
}
