using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eLanguage
{
    NONE = 0,
    English = 1,
    German = 2,
    Polish = 3,
    Spanish = 4,
    Italian = 5
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
            case eLanguage.Polish:
                return "pl";
            case eLanguage.Spanish:
                return "es";
            case eLanguage.Italian:
                return "it";

            default:
                Debug.LogError($"eLanguage\tShort language version is UNDEFINED for {language}!\n");
                return language.ToString();
        }
    }
}