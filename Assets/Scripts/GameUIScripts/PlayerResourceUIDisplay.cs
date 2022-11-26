using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerResourceUIDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text food;
    [SerializeField] TMP_Text wood;
    [SerializeField] TMP_Text ore;
    [SerializeField] TMP_Text population;

    [SerializeField] bool showType;
    [SerializeField] bool isLocalPlayerValues;

    GameResources usedResources;
    GameResources gainPerTurn;

    private void OnEnable()
    {
        if (isLocalPlayerValues)
        {
            PlayerValues.OnValuesChanged += CheckIfLocalPlayerValuesChanged;
            GameManager.LocalPlayerChanged += ShowLocalPlayerValues;

            ShowLocalPlayerValues();
        }

        Localisation.OnLanguageChanged += UpdateTexts;
    }

    private void OnDisable()
    {
        if (isLocalPlayerValues)
        {
            PlayerValues.OnValuesChanged -= CheckIfLocalPlayerValuesChanged;
            GameManager.LocalPlayerChanged -= ShowLocalPlayerValues;
        }

        Localisation.OnLanguageChanged -= UpdateTexts;
    }

    /// <summary>
    /// If the given value is identical to the local player id, the shown values are updated.
    /// </summary>
    private void CheckIfLocalPlayerValuesChanged(int changedPlayerID)
    {
        if (changedPlayerID == GameManager.LocalPlayerID)
            ShowLocalPlayerValues();
    }

    /// <summary>
    /// Update resources and (if existing) population count with the current values of the local player.
    /// </summary>
    private void ShowLocalPlayerValues()
    {
        if (!GameManager.PlayerValueProvider.TryGetPlayerValues(GameManager.LocalPlayerID, out PlayerValues localPlayerValues))
            return;

        ShowResources(localPlayerValues.PlayerResources, localPlayerValues.RessourcesPerTurn);

        UpdatePopulationText(localPlayerValues);
    }

    /// <summary>
    /// Sets Texts with values of given resources.
    /// </summary>
    public void ShowResources(GameResources playerResources, GameResources gainPerTurn)
    {
        usedResources = playerResources;
        this.gainPerTurn = gainPerTurn;

        UpdateRessourceTexts();
    }

    /// <summary>
    /// Updates Resource and population text.
    /// </summary>
    private void UpdateTexts()
    {
        UpdateRessourceTexts();
        UpdatePopulationText();
    }

    private void UpdateRessourceTexts()
    {
        UpdateRessourceTexts(eResourceType.Food);
        UpdateRessourceTexts(eResourceType.Wood);
        UpdateRessourceTexts(eResourceType.Ore);
    }

    private void UpdateRessourceTexts(eResourceType resource)
    {
        TMP_Text usedText;
        switch (resource)
        {
            case eResourceType.Food:
                usedText = food;
                break;
            case eResourceType.Wood:
                usedText = wood;
                break;
            case eResourceType.Ore:
                usedText = ore;
                break;

            default:
                Debug.LogError($"[Resource Display] UNDEFINED for {resource}", this);
                return;
        }

        usedText.text = usedResources[resource] + (gainPerTurn[resource] != 0 ? $" (+{gainPerTurn[resource]})" : "")
                        + (showType ? "\n" + AnimperiumLocalisation.Get(resource) : "");
    }

    private void UpdatePopulationText(PlayerValues localPlayerValues = null)
    {
        if (population == null) return;

        if (localPlayerValues == null
           && !GameManager.PlayerValueProvider.TryGetPlayerValues(GameManager.LocalPlayerID, out localPlayerValues))
        {
            return;
        }

        population.text = localPlayerValues.PopulationCount + "/" + localPlayerValues.PopulationMax
                          + (showType ? " " + Localisation.Instance.Get(AnimperiumLocalisation.ID_Population) : "");
    }
}
