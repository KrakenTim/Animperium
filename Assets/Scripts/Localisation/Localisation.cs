using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Localisation : MonoBehaviour
{
    private const string LANGUAGE = "Language";

    public static System.Action OnLanguageChanged;

    [SerializeField] eLanguage usedLanguage = eLanguage.English;
    public eLanguage CurrentLanguage => usedLanguage;

    public static Localisation Instance { get; private set; }

    [SerializeField] LocalisationData localisationData;

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);

        if (PlayerPrefs.HasKey(LANGUAGE)
            && System.Enum.TryParse(PlayerPrefs.GetString(LANGUAGE), out eLanguage preferredLanguage)
            && preferredLanguage != eLanguage.NONE)
        {
            usedLanguage = preferredLanguage;
        }

        localisationData.Initialise();
        SetLanguage(usedLanguage, enforce: true);
    }

    protected virtual void OnDestroy()
    {
        if (Instance == null) Instance = null;

        SaveLanguage();
    }

    /// <summary>
    /// Return localisation of given identifier in the currently used language.
    /// </summary>
    public string Get(string identifier) => Get(identifier, usedLanguage);

    /// <summary>
    /// Return localisation of given identifier in the given language.
    /// </summary>
    public string Get(string identifier, eLanguage language)
    {
        return localisationData.Get(identifier, language);
    }

    /// <summary>
    /// Changes from the current to a different language;
    /// </summary>
    public void SetLanguage(eLanguage newLanguage, bool enforce = false)
    {
        if (!enforce && newLanguage == usedLanguage) return;

        usedLanguage = newLanguage;

        OnLanguageChanged?.Invoke();
    }

    /// <summary>
    /// stores the used language in the player prefs
    /// </summary>
    private void SaveLanguage()
    {
        if (usedLanguage != eLanguage.NONE)
            PlayerPrefs.SetString(LANGUAGE, usedLanguage.ToString());
    }
}