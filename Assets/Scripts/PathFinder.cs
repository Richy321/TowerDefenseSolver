using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PathFinder
{
    const int cost = 10;

    public List<PathNode> FindPath(PathNode start, PathNode end, PathNode[,] grid)
    {
        List<PathNode> openSet = new List<PathNode>();
        HashSet<PathNode> closedSet = new HashSet<PathNode>();

        openSet.Add(start);

        while (openSet.Count > 0)
        {
            PathNode currentNode = openSet[0];
            
            //todo optimise this crap
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                    openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                    currentNode = openSet[i];
            }

            openSet.Remove(currentNode);
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
