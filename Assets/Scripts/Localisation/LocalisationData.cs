using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LocalisationData : ScriptableObject
{
    [System.Serializable]
    public class StringLocalisation
    {
        [HideInInspector] public string identifier;
        public string english;
        public string german;

        /// <summary>
        /// Returns value for the given language.
        /// </summary>
        public string Get(eLanguage language)
        {
            switch (language)
            {
                case eLanguage.English:
                    return english;
                case eLanguage.German:
                    return german;

                default:
                    Debug.LogError($"Localisation UNDEFINED for {language}\n");
                    return $"<{identifier}>";
            }
        }

        /// <summary>
        /// Sets value for the given language.
        /// </summary>
        public void Set(eLanguage language, string value)
        {
            switch (language)
            {
                case eLanguage.English:
                    english = value;
                    break;
                case eLanguage.German:
                    german = value;
                    break;

                default:
                    Debug.LogError($"Localisation UNDEFINED for {language}\n");
                    break;
            }
        }
    }

    // List with all values, to be visible in the editor.
    [SerializeField] private List<StringLocalisation> localisation;
    // Actually used dictionary, since it's more efficient.
    private Dictionary<string, StringLocalisation> organised;

    /// <summary>
    /// Sets up dictionary for more efficient
    /// </summary>
    public void Initialise()
    {
        organised = new Dictionary<string, StringLocalisation>();
        if (localisation == null)
        {
            Debug.LogError($"LocalisationData\tNo localisation stored in {name}!\n", this);
            return;
        }

        foreach (var item in localisation)
            organised.Add(item.identifier, item);
    }

    /// <summary>
    /// Returns localisation for identifier in given language.
    /// </summary>
    public string Get(string identifier, eLanguage language)
    {
        if (organised == null) Initialise();

        if (!organised.ContainsKey(identifier))
        {
            Debug.LogError($"LocalisationData\tNo Entry for identifier '{identifier}' in {name}\n", this);
            return NotFoundPlaceholder(identifier, language);
        }

        return organised[identifier].Get(language);
    }

    /// <summary>
    /// Creates placeholder string for missing localisations
    /// </summary>
    public static string NotFoundPlaceholder(string identifier, eLanguage language)
    {
        return $"<{identifier}_{language.Abbrevation()}>";
    }

    /// <summary>
    /// Updates the list with the given values, sorts them alphabetically, using the identifier.
    /// If called in Editor the asset is set dirty.
    /// </summary>
    public void UpdateList(List<StringLocalisation> newLocalisation)
    {
        localisation = newLocalisation;
        localisation.Sort((x, y) => x.identifier.CompareTo(y.identifier));

        Initialise();

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}


