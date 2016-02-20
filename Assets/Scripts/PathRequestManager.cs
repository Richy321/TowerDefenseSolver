using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;

public class PathRequestManager : MonoBehaviour
{

    public Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    public PathRequest currentPathRequest;
    public PathFinder pathFinder;
    public bool isProcessing;

    private static PathRequestManager instance;
    public static PathRequestManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PathRequestManager>();
            return instance;
        }
    }

    public static void RequestPath(GridNode pathStart, GridNode pathEnd, List<List<GridNode>> pathGrid, Action<GridNode[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, pathGrid, callback);

        Instance.pathRequestQueue.Enqueue(newRequest);
        Instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessing && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessing = true;
            pathFinder.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, currentPathRequest.pathGrid);
        }
    }

    public void FinishedProcessingPath(GridNode[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessing = false;
        TryProcessNext();
    }

    public struct PathRequest
    {
        public GridNode pathStart;
        public GridNode pathEnd;
        public List<List<GridNode>> pathGrid;
        public Action<GridNode[], bool> callback;

        public PathRequest(GridNode start, GridNode end, List<List<GridNode>> grid, Action<GridNode[], bool> callback)
        {
            pathStart = start;
            pathEnd = end;
            pathGrid = grid;
            this.callback = callback;
        }

    }


}
