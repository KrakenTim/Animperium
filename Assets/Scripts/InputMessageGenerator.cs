using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputMessageGenerator
{
    public static InputMessage MoveToHex(PlayerPawn movingPawn, HexCell targetCell)
    {
        InputMessage message = new InputMessage();
        message.startCell = movingPawn.HexCoordinates;
        message.targetCell = targetCell.coordinates;
        message.action = ePlayeractionType.Move;

        Debug.Log("InputMessageGenerator created" + message.ToString());

        return message;
    }
}
