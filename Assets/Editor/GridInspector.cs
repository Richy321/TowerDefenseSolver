using UnityEngine;
using System.Collections;
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
            curMapScript.path = curMapScript.pathFinder.FindPath(curMapScript.startNode, curMapScript.endNode, curMapScript.grid);
            curMapScript.UpdatePathLineRenderer();
        }
    }
}
