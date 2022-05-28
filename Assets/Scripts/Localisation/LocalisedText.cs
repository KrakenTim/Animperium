using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles localisation of a string in a Text field.
/// Provides field to set the used identifier.
/// </summary>
public class LocalisedText : LocalisedTextBase
{
    [SerializeField] private string usedIdentifier;

    /// <summary>
    /// The Identifier used for localisation.
    /// if set, the text is updated. 
    /// </summary>
    public override string Identifier
    {
        get => usedIdentifier;
        protected set
        {
            usedIdentifier = value;
            UpdateText();
        }
    }

    /// <summary>
    /// Changes the used identifier.
    /// </summary>
    public void Set(string identifier)
    {
        Identifier = identifier;
    }

    /// <summary>
    /// Changes the used identifier and the values used for variables in the localised string.
    /// </summary>
    public void Set(string identifier, string[] values)
    {
        SetValues(values);

        Identifier = identifier;
    }

    /// <summary>
    /// Changes the used identifier and the value used for the first variable in the localised string.
    /// </summary>
    public void Set(string identifier, string value)
    {
        SetValue(value);

        Identifier = identifier;
    }
}
