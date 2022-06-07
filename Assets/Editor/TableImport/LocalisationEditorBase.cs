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

        if(Application.isPlaying)
        {
            if (GUILayout.Button("Switch To English"))
                Localisation.Instance.SetLanguage(eLanguage.English);
            if (GUILayout.Button("Switch To German"))
                Localisation.Instance.SetLanguage(eLanguage.German);
        }
    }
}
