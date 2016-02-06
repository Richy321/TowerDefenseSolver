using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;

public class PathNode : MonoBehaviour, IHeapItem<PathNode>
{
    public PathNode north;
    public PathNode east;
    public PathNode south;
    public PathNode west;

    public PathNode parent;

    public bool walkable;

    public int gCost;
    public int hCost;

    public int gridX;
    public int gridY;

    public int heapIndex;

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


    public List<PathNode> GetNeighbours()
    {
        List<PathNode> neighbours = new List<PathNode>();
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

    public int CompareTo(PathNode nodeToCompare)
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
