using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space(25);
        if (GUILayout.Button("Automatic Victory"))
        {
            GameManager manager = target as GameManager;
            manager.RunVictorySequence();
        }
    }
}