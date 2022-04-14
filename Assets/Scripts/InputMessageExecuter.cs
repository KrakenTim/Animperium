using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMessageExecuter : MonoBehaviour
{

    public static HexGrid HexGrid => GameManager.HexGrid;
    public static void Send(InputMessage message)
    {
        //TODO(14.04.2022): add non hotseat stuff here

        Recieve(message.ToString());
    }

    public static void Recieve(string messageString)
    {
        if (!InputMessageInterpreter.TryParseMessage(messageString, out InputMessage order))
        {
            Debug.LogError("InputMessageExecuter recieved unexpected message: " + messageString);
            return;
        }


        HexCell startCell = HexGrid.GetHexCell(order.startCell);
        HexCell targetCell = HexGrid.GetHexCell(order.targetCell);

        PlayerPawn startPawn = startCell.Pawn;
        PlayerPawn targetPawn = targetCell.Pawn;

        switch (order.action)
        {
            case ePlayeractionType.Move:
                startPawn.MoveTo(targetCell);
                break;
            case ePlayeractionType.Attack:
                startPawn.Attack(targetPawn);
                break;
            case ePlayeractionType.Collect:
                break;
            case ePlayeractionType.SpawnVillager:
                break;
            case ePlayeractionType.SpawnSwordfighter:
                break;

            default:
                Debug.LogError("InputMessageExecuter Recieved UNDEFINED for " + order.action);
                return;
        }
    }
}
