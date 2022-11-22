using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the menu to place buildings and upgrade units.
/// </summary>
public class InteractionMenuManager : MonoBehaviour
{
    private static InteractionMenuManager instance;

    [SerializeField] InteractionContextButton buttonPrefab;
    [Space]
    [SerializeField] GameObject buttonFrame;
    [SerializeField] GameObject mouseRaycastBlocker;

    List<InteractionContextButton> buttonList;

    private void Awake()
    {
        instance = this;

        GameManager.TurnStarted += TurnStarted;

        buttonList = new List<InteractionContextButton>(buttonFrame.GetComponentsInChildren<InteractionContextButton>());

        SetVisible(false);
    }

    private void OnDestroy()
    {
        if (instance = this)
            instance = null;

        GameManager.TurnStarted -= TurnStarted;
    }

    public static void OpenUpgradeMenu(HexCell targetCell, PlayerPawnData actingUnit) =>
       instance.OpenPawnCreationMenu(targetCell, actingUnit, isBuildNotUpgrade: false);

    public static void OpenBuildMenu(HexCell targetCell, PlayerPawnData actingUnit) =>
       instance.OpenPawnCreationMenu(targetCell, actingUnit, isBuildNotUpgrade: true);

    public static void Close() => instance.SetVisible(false);

    private void OpenPawnCreationMenu(HexCell targetCell, PlayerPawnData actingUnit, bool isBuildNotUpgrade)
    {
        RemoveButtons();

        if (isBuildNotUpgrade)
            CreateButtons_Build(actingUnit.type, targetCell);
        else
        {
            CreateButtons_UnitUpgrade(targetCell, actingUnit);
            CreateButtons_BuildingUpgrade(targetCell, actingUnit);
        }

        SetVisible(instance.buttonList.Count > 0);
    }

    private void CreateButtons_Build(ePlayerPawnType builderType, HexCell targetCell)
    {
            instance.CreateButtons(GameManager.GetPossibleBuildingDatas(builderType),
                                         ePlayeractionType.Build, targetCell, null);
    }

    private void CreateButtons_UnitUpgrade(HexCell targetCell, PlayerPawnData actingUnit)
    {
        CreateButtons(actingUnit.AllPossiblesUnitUpgrades(targetCell.Pawn.PawnData.tier),
                                        ePlayeractionType.UnitUpgrade, targetCell, actingUnit);
    }

    private void CreateButtons_BuildingUpgrade(HexCell targetCell, PlayerPawnData actingUnit)
    {
        if (!targetCell.HasPawn || actingUnit.type != ePlayerPawnType.Villager) return;

        if (targetCell.Pawn.PawnData.linearUpgrade != ePlayerPawnType.NONE)
        {
            PlayerPawnData upgradedPawn = GameManager.GetPawnData(targetCell.Pawn.PawnData.linearUpgrade);

            CreateButtons(new List<PlayerPawnData>() { upgradedPawn }, ePlayeractionType.BuildingUpgrade, targetCell, actingUnit);
        }
    }
    private void CreateButtons(List<PlayerPawnData> pawnOptions, ePlayeractionType actionType, HexCell targetCell, PlayerPawnData actingUnit)
    {
        // sort by name
        pawnOptions.Sort((x, y) => x.FriendlyName.CompareTo(y.FriendlyName));
        // sort by tier
        pawnOptions.Sort((x, y) => x.tier.CompareTo(y.tier));

        foreach (var pawnData in pawnOptions)
        {
            if (pawnData.GetPawnPrefab(GameManager.LocalPlayerID) == null)
            {
                Debug.LogWarning($"PlayerPawnData\tNo Prefab for Player {GameManager.LocalPlayerID} for {pawnData.type}\n", pawnData);
                continue;
            }
            InteractionContextButton nextButton = Instantiate(instance.buttonPrefab, instance.buttonFrame.transform);

            nextButton.Initialise(pawnData, targetCell, actingUnit, actionType);

            instance.buttonList.Add(nextButton);
        }
    }


    /// <summary>
    /// Reacts to a new player taking over their turn.
    /// </summary>
    private void TurnStarted(int playerID)
    {
        InteractionMenuManager.Close();
    }


    private void SetVisible(bool isVisible)
    {
        instance.buttonFrame.SetActive(isVisible);
        instance.mouseRaycastBlocker.SetActive(isVisible);
    }

    private void RemoveButtons()
    {
        // destroy old Buttons
        foreach (var button in instance.buttonList)
            Destroy(button.gameObject);

        instance.buttonList.Clear();
    }
}
