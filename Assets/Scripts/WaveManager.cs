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


    void Start ()
    {
        if(maps.Count == 0)
            maps = FindObjectsOfType<Map>().ToList();

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

    private IEnumerator DoWave(Wave wave)
    {
        foreach (Map map in maps)
        {
            for (waveSpawnIndex = 0; waveSpawnIndex < wave.count; waveSpawnIndex++)
            {
                map.SpawnEnemy(wave.type);
                yield return new WaitForSeconds(wave.spawnInterval);
            }
        }
        isInWave = false;
        waveIndex++;
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
}
