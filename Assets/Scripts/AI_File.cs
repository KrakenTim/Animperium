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

    #region Editor Only

#if UNITY_EDITOR
    public static Dictionary<string, Asset> EDITOR_GetAssets<Asset>(string path, bool includeSubfolders = true)
                                        where Asset : Object
    {
        List<string> assetPaths = new List<string>(Directory.GetFiles(path, "*.asset",
                                               includeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));

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

    #endregion Editor Only
}
