using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// provides Button to clear all stored data
/// </summary>
[UnityEditor.CustomEditor(typeof(PersistingMatchData))]
public class PersistentMatchDataEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Clear All Data"))
            ((PersistingMatchData)target).ResetAll();

        GUILayout.Space(5);

        base.OnInspectorGUI();
    }
}
