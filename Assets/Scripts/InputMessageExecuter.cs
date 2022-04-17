using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMessageExecuter : MonoBehaviour
{
    private static HexGrid HexGrid => GameManager.HexGrid;

    public static void Send(InputMessage message)
    {
        //TODO(14.04.2022): add non hotseat stuff here

        Recieve(message.ToString());
    }

    public static void Recieve(string messageString)
    {
        if (!InputMessageInterpreter.TryParseMessage(messageString, out InputMessage order))
        {
            Debug.LogError("MessageExecuter\t recieved unexpected message.\n\t\t" + messageString);
            return;
        }

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
                GameManager.SpawnPawn(startPawn, targetCell);
                break;
            case ePlayeractionType.Learn:
                break;

            default:
                Debug.LogError($"MessageExecuter\t{nameof(ExecuteHexMessage)} UNDEFINED for {hexOrder.action}\n");
                return;
        }
    }

    private static void ExecuteGeneralMessage(InputMessage generalOrder)
    {
        switch (generalOrder.action)
        {
            case ePlayeractionType.EndTurn:
                GameManager.EndTurn();
                break;

            default:
                Debug.LogError($"MessageExecuter\t{nameof(ExecuteGeneralMessage)} UNDEFINED for {generalOrder.action}\n");
                break;
        }
    }
}
