using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Random = UnityEngine.Random;

public class Map : MonoBehaviour, IMap
{
    public int nodeCountX;
    public int nodeCountZ;

    public GameObject nodePrefab;
    public ObjectPool objectPool;
    public GameObject enemyContainer;
    public GameObject towerContainer;
    public GameObject geneContainer;

    public BuildDecisionsChromosome buildDecisionsChromosome;

    public List<List<GridNode>> grid;
    public Material altMaterial;
    public Material startMaterial;
    public Material endMaterial;
    public GridNode startNode;
    public GridNode endNode;

    public List<GridNode> path = new List<GridNode>();
    public LineRenderer pathLine;

    public List<GameObject> enemies = new List<GameObject>(); 
    public List<GameObject> towers = new List<GameObject>(); 

    private static float nodeWidth;
    private static float nodeHeight;

    public const int StartingResources = 15;
    public int resources = StartingResources;

    public const int StartingLives = 10;
    public int lives = StartingLives;

    private int resourcesCollected;

    public int currentWaveEnemiesLeft;

    public enum MapState
    {
        Idle,
        Running,
        CompletedWave,
        FinishedGame
    }
    public MapState state;

    // Use this for initialization
    void Start ()
    {
    }

    void Awake()
    {
        if (!buildDecisionsChromosome) buildDecisionsChromosome = GetComponent<BuildDecisionsChromosome>();
        if (!pathLine) pathLine = GetComponent<LineRenderer>();
        if (!objectPool) objectPool = FindObjectOfType<ObjectPool>();

        BuildGrid();
        state = MapState.Idle;
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
            foreach (List<GridNode> list in grid)
            {
                foreach (GridNode pathNode in list)
                {
                    if (pathNode != null)
                    {
                        Gizmos.color = Color.grey;

                        if (!pathNode.placeable) 
                            Gizmos.color = Color.blue;
                        if (!pathNode.walkable)
                            Gizmos.color = Color.magenta;
                        if (path.Contains(pathNode))
                            Gizmos.color = Color.yellow;
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

    #region Grid
    public void BuildGrid()
    {
        ClearGrid();
        grid = new List<List<GridNode>>(nodeCountX);
        for (int i = 0; i < nodeCountX; i++)
        {
            grid.Add(new List<GridNode>());
        }
        MeshFilter nodeMesh = nodePrefab.GetComponent<MeshFilter>();

        nodeWidth = nodeMesh.sharedMesh.bounds.size.x;
        nodeHeight = nodeMesh.sharedMesh.bounds.size.y;
        bool isAltMaterial = false;

        for (int z = 0; z < nodeCountZ; z++)
        {
            for (int x = 0; x < nodeCountX; x++)
            {
                Vector3 pos = transform.position + new Vector3(x * nodeWidth, 0.0f, z * nodeHeight);
                GameObject obj = Instantiate(nodePrefab, pos, nodePrefab.transform.localRotation) as GameObject;
                obj.name = "Grid " + x + "," + z;
                if (isAltMaterial)
                {
                    MeshRenderer objMesh = obj.GetComponent<MeshRenderer>();
                    objMesh.sharedMaterial = altMaterial;
                }

                obj.transform.SetParent(transform);
                grid[z].Add(obj.GetComponent<GridNode>());
                isAltMaterial = !isAltMaterial;
            }
            if (nodeCountZ%2 == 0)
                isAltMaterial = !isAltMaterial;
        }


        startNode = grid[nodeCountZ - 1][nodeCountX / 2];
        endNode = grid[0][nodeCountX / 2];

        BuildPathNodeLinks();
        GenerateFixedPath();
    }

    public void BuildPathNodeLinks()
    {
        GridNode pathNode;
        for (int z = 0; z < nodeCountZ; z++)
        {
            for (int x = 0; x < nodeCountX; x++)
            {
                pathNode = grid[z][x];
                pathNode.gridX = x;
                pathNode.gridZ = z;
                pathNode.walkable = true;
                if (z < nodeCountZ - 1)
                    pathNode.north = grid[z + 1][x];
                if (z > 0)
                    pathNode.south = grid[z - 1][x];
                if (x < nodeCountX - 1)
                    pathNode.east = grid[z][x + 1];
                if (x > 0)
                    pathNode.west = grid[z][x - 1];
            }
        }
    }

    public void ClearGrid()
    {
        if (grid != null && grid.Count >0 && grid[0].Count > 0)
        {
            for (int i = 0; i < grid[0].Count; i++)
            {
                for (int j = 0; j < grid.Count; j++)
                {
                    if (grid[i][j])
                        DestroyImmediate(grid[i][j].gameObject);
                }
            }
        }
        List<GameObject> pathNodes = new List<GameObject>();
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            GridNode pathNode = child.GetComponent<GridNode>();
            if(pathNode != null)
                pathNodes.Add(child);
        }

        pathNodes.ForEach(o => DestroyImmediate(o));


        if (startNode)
            DestroyImmediate(startNode.gameObject);

        if(endNode)
            DestroyImmediate(endNode.gameObject);

        pathLine.SetVertexCount(0);      
    }
    #endregion

    #region Pathing
    public
    void FindPath()
    {
        PathRequestManager.Instance.pathFinder.FindPathImmediate(startNode, endNode, grid, out path);
        UpdatePathLineRenderer();
    }

    public void GenerateFixedPath()
    {
        path.Clear();
        int xCoord = startNode.gridX;
        int zCoord = startNode.gridZ;

        try
        {
            while (true)
            {
                for (; xCoord <= nodeCountX - 2; xCoord++)
                {
                    path.Add(grid[zCoord][xCoord]);
                    if (zCoord == endNode.gridZ && xCoord == endNode.gridX)
                        return;
                }
                xCoord--;

                path.Add(grid[--zCoord][xCoord]);
                if (zCoord > 0)
                    path.Add(grid[--zCoord][xCoord--]);

                for (; xCoord >= 1; xCoord--)
                {
                    path.Add(grid[zCoord][xCoord]);
                    if (zCoord == endNode.gridZ && xCoord == endNode.gridX)
                        return;
                }
                xCoord++;

                path.Add(grid[--zCoord][xCoord]);
                if (zCoord > 0)
                    path.Add(grid[--zCoord][xCoord]);
            }
        }
        finally
        {
            path.ForEach(x => x.placeable = false);
            UpdatePathLineRenderer();
        }
    }
    public void UpdatePathLineRenderer()
    {
        const float lineOffset = 0.2f;
        pathLine.SetVertexCount(path.Count + 4);
        pathLine.SetPosition(0, new Vector3(startNode.gameObject.transform.position.x, startNode.gameObject.transform.position.y + lineOffset, startNode.transform.position.z + nodeHeight *0.5f));
        pathLine.SetPosition(1, new Vector3(startNode.gameObject.transform.position.x, startNode.gameObject.transform.position.y + lineOffset, startNode.transform.position.z));

        for (int i=0; i < path.Count; i++)
        {
            pathLine.SetPosition(i+2, new Vector3(path[i].gameObject.transform.position.x, path[i].gameObject.transform.position.y + lineOffset, path[i].transform.position.z));
        }

        pathLine.SetPosition(path.Count + 2, new Vector3(endNode.gameObject.transform.position.x, endNode.gameObject.transform.position.y + lineOffset, endNode.transform.position.z));
        pathLine.SetPosition(path.Count + 3, new Vector3(endNode.gameObject.transform.position.x, endNode.gameObject.transform.position.y + lineOffset, endNode.transform.position.z - nodeHeight * 0.5f));
    }

    public static List<Vector3> ConvertToVectorPath(List<GridNode> nodePath, float height)
    {
        List<Vector3> path = new List<Vector3>(nodePath.Count + 2);

        path.Add(new Vector3(nodePath[0].gameObject.transform.position.x, height, nodePath[0].transform.position.z + nodeHeight * 0.5f));

        for (int i = 0; i < nodePath.Count; i++)
            path.Add(new Vector3(nodePath[i].gameObject.transform.position.x, height, nodePath[i].transform.position.z));

        path.Add(new Vector3(nodePath.Last().gameObject.transform.position.x, height, nodePath.Last().transform.position.z - nodeHeight * 0.5f));

        return path;
    }

    public bool GetPath()
    {
        return PathRequestManager.Instance.pathFinder.FindPathImmediate(startNode, endNode, grid, out path);
    }
    #endregion

    #region Enemy
    public void SpawnEnemy(EnemyType type, float difficultyMultiplier)
    {
        if (state == MapState.Running)
        {
            GameObject enemy = objectPool.GetEnemy(type);
            enemy.transform.parent = enemyContainer.transform;

            Enemy enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.path = ConvertToVectorPath(path, enemy.transform.position.y); //keep height
            enemy.transform.position = enemyScript.path[0];

            enemyScript.health = enemyScript.startHealth * difficultyMultiplier;
            enemyScript.speed =  enemyScript.startSpeed * difficultyMultiplier;
            enemyScript.waveStartSpeed = enemyScript.speed;
            enemyScript.resourceReward = (int)(enemyScript.startResourceReward * difficultyMultiplier);

            enemies.Add(enemy);
            enemyScript.onReachedEnd += OnEnemyReachesEnd;
            enemyScript.onDied += OnEnemyDied;

            enemyScript.Go();
        }
    }

    private void OnEnemyReachesEnd(GameObject obj)
    {
        Enemy enemyScript = obj.GetComponent<Enemy>();
        enemyScript.Reset();
        enemyScript.onDied -= OnEnemyDied;
        enemyScript.onReachedEnd -= OnEnemyReachesEnd;

        currentWaveEnemiesLeft--;
        lives -= enemyScript.lifeDeduction;
        lives = Math.Max(0, lives);

        enemies.Remove(obj);
        obj.transform.parent = null;
        objectPool.ReleaseEnemy(obj);

        if (lives < 1)
            MapFinish();
        else if (currentWaveEnemiesLeft <= 0)
            state = MapState.CompletedWave;
    }

    private void OnEnemyDied(GameObject obj)
    {
        Enemy enemyScript = obj.GetComponent<Enemy>();
        enemyScript.Reset();
        enemyScript.onDied -= OnEnemyDied;
        enemyScript.onReachedEnd -= OnEnemyReachesEnd;

        resources += enemyScript.resourceReward;
        resourcesCollected += enemyScript.resourceReward;
        currentWaveEnemiesLeft--;

        enemies.Remove(obj);
        obj.transform.parent = null;
        objectPool.ReleaseEnemy(obj);

        if (currentWaveEnemiesLeft <= 0)
            state = MapState.CompletedWave;
    }

    #endregion

    #region Towers
    public void AddTower(int rowIndex, int colIndex, TowerType type, bool checkResources = false)
    {
        bool passedResourceCheck = true;
        if (checkResources)
        {
            if (objectPool.TowerPrefabs[type].GetComponent<BaseTower>().resourceCost > resources)
            {
                passedResourceCheck = false;
            }
        }

        if (passedResourceCheck && grid[rowIndex][colIndex].placeable && type != TowerType.None)
        {
            GameObject tower = objectPool.GetTower(type);
            towers.Add(tower);
            tower.transform.parent = towerContainer.transform;
            tower.transform.position = grid[rowIndex][colIndex].gameObject.transform.position;
            grid[rowIndex][colIndex].walkable = false;
            grid[rowIndex][colIndex].placeable = false;
            grid[rowIndex][colIndex].towerType = type;
            tower.SetActive(true);

            if(checkResources)
                resources -= objectPool.TowerPrefabs[type].GetComponent<BaseTower>().resourceCost;
        }
    }

    public void RemoveTower(int rowIndex, int colIndex)
    {
        grid[rowIndex][colIndex].walkable = true;
        grid[rowIndex][colIndex].placeable = true;
        grid[rowIndex][colIndex].towerType = TowerType.None;
        objectPool.ReleaseTower(grid[rowIndex][colIndex].gameObject);
    }

    public void ClearTowers()
    {
        foreach (GameObject tower in towers)
        {
            objectPool.ReleaseTower(tower);
        }
        towers.Clear();

        foreach (List<GridNode> pathNodes in grid)
        {
            foreach (var pathNode in pathNodes)
            {
                if (!path.Contains(pathNode))
                {
                    pathNode.placeable = true;
                    pathNode.towerType = TowerType.None;
                }
            }
        }
    }

    public void GetLayout()
    {
        
    }

    public void InitialiseTestTowerLayout()
    {
        ClearTowers();
        AddTower(0,0, TowerType.SingleDamage);
        AddTower(1,0, TowerType.SingleDamage);
        AddTower(2,0, TowerType.SingleDamage);
        AddTower(3,0, TowerType.SingleDamage);
        AddTower(4,0, TowerType.SingleDamage);
    }

    public void CreateRandomTowerLayout()
    {
        //mapChromosomes.Add(new MapChromosome());
        //mapChromosomes[0].Initialise();
        //mapChromosomes[0].Randomise();
        //SetTowers(mapChromosomes[0]);
    }

    #endregion

    #region Layouts
    public void SetTowers(MapChromosome towerLayout)
    {
        ClearTowers();
        for (int z = 0; z < towerLayout.chromosome.Count; z++)
        {
            for (int x = 0; x < towerLayout.chromosome[z].Count; x++)
            {
                AddTower(z, x, towerLayout.chromosome[z][x]);
            }
        }
    }

    public void SaveLayout()
    {
        /*MapChromosome chromosome = geneContainer.AddComponent<MapChromosome>();
        chromosome.map = this;
        chromosome.Initialise();

        for (int z = 0; z < grid.Count; z++)
        {
            for (int x = 0; x < grid[0].Count; x++)
            {
                chromosome.chromosome[z][x] = grid[z][x].towerType;
            }
        }
        mapChromosomes.Add(chromosome);*/
    }
    #endregion

    public void MapFinish()
    {
        buildDecisionsChromosome.Fitness = resourcesCollected + (lives*5);
        state = MapState.FinishedGame;
    }

    public void MapStart()
    {
        state = MapState.Running;   
    }

    public void MapStartWave()
    {
        if(state != MapState.FinishedGame)
            state = MapState.Running;
    }

    #region Genetic Algorithm functions
    public void SimulateDecisionTick(int tickNumber)
    {
        if (buildDecisionsChromosome.buildDecisionGenes.Count > tickNumber)
        {
            TowerType towerType = buildDecisionsChromosome.buildDecisionGenes[tickNumber].towerType;
            if (towerType != TowerType.None)
            {
                int gridX = buildDecisionsChromosome.buildDecisionGenes[tickNumber].gridXCoord;
                int gridZ = buildDecisionsChromosome.buildDecisionGenes[tickNumber].gridZCoord;

                AddTower(gridZ, gridX, towerType, true);
            }
        }
    }

    public void Reset()
    {
        resources = StartingResources;
        lives = StartingLives;
        ClearTowers();
        for (int i = enemies.Count-1; i > 0; i--)
            enemies[i].GetComponent<Enemy>().onDied(enemies[i]);

        state = MapState.Idle;
    }
    #endregion

    #region Utils
    public static TowerType GetRandomTowerType()
    {
        return (TowerType)Random.Range(0, Enum.GetValues(typeof(TowerType)).Length);
    }

    public GridNode GetRandomNode()
    {
        return grid[Random.Range(0, grid.Count)][Random.Range(0, grid[0].Count)];
    }

    public TowerType GetRandomAffordableTowerType()
    {
        List<TowerType> affordableTypes = (from keyValuePair in objectPool.TowerPrefabs
                                           let towerScript = keyValuePair.Value.GetComponent<BaseTower>()
                                           where towerScript.resourceCost <= resources
                                           select keyValuePair.Key).ToList();

        if(affordableTypes.Count ==0)
            return TowerType.None;

        return affordableTypes[Random.Range(0, affordableTypes.Count)];
    }

    public GridNode GetRandomPlaceableNode()
    {
        List<GridNode> availableGridNodes = new List<GridNode>();

        foreach (List<GridNode> t in grid)
        {
            for (int x = 0; x < grid[0].Count; x++)
            {
                if (t[x].towerType == TowerType.None && t[x].placeable)
                    availableGridNodes.Add(t[x]);
            }
        }

        return availableGridNodes[Random.Range(0, availableGridNodes.Count)];
    }
    #endregion
}
