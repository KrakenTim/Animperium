using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InputLogHandler : MonoBehaviour
{
    private string PathCurrentLog => AI_File.PathInputLogs + currentLogFileName;
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
    /// creates a new log file in the corresponding folder
    /// </summary>
    private void InitialiseLog(string firstEntry)
    {
        currentLogFileName = "Game_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + ".ilog";
        AI_File.WriteUTF8(firstEntry + Environment.NewLine, PathCurrentLog);
    }

    private void RecievedMessage(string messageString)
    {
        // setup new log
        if (String.IsNullOrWhiteSpace(currentLogFileName))
        {
            InitialiseLog(messageString);
            return;
        }

        AI_File.AppendUTF8(messageString + Environment.NewLine, PathCurrentLog);
    }
}
