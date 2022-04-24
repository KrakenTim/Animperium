using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PawnUpgradeController
{
    public static void UpgradeUnit(PlayerPawn upgradedUnit, PlayerPawn school)
    {
        if (!TryUpgradePossible(upgradedUnit.PawnType, school.PawnType.Teaches(),
                                      upgradedUnit.PlayerID, out PlayerPawnData newUnitData))
        {
            Debug.LogError($"Called upgradedUnit unexpectedly!\nUnit\t{upgradedUnit.ToString()}\nSchool\t{school.ToString()}");
            return;
        }

        HexCell position = upgradedUnit.HexCell;
        GameManager.RemovePawn(upgradedUnit);
        GameManager.PlaceNewPawn(newUnitData, position);

    }

    /// <summary>
    /// Return true if the unit will upgrade throu the given knowledge one the player and the needed resources. 
    /// </summary>
    public static bool TryUpgradePossible(ePlayerPawnType oldUnit, eKnowledge knowledge,
                                      int playerID, out PlayerPawnData newUnitData)
    {
        PlayerPawnData oldUnitData = GameManager.GetPawnData(oldUnit);

        // Check if unit profits from learning
        if (!oldUnitData.CanLearn(knowledge, out ePlayerPawnType newUnitType))
        {
            newUnitData = null;
            return false;
        }

        newUnitData = GameManager.GetPawnData(newUnitType);

        GameResources upgradeCost = UpgradeCost(oldUnitData, newUnitData);

        //Check if player has enough resources
        return GameManager.CanAfford(playerID, upgradeCost);

    }

    /// <summary>
    /// Returns how much upgrading from old to new will cost
    /// </summary>
    public static GameResources UpgradeCost(PlayerPawnData oldUnitData, PlayerPawnData newUnitData)
                                       
    {
        GameResources costs;

        costs.food = Mathf.Max(newUnitData.resourceCosts.food - oldUnitData.resourceCosts.food, 0);

        return costs;
    }
}
