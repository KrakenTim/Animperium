using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InputMessage
{
   public HexCoordinates startCell;
   public HexCoordinates targetCell;
   public ePlayeractionType action;


    public override string ToString()
    {
        return $"{startCell.ToString()} {action.ToString()} {targetCell.ToString()}";
    }
}
