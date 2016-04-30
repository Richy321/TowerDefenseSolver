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
        public float difficultyMultiplier = 1.0f;
    }

    public bool isActive;
    public bool isInWave;
    public int waveIndex;
    public int waveSpawnIndex;
    public List<Map> maps = new List<Map>();
    public List<Wave> waves = new List<Wave>();

    public Action<int> OnWaveFinished;
    public Action<int> OnWaveStart; 
     
    public float difficultyModifier = 1.5f;
    

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

        CreateIncreasingDifficultyWaves(10);
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
        StopCoroutine("DoWave");
        StartCoroutine("DoWave", waves[waveIndex]);
    }

    public void SpawnNextWave()
    {
        if (waveSpawnIndex != waves.Count - 1)
            SpawnWave(++waveIndex);
    }


    private IEnumerator DoWave(Wave wave)
    {
        foreach (Map mapInstance in maps)
            mapInstance.currentWaveEnemiesLeft = wave.count;

        if (OnWaveStart != null) OnWaveStart(wave.count);

        for (waveSpawnIndex = 0; waveSpawnIndex < wave.count; waveSpawnIndex++)
        {
            foreach (Map map in maps)
            {
                if(map.state != Map.MapState.FinishedGame)
                    map.SpawnEnemy(wave.type, wave.difficultyMultiplier);
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

    private void CreateIncreasingDifficultyWaves(int count)
    {
        float difficultyMultiplier = 1.0f;

        for (int i = 0; i < count; i++)
        {
            Wave wave = new Wave();
            wave.type = EnemyType.Fast;
            wave.count = (int)(10 * difficultyMultiplier);
            wave.spawnInterval = 2.0f;
            wave.difficultyMultiplier = difficultyMultiplier;
            difficultyMultiplier += difficultyModifier;
            waves.Add(wave);
        }
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
