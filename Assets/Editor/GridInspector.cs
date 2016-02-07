using System;
using UnityEngine;
using System.Collections;
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
            PathRequestManager.RequestPath(curMapScript.startNode, curMapScript.endNode, curMapScript.grid, PathResultCallback);

        }
    }

    private void PathResultCallback(PathNode[] pathNodes, bool b)
    {
        Map curMapScript = (Map)target;
        curMapScript.path = pathNodes.ToList();
        curMapScript.UpdatePathLineRenderer();
        SceneView.RepaintAll();
        EditorUtility.SetDirty(target);
        HandleUtility.Repaint();
    }
}
