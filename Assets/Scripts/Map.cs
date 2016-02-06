using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour
{
    public int nodeCountX;
    public int nodeCountZ;

    public GameObject nodePrefab;
    public GameObject[,] grid;
    public Material altMaterial;
    public Material startMaterial;
    public Material endMaterial;
    public GameObject startNode;
    public GameObject endNode;

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
        grid = new GameObject[nodeCountX, nodeCountZ];

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
                grid[x, z] = obj;
                isAltMaterial = !isAltMaterial;
            }
        }

        Vector3 startPos = new Vector3((nodeCountX * nodeWidth) * 0.5f - (nodeWidth * 0.5f), 0.0f, nodeCountZ * nodeHeight);
        startNode = Instantiate(nodePrefab, startPos, nodePrefab.transform.localRotation) as GameObject;
        startNode.transform.parent = transform;
        MeshRenderer meshRenderer = startNode.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = startMaterial;

        Vector3 endPos = new Vector3((nodeCountX * nodeWidth) * 0.5f - (nodeWidth * 0.5f), 0.0f, -nodeHeight);
        endNode = Instantiate(nodePrefab, endPos, nodePrefab.transform.localRotation) as GameObject;
        endNode.transform.parent = transform;
        meshRenderer = endNode.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = endMaterial;


        BuildPathNodeLinks();
    }

    public void BuildPathNodeLinks()
    {
        PathNode pathNode;
        for (int x = 0; x < nodeCountX; x++)
        {
            for (int z = 0; z < nodeCountZ; z++)
            {
                pathNode = grid[x, z].GetComponent<PathNode>();
                pathNode.walkable = true;
                if (z < nodeCountZ - 1)
                    pathNode.north = grid[x, z + 1].GetComponent<PathNode>();
                if (z > 0)
                    pathNode.south = grid[x, z - 1].GetComponent<PathNode>();
                if (x < nodeCountX - 1)
                    pathNode.east = grid[x + 1, z].GetComponent<PathNode>();
                if (x > 0)
                    pathNode.west = grid[x - 1, z].GetComponent<PathNode>();
            }
        }

        pathNode = startNode.GetComponent<PathNode>();
        PathNode midTop = grid[nodeCountX / 2, nodeCountZ-1].GetComponent<PathNode>();
        pathNode.south = midTop;
        midTop.north = pathNode;

        pathNode = endNode.GetComponent<PathNode>();
        PathNode midBottom = grid[nodeCountX/2, 0].GetComponent<PathNode>();
        pathNode.north = midBottom;
        midBottom.south = pathNode;
    }

    public void ClearGrid()
    {
        if (grid != null && grid.Length > 0)
        {
            foreach (GameObject o in grid)
            {
                DestroyImmediate(o);
            }
        }

        if(startNode)
            DestroyImmediate(startNode);

        if(endNode)
            DestroyImmediate(endNode);
    }
}
