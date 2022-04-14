using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputMessageInterpreter
{
    /// <summary>
    /// Tries to understand given message, returns true if sucessfull
    /// </summary>
    public static bool TryParseMessage(string message, out InputMessage result)
    {
        message = message.Replace(" ", "");
        message = message.Replace("(", " ").Replace(")", " ");
        message = message.Trim(); //removes white Space (Space)

        result = new InputMessage();

        string[] splitMessage = message.Split(' ');

        if(splitMessage.Length != 3) return false;

        // get action
        if(!System.Enum.TryParse(splitMessage[1], out result.action))
            return false;

        Debug.Log("Action found");

        // get startCell
        string[] startCoordinates = splitMessage[0].Split(',');
        int nextX;
        int nextZ;

        if (startCoordinates.Length != 3) return false;

        if(!int.TryParse(startCoordinates[0], out nextX) || !int.TryParse(startCoordinates[2], out nextZ))
            return false;

        result.startCell = new HexCoordinates(nextX, nextZ);

        Debug.Log("StartCell found");

        // get targetCell
        string[] targetCoordinates = splitMessage[2].Split(',');

        if (targetCoordinates.Length != 3) return false;

        if (!int.TryParse(targetCoordinates[0], out nextX) || !int.TryParse(targetCoordinates[2], out nextZ))
            return false;

        result.targetCell = new HexCoordinates(nextX, nextZ);

        Debug.Log("InputMessageInterpreter recieved" + result.ToString());

        return true;
    }
}
