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

    public bool autoEvolve = false;

    public float decisionFrequency = 2000.0f; //frequency of the AI decision making in seconds

    public float decisionTimeCounter =0;
    public int decisionTickCounter = 0;

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

	    bool allMapsFinishedWave = mapInstances.All(mapInstance => mapInstance.state == Map.MapState.FinishedGame || mapInstance.state == Map.MapState.CompletedWave);
	    if (allMapsFinishedWave)
	    {
	        if (waveManager.waveIndex == waveManager.waves.Count - 1)
	        {
	            foreach (Map mapInstance in mapInstances)
	                mapInstance.state = Map.MapState.FinishedGame;
	        }
	        else
	        {
	            waveManager.SpawnNextWave();
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
                //set fitness to population
                /*for (int i = 0; i < ga.population.Count; i++)
                {
                    //might not need this - passing by ref
                    ga.population[i].Fitness = mapInstances[i].buildDecisionsChromosome.Fitness;
                }*/

                if (autoEvolve)
	                StartEvolve();
	        }
	    }

	    if (state == SceneState.GeneratingInitialPopulation)
	    {
	        if (allMapsFinishedGame)
	        {
                List<BuildDecisionsChromosome> initial = new List<BuildDecisionsChromosome>(mapInstances.Count);
	            initial.AddRange(mapInstances.Select(mapInstance => mapInstance.buildDecisionsChromosome));
	            ga.SetInitialPopulation(initial);
	            state = SceneState.FindingFitnessValues;
	            isSimulating = true;
                StartEvolve();
	        }
	    }

	    if (decisionTimeCounter > decisionFrequency)
	    {
            //DecisionTick(decisionTickCounter++); - time based ticks disabled for the time being
	        decisionTimeCounter = 0;
	    }
	}

    public void StartGA()
    {
        //Create initial population
        isSimulating = false;
        decisionTimeCounter = 0.0f;
        decisionTickCounter = 0;
        state = SceneState.GeneratingInitialPopulation;

        foreach (Map mapInstance in mapInstances)
            mapInstance.MapStart();

        waveManager.SpawnWave(0);
    }

    private void StartEvolve()
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
    }


    private void OnWaveFinished(int i)
    {
        DecisionTick(decisionTickCounter++);
    }

    private void OnWaveStart(int count)
    {
        foreach (Map mapInstance in mapInstances)
            mapInstance.currentWaveEnemiesLeft = count;

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
}
