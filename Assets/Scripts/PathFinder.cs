using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PathFinder
{
    const int cost = 10;

    public List<PathNode> FindPath(PathNode start, PathNode end, PathNode[,] grid)
    {
        Heap<PathNode> openSet = new Heap<PathNode>(grid.GetLength(0) * grid.GetLength(1));
        HashSet<PathNode> closedSet = new HashSet<PathNode>();
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            PathNode currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            //path found
            if (currentNode == end)
                return RetracePath(start, end);

            foreach (PathNode neighbour in currentNode.GetNeighbours())
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int newNeighbourMoveCost = currentNode.gCost + GetDistanceManhatten(currentNode, neighbour);
                if (newNeighbourMoveCost < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newNeighbourMoveCost;
                    neighbour.hCost = GetDistanceManhatten(neighbour, end);
                    neighbour.parent = currentNode;

                    if(!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else
                        openSet.UpdateItem(neighbour);
                }
            }
        }

        return new List<PathNode>();
    }

    int GetDistanceManhatten(PathNode a, PathNode b)
    {
        return Mathf.Abs(a.gridX - b.gridX) * cost + Mathf.Abs(a.gridY - b.gridY) * cost;
    }

    List<PathNode> RetracePath(PathNode start, PathNode end)
    {
        Stack<PathNode> path = new Stack<PathNode>();
        PathNode curNode = end; //trace backwards

        while (curNode != start)
        {
            path.Push(curNode.parent);
            curNode = curNode.parent;
        }

        return path.ToList();
    }

}
