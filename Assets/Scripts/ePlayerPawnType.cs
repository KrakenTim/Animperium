using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePlayerPawnType
{
    NONE = 0,
    TownHall = 1,
    Villager = 2,
    Swordfighter = 3,
    School = 4,
}

public static class ePlayerPawnTypeExtensions
{
    public static bool IsBuilding(this ePlayerPawnType pawn)
    {
        return pawn == ePlayerPawnType.TownHall || pawn == ePlayerPawnType.School;
    }

    public static bool IsUnit(this ePlayerPawnType pawn)
    {
        return pawn == ePlayerPawnType.Villager || pawn == ePlayerPawnType.Swordfighter;
    }
}
