using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// provides Buttons to open and import localisation table
/// </summary>
[UnityEditor.CustomEditor(typeof(LocalisedText))]
public class LocalisedTextEditor : LocalisationEditorBase
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!Application.isPlaying)
        {
            if (GUILayout.Button("Fill Texts (English)"))
                LocalisedText.FillAllTextsWithBaseLocalisation(eLanguage.English);
            if (GUILayout.Button("Fill Texts (German)"))
                LocalisedText.FillAllTextsWithBaseLocalisation(eLanguage.German);
        }
    }
}