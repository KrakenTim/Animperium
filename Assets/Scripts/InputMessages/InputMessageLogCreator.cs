using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMessageLogCreator : MonoBehaviour
{
    private string PathCurrentLog => AI_File.PathInputLogs + currentLogFileName;

    /// <summary>
    /// Generates a unique log name using the coordinated universal time.
    /// </summary>
    private string LogName => "Game_" + DateTime.UtcNow.ToString("yyMMdd_HHmmss") + ".txt";

    private string currentLogFileName;

    private void Awake()
    {
        InputMessageExecuter.RecievedMessage += RecievedMessage;
    }

    private void OnDestroy()
    {
        InputMessageExecuter.RecievedMessage -= RecievedMessage;
    }

    /// <summary>
    /// Appends the given message in the current input log,
    /// calls log initialise method if it doesn't exist already.
    /// </summary>
    private void RecievedMessage(string messageString)
    {
        // setup new log
        if (String.IsNullOrWhiteSpace(currentLogFileName))
        {
            InitialiseLog(messageString);
            return;
        }

        AI_File.AppendUTF8(messageString + "\n", PathCurrentLog);
    }

    /// <summary>
    /// creates a new log file in the corresponding folder
    /// </summary>
    private void InitialiseLog(string firstEntry)
    {
        currentLogFileName = LogName;
        AI_File.WriteUTF8(firstEntry + "\n", PathCurrentLog);
    }
}
