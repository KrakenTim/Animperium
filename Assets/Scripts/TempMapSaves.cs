using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Should be placed on same GameObject as HexGrid
/// </summary>
public class TempMapSaves : MonoBehaviour
{
    HexGrid grid;
    [SerializeField] HexMapEditor editor;

    [SerializeField] GameObject hideInBuild;

    [Space]
    [SerializeField] public bool loadMapOnAwake = true;
    public bool LoadsInsteadOfHexGrid => loadMapOnAwake && !string.IsNullOrWhiteSpace(loadMap);
    [SerializeField] [TextArea] string loadMap;

    /// <summary>
    /// log file extension (without the dot)
    /// </summary>
    const string logFileExtension = "tempMap";

    private void Awake()
    {
        if (grid == null)
            grid = GetComponent<HexGrid>();

        if (editor == null)
            editor = FindObjectOfType<HexMapEditor>();

        if (loadMapOnAwake)
            LoadString();

#if !UNITY_EDITOR
        if (hideInBuild != null)
            hideInBuild.SetActive(false);
#endif
    }

    public void Button_SaveMap() => CreateSave();

    public void Button_LoadMap() => LoadString();

    public void Button_MirrorLowerHalf() => MirrorLowerHalf();
    public void Button_MirrorUpperHalf() => MirrorUpperHalf();

    private void CreateSave()
    {
        string mapSave = grid.chunkCountX + "\t" + grid.chunkCountZ + "\n";
        foreach (var cell in grid.GetAllCells())
        {
            mapSave += cell.Elevation + "\t" + cell.tempSaveColorID + "\n";
        }

        loadMap = mapSave.TrimEnd();
        CreateFile(mapSave.TrimEnd());
    }

    /// <summary>
    /// Creates a new map file in the corresponding folder
    /// </summary>
    private void CreateFile(string content)
    {
        string logFileName = $"TempMap_{DateTime.Now.ToString("yyMMdd_HHmmss")}.{logFileExtension}";
        AI_File.WriteUTF8(content, AI_File.PathTempMaps + logFileName);

        Debug.Log($"Created new temporary Map named {logFileName}\nAt: {AI_File.PathTempMaps + logFileName}");
    }

    private void LoadString()
    {
        if (string.IsNullOrWhiteSpace(loadMap)) return;

        string[] splitted = loadMap.Split('\n');
        string[] nextLine = splitted[0].Split('\t');

        Debug.Log($"Loading new {nextLine[0]}x{nextLine[1]} Map.\n");

        if (loadMapOnAwake || grid.chunkCountX != int.Parse(nextLine[0]) || grid.chunkCountZ != int.Parse(nextLine[1]))
        {
            loadMapOnAwake = false;

            grid.chunkCountX = int.Parse(nextLine[0]);
            grid.chunkCountZ = int.Parse(nextLine[1]);

            grid.Clear();
            grid.Awake();
        }

        HexCell[] cells = grid.GetAllCells();
        int length = Mathf.Min(cells.Length, splitted.Length - 1);

        for (int cellID = 0; cellID < length; cellID++)
        {
            nextLine = splitted[cellID + 1].Split('\t');

            cells[cellID].Elevation = int.Parse(nextLine[0]);

            if (editor)
            {
                cells[cellID].Color = editor.colors[int.Parse(nextLine[1])];
                cells[cellID].tempSaveColorID = int.Parse(nextLine[1]);
            }
        }

        foreach (var item in grid.GetAllChunks())
            item.Refresh();
    }

    private void MirrorLowerHalf()
    {
        HexCell[] cells = grid.GetAllCells();

        for (int i = 0; i < cells.Length / 2f; i++)
            cells[cells.Length - (1 + i)].Copy(cells[i]);
    }

    private void MirrorUpperHalf()
    {
        HexCell[] cells = grid.GetAllCells();

        for (int i = 0; i < cells.Length / 2f; i++)
            cells[i].Copy(cells[cells.Length - (1 + i)]);
    }
}

#if UNITY_EDITOR
/// <summary>
/// This Class extends the default Unity Editor for the Class.
/// Buttons:
/// - Load: loads the given map string
/// </summary>
[CustomEditor(typeof(TempMapSaves))]
public class SaveMapButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Save Map"))
            ((TempMapSaves)target).Button_SaveMap();

        if (GUILayout.Button("Load Map String"))
            ((TempMapSaves)target).Button_LoadMap();

        GUILayout.Space(10);

        if (GUILayout.Button("Mirror Lower Half"))
            ((TempMapSaves)target).Button_MirrorLowerHalf();

        if (GUILayout.Button("Mirror Upper Half"))
            ((TempMapSaves)target).Button_MirrorUpperHalf();

        GUILayout.Space(10);

        base.OnInspectorGUI();
    }
}
#endif // UNITY_EDITOR