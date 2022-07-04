using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapSelectionMenu : MonoBehaviour
{
    private const string SCENE_MainMenu = "MainMenu";

    public InputField nameInput;
    public RectTransform listContent;
    [Space]
    public SaveLoadItem itemPrefab;

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

    void Load(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("File does not exist " + path);
            return;
        }
        using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
        {
            int header = reader.ReadInt32();
            if (header <= 1)
            {
                //hexGrid.Load(reader, header);
                Debug.Log($"LOADING {Path.GetFileNameWithoutExtension(path)} from {path}\n", this);
            }
            else
            {
                Debug.LogWarning("Unknown map format " + header);
            }
        }
    }
    public void Action()
    {
        string path = GetSelectedPath();
        if (path == null) return;

        Load(path);
    }

    /// <summary>
    /// Updates the menu with the item selected in scrolldown
    /// </summary>
    public void SelectItem(string name)
    {
        nameInput.text = name;
    }

    /// <summary>
    /// fills listContent with maps found in Application.persistentDataPath
    /// </summary>
    void FillList()
    {
        for (int i = 0; i < listContent.childCount; i++)
        {
            Destroy(listContent.GetChild(i).gameObject);
        }

        string[] paths = Directory.GetFiles(Application.persistentDataPath, "*.map");
        Array.Sort(paths);

        for (int i = 0; i < paths.Length; i++)
        {
            SaveLoadItem item = Instantiate(itemPrefab);
            item.selectionMenu = this;
            item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
            item.transform.SetParent(listContent, false);
        }
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
        SceneManager.LoadScene(SCENE_MainMenu);
    }
}
