using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Linq;
using Assets.Scripts;
using System;

public class Map : MonoBehaviour, IMap
{
    public int nodeCountX;
    public int nodeCountZ;

    public GameObject nodePrefab;
    public ObjectPool objectPool;
    public GameObject enemyContainer;
    public GameObject towerContainer;

    public List<List<PathNode>> grid;
    public Material altMaterial;
    public Material startMaterial;
    public Material endMaterial;
    public PathNode startNode;
    public PathNode endNode;

    public List<PathNode> path = new List<PathNode>();
    public LineRenderer pathLine;

    public List<GameObject> enemies = new List<GameObject>(); 
    public List<GameObject> towers = new List<GameObject>(); 

    private static float nodeWidth;
    private static float nodeHeight;

    private int enemyReachesEndCount = 0;


    // Use this for initialization
    void Start ()
    {
        InitialiseTestTowerSetup();
        FindPath();
    }

    void Awake()
    {
        pathLine = GetComponent<LineRenderer>();
        BuildGrid();
    }
	
	// Update is called once per frame
	void Update ()
	{
	    //path = pathFinder.FindPath(startNode, endNode, grid);
	    //UpdatePathLineRenderer();
	}

    void OnDrawGizmos()
    {
        if (grid != null)
        {
            foreach (List<PathNode> list in grid)
            {
                foreach (PathNode pathNode in list)
                {
                    if (pathNode != null)
                    {
                        Gizmos.color = Color.grey;

                        if (path.Contains(pathNode))
                            Gizmos.color = Color.yellow;
                        if (!pathNode.walkable)
                            Gizmos.color = Color.magenta;
                        if (pathNode == startNode)
                            Gizmos.color = Color.green;
                        if (pathNode == endNode)
                            Gizmos.color = Color.red;
                        Gizmos.DrawCube(pathNode.transform.position, new Vector3(nodeWidth * 0.25f, 0.1f, nodeHeight * 0.25f));
                    }
                }
            }
        }
    }

