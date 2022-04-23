using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class TSVTable
{
    const string SEPARATOR = "\t";

    // colum name, column position
    Dictionary<string, int> header = new Dictionary<string, int>();

    // each line stores an array of values
    private string[][] dataLines;

    string tsv;
    string separator;

    public int Length => dataLines.Length;

    public Line this[int lineIndex] => new Line(this, lineIndex);

    private TSVTable(string tableAsString, string separator = SEPARATOR)
    {
        tsv = tableAsString;
        this.separator = separator;
    }

    public static TSVTable Create(string tsvContent, string separator = SEPARATOR)
    {
        TSVTable result = new TSVTable(tsvContent, separator);

        string[] lines = tsvContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        result.dataLines = new string[lines.Length - 1][];

        // Header
        string[] values = Regex.Split(lines[0], separator);
        string nextEntry;
        for (int i = 0; i < values.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(values[i])) continue;

            nextEntry = values[i].Trim();

            if (!result.header.ContainsKey(nextEntry))
                result.header.Add(nextEntry, i);
            else
                Debug.LogError($"TSVTable\tTable Header contains {nextEntry} twice.\n\t\t{lines[0]}\n");
        }

        // Content
        for (int lineIdx = 1; lineIdx < lines.Length; ++lineIdx)
            result.dataLines[lineIdx - 1] = Regex.Split(lines[lineIdx], separator);

        return result;
    }

    public IEnumerator<Line> GetEnumerator()
    {
        for (int i = 0; i < dataLines.Length; ++i)
            yield return new Line(this, i);
    }

    public override string ToString() => tsv;

    #region Line SubClass

    public class Line
    {
        private readonly int lineIndex;
        private readonly TSVTable owningTSV;

        public Line(TSVTable tsvHelper, int lineIndex)
        {
            owningTSV = tsvHelper;
            this.lineIndex = lineIndex;
        }

        public bool Contains(string columnName) => owningTSV.header.ContainsKey(columnName);

        public string this[string columnName]
        {
            get
            {
                int columnIndex = owningTSV.header[columnName];
                return this[columnIndex];
            }
        }

        public string this[int columnIndex]
        {
            get
            {
                Debug.Assert(columnIndex >= 0 && columnIndex < owningTSV.dataLines[lineIndex].Length);

                return owningTSV.dataLines[lineIndex][columnIndex];
            }
        }

        public override string ToString()
        {
            string[] lineEntries = owningTSV.dataLines[lineIndex];
            if (lineEntries.Length == 0) return "";

            string lineAsString = lineEntries[0];
            for (int i = 1; i < lineEntries.Length; i++)
            {
                lineAsString += owningTSV.separator + lineEntries[i];
            }
            return lineAsString;
        }
    }

    #endregion Line SubClass
}