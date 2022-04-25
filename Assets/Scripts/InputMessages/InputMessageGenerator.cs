using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides methods to generate serialisable InputMessages.
/// </summary>
public static class InputMessageGenerator
{
    /// <summary>
    /// Creates an Input message from the given parameters.
    /// </summary>
    public static InputMessage CreateHexMessage(PlayerPawn actingPawn, HexCell targetCell, ePlayeractionType action)
    {
        InputMessage message = CreateBasicMessage(action);

        message.startCell = actingPawn.HexCoordinates;
        message.targetCell = targetCell.coordinates;

        return message;
    }

    /// <summary>
    /// Creates a general message without HexGrid information.
    /// </summary>
    public static InputMessage CreateBasicMessage(ePlayeractionType action)
    {
        InputMessage message = new InputMessage();
        message.action = action;
        message.turn = GameManager.Turn;

        return message;
    }
}
