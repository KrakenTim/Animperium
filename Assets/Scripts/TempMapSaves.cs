using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Should be placed on same GameObject as HexGrid
/// </summary>
public class TempMapSaves : MonoBehaviour
{
    HexGrid grid;

    [SerializeField] GameObject hideInBuild;

    [Space]
    [SerializeField] PersistingMatchData currentMatchData;
    [SerializeField] public bool loadPathOnAwake = true;

    [SerializeField] public bool loadTempMapOnAwake = true;
    public bool LoadsInsteadOfHexGrid => loadTempMapOnAwake && !string.IsNullOrWhiteSpace(loadMap);
    [SerializeField] [TextArea] string loadMap;

    /// <summary>
    /// log file extension with the dot
    /// </summary>
    const string logFileExtension = ".tempMap";
    const string autoFileNamePrefix = "TempMap_";

    private void Awake()
    {
        if (grid == null)
            grid = GetComponent<HexGrid>();

        if (loadPathOnAwake && currentMatchData != null && currentMatchData.IsMapPathValid)
        {
            LoadPath(currentMatchData.MapPath);
            return;
        }
        else if (loadTempMapOnAwake)
            LoadString();

#if !UNITY_EDITOR
        if (hideInBuild != null)
            hideInBuild.SetActive(false);
#endif
    }

    private void OnDisable()
    {
        if (!GameManager.InGame)
            CreateSave(onlyIfNew: true);
    }

    public void Button_SaveMap() => CreateSave();

    public void Button_LoadMap() => LoadString();

    public void Button_MirrorLowerHalf() => MirrorLowerHalf();
    public void Button_MirrorUpperHalf() => MirrorUpperHalf();

    private void CreateSave(bool onlyIfNew = false)
    {
        string mapSave = grid.ChunkCountX + "\t" + grid.ChunkCountZ + "\n";
        foreach (var cell in grid.GetAllCells())
        {
            mapSave += cell.Elevation + "\t" + cell.tempSaveColorID + "\n";
        }

        loadMap = mapSave.TrimEnd();
        CreateFile(loadMap, onlyIfNew);
    }

    /// <summary>
    /// Creates a new map file in the corresponding folder
    /// </summary>
    private void CreateFile(string content, bool onlyIfNew = false)
    {
        if (onlyIfNew)
        {
            if (TryGetNewestTempMapPath(out string newestTempMapPath)
                && content.CompareTo(File.ReadAllText(newestTempMapPath)) == 0)
                return;
        }

        string logFileName = $"{autoFileNamePrefix}{DateTime.Now.ToString("yyMMdd_HHmmss")}{logFileExtension}";
        AI_File.WriteUTF8(content, Path.Combine(AI_File.PathTempMaps, logFileName));

        Debug.Log($"Created new temporary Map named {logFileName}\nAt: { Path.Combine(AI_File.PathTempMaps, logFileName)}");
    }

    private void LoadPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            Debug.LogError("File does not exist " + path, this);
            return;
        }

        using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
        {
            int header = reader.ReadInt32();
            if (header <= 1)
            {
                grid.Load(reader, header);
                HexMapCamera.ValidatePosition();
            }
            else
                Debug.LogWarning($"Unknown map format {header} in file!\n{path}", this);
        }
    }

    private void LoadString()
    {
        if (string.IsNullOrWhiteSpace(loadMap)) return;

        string[] splitted = loadMap.Split('\n');
        string[] nextLine = splitted[0].Split('\t');

        Debug.Log($"Loading new {nextLine[0]}x{nextLine[1]} Map.\n");

        if (loadTempMapOnAwake || grid.ChunkCountX != int.Parse(nextLine[0]) || grid.ChunkCountZ != int.Parse(nextLine[1]))
        {
            loadTempMapOnAwake = false;

            grid.cellCountX = int.Parse(nextLine[0]) * HexMetrics.chunkSizeX;
            grid.cellCountZ = int.Parse(nextLine[1]) * HexMetrics.chunkSizeZ;

            grid.Clear();
            grid.Awake();
        }

        HexCell[] cells = grid.GetAllCells();
        int length = Mathf.Min(cells.Length, splitted.Length - 1);

        for (int cellID = 0; cellID < length; cellID++)
        {
            nextLine = splitted[cellID + 1].Split('\t');

            cells[cellID].Elevation = int.Parse(nextLine[0]);

            cells[cellID].TerrainTypeIndex = int.Parse(nextLine[1]);
        }

        foreach (var item in grid.GetAllChunks())
            item.Refresh();
    }

    private void MirrorLowerHalf()
    {
        HexCell[] cells = grid.GetAllCells();

        for (int i = 0; i < cells.Length; i++)
        {
            int a = i;
            int b = cells.Length - (1 + i);

            if (a > b) break;

            cells[b].Copy(cells[a]);
        }

        grid.enabled = true;
    }

    private void MirrorUpperHalf()
    {
        HexCell[] cells = grid.GetAllCells();

        for (int i = 0; i < cells.Length; i++)
        {
            int a = i;
            int b = cells.Length - (1 + i);

            if (a > b) break;

            cells[a].Copy(cells[b]);
        }

        grid.enabled = true;
    }

    private Dictionary<string, string> GetTempMaps()
    {
        return AI_File.GetFiles(AI_File.PathTempMaps, $"{autoFileNamePrefix}*{logFileExtension}", true);
    }

    private bool TryGetNewestTempMapPath(out string result)
    {
        List<string> paths = new List<string>(GetTempMaps().Values);

        if (paths.Count == 0)
        {
            result = string.Empty;
            return false;
        }

        result = paths[paths.Count - 1];
        return true;
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
