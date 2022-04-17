using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputMessageInterpreter
{
    /// <summary>
    /// Tries to understand given message, returns true if sucessfull
    /// </summary>
    public static bool TryParseMessage(string message, out InputMessage inputMessage)
    {
        inputMessage = new InputMessage();

        string[] splitMessage = message.Split('_');

        // check length
        if (splitMessage.Length != InputMessage.PARTCount)
        {
            Debug.Log($"Interpreter\tMessage didn't consist of {InputMessage.PARTCount} parts.\n\t\t{message}");
            return false;
        }

        // get action
        if (!System.Enum.TryParse(splitMessage[InputMessage.POSITIONAction], out inputMessage.action))
        {
            Debug.Log($"Interpreter\tCouldn't interpret Action '{splitMessage[InputMessage.POSITIONAction]}'.\n\t\t{message}");
            return false;
        }

        // get startCell
        if (!TryParseHexCoordinate(splitMessage[InputMessage.POSITIONStart], out inputMessage.startCell))
        {
            Debug.Log($"Interpreter\tCouldn't parse StartCell '{splitMessage[InputMessage.POSITIONStart]}'.\n\t\t{message}");
            return false;
        }

        // get targetCell        
        if (!TryParseHexCoordinate(splitMessage[InputMessage.POSITIONTarget], out inputMessage.targetCell))
        {
            Debug.Log($"Interpreter\tCouldn't parse TargetCell '{splitMessage[InputMessage.POSITIONTarget]}'.\n\t\t{message}");
            return false;
        }

        Debug.Log($"Interpreter\tRecieved {inputMessage.action}.\n\t\t{inputMessage.ToString()}\n");
        return true;
    }

    /// <summary>
    /// Tries to parse given string to a Hex Coordinate
    /// </summary>
    private static bool TryParseHexCoordinate(string coordinateString, out HexCoordinates hexCoordinates)
    {
        // remove brackets
        coordinateString = coordinateString.Substring(1, coordinateString.Length - 2);

        string[] targetCoordinates = coordinateString.Split(',');

        if (targetCoordinates.Length != 3
            || !int.TryParse(targetCoordinates[0], out int nextX)   // get X-Position
            || !int.TryParse(targetCoordinates[2], out int nextZ))  // get Z-Position
        {
            Debug.LogError($"Interpreter\tCouldn't parse given HexCoordinate '{coordinateString}'\n");
            hexCoordinates = new HexCoordinates();
            return false;
        }

        hexCoordinates = new HexCoordinates(nextX, nextZ);
        return true;
    }
}
