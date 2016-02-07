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

    public void StartFindPath(PathNode pathStart, PathNode pathEnd, List<List<PathNode>> grid)
    {
        StartCoroutine(FindPath(pathStart, pathEnd, grid));
    }

    public bool FindPathImmediate(PathNode start, PathNode end, List<List<PathNode>> grid, out List<PathNode> path)
    {
        path = new List<PathNode>();
        bool success = false;
        if (start.walkable && end.walkable)
        {
            Heap<PathNode> openSet = new Heap<PathNode>(grid.Count * grid[0].Count);
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

        return success;
    }

    public IEnumerator FindPath(PathNode start, PathNode end, List<List<PathNode>> grid)
    {
        List<PathNode> path;
        bool success = FindPathImmediate(start, end, grid, out path);

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

        while (curNode != null)
        {
            path.Push(curNode);
            curNode = curNode.parent;
        }
        
        return SimplifyPath(path);
    }

    List<PathNode> SimplifyPath(Stack<PathNode> pathStack)
    {
        List<PathNode> simplifiedPath = new List<PathNode>();
        Vector2 oldDirection = Vector2.zero;
        PathNode curNode;

        while (pathStack.Count > 0)
        {
            curNode = pathStack.Pop();
            Vector2 curDirection = Vector2.zero;

            if (pathStack.Count != 0) //last
            {
                PathNode nextNode = pathStack.Peek();
                curDirection = new Vector2(nextNode.gridX - curNode.gridX, nextNode.gridY - curNode.gridY);
            }

            if (curDirection != oldDirection)
            {
                simplifiedPath.Add(curNode);
            }
            oldDirection = curDirection;
        }

        return simplifiedPath;
    }
}
