#if UNITY_EDITOR //whole file
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// allows importing the pawn balance table, provides method to update PawnData
/// </summary>
public static class BalanceTableImport
{
    const string PATH_PawnDataFolder = "Assets/ScriptableObjects";

    // tsv download link to google table with balancing values
    const string LINK_PawnBalanceTable = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQsO5q96lexrT0dgMvmnYmVbh0LaXsPRdO5cpeE8OoUk35qHsWVbuYhGjp6uMZ-D-FpfVSVu8Td94jB/pub?gid=0&single=true&output=tsv";

    const string NAME_BalancingTable = "Pawn Balance Table";

    const string COLUMN_PawnType = "PawnType";
    const string COLUMN_MaxHealth = "MaxHealth";
    const string COLUMN_MaxMovement = "MaxMovement";
    const string COLUMN_AttackPower = "AttackPower";

    const string COLUMN_FoodCost = "FoodCost";

    const string COLUMN_SpawnedPawn = "SpawnedPawn";
    const string COLUMN_LearnFight = "LearnFight";
    const string COLUMN_LearnMagic = "LearnMagic";
    const string COLUMN_LearnDigging = "LearnDigging";

    /// <summary>
    /// gets the current table fom google and applies it's values to the scriptable objects
    /// </summary>
    [MenuItem("Tools/Import Table/Pawn Balancing")]
    public static void UpdateBalancingTable()
    {
        string tableAsString = GetTSVTableViaLink(LINK_PawnBalanceTable, NAME_BalancingTable);

        var pawnDatas = AI_File.EDITOR_GetAssets<PlayerPawnData>(PATH_PawnDataFolder);

        ePlayerPawnType nextPawnType;
        foreach (var line in TSVTable.Create(tableAsString))
        {
            if (!TryParse(line, COLUMN_PawnType, out nextPawnType)) continue;

            foreach (var pawnData in pawnDatas.Values)
            {
                if (pawnData.type == nextPawnType)
                {
                    ApplyTSVLine(pawnData, line);
                    continue;
                }
            }
        }
        Debug.Log("Balancing Table Import finished.");
    }

    static void ApplyTSVLine(PlayerPawnData pawnData, TSVTable.Line pawnTableRow)
    {
        ePlayerPawnType nextPawnType;
        int nextValue;
        bool wasChanged = false;

        // stats

        if (TryParse(pawnTableRow, COLUMN_MaxHealth, out nextValue) && pawnData.maxHealth != nextValue)
        {
            pawnData.maxHealth = nextValue;
            wasChanged = true;
        }

        if (TryParse(pawnTableRow, COLUMN_MaxMovement, out nextValue) && pawnData.maxMovement != nextValue)
        {
            pawnData.maxMovement = nextValue;
            wasChanged = true;
        }

        if (TryParse(pawnTableRow, COLUMN_AttackPower, out nextValue) && pawnData.attackPower != nextValue)
        {
            pawnData.attackPower = nextValue;
            wasChanged = true;
        }

        // resource costs

        if (TryParse(pawnTableRow, COLUMN_FoodCost, out nextValue) && pawnData.food != nextValue)
        {
            pawnData.food = nextValue;
            wasChanged = true;
        }

        // spawn

        if (TryParse(pawnTableRow, COLUMN_SpawnedPawn, out nextPawnType, allowEmpty: true) && pawnData.spawnedPawn != nextPawnType)
        {
            pawnData.spawnedPawn = nextPawnType;
            wasChanged = true;
        }

        // learning

        if (TryParse(pawnTableRow, COLUMN_LearnFight, out nextPawnType, allowEmpty: true) && pawnData.learnsFight != nextPawnType)
        {
            pawnData.learnsFight = nextPawnType;
            wasChanged = true;
        }
        if (TryParse(pawnTableRow, COLUMN_LearnMagic, out nextPawnType, allowEmpty: true) && pawnData.learnsMagic != nextPawnType)
        {
            pawnData.learnsMagic = nextPawnType;
            wasChanged = true;
        }
        if (TryParse(pawnTableRow, COLUMN_LearnDigging, out nextPawnType, allowEmpty: true) && pawnData.learnsDigging != nextPawnType)
        {
            pawnData.learnsDigging = nextPawnType;
            wasChanged = true;
        }

        if (wasChanged)
        {
            Debug.Log($"Balancing Import updated {pawnData.type}\n");
            EditorUtility.SetDirty(pawnData);
        }
    }

    /// <summary>
    /// returns website given by link as string
    /// </summary>
    static string GetTSVTableViaLink(string link, string tableName)
    {
        if (string.IsNullOrWhiteSpace(link)) return "";

#pragma warning disable CS0618 // obsolete
        WWW www = new WWW(link);
#pragma warning restore CS0618

        //wait until it's done, causes issues without web connection
        while (!www.isDone)
        {
        }

        // worked without errors
        if (string.IsNullOrWhiteSpace(www.error)) return www.text;

        // display error
        Debug.LogError($"Import\t{tableName}: WWW ERROR!\n\t{www.error}\n{link}\n");
        return "";
    }

    static bool TryParse<TEnum>(TSVTable.Line line, string columnName, out TEnum setValue, bool allowEmpty = false) where TEnum : struct
    {
        if (!Enum.TryParse(line[columnName], out setValue))
        {
            if (!(allowEmpty && line[columnName].Length == 0))
            {
                ParsingError(line, columnName, typeof(TEnum).Name);
                return false;
            }
        }
        return true;
    }

    static bool TryParse(TSVTable.Line line, string columnName, out int setValue)
    {
        if (int.TryParse(line[columnName], out setValue) == false)
        {
            ParsingError(line, columnName, typeof(int).Name);
            return false;
        }
        return true;
    }

    static void ParsingError(TSVTable.Line line, string columnName, string expectedType) =>
       Debug.LogError($"Balancing\tGot no {expectedType} with '{line[columnName]}' in {columnName}\n\t\t{line.ToString()}\n");
}

#endif // UNITY_EDITOR - whole file