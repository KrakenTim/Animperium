using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Contains information that should be set and kept before and after a match
/// </summary>
[CreateAssetMenu]
public class PersistingMatchData : ScriptableObject
{
    [SerializeField] private string mapPath;
    public string MapPath { get => mapPath; set => mapPath = value; }

    public bool IsMapPathValid => !string.IsNullOrEmpty(MapPath) && File.Exists(MapPath);

    public void ResetAll()
    {
        MapPath = string.Empty;
    }
}
