using UnityEngine;
using System.Collections;

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

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void BuildGrid()
    {
        ClearGrid();
        grid = new PathNode[nodeCountX, nodeCountZ];

        MeshFilter nodeMesh = nodePrefab.GetComponent<MeshFilter>();

        float nodeWidth = nodeMesh.sharedMesh.bounds.size.x;
        float nodeHeight = nodeMesh.sharedMesh.bounds.size.y;
        bool isAltMaterial = false;

        for (int x = 0; x < nodeCountX; x++)
        {
            for (int z = 0; z < nodeCountZ; z++)
            {
                Vector3 pos = new Vector3(x * nodeWidth, 0.0f, z * nodeHeight);
                GameObject obj = Instantiate(nodePrefab, pos, nodePrefab.transform.localRotation) as GameObject;
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

        PathNode midTop = grid[nodeCountX / 2, nodeCountZ - 1].GetComponent<PathNode>();
        PathNode midBottom = grid[nodeCountX / 2, 0].GetComponent<PathNode>();

        Vector3 startPos = new Vector3(midTop.transform.position.x, 0.0f, midTop.transform.position.z + nodeHeight);
        GameObject startNodeGO = Instantiate(nodePrefab, startPos, nodePrefab.transform.localRotation) as GameObject;
        startNodeGO.transform.parent = transform;
        startNode = startNodeGO.GetComponent<PathNode>();
        MeshRenderer meshRenderer = startNodeGO.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = startMaterial;

        Vector3 endPos = new Vector3(midBottom.transform.position.x, 0.0f, midBottom.transform.position.z - nodeHeight);
        GameObject endNodeGO = Instantiate(nodePrefab, endPos, nodePrefab.transform.localRotation) as GameObject;
        endNode = endNodeGO.GetComponent<PathNode>();
        endNodeGO.transform.parent = transform;
        meshRenderer = endNodeGO.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = endMaterial;

        BuildPathNodeLinks(midTop, midBottom);
    }

    public void BuildPathNodeLinks(PathNode midTop, PathNode midBottom)
    {
        PathNode pathNode;
        for (int x = 0; x < nodeCountX; x++)
        {
            for (int z = 0; z < nodeCountZ; z++)
            {
                pathNode = grid[x, z];
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

        pathNode = startNode.GetComponent<PathNode>();
        
        pathNode.south = midTop;
        midTop.north = pathNode;

        pathNode = endNode.GetComponent<PathNode>();
        pathNode.north = midBottom;
        midBottom.south = pathNode;
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
    }
}
