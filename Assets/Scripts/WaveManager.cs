using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WaveManager : MonoBehaviour
{
    public class Wave
    {
        public EnemyType type;
        public int count;
        public float spawnInterval;
    }

    public bool isActive;
    public bool isInWave;
    public int waveIndex;
    public int waveSpawnIndex;
    public List<Map> maps = new List<Map>();
    public List<Wave> waves = new List<Wave>();

    public Action<int> OnWaveFinished;
    public Action<int> OnWaveStart; 
     
    private static WaveManager instance;

    public static WaveManager Instance
    {
        get
        {
            if (!instance)
                instance = FindObjectOfType<WaveManager>();
            return instance;
        }
    }

    void Start ()
    {
        if(maps.Count == 0)
            maps = FindObjectsOfType<Map>().ToList();

        waves.Add(CreateTestWave());
        waves.Add(CreateTestWave());
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (isActive && !isInWave)
	    {
	        if (waveIndex < waves.Count)
	            SpawnWave(waveIndex);
        }
	}

    public void SpawnWave(int i)
    {
        isInWave = true;
        StartCoroutine("DoWave", waves[waveIndex]);
    }

    public void SpawnNextWave()
    {
        if (waveSpawnIndex != waves.Count - 1)
            SpawnWave(++waveIndex);
    }


    private IEnumerator DoWave(Wave wave)
    {
        if (OnWaveStart != null) OnWaveStart(wave.count);
        for (waveSpawnIndex = 0; waveSpawnIndex < wave.count; waveSpawnIndex++)
        {
            foreach (Map map in maps)
            {
                if(map.state != Map.MapState.FinishedGame)
                    map.SpawnEnemy(wave.type);
            }
            yield return new WaitForSeconds(wave.spawnInterval);
        }

        isInWave = false;
        if(OnWaveFinished != null) OnWaveFinished(waveIndex);
        yield return null;
    }


    private Wave CreateTestWave()
    {
        Wave test = new Wave();
        test.type = EnemyType.Fast;
        test.count = 10;
        test.spawnInterval = 2.0f;

        return test;
    }

    public int MaxEnemyCount
    {
        get
        {
            return waves.Sum(wave => wave.count);
        }
    }


    public void StartWaves()
    {
        isActive = true;
    }

    public void StopWaves()
    {
        isActive = false;
    }

    public void ResetWaves()
    {
        waveIndex = 0;
    }

}
