using UnityEditor;
using UnityEngine;

/// <summary>
/// allows importing the pawn balance table, provides method to update PawnData
/// </summary>
public static class BalanceTableImport
{
    const string PATH_PawnDataFolder = "Assets/ScriptableObjects";

    // tsv download link to google table with balancing values
    const string DOWNLOAD_PawnBalanceTable = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQsO5q96lexrT0dgMvmnYmVbh0LaXsPRdO5cpeE8OoUk35qHsWVbuYhGjp6uMZ-D-FpfVSVu8Td94jB/pub?gid=0&single=true&output=tsv";

    const string NAME_BalancingTable = "Pawn Balance Table";

    #region table column names
    //Characteristics
    const string COLUMN_PawnType = "PawnType";
    const string COLUMN_FriendlyName = "FriendlyName";
    const string COLUMN_Tier = "Tier";
    const string COLUMN_UpgradesForUnlock = "UpgradesForUnlock";

    // Stats
    const string COLUMN_MaxHealth = "MaxHealth";
    const string COLUMN_MaxMovement = "MaxMovement";
    const string COLUMN_AttackPower = "AttackPower";
    const string COLUMN_ViewRange = "ViewRange";
    const string COLUMN_SpecialPower = "SpecialPower";

    // Costs
    const string COLUMN_FoodCost = "FoodCost";
    const string COLUMN_WoodCost = "WoodCost";
    const string COLUMN_OreCost = "OreCost";
    const string COLUMN_PopulationCount = "PopulationCount";

    // Spawn & Upgrades
    const string COLUMN_SpawnedPawn = "SpawnedPawn";
    const string COLUMN_LearnFight = "LearnFight";
    const string COLUMN_LearnMagic = "LearnMagic";
    const string COLUMN_LearnDigging = "LearnDigging";
    const string COLUMN_LinearUpgrade = "LinearUpgrade";
    #endregion table column names

    /// <summary>
    /// gets the current table fom google and applies it's values to the scriptable objects
    /// </summary>
    [MenuItem("Tools/Import Table/Pawn Balancing")]
    public static void UpdateTable()
    {
        string tableAsString = TSVImportHelper.GetWWWViaLink(DOWNLOAD_PawnBalanceTable, NAME_BalancingTable);

        if (tableAsString.Length == 0)
        {
            Debug.LogError("Table Download Failed, Update stopped.\n");
            return;
        }

        var pawnDatas = AI_File.EDITOR_GetAssets<PlayerPawnData>(PATH_PawnDataFolder);

        ePlayerPawnType nextPawnType;
        foreach (var line in TSVTable.Create(tableAsString))
        {
            if (!TSVImportHelper.TryParse(line, COLUMN_PawnType, out nextPawnType)) continue;

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

        // Friendly Name
        string newName = pawnTableRow[COLUMN_FriendlyName];
        if (string.IsNullOrWhiteSpace(newName))
            newName = TSVImportHelper.SplitCamelCase(pawnData.type.ToString());
        if (pawnData.friendlyName != newName)
        {
            pawnData.friendlyName = newName;
            wasChanged = true;
        }

        if (TSVImportHelper.TryParse(pawnTableRow, COLUMN_Tier, out nextValue) && pawnData.tier != nextValue)
        {
            pawnData.tier = nextValue;
            wasChanged = true;
        }
        if (TSVImportHelper.TryParse(pawnTableRow, COLUMN_UpgradesForUnlock, out nextValue, allowEmpty: true) && pawnData.upgradesForUnlock != nextValue)
        {
            pawnData.upgradesForUnlock = nextValue;
            wasChanged = true;
        }

        // Stats       
        if (TSVImportHelper.TryParse(pawnTableRow, COLUMN_MaxHealth, out nextValue) && pawnData.maxHealth != nextValue)
        {
            pawnData.maxHealth = nextValue;
            wasChanged = true;
        }
        if (TSVImportHelper.TryParse(pawnTableRow, COLUMN_MaxMovement, out nextValue) && pawnData.maxMovement != nextValue)
        {
            pawnData.maxMovement = nextValue;
            wasChanged = true;
        }
        if (TSVImportHelper.TryParse(pawnTableRow, COLUMN_AttackPower, out nextValue) && pawnData.attackPower != nextValue)
        {
            pawnData.attackPower = nextValue;
            wasChanged = true;
        }
        if (TSVImportHelper.TryParse(pawnTableRow, COLUMN_ViewRange, out nextValue) && pawnData.viewRange != nextValue)
        {
            pawnData.viewRange = nextValue;
            wasChanged = true;
        }
        if (TSVImportHelper.TryParse(pawnTableRow, COLUMN_SpecialPower, out nextValue, allowEmpty:true) && pawnData.specialPower != nextValue)
        {
            pawnData.specialPower = nextValue;
            wasChanged = true;
        }

        // Costs
        if (TSVImportHelper.TryParse(pawnTableRow, COLUMN_FoodCost, out nextValue) && pawnData.resourceCosts.food != nextValue)
        {
            pawnData.resourceCosts.food = nextValue;
            wasChanged = true;
        }
        if (TSVImportHelper.TryParse(pawnTableRow, COLUMN_WoodCost, out nextValue) && pawnData.resourceCosts.wood != nextValue)
        {
            pawnData.resourceCosts.wood = nextValue;
            wasChanged = true;
        }
        if (TSVImportHelper.TryParse(pawnTableRow, COLUMN_OreCost, out nextValue) && pawnData.resourceCosts.ore != nextValue)
        {
            pawnData.resourceCosts.ore = nextValue;
            wasChanged = true;
        }
        if (TSVImportHelper.TryParse(pawnTableRow, COLUMN_PopulationCount, out nextValue) && pawnData.populationCount != nextValue)
        {
            pawnData.populationCount = nextValue;
            wasChanged = true;
        }

        // Spawn
        if (TSVImportHelper.TryParse(pawnTableRow, COLUMN_SpawnedPawn, out nextPawnType, allowEmpty: true) && pawnData.spawnedPawn != nextPawnType)
        {
            pawnData.spawnedPawn = nextPawnType;
            wasChanged = true;
        }

        // Upgrades
        if (TSVImportHelper.TryParse(pawnTableRow, COLUMN_LearnFight, out nextPawnType, allowEmpty: true) && pawnData.learnsFight != nextPawnType)
        {
            pawnData.learnsFight = nextPawnType;
            wasChanged = true;
        }
        if (TSVImportHelper.TryParse(pawnTableRow, COLUMN_LearnMagic, out nextPawnType, allowEmpty: true) && pawnData.learnsMagic != nextPawnType)
        {
            pawnData.learnsMagic = nextPawnType;
            wasChanged = true;
        }
        if (TSVImportHelper.TryParse(pawnTableRow, COLUMN_LearnDigging, out nextPawnType, allowEmpty: true) && pawnData.learnsDigging != nextPawnType)
        {
            pawnData.learnsDigging = nextPawnType;
            wasChanged = true;
        }
        if (TSVImportHelper.TryParse(pawnTableRow, COLUMN_LinearUpgrade, out nextPawnType, allowEmpty: true) && pawnData.linearUpgrade != nextPawnType)
        {
            pawnData.linearUpgrade = nextPawnType;
            wasChanged = true;
        }

        if (wasChanged)
        {
            Debug.Log($"Balancing Import updated {pawnData.type}\n", pawnData);
            EditorUtility.SetDirty(pawnData);
        }
    }   
}