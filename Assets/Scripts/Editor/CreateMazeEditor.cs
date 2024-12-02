using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CreateMaze))]
public class CreateMazeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var createMaze = (CreateMaze)target;

        if (GUILayout.Button("çÏÇÈ"))
        {
            createMaze.Create();
        }
        if (GUILayout.Button("è¡Ç∑"))
        {
            createMaze.Clear();
        }
    }
}
