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

    private void Awake()
    {
        instance = this;

        GameManager.TurnStarted += BackgroundFrame;

        buttonList = new List<InteractionContextButton>(buttonFrame.GetComponentsInChildren<InteractionContextButton>());

        SetVisible(false);
    }

    private void OnDestroy()
    {
        if (instance = this)
            instance = null;

        GameManager.TurnStarted -= BackgroundFrame;
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
            int maxTier = targetCell.Pawn.PawnData.tier;

            List<PlayerPawnData> allUpgrades = new List<PlayerPawnData>();
            List<PlayerPawnData> upgradesToCheck = actingUnit.PossibleUnitUpgrades(maxTier);
            List<PlayerPawnData> newUpgrades = new List<PlayerPawnData>();

            PlayerPawnData next = actingUnit;

            while (upgradesToCheck.Count > 0)
            {
                foreach(var nextPawn in upgradesToCheck)
                {
                    if (!allUpgrades.Contains(nextPawn))
                    allUpgrades.Add(nextPawn);
                    newUpgrades.AddRange(nextPawn.PossibleUnitUpgrades(maxTier));
                }
                upgradesToCheck.Clear();
                upgradesToCheck.AddRange(newUpgrades);
                newUpgrades.Clear();
            }
           
            instance.CreateButtonEntries(allUpgrades, ePlayeractionType.UnitUpgrade, targetCell, actingUnit);

            instance.AddPossibleTargetUpgrades(targetCell, actingUnit);
        }
        else
            instance.CreateButtonEntries(GameManager.GetBuildingData(withoutUpgrades: true), ePlayeractionType.Build, targetCell, actingUnit);

        instance.SetVisible(instance.buttonList.Count > 0);
    }

    private void CreateButtonEntries(List<PlayerPawnData> pawnOptions, ePlayeractionType actionType, HexCell targetCell, PlayerPawnData actingUnit)
    {
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

    private void AddPossibleTargetUpgrades(HexCell targetCell, PlayerPawnData actingUnit)
    {
        if (!targetCell.HasPawn || actingUnit.type != ePlayerPawnType.Villager) return;

        if (targetCell.Pawn.PawnData.linearUpgrade != ePlayerPawnType.NONE)
        {
            PlayerPawnData upgradedPawn = GameManager.GetPawnData(targetCell.Pawn.PawnData.linearUpgrade);

            CreateButtonEntries(new List<PlayerPawnData>() { upgradedPawn }, ePlayeractionType.BuildingUpgrade, targetCell, actingUnit);
        }
    }

    public static void BackgroundFrame(int playerID)
    {
        instance.background.color = GameManager.GetPlayerColor(playerID);
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
