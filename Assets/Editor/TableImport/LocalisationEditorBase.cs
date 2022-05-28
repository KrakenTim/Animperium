using UnityEditor;
using UnityEngine;

public class LocalisationEditorBase : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(5);

        if (GUILayout.Button("Open Table"))
            ShowLinksInEditorMenu.OpenLocalisationTable();

        if (GUILayout.Button("Import Table"))
            LocalisationImport.UpdateTable();
    }
}
