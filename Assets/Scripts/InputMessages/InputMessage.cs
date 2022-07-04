using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct InputMessage
{
    public const int POSITIONSenderLocalID = 0;
    public const int POSITIONAction = 1;

    public const int POSITIONStart = 2;
    public const int POSITIONStartLayer = 3;
    public const int POSITIONTarget = 4;
    public const int POSITIONTargetLayer = 5;

    public const int POSITIONNewPawn = 6;
    public const int POSITIONTurn = 7;

    public const int PARTCount = 8;

    public int senderLocalID;

    public ePlayeractionType action;
    [Space]
    public HexCoordinates startCoordinates;
    public HexCoordinates targetCoordinates;
    /// <summary>
    /// 0 equals surface.
    /// </summary>
    public int startLayer;
    public int targetLayer;
    [Space]
    public ePlayerPawnType newPawn;

    public int turn;

    /// <summary>
    /// True if the action involves Player Pawns and the HexGrid
    /// </summary>
    public bool IsOnHexGrid => action.IsOnHexGrid();

    public override string ToString()
    {
        return $"{senderLocalID}_{action.ToString()}_{startCoordinates.ToString()}_{startLayer}_{targetCoordinates.ToString()}_{targetLayer}_{newPawn}_{turn}";
    }
}
