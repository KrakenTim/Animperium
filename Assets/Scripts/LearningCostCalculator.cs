using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LearningCostCalculator
{
    public static bool TryUpgradeUnit(ePlayerPawnType oldUnit, eKnowledge knowledge,
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

        GameResources upgradeCost = UpgardeCost(oldUnitData, newUnitData);

       // GameManager.

        //Check if player has enough resources
        

        return true;
    }

    /// <summary>
    /// Returns how much upgrading from old to new will cost
    /// </summary>
    public static GameResources UpgardeCost(PlayerPawnData oldUnitData, PlayerPawnData newUnitData)
                                       
    {
        GameResources costs;

        costs.food = Mathf.Max(newUnitData.resourceCosts.food - oldUnitData.resourceCosts.food, 0);

        return costs;
    }
}
