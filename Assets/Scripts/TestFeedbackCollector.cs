using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class TestFeedbackCollector
{
    /// <summary>
    /// Application.persistentDataPath + "/Feedback"
    /// </summary>
    public static string PathTempMaps => Path.Combine(Application.persistentDataPath, "Feedback");

    /// <summary>
    /// Expected Format: 0 = yyMMdd, 1 = hhmm
    /// </summary>
    const string fileName = "Feedback_{0}_{1}.txt";
    const string filterSameDay = "Feedback_{0}_*.txt";

    const int charLimitPerFile = 5000;
    const int lineLimitPerFile = 100;

    static string lastStoredFeedback = string.Empty;
    static string currentFeedbackFile = string.Empty;

    public static void AddFeedback(string newFeedback)
    {
        if (string.IsNullOrWhiteSpace(newFeedback)) return;

        newFeedback = newFeedback.Trim();
        if (newFeedback.CompareTo(lastStoredFeedback) == 0)
        {
            Debug.LogWarning("[FeedbackCollector] Same Feedback received twice in a row.");
            return;
        }
        lastStoredFeedback = newFeedback;

        string newFeedbackEntry = CreateFeedbackEntry(newFeedback);

        bool appendOnExisting = false;
        string usedPath;

        if (TryGetLastFeedbackFileOfToday(out usedPath))
        {
            currentFeedbackFile = File.ReadAllText(usedPath);
            appendOnExisting = true;

            // check if new file would be too big
            if (currentFeedbackFile.Length + newFeedbackEntry.Length > charLimitPerFile)
            {
                appendOnExisting = false;
            }
            else
            {
                string[] lines = File.ReadAllLines(usedPath);

                // check if new file would be too long
                if (lines.Length + newFeedbackEntry.Split('\n').Length > lineLimitPerFile)
                    appendOnExisting = false;
            }
        }
        
        if (!appendOnExisting)
        {
            DateTime dateTime = DateTime.Now;
            usedPath = Path.Combine(PathTempMaps,
                                    string.Format(fileName, dateTime.ToString("yyMMdd"), dateTime.ToString("HHmmss")));
        }

        AddTextToFile(usedPath, newFeedbackEntry);
    }

    private static bool TryGetLastFeedbackFileOfToday(out string path)
    {
        path = string.Empty;

        if (!Directory.Exists(PathTempMaps)) return false;

        string searchFilter = string.Format(filterSameDay, DateTime.Now.ToString("yyMMdd"));
        string[] searchResultPaths = Directory.GetFiles(PathTempMaps, searchFilter);
        
        if (searchResultPaths.Length > 0)
        {
            path = searchResultPaths[searchResultPaths.Length - 1];
            return true;
        }

        return false;
    }

    /// <summary>
    /// Adds given Text if the File exists, otherwise creates a new file.
    /// </summary>
    private static void AddTextToFile(string usedPath, string text)
    {
        if (File.Exists(usedPath))
            File.AppendAllText(usedPath, text, Encoding.UTF8);
        else
        {
            text = text.TrimStart();

            Directory.CreateDirectory(Path.GetDirectoryName(usedPath));
            File.WriteAllText(usedPath, text, Encoding.UTF8);
        }

        Debug.Log($"[FeedbackCollector] Successfully saved in {usedPath}\n{text}");
    }

    private static string CreateFeedbackEntry(string feedback)
    {
        return $"{Environment.NewLine}{DateTime.Now.ToLocalTime()}{Environment.NewLine}{feedback}{Environment.NewLine}";
    }
}
