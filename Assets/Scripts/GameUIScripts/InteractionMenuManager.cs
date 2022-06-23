using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InteractionMenuManager : MonoBehaviour
{
    private static InteractionMenuManager instance;

    [SerializeField] Image background;
    [SerializeField] InteractionContextButton buttonPrefab;
    [Space]
    [SerializeField] GameObject buttonFrame;
    [SerializeField] GameObject mouseRaycastBlocker;

    List<InteractionContextButton> buttonList;

    Color currentPlayerColor;

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

    // destroy old Buttons
    public static void OpenPawnCreationMenu(HexCell targetCell, PlayerPawnData actingUnit = null)
    {
        foreach (var button in instance.buttonList)
        {
            Destroy(button.gameObject);
        }
        instance.buttonList.Clear();

        if (actingUnit != null)
        {
            instance.CreateButtonEntries(actingUnit.AllPossiblesUnitUpgrades(targetCell.Pawn.PawnData.tier), ePlayeractionType.UnitUpgrade, targetCell, actingUnit);

            instance.AddPossibleTargetUpgrades(targetCell, actingUnit);
        }
        else if (ePlayerPawnType.Villager == GameInputManager.SelectedPawn.PawnType)
            instance.CreateButtonEntries(GameManager.GetBuildingDatas(withoutUpgrades: true, excludeTownHall: true), ePlayeractionType.Build, targetCell, actingUnit);

        else if (ePlayerPawnType.Digger == GameInputManager.SelectedPawn.PawnType)
        {
            GameManager.GetPawnData(ePlayerPawnType.TunnelEntry);
            instance.CreateButtonEntries(new List<PlayerPawnData>() { GameManager.GetPawnData(ePlayerPawnType.TunnelEntry) }, ePlayeractionType.Build, targetCell, actingUnit);
        }

        instance.SetVisible(instance.buttonList.Count > 0);
    }

    private void CreateButtonEntries(List<PlayerPawnData> pawnOptions, ePlayeractionType actionType, HexCell targetCell, PlayerPawnData actingUnit)
    {
        // sort by name
        pawnOptions.Sort((x,y)=> x.FriendlyName.CompareTo(y.FriendlyName));
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

            nextButton.Initialise(pawnData, targetCell, actingUnit, actionType, currentPlayerColor);

            instance.buttonList.Add(nextButton);
        }
    }

    private void AddPossibleTargetUpgrades(HexCell targetCell, PlayerPawnData actingUnit)
    {
        if (!targetCell.HasPawn || actingUnit.type != ePlayerPawnType.Villager) return;

        if (targetCell.Pawn.PawnData.linearUpgrade != ePlayerPawnType.NONE)
        {
            PlayerPawnData upgradedPawn = GameManager.GetPawnData(targetCell.Pawn.PawnData.linearUpgrade);

            CreateButtonEntries(new List<PlayerPawnData>() { upgradedPawn }, ePlayeractionType.BuildingUpgrade, targetCell, actingUnit);
        }
    }

    /// <summary>
    /// Reacts to a new player taking over their turn.
    /// </summary>
    private void TurnStarted(int playerID)
    {
        InteractionMenuManager.Close();

        instance.currentPlayerColor = GameManager.GetPlayerColor(playerID);
        instance.background.color = instance.currentPlayerColor;
    }

    public static void Close()
    {
        instance.SetVisible(false);
    }

    private void SetVisible(bool isVisible)
    {
        instance.buttonFrame.SetActive(isVisible);
        instance.mouseRaycastBlocker.SetActive(isVisible);
    }
}
