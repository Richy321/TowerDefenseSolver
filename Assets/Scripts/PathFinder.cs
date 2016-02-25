using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinder : MonoBehaviour
{
    public const int MoveCost = 10;
    public PathRequestManager requestManager;

    public void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
    }

    public void StartFindPath(GridNode pathStart, GridNode pathEnd, List<List<GridNode>> grid)
    {
        StartCoroutine(FindPath(pathStart, pathEnd, grid));
    }

    public bool FindPathImmediate(GridNode start, GridNode end, List<List<GridNode>> grid, out List<GridNode> path)
    {
        path = new List<GridNode>();
        bool success = false;
        if (start.walkable && end.walkable)
        {
            Heap<GridNode> openSet = new Heap<GridNode>(grid.Count * grid[0].Count);
            HashSet<GridNode> closedSet = new HashSet<GridNode>();
            openSet.Add(start);

            while (openSet.Count > 0)
            {
                GridNode currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                //path found
                if (currentNode == end)
                {
                    success = true;
                    break;
                }

                foreach (GridNode neighbour in currentNode.GetNeighbours())
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

    public IEnumerator FindPath(GridNode start, GridNode end, List<List<GridNode>> grid)
    {
        List<GridNode> path;
        bool success = FindPathImmediate(start, end, grid, out path);

        yield return null;

        requestManager.FinishedProcessingPath(path.ToArray(), success);
    }

    int GetDistanceManhatten(GridNode a, GridNode b)
    {
        return Mathf.Abs(a.gridX - b.gridX) * MoveCost + Mathf.Abs(a.gridZ - b.gridZ) * MoveCost;
    }

    List<GridNode> RetracePath(GridNode start, GridNode end)
    {
        Stack<GridNode> path = new Stack<GridNode>();
        GridNode curNode = end; //trace backwards

        while (curNode != null)
        {
            path.Push(curNode);
            curNode = curNode.parent;
        }
        
        return SimplifyPath(path);
    }

    List<GridNode> SimplifyPath(Stack<GridNode> pathStack)
    {
        List<GridNode> simplifiedPath = new List<GridNode>();
        Vector2 oldDirection = Vector2.zero;
        GridNode curNode;

        while (pathStack.Count > 0)
        {
            curNode = pathStack.Pop();
            Vector2 curDirection = Vector2.zero;

            if (pathStack.Count != 0) //last
            {
                GridNode nextNode = pathStack.Peek();
                curDirection = new Vector2(nextNode.gridX - curNode.gridX, nextNode.gridZ - curNode.gridZ);
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
