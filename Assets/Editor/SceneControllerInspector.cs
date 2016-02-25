﻿using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    [CustomEditor(typeof (SceneController))]
    class SceneControllerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            SceneController curScript = (SceneController) target;

            if (GUILayout.Button("Start Simulation"))
            {
                curScript.StartSimulation();
            }
        }
    }
}