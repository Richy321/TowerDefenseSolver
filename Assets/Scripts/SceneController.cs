using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;

public class SceneController : MonoBehaviour 
{
    public enum DisplayMode
    {
        None,
        Lightweight,
        Full,
    }

    public enum SceneState
    {
        Idle,
        GeneratingInitialPopulation,
        FindingFitnessValues,
        Finished
    }

    public SceneState state;

    public static DisplayMode displayMode = DisplayMode.Full;

    public List<Map> mapInstances = new List<Map>();
    public GameObject mapPrefab;

    public WaveManager waveManager;
    public GeneticAlgorithm ga;

    public int generateMapsCount = 16;

    public bool isSimulating = false; //is simulating or generating initial population

    private bool autoEvolve = true;
    public bool AutoEvolve
    {
        get { return autoEvolve; }
        set { autoEvolve = value; }
    }

    public float decisionFrequency = 2000.0f; //frequency of the AI decision making in seconds

    public float decisionTimeCounter;
    public int decisionTickCounter;

    [Range(1.0f, 50.0f)]
    public float timeScale = 1.0f;

    public float TimerScale { get { return timeScale;} set { timeScale = value; UpdateTimeScale(); } }

    public int endCaseSolutionsFound = int.MaxValue;
    public int endCaseGenerations = int.MaxValue;
    public int endCaseFitnessValue = int.MaxValue;

    public List<BuildDecisionsChromosome> solutions = new List<BuildDecisionsChromosome>();

    public ResultStats resultsStats = new ResultStats();

    private object mapInstanceLock = new object();

    public int RemainingEnemies
    {
        get
        {
            return mapInstances.Sum(mapInstance => mapInstance.currentWaveEnemiesLeft);
        }
    }

    // Use this for initialization
    void Start ()
    {
        mapInstances = FindObjectsOfType<Map>().ToList();
        if (!waveManager)
            waveManager = FindObjectOfType<WaveManager>();
        waveManager.OnWaveFinished += OnWaveFinished;
        waveManager.OnWaveStart += OnWaveStart; 
        ga = GetComponent<GeneticAlgorithm>();

        state = SceneState.Idle;
    }

	void Update ()
	{
	    if (state == SceneState.Idle)
	        return;

	    lock (mapInstanceLock)
	    {
	        bool allMapsFinishedWave = true;
            foreach (Map mapInstance in mapInstances)
            {
                if (mapInstance.state != Map.MapState.CompletedWave && mapInstance.state != Map.MapState.FinishedGame)
                {
                    allMapsFinishedWave = false;
                    break;
                }
            }

	        if (allMapsFinishedWave)
	        {
	            if (waveManager.waveIndex < waveManager.waves.Count - 1)
	            {
	                foreach (Map mapInstance in mapInstances)
	                    mapInstance.MapStartWave();
	                waveManager.SpawnNextWave();
	            }
	            else
	            {
	                foreach (Map mapInstance in mapInstances)
	                    mapInstance.MapFinish();
	            }
	        }

	        bool allMapsFinishedGame = mapInstances.All(mapInstance => mapInstance.state == Map.MapState.FinishedGame);

	        if (state == SceneState.FindingFitnessValues || state == SceneState.GeneratingInitialPopulation)
	        {
	            decisionTimeCounter += Time.deltaTime;
	        }

	        if (state == SceneState.FindingFitnessValues)
	        {
	            if (allMapsFinishedGame)
	            {
	                state = SceneState.Idle;

                    //check for solutions
	                foreach (Map mapInstance in mapInstances)
	                {
	                    if (mapInstance.lives > 0)
	                        if (solutions.Count == 0)
	                            resultsStats.firstSolutionGeneration = ga.generationNo;
	                        solutions.Add(mapInstance.buildDecisionsChromosome);
	                }

                    if (AutoEvolve)
	                    StartEvolve();
	            }
	        }
	    }

	    if (decisionTimeCounter > decisionFrequency)
	    {
            //DecisionTick(decisionTickCounter++); - time based ticks disabled for the time being
	        decisionTimeCounter = 0;
	    }

        //end cases
	    if (ga.generationNo >= endCaseGenerations)
	    {
	        OnGAFinish();
	    }
        else if (solutions.Count > endCaseSolutionsFound)
        {
            OnGAFinish();
        }
        else if (ga.highestFitness >= endCaseFitnessValue)
        {
            OnGAFinish();
        }
	}

    public void StartGA()
    {
        //Create initial population
        isSimulating = false;
        decisionTimeCounter = 0.0f;
        decisionTickCounter = 0;

        state = SceneState.GeneratingInitialPopulation;

        InitialiseGAPopulation(waveManager.waves.Count * 2); //at the moment 
        state = SceneState.FindingFitnessValues;

        foreach (Map mapInstance in mapInstances)
            mapInstance.MapStart();

        waveManager.SpawnWave(0);
    }

    private void InitialiseGAPopulation(int decisionCount)
    {
        foreach (Map mapInstance in mapInstances)
            mapInstance.buildDecisionsChromosome.Randomise(decisionCount);

        List<BuildDecisionsChromosome> initial = new List<BuildDecisionsChromosome>(mapInstances.Count);
        initial.AddRange(mapInstances.Select(mapInstance => mapInstance.buildDecisionsChromosome));
        ga.SetInitialPopulation(initial);
        state = SceneState.FindingFitnessValues;
        isSimulating = true;
    }

    public void StartEvolve()
    {
        //add stats for last generation
        resultsStats.AddGenerationStats(ga.generationNo, ga.population);

        waveManager.ResetWaves();
        ga.EvolvePopulation();
        for (int i = 0; i < ga.population.Count; i++)
        {
            mapInstances[i].Reset();
            mapInstances[i].buildDecisionsChromosome = ga.population[i];
        }
        decisionTimeCounter = 0.0f;
        decisionTickCounter = 0;
        state = SceneState.FindingFitnessValues;

        foreach (Map mapInstance in mapInstances)
            mapInstance.MapStart();

        waveManager.SpawnWave(0);
    }

    private void OnGAFinish()
    {
        Debug.Log("GA Finished");
        resultsStats.AppendUsageStatsToLog(solutions);
    }

    private void OnWaveFinished(int i)
    {
    }

    private void OnWaveStart(int count)
    {
        DecisionTick(decisionTickCounter++);
        DecisionTick(decisionTickCounter++);
    }

    private void DecisionTick(int i)
    {
        foreach (Map mapInstance in mapInstances)
        {
            if(isSimulating)
                mapInstance.SimulateDecisionTick(i);
            else
                mapInstance.GenerateDecisionTick(i);
        }
    }

    public void UpdateTimeScale()
    {
        Time.timeScale = timeScale;
    }

    public void GenerateMaps()
    {
        GenerateMaps(generateMapsCount);
    }

    private void GenerateMaps(int count)
    {
        int sqrtWhole = (int)Math.Floor(Mathf.Sqrt(count));
        int remainder = count - sqrtWhole*sqrtWhole;

        float gapX = 5.0f;
        float gapY = 5.0f;

        float mapWidth = 20.0f;
        float mapHeight = 20.0f;

        for (int i = 0; i <= sqrtWhole; i++)
        {
            int zCount = i == sqrtWhole ? remainder : sqrtWhole;
            for (int j = 0; j < zCount; j++)
            {
                GameObject map = GameObject.Instantiate(mapPrefab);
                map.name = "Map (" + j + "," + i + ")";
                map.transform.position = new Vector3(j * (mapWidth + gapX), 0.0f, i * (mapHeight + gapY));
            }
        }
    }
}
