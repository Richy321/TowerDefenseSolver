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


    public override bool Equals(object obj)
    {
        // If parameter cannot be cast to ThreeDPoint return false:
        BuildDecision p = obj as BuildDecision;
        if (p == null)
        {
            return false;
        }

        return Equals(p);
    }

    public bool Equals(BuildDecision other)
    {
        return towerType == other.towerType &&
               gridXCoord == other.gridXCoord &&
               gridZCoord == other.gridZCoord;
    }
}
