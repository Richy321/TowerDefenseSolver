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
            SceneView.RepaintAll();
            EditorUtility.SetDirty(target);
        }


        if (GUILayout.Button("Test Tower Layout"))
        {
            curMapScript.CreateRandomTowerLayout();
            SceneView.RepaintAll();
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Randomise Towers"))
        {
            curMapScript.CreateRandomTowerLayout();
            SceneView.RepaintAll();
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Clear Towers"))
        {
            curMapScript.ClearTowers();
            SceneView.RepaintAll();
            EditorUtility.SetDirty(target);
        }

    }

    private void PathResultCallback(PathNode[] pathNodes, bool b)
    {
        Map curMapScript = (Map)target;
        curMapScript.path = pathNodes.ToList();
        curMapScript.UpdatePathLineRenderer();
    }
}
