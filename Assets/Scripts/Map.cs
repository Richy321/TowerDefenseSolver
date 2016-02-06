using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
    public int nodeCountX;
    public int nodeCountZ;

    public GameObject nodePrefab;
    public PathNode[,] grid;
    public Material altMaterial;
    public Material startMaterial;
    public Material endMaterial;
    public PathNode startNode;
    public PathNode endNode;

    public PathFinder pathFinder = new PathFinder();
    public List<PathNode> path = new List<PathNode>();

    public LineRenderer pathLine;

    private float nodeWidth;
    private float nodeHeight;

    // Use this for initialization
    void Start ()
    {
        pathLine = GetComponent<LineRenderer>();
    }
	
	// Update is called once per frame
	void Update ()
	{
	    path = pathFinder.FindPath(startNode, endNode, grid);
	    UpdatePathLineRenderer();

	}

    void OnDrawGizmos()
    {
        if (grid != null && grid.Length > 0)
        {
            foreach (PathNode pathNode in grid)
            {
                if (pathNode != null)
                {
                    Gizmos.color = Color.grey;
                    Gizmos.DrawCube(pathNode.transform.position, new Vector3(nodeWidth*0.25f, 0.1f, nodeHeight*0.25f));
                    if (path.Contains(pathNode))
                        Gizmos.color = Color.yellow;
                    if (!pathNode.walkable)
                        Gizmos.color = Color.black;

                    if(pathNode == startNode)
                        Gizmos.color = Color.green;
                    if(pathNode == endNode)
                        Gizmos.color = Color.red;
                }
            }
        }
    }

    public void BuildGrid()
    {
        ClearGrid();
        grid = new PathNode[nodeCountX, nodeCountZ];

        MeshFilter nodeMesh = nodePrefab.GetComponent<MeshFilter>();

        nodeWidth = nodeMesh.sharedMesh.bounds.size.x;
        nodeHeight = nodeMesh.sharedMesh.bounds.size.y;
        bool isAltMaterial = false;

        for (int x = 0; x < nodeCountX; x++)
        {
            for (int z = 0; z < nodeCountZ; z++)
            {
                Vector3 pos = new Vector3(x * nodeWidth, 0.0f, z * nodeHeight);
                GameObject obj = Instantiate(nodePrefab, pos, nodePrefab.transform.localRotation) as GameObject;
                obj.name = "Grid " + x + "," + z;
                if (isAltMaterial)
                {
                    MeshRenderer objMesh = obj.GetComponent<MeshRenderer>();
                    objMesh.sharedMaterial = altMaterial;
                }

                obj.transform.SetParent(transform);
                grid[x, z] = obj.GetComponent<PathNode>();
                isAltMaterial = !isAltMaterial;
            }
            if (nodeCountZ%2 == 0)
                isAltMaterial = !isAltMaterial;
        }


        startNode = grid[nodeCountX/2, nodeCountZ - 1];
        endNode = grid[nodeCountX/2, 0];

        BuildPathNodeLinks();
    }

    public void BuildPathNodeLinks()
    {
        PathNode pathNode;
        for (int x = 0; x < nodeCountX; x++)
        {
            for (int z = 0; z < nodeCountZ; z++)
            {
                pathNode = grid[x, z];
                pathNode.gridX = x;
                pathNode.gridY = z;
                pathNode.walkable = true;
                if (z < nodeCountZ - 1)
                    pathNode.north = grid[x, z + 1];
                if (z > 0)
                    pathNode.south = grid[x, z - 1];
                if (x < nodeCountX - 1)
                    pathNode.east = grid[x + 1, z];
                if (x > 0)
                    pathNode.west = grid[x - 1, z];
            }
        }
    }

    public void ClearGrid()
    {
        if (grid != null && grid.Length > 0)
        {
            foreach (PathNode o in grid)
            {
                if(o)
                    DestroyImmediate(o.gameObject);
            }
        }

        if(startNode)
            DestroyImmediate(startNode.gameObject);

        if(endNode)
            DestroyImmediate(endNode.gameObject);

        pathLine.SetPositions(new Vector3[0]);
        pathLine.SetVertexCount(0);
    }


    public void UpdatePathLineRenderer()
    {
        const float lineHeight = 0.2f;
        pathLine.SetVertexCount(path.Count + 4);
        pathLine.SetPosition(0, new Vector3(startNode.gameObject.transform.localPosition.x, lineHeight, startNode.transform.localPosition.z + nodeHeight *0.5f));
        pathLine.SetPosition(1, new Vector3(startNode.gameObject.transform.localPosition.x, lineHeight, startNode.transform.localPosition.z));

        for (int i=0; i < path.Count; i++)
        {
            pathLine.SetPosition(i+2, new Vector3(path[i].gameObject.transform.localPosition.x, lineHeight, path[i].transform.localPosition.z));
        }

        pathLine.SetPosition(path.Count + 2, new Vector3(endNode.gameObject.transform.localPosition.x, lineHeight, endNode.transform.localPosition.z));
        pathLine.SetPosition(path.Count + 3, new Vector3(endNode.gameObject.transform.localPosition.x, lineHeight, endNode.transform.localPosition.z - nodeHeight * 0.5f));
    }
}
