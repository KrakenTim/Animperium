using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides methods to generate serialisable InputMessages.
/// </summary>
public static class InputMessageGenerator
{
    public static InputMessage CreatePawnMessage(PlayerPawn actingPawn, HexCell targetCell, ePlayeractionType action, ePlayerPawnType newPawn)
    {
        InputMessage message = CreateHexMessage(actingPawn, targetCell, action);

        message.newPawn = newPawn;

        return message;
    }

    /// <summary>
    /// Creates an Input message from the given parameters.
    /// </summary>
    public static InputMessage CreateHexMessage(PlayerPawn actingPawn, HexCell targetCell, ePlayeractionType action)
    {
        InputMessage message = CreateBasicMessage(action);

        message.startCoordinates = actingPawn.HexCoordinates;
        message.startLayer = (int)actingPawn.HexCell.gridLayer;

        message.targetCoordinates = targetCell.coordinates;
        message.targetLayer = (int)targetCell.gridLayer;

        return message;
    }

    /// <summary>
    /// Creates a general message without HexGrid information.
    /// </summary>
    public static InputMessage CreateBasicMessage(ePlayeractionType action)
    {
        InputMessage message = new InputMessage();
        message.senderLocalID = GameManager.InGame ? GameManager.LocalPlayerID : OnlineGameManager.LocalPlayerID;
        message.action = action;
        message.turn = GameManager.Turn;

        return message;
    }

    public static InputMessage CreateRandomKeyMessage()
    {
        InputMessage message = CreateBasicMessage(ePlayeractionType.SendRandomGenerationKey);

        message.turn = System.DateTime.UtcNow.Millisecond & System.DateTime.UtcNow.DayOfYear;

        return message;
    }

    public static int GetRandomKey(InputMessage message)
    {
        if (message.action != ePlayeractionType.SendRandomGenerationKey)
        {
            Debug.LogError($"Recieve Message to create randomkey which had the wrong type: {message.action}\n{message}");
            return 0;
        }

        return message.turn;
    }
}
