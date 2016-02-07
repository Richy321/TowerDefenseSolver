using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PathFinder : MonoBehaviour
{
    public const int MoveCost = 10;
    public PathRequestManager requestManager;

    public void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
    }

    public void StartFindPath(PathNode pathStart, PathNode pathEnd, PathNode[,] grid)
    {
        StartCoroutine(FindPath(pathStart, pathEnd, grid));
    }

    public IEnumerator FindPath(PathNode start, PathNode end, PathNode[,] grid)
    {
        List<PathNode> path = new List<PathNode>();
        bool success = false;
        if (start.walkable && end.walkable)
        {
            Heap<PathNode> openSet = new Heap<PathNode>(grid.GetLength(0)*grid.GetLength(1));
            HashSet<PathNode> closedSet = new HashSet<PathNode>();
            openSet.Add(start);

            while (openSet.Count > 0)
            {
                PathNode currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                //path found
                if (currentNode == end)
                {
                    success = true;
                    break;
                }

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

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }

        if (success)
        {
            path = RetracePath(start, end);
        }

        yield return null;

        requestManager.FinishedProcessingPath(path.ToArray(), success);
    }

    int GetDistanceManhatten(PathNode a, PathNode b)
    {
        return Mathf.Abs(a.gridX - b.gridX) * MoveCost + Mathf.Abs(a.gridY - b.gridY) * MoveCost;
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
        
        return SimplifyPath(path);
    }

    List<PathNode> SimplifyPath(Stack<PathNode> pathStack)
    {
        List<PathNode> simplifiedPath = new List<PathNode>();
        Vector2 oldDirection = Vector2.zero;
        PathNode curNode;

        while (pathStack.Count > 1)
        {
            curNode = pathStack.Pop();
            PathNode nextNode = pathStack.Peek();
            Vector2 curDirection = new Vector2(nextNode.gridX - curNode.gridX, nextNode.gridY - curNode.gridY);
            if (curDirection != oldDirection)
            {
                simplifiedPath.Add(curNode);
            }
            oldDirection = curDirection;
        }
        simplifiedPath.Add(pathStack.Pop());

        return simplifiedPath;
    } 

}
