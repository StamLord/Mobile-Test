using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Evolution Garden", menuName="Evolution Garden")]
public class EvolutionGarden : ScriptableObject
{
    public EvolutionTree[] evolutionTrees = new EvolutionTree[1];

    public EvolutionTree GetTree(string treeName)
    {
        foreach(EvolutionTree tree in evolutionTrees)
        {
            if(tree.name == treeName)
            {    
                return tree;
            }
        }
        return null;
    }
}
