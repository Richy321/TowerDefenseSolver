using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SceneController : MonoBehaviour 
{
    public enum DisplayMode
    {
        None,
        Lightweight,
        Full,
    }

    public static DisplayMode displayMode = DisplayMode.Full;

    public List<Map> mapInstances = new List<Map>();
    public WaveManager waveManager;

    // Use this for initialization
    void Start ()
    {
        mapInstances = FindObjectsOfType<Map>().ToList();
        if (!waveManager)
            waveManager = FindObjectOfType<WaveManager>();
    }
	
	// Update is called once per frame
	void Update () 
	{
	
	}

    public void StartSimulation()
    {
        waveManager.isActive = true;
    }
}
