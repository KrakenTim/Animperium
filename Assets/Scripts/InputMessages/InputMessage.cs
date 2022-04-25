using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct InputMessage
{
    public const int POSITIONAction = 0;
    public const int POSITIONStart = 1;
    public const int POSITIONTarget = 2;
    public const int POSITIONTurn = 3;

    public const int PARTCount = 4;

    public ePlayeractionType action;
    [Space]
    public HexCoordinates startCell;
    public HexCoordinates targetCell;

    public int turn;

    /// <summary>
    /// True if the action involves Player Pawns and the HexGrid
    /// </summary>
    public bool IsOnHexGrid => action.IsOnHexGrid();

    public override string ToString()
    {
        return $"{action.ToString()}_{startCell.ToString()}_{targetCell.ToString()}_{turn}";
    }
}
