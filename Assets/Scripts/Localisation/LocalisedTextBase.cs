using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles localisation of a string in a Text field.
/// </summary>
public abstract class LocalisedTextBase : MonoBehaviour
{
    public abstract string Identifier { get; protected set; }

    string[] values = null;

    TMPro.TMP_Text myTextfield;

    private void Awake()
    {
        myTextfield = GetComponent<TMPro.TMP_Text>();

        if (myTextfield == null)
            Debug.LogError($"LocalisedString placed on {name} misses a TMPro_Text\n", this);

        Localisation.OnLanguageChanged += UpdateText;

        UpdateText();
    }

    private void OnDestroy()
    {
        Localisation.OnLanguageChanged -= UpdateText;
    }

    /// <summary>
    /// Changes or sets a value used for a variable in the localisation string.
    /// </summary>
    public void SetValue(string value, int pos = 0)
    {
        if (values == null)
            values = new string[pos + 1];

        if (values.Length <= pos)
        {
            string[] newValues = new string[pos + 1];
            values.CopyTo(newValues, 0);
            values = newValues;
        }

        values[pos] = value;

        UpdateText();
    }

    /// <summary>
    /// Changes or sets all values used in variables of the localisation string.
    /// </summary>
    public void SetValues(string[] values)
    {
        this.values = values;

        UpdateText();
    }

    /// <summary>
    /// Gets the localised string of the current language and fills it's variables.
    /// </summary>
    protected void UpdateText()
    {
        if (myTextfield == null) return;

        string shownString = Localisation.Instance.Get(Identifier);

        if (values != null)
        {
            for (int i = 0; i < values.Length; i++)
                shownString = shownString.Replace("{value" + (i + 1) + "}", values[i]);
        }

        myTextfield.text = shownString;
    }

    /// <summary>
    /// Enables and Disables the connected text field.
    /// </summary>
    public void SetVisible(bool textVisible)
    {
        myTextfield.enabled = textVisible;
    }

    /// <summary>
    /// Seeks LocalisedTexts and Localisation in the Scene and fills them with their localisation string.
    /// Aimed at use in Editor.
    /// </summary>
    public static void FillAllTextsWithBaseLocalisation(eLanguage usedLanguage = eLanguage.English)
    {
        LocalisedText[] localisedTexts = FindObjectsOfType<LocalisedText>(true);

        Localisation localisation = Localisation.Instance;
        if (localisation == null)
            localisation = FindObjectOfType<Localisation>();

        if (!localisation)
        {
            Debug.LogError("No Localisation Prefab in Scene\n");
            return;
        }

        foreach (var textElement in localisedTexts)
        {
            string nextLocalisation = localisation.Get(textElement.Identifier, usedLanguage);

            TMP_Text textField = textElement.GetComponent<TMP_Text>();

            if (textField.text != nextLocalisation)
            {
                textField.text = nextLocalisation;

#if UNITY_EDITOR // only part of code compiled for editor

                // flag element dirty for saving, if function wasn't called in play mode
                if (!Application.isPlaying)
                    UnityEditor.EditorUtility.SetDirty(textElement);
#endif // UNITY_EDITOR
            }
        }
    }
}
