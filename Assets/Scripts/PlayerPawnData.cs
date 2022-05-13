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
    public string PawnName => type.ToString();

    [SerializeField] private ColorableIconData pawnIcon;
    public ColorableIconData PawnIcon => IconProvider.GetCheckedPawn(pawnIcon, type);

    [Header("Stats")]
    public int maxHealth;
    public int maxMovement;
    public int attackPower;

    [Header("Costs")]
    public GameResources resourceCosts;
    public int populationCount;

    [Header("Spawns")]
    [SerializeField] public ePlayerPawnType spawnedPawn;
    [Space]
    [SerializeField] public ePlayerPawnType learnsFight;
    [SerializeField] public ePlayerPawnType learnsMagic;
    [SerializeField] public ePlayerPawnType learnsDigging;

    [Header("Prefab")]
    [SerializeField] PlayerPawn[] playerPrefabs;

    public bool IsBuilding => type.IsBuilding();

    public bool IsUpgradePossible => learnsFight != ePlayerPawnType.NONE
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

    public List<PlayerPawnData> PossibleUnitUpgrades()
    {
        List<PlayerPawnData> upgrade = new List<PlayerPawnData>();

        if (learnsFight != ePlayerPawnType.NONE)
            upgrade.Add(GameManager.GetPawnData(learnsFight));

        if (learnsMagic != ePlayerPawnType.NONE)
            upgrade.Add(GameManager.GetPawnData(learnsMagic));

        if (learnsDigging != ePlayerPawnType.NONE)
            upgrade.Add(GameManager.GetPawnData(learnsDigging));
        
        return upgrade;
    }
}
