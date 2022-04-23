using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerPawnData : ScriptableObject
{
    public ePlayerPawnType type;

    [SerializeField] public Sprite pawnIcon;

    [Header("Stats")]
    public int maxHealth;
    public int maxMovement;
    public int attackPower;

    [Header("Costs")]
    public GameResources resourceCosts;
    public int food;

    [Header("Spawns")]
    [SerializeField] public ePlayerPawnType spawnedPawn;
    [SerializeField] public ePlayerPawnType learnsFight;
    [SerializeField] public ePlayerPawnType learnsMagic;
    [SerializeField] public ePlayerPawnType learnsDigging;

    [Header("Prefab")]
    [SerializeField] PlayerPawn[] playerPrefabs;

    public PlayerPawn GetPawnPrefap(int playerID)
    {
        if (playerID >= 0 && playerID < playerPrefabs.Length)
            return playerPrefabs[playerID];
        else
            Debug.LogError("PlayerPawnData: An unexpected ID was requested: " + playerID, this);

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
                Debug.LogError("PlayerPawnData: CanLearn UNDEFINED for "+newKnowledge);
                newType = ePlayerPawnType.NONE;
                break;
        }
        return newType != ePlayerPawnType.NONE;

    }
}
