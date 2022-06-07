using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extends Localisation with project specific elements
/// </summary>
public class AnimperiumLocalisation : Localisation
{
    public const string IDENTIFIER_Population = "Population";
    public const string IDENTIFIER_TurnTime = "TurnTime";

    /// <summary>
    /// Returns localisation for given resource in current language.
    /// </summary>
    public static string Get(eResourceType resource)
    {
        return Instance.Get(GetIdentifier(resource));
    }

    /// <summary>
    /// Returns localisation for given pawn in current language.
    /// </summary>
    public static string Get(ePlayerPawnType pawn)
    {
        return Instance.Get(GetIdentifier(pawn));
    }

    /// <summary>
    /// Returns identifier for given resource.
    /// </summary>
    public static string GetIdentifier(eResourceType resource)
    {
        return resource.ToString();
    }

    /// <summary>
    /// Returns localisation for given pawn.
    /// </summary>
    public static string GetIdentifier(ePlayerPawnType pawn)
    {
        return pawn.ToString();
    }
}
