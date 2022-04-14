using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputMessageGenerator
{
    public static InputMessage CreateMessage(PlayerPawn actingPawn, HexCell targetCell, ePlayeractionType action)
    {
        InputMessage message = new InputMessage();
        message.startCell = actingPawn.HexCoordinates;
        message.targetCell = targetCell.coordinates;
        message.action = action;

      //  Debug.Log("InputMessageGenerator created" + message.ToString());

        return message;
    }
}