    public void BuildGrid()
    {
        ClearGrid();
        grid = new List<List<PathNode>>(nodeCountX);
        for (int i = 0; i < nodeCountX; i++)
        {
            grid.Add(new List<PathNode>());
        }
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
                grid[x].Add(obj.GetComponent<PathNode>());
                isAltMaterial = !isAltMaterial;
            }
            if (nodeCountZ%2 == 0)
                isAltMaterial = !isAltMaterial;
        }


        startNode = grid[nodeCountX/2][nodeCountZ - 1];
        endNode = grid[nodeCountX/2][0];

        BuildPathNodeLinks();
    }

    public void BuildPathNodeLinks()
    {
        PathNode pathNode;
        for (int x = 0; x < nodeCountX; x++)
        {
            for (int z = 0; z < nodeCountZ; z++)
            {
                pathNode = grid[x][z];
                pathNode.gridX = x;
                pathNode.gridY = z;
                pathNode.walkable = true;
                if (z < nodeCountZ - 1)
                    pathNode.north = grid[x][z + 1];
                if (z > 0)
                    pathNode.south = grid[x][z - 1];
                if (x < nodeCountX - 1)
                    pathNode.east = grid[x + 1][z];
                if (x > 0)
                    pathNode.west = grid[x - 1][z];
            }
        }
    }

    public void ClearGrid()
    {
        if (grid != null && grid.Count >0 && grid[0].Count > 0)
        {
            for (int i = 0; i < nodeCountX; i++)
            {
                for (int j = 0; j < nodeCountZ; j++)
                {
                    if (grid[i][j])
                        DestroyImmediate(grid[i][j].gameObject);
                }
            }
        }
        List<GameObject> pathNodse = new List<GameObject>();
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            PathNode pathNode = child.GetComponent<PathNode>();
            if(pathNode != null)
                pathNodse.Add(child);
        }

        pathNodse.ForEach(o => DestroyImmediate(o));


        if (startNode)
            DestroyImmediate(startNode.gameObject);

        if(endNode)
            DestroyImmediate(endNode.gameObject);

        pathLine.SetVertexCount(0);      
    }

    public void FindPath()
    {
        PathRequestManager.Instance.pathFinder.FindPathImmediate(startNode, endNode, grid, out path);
        UpdatePathLineRenderer();
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

    public static List<Vector3> ConvertToVectorPath(List<PathNode> nodePath)
    {
        const float height = 0.2f;
        List<Vector3> path = new List<Vector3>(nodePath.Count + 2);

        path.Add(new Vector3(nodePath[0].gameObject.transform.localPosition.x, height, nodePath[0].transform.localPosition.z + nodeHeight * 0.5f));

        for (int i = 0; i < nodePath.Count; i++)
            path.Add(new Vector3(nodePath[i].gameObject.transform.localPosition.x, height, nodePath[i].transform.localPosition.z));

        path.Add(new Vector3(nodePath.Last().gameObject.transform.localPosition.x, height, nodePath.Last().transform.localPosition.z - nodeHeight * 0.5f));

        return path;
    }

    public void SpawnEnemy(EnemyType type)
    {
        GameObject enemy = objectPool.GetEnemy(type);
        enemy.transform.parent = enemyContainer.transform;
        enemy.transform.position = new Vector3(startNode.transform.position.x, enemy.transform.position.y, startNode.transform.position.z); //keep height

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.path = ConvertToVectorPath(path);
        
        enemies.Add(enemy);
        enemyScript.onReachedEnd += OnEnemyReachesEnd;
        enemyScript.onDied += OnEnemyDied;

        enemyScript.Go();
    }

    public bool GetPath()
    {
        return PathRequestManager.Instance.pathFinder.FindPathImmediate(startNode, endNode, grid, out path);
    }

    private void OnEnemyReachesEnd(GameObject obj)
    {
        obj.GetComponent<Enemy>().onReachedEnd -= OnEnemyReachesEnd;
        enemies.Remove(obj);
        obj.transform.parent = null;
        objectPool.ReleaseEnemy(obj);
        enemyReachesEndCount++;
        Debug.Log("Enemy Reached End");
    }

    private void OnEnemyDied(GameObject obj)
    {
        obj.GetComponent<Enemy>().onDied -= OnEnemyDied;
        enemies.Remove(obj);
        obj.transform.parent = null;
        objectPool.ReleaseEnemy(obj);
        Debug.Log("Enemy Died");
    }

    public void AddTower(int rowIndex, int colIndex, TowerType type)
    {
        if (grid[rowIndex][colIndex].walkable)
        {
            GameObject tower = objectPool.GetTower(type);
            towers.Add(tower);
            tower.transform.parent = towerContainer.transform;
            tower.transform.position = grid[rowIndex][colIndex].gameObject.transform.position;
            grid[rowIndex][colIndex].walkable = false;
            tower.SetActive(true);
        }
    }

    public void RemoveTower(int rowIndex, int colIndex)
    {
        grid[rowIndex][colIndex].walkable = true;
        objectPool.ReleaseTower(grid[rowIndex][colIndex].gameObject);
    }

    public void ClearTowers()
    {
        foreach (GameObject tower in towers)
        {
            objectPool.ReleaseTower(tower);
        }
        foreach (List<PathNode> pathNodes in grid)
        {
            foreach (var pathNode in pathNodes)
            {
                pathNode.walkable = true;
            }
        }
    }

    public void SetTowers(TowerType[,] towerLayout)
    {
        throw new NotImplementedException();
    }



    public void InitialiseTestTowerSetup()
    {
        ClearTowers();
        AddTower(0,0, TowerType.SingleDamage);
        AddTower(5,5, TowerType.SingleDamage);
        AddTower(5,6, TowerType.SingleDamage);
        AddTower(5, 7, TowerType.SingleDamage);
        AddTower(5, 8, TowerType.SingleDamage);
    }
}
