using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapSelectionMenu : MonoBehaviour
{
    [SerializeField] PersistingMatchData currentMatchData;

    [Space]
    public InputField nameInput;
    public RectTransform listContent;
    [Space]
    public SaveLoadItem itemPrefab;

    private SaveLoadItem currentSelectedMap;

    private void OnEnable()
    {
        FillList();
    }

    string GetSelectedPath()
    {
        string mapName = nameInput.text;
        if (string.IsNullOrWhiteSpace(mapName)) return null;

        string path = Path.Combine(Application.persistentDataPath, mapName + ".map");

        if (File.Exists(path))
            return path;
        else
            return null;
    }

    /// <summary>
    /// Checks if the file at the given path exists and is a valid version
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    bool CheckVersion(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("File does not exist " + path);
            return false;
        }
        using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
        {
            int header = reader.ReadInt32();

            if (header <= 1)
                return true;
            else
            {
                Debug.LogWarning("Unknown map format " + header);
                return false;
            }
        }
    }

    public void Action()
    {
        string path = GetSelectedPath();
        if (path == null) return;

        if (currentMatchData.IsMapPathValid && CheckVersion(currentMatchData.MapPath))
            SceneManager.LoadScene(AI_Scene.SCENENAME_Game);
    }

    /// <summary>
    /// Updates the menu with the item selected in scrolldown
    /// </summary>
    public void SelectItem(SaveLoadItem item)
    {
        currentSelectedMap = item;

        nameInput.text = item.MapName;

        currentMatchData.MapPath = item.mapPath;
    }

    /// <summary>
    /// fills listContent with maps found in Application.persistentDataPath
    /// </summary>
    void FillList()
    {
        // TODO(2022-07-04) find more elegant solution
        for (int i = 0; i < listContent.childCount; i++)
            Destroy(listContent.GetChild(i).gameObject);

        string[] paths = Directory.GetFiles(Application.persistentDataPath, "*.map");
        Array.Sort(paths);

        SaveLoadItem first = null;
        for (int i = 0; i < paths.Length; i++)
        {
            SaveLoadItem item = Instantiate(itemPrefab);
            item.selectionMenu = this;
            item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
            item.mapPath = paths[i];
            item.transform.SetParent(listContent, false);

            if (i == 0) first = item;
        }

        if (first != null)
            SelectItem(first);
    }

    // Might be usefull in the future
    public void Delete()
    {
        string path = GetSelectedPath();
        if (path == null) return;

        if (File.Exists(path))
        {
            File.Delete(path);
        }
        nameInput.text = "";
        FillList();
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(AI_Scene.SCENENAME_MainMenu);
    }
}
