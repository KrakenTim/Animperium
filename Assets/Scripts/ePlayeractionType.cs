using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Type of input action the player performed, used in input serialisation.
/// </summary>
public enum ePlayeractionType
{
    // don't use underscores '_', which we use for input message parsing    

    NONE = 0,

    // Hex Actions 1-99
    Move = 1,
    Attack = 2,
    Collect = 3,
    Spawn = 4,
    Learn = 5,
    Build = 6,

    // General, non-Hex Actions 100+
    EndTurn = 100,
    StartGame = 101,
    Resign = 102
}

/// <summary>
/// Provides ways to differentiate between HexGrid and general methods.
/// </summary>
public static class ePlayeractionTypeExtensions
{
    public static bool IsOnHexGrid(this ePlayeractionType action)
    {
        return (int)action < 100 && action != ePlayeractionType.NONE;
    }

    public static bool IsGeneral(this ePlayeractionType action)
    {
        return (int)action >= 100;
    }
}