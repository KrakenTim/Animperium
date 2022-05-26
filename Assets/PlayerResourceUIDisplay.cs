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

    private void OnEnable()
    {
        if (isLocalPlayerValues)
        {
            PlayerValues.OnValuesChanged += CheckIfLocalPlayerValuesChanged;
            ShowLocalPlayerValues();
        }
    }

    private void OnDisable()
    {
        if (isLocalPlayerValues)
            PlayerValues.OnValuesChanged -= CheckIfLocalPlayerValuesChanged;
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
    public void ShowLocalPlayerValues()
    {
        if (!GameManager.PlayerValueProvider.TryGetPlayerValues(GameManager.LocalPlayerID, out PlayerValues localPlayerValues))
            return;

        ShowResources(localPlayerValues.PlayerResources);

        if (population)
            population.text = localPlayerValues.PopulationCount + "/" + GameManager.MaxPopulation + (showType ? " Population" : "");
    }

    /// <summary>
    /// Sets Texts with values of given resources.
    /// </summary>
    public void ShowResources(GameResources resources)
    {
        food.text = resources.food + (showType ? " Food" : "");
        wood.text = resources.wood + (showType ? " Wood" : "");
        ore.text = resources.ore + (showType ? " Ore" : "");
    }
}
