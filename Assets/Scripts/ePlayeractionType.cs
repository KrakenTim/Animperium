using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // General, non-Hex Actions 100+
    EndTurn = 100
}

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