using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PawnUpgradeController
{
    /// <summary>
    /// Upgrades a given unit according to knowledge learned from school.
    /// Return Upgrade costs
    /// </summary>
    public static bool TryUpgradePawn(PlayerPawn upgradedUnit, ePlayerPawnType newUnitType, out GameResources resultingCosts)
    {
        if (!TryUpgradePossible(upgradedUnit.PawnData, newUnitType,
                                      upgradedUnit.PlayerID, out PlayerPawnData newUnitData, out GameResources upgradeCost))
        {
            Debug.LogError($"Called upgradedUnit unexpectedly!\nUnit\t{upgradedUnit.ToString()}\n");
            resultingCosts = upgradeCost;
            return false;
        }

        HexCell position = upgradedUnit.HexCell;
        GameManager.RemovePlayerPawn(upgradedUnit);

        PlayerPawn newPawn = GameManager.PlaceNewPawn(newUnitData, position, upgradedUnit.PlayerID);

        newPawn.Initialize(upgradedUnit.PlayerID, newUnitData.maxHealth - (upgradedUnit.PawnData.maxHealth - upgradedUnit.HP), upgradedUnit.MP, false);
        resultingCosts = upgradeCost;

        return true;
    }

    /// <summary>
    /// Return true if the unit will upgrade is possible and the player has the needed resources. 
    /// </summary>
    public static bool TryUpgradePossible(PlayerPawnData oldUnitData, ePlayerPawnType newUnit,
                                      int playerID, out PlayerPawnData newUnitData, out GameResources upgradeCosts)
    {
        newUnitData = null;

        // Check if unit profits from learning
        foreach (var possibleUpgrade in oldUnitData.AllPossiblesUnitUpgrades())
        {
            if (possibleUpgrade.type == newUnit)
            {
                newUnitData = possibleUpgrade;
                break;
            }
        }
        if (newUnitData == null)
        {
            upgradeCosts = new GameResources();
            return false;
        }

        upgradeCosts = GetUpgradeCost(oldUnitData, newUnitData);

        //Check if player has enough resources
        return GameManager.CanAfford(playerID, upgradeCosts);

    }

    /// <summary>
    /// Returns how much upgrading from old to new will cost.
    /// </summary>
    public static GameResources GetUpgradeCost(PlayerPawnData oldUnitData, PlayerPawnData newUnitData)
    {
        GameResources costs;

        costs.food = Mathf.Max(newUnitData.resourceCosts.food - oldUnitData.resourceCosts.food, 0);
        costs.wood = Mathf.Max(newUnitData.resourceCosts.wood - oldUnitData.resourceCosts.wood, 0);
        costs.ore = Mathf.Max(newUnitData.resourceCosts.ore - oldUnitData.resourceCosts.ore, 0);

        return costs;
    }
}
