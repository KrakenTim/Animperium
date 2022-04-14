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

      //  Debug.Log("InputMessageGenerator created" + message.ToString());

        return message;
    }

    public static InputMessage Attack(PlayerPawn attacker, PlayerPawn victim)
    {
        InputMessage message = new InputMessage();
        message.startCell = attacker.HexCoordinates;
        message.targetCell = victim.HexCoordinates;
        message.action = ePlayeractionType.Attack;

        Debug.Log("InputMessageGenerator created" + message.ToString());

        return message;
    }
}
