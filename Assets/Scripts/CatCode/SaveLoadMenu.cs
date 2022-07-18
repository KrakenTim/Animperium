using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class SaveLoadMenu : MonoBehaviour
{
    public Text menuLabel, actionButtonLabel;
    public InputField nameInput;
    public RectTransform listContent;
    public SaveLoadItem itemPrefab;
    public HexGrid hexGrid;

    bool saveMode;

    public void Open(bool saveMode)
    {
        this.saveMode = saveMode; if (saveMode)
        {
            menuLabel.text = "Save Map";
            actionButtonLabel.text = "Save";
        }
        else
        {
            menuLabel.text = "Load Map";
            actionButtonLabel.text = "Load";
        }
        FillList();
        gameObject.SetActive(true);
        HexMapCamera.Locked = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        HexMapCamera.Locked = false;
    }
    string GetSelectedPath()
    {
        string mapName = nameInput.text;
        if (mapName.Length == 0)
        {
            return null;
        }
        //return Path.Combine(Application.persistentDataPath, mapName + ".map");
        return Path.Combine(AI_File.PathSelfmadeMaps, mapName + ".map");
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
            item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
            item.transform.SetParent(listContent, false);
        }
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