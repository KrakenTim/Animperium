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

    //buildings 100+199
    TownHall = 100,
    SchoolFight = 101,
    SchoolMagic = 102,
    SchoolDigging = 103,
    FarmHouse = 104,
    TunnelEntry = 105,
    Wall = 106,

    // non player pawn 200+
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
    public static bool IsNonPlayer(this ePlayerPawnType pawn)
    {
        return (int)pawn >= 200 && (int)pawn < 300;
    }

    public static bool IsSchool(this ePlayerPawnType pawn)
    {
        return pawn.Teaches() != eKnowledge.NONE;
    }

    public static eKnowledge Teaches(this ePlayerPawnType pawn)
    {
        switch (pawn)
        {
            case ePlayerPawnType.SchoolFight:
                return eKnowledge.Fight;
            case ePlayerPawnType.SchoolMagic:
                return eKnowledge.Magic;
            case ePlayerPawnType.SchoolDigging:
                return eKnowledge.Digging;

                default:
                    return eKnowledge.NONE;
        }
    }
}
