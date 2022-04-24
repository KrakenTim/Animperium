using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PawnUpgradeController
{
    /// <summary>
    /// Upgrades a given unit according to knowledge learned from school.
    /// Return Upgrade costs
    /// </summary>
    public static bool TryUpgradeUnit(PlayerPawn upgradedUnit, PlayerPawn school, out GameResources resultingCosts)
    {
        if (!TryUpgradePossible(upgradedUnit.PawnType, school.PawnType.Teaches(),
                                      upgradedUnit.PlayerID, out PlayerPawnData newUnitData, out GameResources upgradeCost))
        {
            Debug.LogError($"Called upgradedUnit unexpectedly!\nUnit\t{upgradedUnit.ToString()}\nSchool\t{school.ToString()}");
            resultingCosts = upgradeCost;
            return false;
        }

        HexCell position = upgradedUnit.HexCell;
        GameManager.RemovePawn(upgradedUnit);


        PlayerPawn newPawn = GameManager.PlaceNewPawn(newUnitData, position, upgradedUnit.PlayerID);

        newPawn.Initialize(upgradedUnit.HP, upgradedUnit.MP, false);
        resultingCosts = upgradeCost;

        return true;
    }

    /// <summary>
    /// Return true if the unit will upgrade through the given knowledge and the player has the needed resources. 
    /// </summary>
    public static bool TryUpgradePossible(ePlayerPawnType oldUnit, eKnowledge knowledge,
                                      int playerID, out PlayerPawnData newUnitData, out GameResources upgradeCosts)
    {
        PlayerPawnData oldUnitData = GameManager.GetPawnData(oldUnit);

        // Check if unit profits from learning
        if (!oldUnitData.CanLearn(knowledge, out ePlayerPawnType newUnitType))
        {
            newUnitData = null;
            upgradeCosts = new GameResources();
            return false;
        }

        newUnitData = GameManager.GetPawnData(newUnitType);

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

        return costs;
    }
}
