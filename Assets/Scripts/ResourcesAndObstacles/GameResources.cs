using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Struct used to represent a player's collected resurces, the price of units etc.
/// </summary>
[System.Serializable]
public struct GameResources
{
    public int food;
    public int wood;
    public int ore;
}
