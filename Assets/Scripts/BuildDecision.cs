using UnityEngine;

public class BuildDecision
{
    public BuildDecision()
    {
    }

    public BuildDecision(BuildDecision other)
    {
        towerType = other.towerType;
        gridXCoord = other.gridXCoord;
        gridZCoord = other.gridZCoord;
    }

    public TowerType towerType;
    public int gridXCoord;
    public int gridZCoord;


}
