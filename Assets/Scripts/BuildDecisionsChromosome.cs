using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts;
using System;

public class BuildDecisionsChromosome : MonoBehaviour, IChromosome<BuildDecisionsChromosome>
{
    public List<BuildDecision> buildDecisionGenes = new List<BuildDecision>();
    public Map map;

    #region IChromosome Implementation
    public int Fitness { get; set; }

    public void OnAwake()
    {
        if (!map) map = GetComponent<Map>();
    }

    public int Length
    {
        get { return buildDecisionGenes.Count; }
    }

    public BuildDecisionsChromosome Clone()
    {
        BuildDecisionsChromosome clone = (BuildDecisionsChromosome)MemberwiseClone();
        clone.buildDecisionGenes = new List<BuildDecision>(buildDecisionGenes);
        return clone;
    }

    public void Crossover(int pos, BuildDecisionsChromosome other)
    {
        if(pos >= buildDecisionGenes.Count)
            throw new UnityException("Crossover has invalid parameters: pos is >= count");

        for (int i = 0; i <= pos; i++)
        {
            BuildDecision tmp = buildDecisionGenes[i];
            buildDecisionGenes[i] = other.buildDecisionGenes[i];
            other.buildDecisionGenes[i] = tmp;
        }
    }

    public void Mutate(int geneIndex)
    {
        if (geneIndex >= buildDecisionGenes.Count)
            throw new UnityException("Mutate has invalid parameters: geneIndex is >= count");

        buildDecisionGenes[geneIndex].towerType = Map.GetRandomTowerType();
        if (buildDecisionGenes[geneIndex].towerType != TowerType.None)
        {
            GridNode randNode = map.GetRandomPlaceableNode();
            buildDecisionGenes[geneIndex].gridXCoord = randNode.gridX;
            buildDecisionGenes[geneIndex].gridZCoord = randNode.gridZ;
        }
    }

    public void Randomise()
    {
        throw new NotImplementedException();
    }
    #endregion

}

