using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputMessageGenerator
{
    public static InputMessage CreateHexMessage(PlayerPawn actingPawn, HexCell targetCell, ePlayeractionType action)
    {
        InputMessage message = new InputMessage();
        message.startCell = actingPawn.HexCoordinates;
        message.targetCell = targetCell.coordinates;
        message.action = action;

        return message;
    }

    public static InputMessage CreateGeneralMessage(ePlayeractionType action)
    {
        InputMessage message = new InputMessage();
        message.action = action;

        return message;
    }
}
