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
    public WaveManager waveManager;
    public GeneticAlgorithm ga;

    public bool isSimulating = false; //is simulating or generating initial population

    private bool autoEvolve = false;
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

    private object mapInstanceLock = new object();

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
	        bool allMapsFinishedWave =
	            mapInstances.All(
	                mapInstance =>
	                    mapInstance.state == Map.MapState.FinishedGame || mapInstance.state == Map.MapState.CompletedWave);
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

	                if (AutoEvolve)
	                    StartEvolve();
	            }
	        }
	    }
	    /*if (state == SceneState.GeneratingInitialPopulation)
	    {
	        if (allMapsFinishedGame)
	        {
                List<BuildDecisionsChromosome> initial = new List<BuildDecisionsChromosome>(mapInstances.Count);
	            initial.AddRange(mapInstances.Select(mapInstance => mapInstance.buildDecisionsChromosome));
	            ga.SetInitialPopulation(initial);
	            state = SceneState.FindingFitnessValues;
	            isSimulating = true;
                if (AutoEvolve)
                    StartEvolve();
	        }
	    }*/

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
        {
            for (int i = 0; i < decisionCount; i++)
            {
                mapInstance.buildDecisionsChromosome.Randomise(decisionCount);
            }
        }
        List<BuildDecisionsChromosome> initial = new List<BuildDecisionsChromosome>(mapInstances.Count);
        initial.AddRange(mapInstances.Select(mapInstance => mapInstance.buildDecisionsChromosome));
        ga.SetInitialPopulation(initial);
        state = SceneState.FindingFitnessValues;
        isSimulating = true;

    }

    public void StartEvolve()
    {
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

        waveManager.ResetWaves();
        waveManager.SpawnWave(0);
    }

    private void OnGAFinish()
    {
        Debug.Log("GA Finshed");
    }

    private void OnWaveFinished(int i)
    {
    }

    private void OnWaveStart(int count)
    {
        foreach (Map mapInstance in mapInstances)
            mapInstance.currentWaveEnemiesLeft = count;

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
}
