using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;


[CustomEditor(typeof(Map))]
public class GridInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Map curMapScript = (Map) target;

        if (GUILayout.Button("Build Grid"))
        {
            curMapScript.BuildGrid();
        }

        if (GUILayout.Button("Clear Grid"))
        {
            curMapScript.ClearGrid();
        }

        if (GUILayout.Button("Find Path"))
        {
            curMapScript.FindPath();
        }

        if (GUILayout.Button("Fixed Path"))
        {
            curMapScript.GenerateFixedPath();
        }

        if (GUILayout.Button("Test Tower Layout"))
        {
            curMapScript.CreateRandomTowerLayout();
        }

        if (GUILayout.Button("Randomise Towers"))
        {
            curMapScript.CreateRandomTowerLayout();
        }

        if (GUILayout.Button("Clear Towers"))
        {
            curMapScript.ClearTowers();
        }
    }

    private void PathResultCallback(GridNode[] pathNodes, bool b)
    {
        Map curMapScript = (Map)target;
        curMapScript.path = pathNodes.ToList();
        curMapScript.UpdatePathLineRenderer();
    }
}
