using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Decribes the type of pawn on the hexgrid. 
/// </summary>
public enum ePlayerPawnType
{
    NONE = 0,

    // units 1-99
    Villager = 1,
    Swordfighter = 2,
    Healer = 3,
    Digger = 4,
    Warmage = 5,
    Sneaker = 6,
    Blaster = 7,
    Veteran = 8,

    // Buildings 100 - 199 
    //base buildings 100-149
    TownHall = 100,
    School = 101, // formerly SchoolFight
    FarmHouse = 104,
    TunnelEntry = 105,
    Wall = 106,

    // building upgrades 150 - 199
    School2 = 150, // formerly SchoolFight Lv.2

    // non-player pawn 200+
    ObsctacleStone = 200
}

public static class ePlayerPawnTypeExtensions
{
    public static bool IsUnit(this ePlayerPawnType pawn)
    {
        return (int)pawn < 100 && pawn != ePlayerPawnType.NONE;
    }
    public static bool IsBuilding(this ePlayerPawnType pawn)
    {
        return (int)pawn >= 100 && (int)pawn < 200;
    }
    public static bool IsBuildingUpgrade(this ePlayerPawnType pawn)
    { 
        return (int)pawn >= 150 && (int)pawn < 200;
    }

    public static bool IsNonPlayer(this ePlayerPawnType pawn)
    {
        return (int)pawn >= 200 && (int)pawn < 300;
    }

    /// <summary>
    /// True if the pawn teaches some Knowledge.
    /// </summary>
    public static bool IsSchool(this ePlayerPawnType pawn)
    {
        return pawn == ePlayerPawnType.School || pawn == ePlayerPawnType.School2;
    }
}
