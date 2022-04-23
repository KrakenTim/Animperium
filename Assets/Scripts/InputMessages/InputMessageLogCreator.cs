using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InputMessageLogCreator : MonoBehaviour
{
    /// <summary>
    /// how many logs are kept
    /// </summary>
    const int logLimit = 10;

    /// <summary>
    /// input log file extension (without the dot)
    /// </summary>
    const string logFileExtension = "iLog";

    private string PathCurrentLog => AI_File.PathInputLogs + currentLogFileName;

    private string currentLogFileName;

    private void Awake()
    {
        InputMessageExecuter.RecievedOrder += RecievedMessage;
    }

    private void OnDestroy()
    {
        InputMessageExecuter.RecievedOrder -= RecievedMessage;

        ClearLogsExceptNewest(logLimit);
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
        currentLogFileName = CreateLogName();
        AI_File.WriteUTF8(firstEntry + "\n", PathCurrentLog);
    }

    /// <summary>
    /// Generates a unique log name using the coordinated universal time.
    /// </summary>
    private string CreateLogName()
    {
        return $"Game_{DateTime.UtcNow.ToString("yyMMdd_HHmmss")}.{logFileExtension}";
    }

    /// <summary>
    /// removes all logs except the newest, if a number of logs that should be kept is given.
    /// </summary>
    public static void ClearLogsExceptNewest(int keptLogs)
    {
        // get paths of input logs
        List<string> logPaths = new List<string>(Directory.GetFiles(AI_File.PathInputLogs, $"*.{logFileExtension}",
                                                                      SearchOption.AllDirectories));

        // remove logs, except the newest
        for (int i = 0; i < logPaths.Count - keptLogs; i++)
        {
            File.Delete(logPaths[i]);
            File.Delete(logPaths[i] + ".meta");
        }
    }
}