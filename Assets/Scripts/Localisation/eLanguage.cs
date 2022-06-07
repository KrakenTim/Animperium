using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eLanguage
{
    NONE = 0,
    English = 1,
    German = 2
}

public static class eLanguageExtensions
{
    public static string Abbrevation(this eLanguage language)
    {
        switch (language)
        {
            case eLanguage.English:
                return "en";
            case eLanguage.German:
                return "de";

            default:
                Debug.LogError($"eLanguage\tShort language version is UNDEFINED for {language}!\n");
                return language.ToString();
        }
    }
}