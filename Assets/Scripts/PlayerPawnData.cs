using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all permanent Data related to a specific PawnType.
/// </summary>
[CreateAssetMenu]
public class PlayerPawnData : ScriptableObject
{
    public ePlayerPawnType type;
    public string friendlyName;
    public int tier;

    [SerializeField] private ColorableIconData pawnIcon;
    public ColorableIconData PawnIcon => IconProvider.GetCheckedPawn(pawnIcon, type);

    [Header("Stats")]
    public int maxHealth = 1;
    public int maxMovement;
    public int attackPower;
    public int attackRange = 1;
    public int viewRange = 2;
    [Space]
    [Tooltip("Generic field for Unit specific ability.\nHealer: Healing Power")]
    public int specialPower;

    [Header("Costs")]
    public GameResources resourceCosts;
    public int populationCount;

    [Header("Upgrades")]
    public ePlayerPawnType linearUpgrade;
    [Space]
    public ePlayerPawnType learnsFight;
    public ePlayerPawnType learnsMagic;
    public ePlayerPawnType learnsDigging;

    [Header("Other")]
    public int upgradesForUnlock;
    public ePlayerPawnType spawnedPawn;

    [Header("Prefab")]
    [SerializeField] PlayerPawn[] playerPrefabs;

    public bool IsBuilding => type.IsBuilding();

    public bool IsLearnUpgradePossible => learnsFight != ePlayerPawnType.NONE
                                  || learnsMagic != ePlayerPawnType.NONE
                                  || learnsDigging != ePlayerPawnType.NONE;
    public PlayerPawn GetPawnPrefab(int playerID)
    {
        if (playerID >= 0 && playerID < playerPrefabs.Length)
        {
            if (playerPrefabs[playerID] == null)
                Debug.LogError($"PlayerPawnData: No Prefab defined for {type} and PlayerID " + playerID, this);
            return playerPrefabs[playerID];
        }
        else
            Debug.LogError($"PlayerPawnData: An unexpected ID was requested for {type}: " + playerID, this);

        return null;
    }

    public bool CanLearn(eKnowledge newKnowledge, out ePlayerPawnType newType)
    {
        switch (newKnowledge)
        {
            case eKnowledge.Fight:
                newType = learnsFight;
                break;
            case eKnowledge.Magic:
                newType = learnsMagic;
                break;
            case eKnowledge.Digging:
                newType = learnsDigging;
                break;
            default:
                Debug.LogError("PlayerPawnData: CanLearn UNDEFINED for " + newKnowledge);
                newType = ePlayerPawnType.NONE;
                break;
        }
        return newType != ePlayerPawnType.NONE;
    }

    public List<PlayerPawnData> PossibleUnitUpgrades(int tierLimit = int.MaxValue)
    {
        List<PlayerPawnData> upgrade = new List<PlayerPawnData>();

        PlayerPawnData nextData;

        if (learnsFight != ePlayerPawnType.NONE)
        {
            nextData = GameManager.GetPawnData(learnsFight);

            if (nextData.tier <= tierLimit)
                upgrade.Add(nextData);
        }

        if (learnsMagic != ePlayerPawnType.NONE)

        {
            nextData = GameManager.GetPawnData(learnsMagic);

            if (nextData.tier <= tierLimit)
                upgrade.Add(nextData);
        }

        if (learnsDigging != ePlayerPawnType.NONE)

        {
            nextData = GameManager.GetPawnData(learnsDigging);

            if (nextData.tier <= tierLimit)
                upgrade.Add(nextData);
        }

        if (linearUpgrade != ePlayerPawnType.NONE)

        {
            nextData = GameManager.GetPawnData(linearUpgrade);

            if (nextData.tier <= tierLimit)
                upgrade.Add(nextData);
        }

        return upgrade;
    }
    /// <summary>
    /// Returns all possible future Upgrades (including upgrades of upgrades etc.)
    /// </summary>
    /// <returns></returns>
    public List<PlayerPawnData> AllPossiblesUnitUpgrades(int tierLimit = int.MaxValue)
    {
        List<PlayerPawnData> allUpgrades = new List<PlayerPawnData>();
        List<PlayerPawnData> upgradesToCheck = PossibleUnitUpgrades(tierLimit);
        List<PlayerPawnData> newUpgrades = new List<PlayerPawnData>();

        while (upgradesToCheck.Count > 0)
        {
            foreach (var nextPawn in upgradesToCheck)
            {
                if (!allUpgrades.Contains(nextPawn))
                    allUpgrades.Add(nextPawn);
                newUpgrades.AddRange(nextPawn.PossibleUnitUpgrades(tierLimit));
            }
            upgradesToCheck.Clear();
            upgradesToCheck.AddRange(newUpgrades);
            newUpgrades.Clear();
        }
        return allUpgrades;
    }
}
