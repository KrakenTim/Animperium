using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class LocalisationImport
{
    const string PATH_LocalisationDataFolder = "Assets/ScriptableObjects";

    // tsv download link to google table with localisation
    const string DOWNLOAD_LocalisationTable = "https://docs.google.com/spreadsheets/d/e/2PACX-1vTnXBX5TCL7N-jj28tutZwsss1pMZmamyOjbzMPjwsb4Ow7Tdh-PT79lZScjUP4Dr34X8gwdy2VjCjS/pub?output=tsv";
    const string NAME_LocalisationTable = "Localisation Table";

    const string COLUMN_IDENTIFIER = "Identifier";

    /// <summary>
    /// gets the current table fom google and applies it's values to the scriptable object
    /// </summary>
    [MenuItem("Tools/Import Table/Localisation")]
    public static void UpdateTable()
    {
        string tableAsString = TSVImportHelper.GetWWWViaLink(DOWNLOAD_LocalisationTable, NAME_LocalisationTable);

        if (tableAsString.Length == 0)
        {
            Debug.LogError("Table Download Failed, Update stopped.\n");
            return;
        }

        if (!TryGetLocalisationData(out LocalisationData localisationData)) return;

        TSVTable table = TSVTable.Create(tableAsString);


        localisationData.UpdateList(CreateLocalisationList(table, NAME_LocalisationTable));

        Debug.Log($"LocalisationImport\t{NAME_LocalisationTable} import finished.\n");
    }

    /// <summary>
    /// Creates a list with all entries of the given table, fills missing entries with placeholders.
    /// </summary>
    private static List<LocalisationData.StringLocalisation> CreateLocalisationList(TSVTable table, string tableName = null)
    {
        List<eLanguage> languages = GetTableLanguages(table, tableName);
        
        List<LocalisationData.StringLocalisation> importLocalisations = new List<LocalisationData.StringLocalisation>();

        HashSet<string> existingIdentifiers = new HashSet<string>();

        foreach (var line in table)
        {
            LocalisationData.StringLocalisation nextEntry = new LocalisationData.StringLocalisation();

            nextEntry.identifier = line[COLUMN_IDENTIFIER];

            // skip rows without proper identifier
            if (string.IsNullOrWhiteSpace(nextEntry.identifier)) continue;

            // fill for all languages
            foreach (var language in languages)
            {
                string nextLocalisation = line[language.ToString()];

                if (string.IsNullOrWhiteSpace(nextLocalisation))
                    nextLocalisation = LocalisationData.NotFoundPlaceholder(nextEntry.identifier, language);

                nextEntry.Set(language, nextLocalisation);
            }

            // check duplicates
            if (existingIdentifiers.Contains(nextEntry.identifier))
            {
                Debug.LogError($"LocalisationImport\t{(tableName!= null?tableName:"Table")} contains two entries for identifier: '{nextEntry.identifier}'\n");
                continue;
            }
            existingIdentifiers.Add(nextEntry.identifier);

            importLocalisations.Add(nextEntry);
        }

        return importLocalisations;
    }

    /// <summary>
    /// Returns list of languages which are present in the table
    /// </summary>
    private static List<eLanguage> GetTableLanguages(TSVTable localisationTable, string tableName = null)
    {
        HashSet<string> tableColumns = localisationTable.Columns;
        List<eLanguage> languages = new List<eLanguage>();

        foreach (eLanguage language in System.Enum.GetValues(typeof(eLanguage)))
        {
            if (language == eLanguage.NONE) continue;

            if (tableColumns.Contains(language.ToString()))
                languages.Add(language);
            else
                Debug.LogWarning($"LocalisationImport\t{(tableName != null ? tableName : "Table")} doesn't contain a column for {language}\n");
        }

        return languages;
    }

    /// <summary>
    /// Tries to find localisation datas and returns the first found if successfull.
    /// </summary>
    private static bool TryGetLocalisationData(out LocalisationData result)
    {
        var localisationDatasInFolder = AI_File.EDITOR_GetAssets<LocalisationData>(PATH_LocalisationDataFolder);

        if (localisationDatasInFolder.Count > 1)
        {
            Debug.LogError($"LocalisationImport\tFound {localisationDatasInFolder.Count} LocalisationDatas in" +
                           $" {PATH_LocalisationDataFolder}, will use the first.\n");
        }

        // returns the first found 
        foreach (var item in localisationDatasInFolder)
        {
            result = item.Value;
            return true;
        }

        Debug.LogError($"LocalisationImport\tFound no LocalisationData in {PATH_LocalisationDataFolder}.\n");

        result = null;
        return false;
    }
}
