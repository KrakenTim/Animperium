using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using TMPro;
using System.Text.RegularExpressions;

public class SaveLoadMenu : MonoBehaviour
{
    public static bool menuOpen { get; private set; }

    public TMP_InputField nameInput;
    public RectTransform listContent;
    public SaveLoadItem itemPrefab;
    public HexGrid hexGrid;

    bool saveMode;

    public void Open(bool saveMode)
    {
        this.saveMode = saveMode;
        FillList();

        gameObject.SetActive(true);
        HexMapCamera.Locked = true;
        menuOpen = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        HexMapCamera.Locked = false;
        menuOpen = false;
    }

    string GetSelectedPath()
    {
        nameInput.text = CleanMapName(nameInput.text);

        if (string.IsNullOrWhiteSpace(nameInput.text)) return null;


        //return Path.Combine(Application.persistentDataPath, mapName + ".map");
        return Path.Combine(AI_File.PathSelfmadeMaps, nameInput.text + ".map");
    }

    /// <summary>
    /// removes whitespace from mapnames
    /// </summary>
    private string CleanMapName(string inputText)
    {
        if (inputText == null) return null;

        string[] nameSnippets = inputText.Trim().Split(' ');

        string result = "";

        foreach (string snippet in nameSnippets)
        {
            if (snippet.Length > 1)
                result += snippet.Substring(0, 1).ToUpper() + snippet.Substring(1);
            else if (snippet.Length == 1)
                result += snippet.ToUpper();
        }

        return result;
    }

    public void Save(string path) => Save(path, hexGrid);

    public static void Save(string path, HexGrid grid)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            writer.Write(1);
            grid.Save(writer);
        }
    }

    void Load(string path) => Load(path, hexGrid);

    public static void Load(string path, HexGrid grid)
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
                grid.Load(reader, header);
                HexMapCamera.ValidatePosition();
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
        if (path == null)
        {
            return;
        }
        if (saveMode)
        {
            Save(path);
        }
        else
        {
            Load(path);
        }
        Close();
    }

    public void SelectItem(string name)
    {
        nameInput.text = name;
    }
    void FillList()
    {
        for (int i = 0; i < listContent.childCount; i++)
            Destroy(listContent.GetChild(i).gameObject);

        if (!Directory.Exists(AI_File.PathSelfmadeMaps))
            Directory.CreateDirectory(AI_File.PathSelfmadeMaps);

        //string[] paths = Directory.GetFiles(Application.persistentDataPath, "*.map");
        string[] paths = Directory.GetFiles(AI_File.PathSelfmadeMaps, "*.map");
        Array.Sort(paths);
        for (int i = 0; i < paths.Length; i++)
        {
            SaveLoadItem item = Instantiate(itemPrefab);
            item.menu = this;
            item.MapName = CamelToTitleCase(Path.GetFileNameWithoutExtension(paths[i]));
            item.transform.SetParent(listContent, false);
        }
    }

    public static string CamelToTitleCase(string text)
    {
        text = text.Substring(0, 1).ToUpper() + text.Substring(1);
        return Regex.Replace(text, @"(\B[A-Z])", @" $1");
    }

    public void Delete()
    {
        string path = GetSelectedPath();
        if (path == null)
        {
            return;
        }
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        nameInput.text = "";
        FillList();
    }
}