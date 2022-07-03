using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extends Localisation with project specific elements
/// </summary>
public class AnimperiumLocalisation : Localisation
{
    // code referenced ids, so we only need to change them here and see where they are used.
    public const string ID_AttackPower = "AttackPower";
    public const string ID_CantDigThrough = "CantDigThrough";
    public const string ID_NotEnoughResources = "NotEnoughResources";
    public const string ID_NotEnoughUpgradesMany = "NotEnoughUpgradesMany";
    public const string ID_NotEnoughUpgradesOne = "NotEnoughUpgradesOne";
    public const string ID_Population = "Population";
    public const string ID_TurnTime = "TurnTime";

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
    /// Returns identifier for given pawn.
    /// </summary>
    public static string GetIdentifier(ePlayerPawnType pawn)
    {
        return pawn.ToString();
    }
}
