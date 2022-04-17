using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //buildings 100+
    TownHall = 100,
    SchoolFight = 101,
    SchoolMagic = 102,
    SchoolDigging = 103,
    FarmHouse = 104,
    TunnelEntry = 105,
    Wall = 106
}

public static class ePlayerPawnTypeExtensions
{
    public static bool IsBuilding(this ePlayerPawnType pawn)
    {
        return (int)pawn >= 100;
    }

    public static bool IsUnit(this ePlayerPawnType pawn)
    {
        return (int)pawn < 100 && pawn != ePlayerPawnType.NONE;
    }
}
