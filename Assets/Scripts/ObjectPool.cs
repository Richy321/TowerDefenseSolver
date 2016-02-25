using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public GameObject fastEnemyPrefab;
    public GameObject strongEnemyPrefab;

    public GameObject singleTowerPrefab;
    public GameObject splashTowerPrefab;
    public GameObject slowTowerPrefab;

    public GameObject EnemyContainer;
    public GameObject TowerContainer;

    public int towerInitialiseCount = 20;
    public int enemyInitialiseCount = 20;

    public Vector3 offscreenHoldingPoint = new Vector3(1000f,1000f,1000f);

    public class Pool
    {
        public List<GameObject> active = new List<GameObject>();
        public List<GameObject> inactive = new List<GameObject>();
    }

    public Dictionary<TowerType, Pool> TowerPools = new Dictionary<TowerType, Pool>();
    public Dictionary<EnemyType, Pool> EnemyPools = new Dictionary<EnemyType, Pool>();

    public Dictionary<TowerType, GameObject> TowerPrefabs = new Dictionary<TowerType, GameObject>(); 

    public int CheapestTowerCost
    {
        get
        {
            int minCost = int.MaxValue;
            foreach (GameObject towerType in TowerPrefabs.Values)
            {
                BaseTower tower = towerType.GetComponent<BaseTower>();
                minCost = Math.Min(tower.resourceCost, minCost);
            }
            return minCost;
        }
    }

    // Use this for initialization
    void Awake ()
    {
        TowerPools.Clear();

        foreach (TowerType towerType in Enum.GetValues(typeof (TowerType)))
        {
            if (towerType == TowerType.None)
                continue;
            TowerPrefabs.Add(towerType, null);
        }
        //Todo custom dictionary inspector to assign these directly
        TowerPrefabs[TowerType.SingleDamage] = singleTowerPrefab;
        TowerPrefabs[TowerType.Slow] = slowTowerPrefab;
        TowerPrefabs[TowerType.SplashDamage] = splashTowerPrefab;

        foreach (TowerType towerType in Enum.GetValues(typeof (TowerType)))
        {
            if (towerType == TowerType.None)
                continue;
            TowerPools.Add(towerType, new Pool());

            for (int i = 0; i < towerInitialiseCount; i++)
                TowerPools[towerType].inactive.Add(CreateTower(towerType));            
        }


        foreach (EnemyType enemyType in Enum.GetValues(typeof (EnemyType)))
        {
            EnemyPools.Add(enemyType, new Pool());
            for (int i = 0; i < enemyInitialiseCount; i++)
                EnemyPools[enemyType].inactive.Add(CreateEnemy(enemyType));
        }
    }

    GameObject CreateTower(TowerType type)
    {
        if (type == TowerType.None)
            throw new UnityException("Create Tower was passed TowerType.None...");

        GameObject tower = Instantiate(TowerPrefabs[type]);
        tower.transform.parent = TowerContainer.transform;
        tower.SetActive(false);
        return tower;
    }

    GameObject CreateEnemy(EnemyType type)
    {
        GameObject enemy;

        switch (type)
        {
            case EnemyType.Fast:
                enemy = Instantiate(fastEnemyPrefab);
                break;
            case EnemyType.Strong:
                enemy = Instantiate(strongEnemyPrefab);
                break;
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }

        enemy.transform.parent = EnemyContainer.transform;
        enemy.SetActive(false);
        return enemy;
    }

    public GameObject GetEnemy(EnemyType type)
    {
        if (EnemyPools.ContainsKey(type))
        {
            Pool pool = EnemyPools[type];
            if (pool.inactive.Count <= 0)
            {
                pool.inactive.Add(CreateEnemy(type));
                Debug.LogWarning("Warning! - Had to create new Enemy, inactive list was empty");
            }

            GameObject obj = pool.inactive[0];
            pool.inactive.RemoveAt(0);
            pool.active.Add(obj);
            obj.SetActive(true);
            return obj;
        }
        else
        {
            Debug.LogError("Asked for enemy type that doesn't exist in the pool");
            return null;
        }
    }

    public GameObject GetTower(TowerType type)
    {
        if (!TowerPools.ContainsKey(type))
        {
            Debug.LogWarning("Warning! - Asked for enemy type that doesn't exist in the pool - Creating on the fly");
            TowerPools.Add(type, new Pool());
            for (int i = 0; i < towerInitialiseCount; i++)
                TowerPools[type].inactive.Add(CreateTower(type));
        }

        Pool pool = TowerPools[type];
        if (pool.inactive.Count <= 0)
        {
            pool.inactive.Add(CreateTower(type));
            Debug.LogWarning("Warning! - Had to create new Tower on the fly, inactive list was empty");
        }

        GameObject obj = pool.inactive[0];
        pool.inactive.RemoveAt(0);
        pool.active.Add(obj);
        obj.SetActive(true);
        return obj;
    }

    public void ReleaseEnemy(GameObject obj)
    {
        Enemy enemy = obj.GetComponent<Enemy>();
        EnemyPools[enemy.type].active.Remove(obj);
        EnemyPools[enemy.type].inactive.Add(obj);
        obj.SetActive(false);

        obj.transform.parent = EnemyContainer.transform;
        obj.transform.position = offscreenHoldingPoint;
    }

    public void ReleaseTower(GameObject obj)
    {
        BaseTower tower = obj.GetComponent<BaseTower>();
        TowerPools[tower.type].active.Remove(obj);
        TowerPools[tower.type].inactive.Add(obj);
        obj.SetActive(false);

        obj.transform.parent = TowerContainer.transform;
        obj.transform.position = offscreenHoldingPoint;
    }
}
