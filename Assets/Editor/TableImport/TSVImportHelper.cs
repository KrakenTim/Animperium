using System;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Contains several helper methods related to tsv table import
/// </summary>
public static class TSVImportHelper
{
    /// <summary>
    /// Returns website given by link as string.
    /// </summary>
    public static string GetWWWViaLink(string link, string friendlyName)
    {
        if (string.IsNullOrWhiteSpace(link)) return "";

#pragma warning disable CS0618 // obsolete
        WWW www = new WWW(link);
#pragma warning restore CS0618

        DateTime maxWaiting = DateTime.UtcNow;
        maxWaiting = maxWaiting.AddSeconds(60);

        //wait until it's done
        while (!www.isDone)
        {
            if (DateTime.UtcNow.CompareTo(maxWaiting) > 0)
            {
                Debug.LogError($"Import\tCould not download {friendlyName} within 60 seconds, download aborted.\n{link}\n");
                return "";
            }
        }

        // worked without errors
        if (string.IsNullOrWhiteSpace(www.error)) return www.text;

        // display error
        Debug.LogError($"Import\t{friendlyName}: WWW ERROR!\n\t{www.error}\n{link}\n");
        return "";
    }

    public static bool TryParse<TEnum>(TSVTable.Line line, string columnName, out TEnum setValue, bool allowEmpty = false) where TEnum : struct
    {
        if (!Enum.TryParse(line[columnName], out setValue))
        {
            if (!(allowEmpty && string.IsNullOrWhiteSpace(line[columnName])))
            {
                ParsingError(line, columnName, typeof(TEnum).Name);
                return false;
            }
        }
        return true;
    }

    public static bool TryParse(TSVTable.Line line, string columnName, out int setValue, bool allowEmpty = false)
    {
        if (!int.TryParse(line[columnName], out setValue))
        {
            if (!(allowEmpty && string.IsNullOrWhiteSpace(line[columnName])))
            {
                ParsingError(line, columnName, typeof(int).Name);
                return false;
            }
        }
        return true;
    }

    public static void ParsingError(TSVTable.Line line, string columnName, string expectedType) =>
        Debug.LogError($"Balancing\tGot no {expectedType} with '{line[columnName]}' in {columnName}\n\t\t{line.ToString()}\n");

    /// <summary>
    /// Recognises Camel Case and places spaces between words.
    /// </summary>
    public static string SplitCamelCase(this string str)
    {
        return Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
    }
}
