using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;

public class GridNode : MonoBehaviour, IHeapItem<GridNode>
{
    public GridNode north;
    public GridNode east;
    public GridNode south;
    public GridNode west;

    public GridNode parent;

    public bool walkable;
    public bool placeable = true;
    public int gCost;
    public int hCost;

    public int gridX;
    public int gridZ;

    public int heapIndex;
    public TowerType towerType = TowerType.None;

    public int fCost
    {
        get { return gCost + hCost; }
    }


    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}


    public List<GridNode> GetNeighbours()
    {
        List<GridNode> neighbours = new List<GridNode>();
        if(north != null)
            neighbours.Add(north);
        if(east != null)
            neighbours.Add(east);
        if(south != null)
            neighbours.Add(south);
        if(west != null)
            neighbours.Add(west);

        return neighbours;
    }

    public int CompareTo(GridNode nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
            compare = hCost.CompareTo(nodeToCompare.hCost);
        return -compare;
    }

    public int HeapIndex
    {
        get { return heapIndex; }
        set { heapIndex = value; }
    }
}
