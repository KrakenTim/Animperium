using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// provides Buttons to open and import balancing table
/// </summary>
[UnityEditor.CustomEditor(typeof(PlayerPawnData))]
public class PlayerPawnDataEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Table"))
            ShowLinksInEditorMenu.OpenBalanceTable();

        if (GUILayout.Button("Import Table"))
            BalanceTableImport.UpdateTable();

        GUILayout.Space(5);

        base.OnInspectorGUI();
    }
}