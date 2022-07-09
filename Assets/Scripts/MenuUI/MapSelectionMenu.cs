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
    public RectTransform listSelfCreatedContent;
    public RectTransform listDefaultContent;
    [Space]
    public SaveLoadItem itemPrefab;

    private SaveLoadItem currentSelectedMap;

    private void OnEnable()
    {
        Cleanup();

        if (Directory.Exists(AI_File.PathSelfmadeMaps))
            FillList(AI_File.PathSelfmadeMaps, listSelfCreatedContent);
        if (Directory.Exists(AI_File.PathDefaultMaps))
            FillList(AI_File.PathDefaultMaps, listDefaultContent);
    }

    string GetSelectedPath()
    {
        string path = currentSelectedMap.mapPath;

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
    void FillList(string folderPath, Transform contentTransform)
    {
        // TODO(2022-07-04) find more elegant solution
        for (int i = 0; i < contentTransform.childCount; i++)
            Destroy(contentTransform.GetChild(i).gameObject);

        string[] paths = Directory.GetFiles(folderPath, "*.map");
        Array.Sort(paths);

        SaveLoadItem first = null;
        for (int i = 0; i < paths.Length; i++)
        {
            SaveLoadItem item = Instantiate(itemPrefab);
            item.selectionMenu = this;
            item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
            item.mapPath = paths[i];
            item.transform.SetParent(contentTransform, false);

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

        if (File.Exists(path)) File.Delete(path);

        nameInput.text = "";

        FillList(AI_File.PathSelfmadeMaps, listSelfCreatedContent);
        FillList(AI_File.PathDefaultMaps, listDefaultContent);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(AI_Scene.SCENENAME_MainMenu);
    }

    /// <summary>
    /// Method to shift maps that have been created in the base folder into their own folder
    /// </summary>
    private void Cleanup()
    {
        string[] paths = Directory.GetFiles(Application.persistentDataPath, "*.map");

        if (paths.Length == 0) return;

        Directory.CreateDirectory(AI_File.PathSelfmadeMaps);

        foreach (var path in paths)
        {
            string fileName = Path.GetFileName(path);
            File.Move(path, Path.Combine(AI_File.PathSelfmadeMaps, fileName));
        }
    }
}
