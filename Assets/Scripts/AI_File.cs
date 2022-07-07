using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Provides some general basic functions for the Project
/// </summary>
public static class AI_File
{
    /// <summary>
    /// Application.dataPath + "/ScriptableObjects/"
    /// </summary>
    public static string PathScriptableObjects => Application.dataPath + "/ScriptableObjects/";

    /// <summary>
    /// Application.streamingAssetsPath + "/InputLogs/"
    /// </summary>
    public static string PathInputLogs => Application.streamingAssetsPath + "/InputLogs/";

    /// <summary>
    /// Application.streamingAssetsPath + "/TempMaps"
    /// </summary>
    public static string PathTempMaps => Path.Combine(Application.streamingAssetsPath, "TempMaps");

    /// <summary>
    /// Application.dataPath + "/DefaultMaps"
    /// </summary>
    public static string PathDefaultMaps => Path.Combine(Application.streamingAssetsPath, "DefaultMaps");

    /// <summary>
    /// Application.persistentDataPath + "/Maps"
    /// </summary>
    public static string PathSelfmadeMaps => Path.Combine(Application.persistentDataPath, "Maps");



    /// <summary>
    /// removes Application.dataPath (which leads to the Assets-folder) of given path
    /// </summary>
    public static string ShortPath(string path) => path.Replace(Application.dataPath + "/", "");

    /// <summary>
    /// writes the text into a UTF8 formated file at the given path, creates directory if necessary
    /// </summary>
    public static void WriteUTF8(string text, string path)
    {
        if (File.Exists(path) == false)
            Directory.CreateDirectory(Path.GetDirectoryName(path));

        File.WriteAllText(path, text, System.Text.Encoding.UTF8);
    }

    public static void AppendUTF8(string text, string path)
    {
        File.AppendAllText(path, text, System.Text.Encoding.UTF8);
    }

    #region Editor Only

#if UNITY_EDITOR
    public static Dictionary<string, Asset> EDITOR_GetAssets<Asset>(string path, bool includeSubfolders = true)
                                        where Asset : Object
    {
        string[] assetPaths = Directory.GetFiles(path, "*.asset",
                                               includeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        Dictionary<string, Asset> result = new Dictionary<string, Asset>();

        Asset asset;
        foreach (var assetPath in assetPaths)
        {
            asset = UnityEditor.AssetDatabase.LoadAssetAtPath<Asset>(assetPath);

            if (asset != null)
                result.Add(asset.name, asset);
        }

        return result;
    }
#endif //UNITY_EDITOR

    /// <summary>
    /// Returns all Files and their paths found in the given path, excluding meta files.
    /// Throws an error if path isn't available in builds.
    /// </summary>
    public static Dictionary<string, string> GetFiles(string path, string searchPattern = "*.*", bool includeSubfolders = true)
    {
        Dictionary<string, string> filePaths = new Dictionary<string, string>();

        if (!path.Contains(Application.streamingAssetsPath) && !path.Contains(Application.persistentDataPath))
        {
            Debug.LogError($"{nameof(AI_File)}\tTried to load from a folder which isn't available in a build.\n\t\t{path}\n");
            return filePaths;
        }

        if (!Directory.Exists(path)) return filePaths;

        string[] rawPaths = Directory.GetFiles(path, searchPattern, includeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        foreach (var nextPath in rawPaths)
        {
            if (Path.HasExtension("meta")) continue;

            filePaths.Add(Path.GetFileNameWithoutExtension(nextPath), nextPath);
        }

        return filePaths;
    }

    #endregion Editor Only
}
