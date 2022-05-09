using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sends and recieves InputMessages. Executes InputMessages by calling the given action.
/// </summary>
public static class InputMessageExecuter
{
    /// <summary>
    /// Called if a recieved InputMessage could be parsed and will be executed.
    /// </summary>
    public static event System.Action<string> RecievedInputMessage;

    private static HexGrid HexGrid => GameManager.HexGrid;

    /// <summary>
    /// Sends the given message out, currently to itself since we've only got hotseat mode.
    /// </summary>
    public static void Send(InputMessage message)
    {
        if (OnlineGameManager.IsOnlineGame)
            OnlineGameManager.SendCommand(message.ToString());
        else // Hot Seat
            Recieve(message.ToString());
    }

    /// <summary>
    /// Tries to parse the given message into an executable order, if successful executes it.
    /// </summary>
    public static void Recieve(string messageString)
    {
        if (!InputMessageInterpreter.TryParseMessage(messageString, out InputMessage order))
        {
            Debug.LogError("MessageExecuter\tRecieved message that couldn't be parsed.\n\t\t" + messageString);
            return;
        }

        Execute(order);
    }

    /// <summary>
    /// Executes the given input message.
    /// Keep in mind that calling this directly in hotseat might hide errors in the InputMessage parsing.
    /// </summary>
    public static void Execute(InputMessage order)
    {
        RecievedInputMessage?.Invoke(order.ToString());

        if (order.IsOnHexGrid)
            ExecuteHexMessage(order);
        else
            ExecuteGeneralMessage(order);
    }

    /// <summary>
    /// Executes a Inputmessage related to Player Pawns and the Hex Grid
    /// </summary>
    private static void ExecuteHexMessage(InputMessage hexOrder)
    {
        HexCell startCell = HexGrid.GetHexCell(hexOrder.startCell);
        HexCell targetCell = HexGrid.GetHexCell(hexOrder.targetCell);

        PlayerPawn startPawn = startCell.Pawn;
        PlayerPawn targetPawn = targetCell.Pawn;

        switch (hexOrder.action)
        {
            case ePlayeractionType.Move:
                startPawn.MoveTo(targetCell);
                break;
            case ePlayeractionType.Attack:
                startPawn.Attack(targetPawn);
                break;
            case ePlayeractionType.Collect:
                startPawn.Collect(targetCell.Resource);
                break;
            case ePlayeractionType.Spawn:
                GameManager.SpawnPawn(startPawn, targetCell, startPawn.Spawn);
                break;
            case ePlayeractionType.Learn:
                GameManager.UpgradePawn(startPawn, targetPawn, hexOrder.newPawn);
                break;
            case ePlayeractionType.Build:
                startPawn.Build(targetCell, hexOrder.newPawn);
                break;

            default:
                Debug.LogError($"MessageExecuter\t{nameof(ExecuteHexMessage)} UNDEFINED for {hexOrder.action}\n");
                return;
        }
    }

    /// <summary>
    /// Executes a Inputmessage unrelated to the Hex Grid
    /// </summary>
    private static void ExecuteGeneralMessage(InputMessage generalOrder)
    {
        switch (generalOrder.action)
        {
            case ePlayeractionType.EndTurn:
                GameManager.EndTurn();
                break;

            case ePlayeractionType.StartGame:
                OnlineGameManager.PrepareGame();
                break;

            case ePlayeractionType.Resign:
                GameManager.PlayerResigned(generalOrder.senderLocalID);
                break;

            default:
                Debug.LogError($"MessageExecuter\t{nameof(ExecuteGeneralMessage)} UNDEFINED for {generalOrder.action}\n");
                break;
        }
    }
}
