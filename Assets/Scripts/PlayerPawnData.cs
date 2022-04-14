using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerPawnData : ScriptableObject
{
    public ePlayerPawnType type;
    [Header("Stats")]

    public int maxHealth;
    public int maxMovement;
    public int attackPower;
    [Header("Costs")]
    public int food;

    [Header("Spawns")]
    [SerializeField]List<ePlayerPawnType> spawnedPawns = new List<ePlayerPawnType>();
    [SerializeField] ePlayerPawnType learnsFight;
    [SerializeField] ePlayerPawnType learnsMagic;
    [SerializeField] ePlayerPawnType learnsDigging;

    [Header("Prefab")]
    [SerializeField] GameObject[] playerPrefabs;

    public GameObject GetPlayerPrefab(int playerID)
    {
        if (playerID >= 0 && playerID < playerPrefabs.Length)
            return playerPrefabs[playerID];
        else
            Debug.LogError("PlayerPawnData: An unexpected ID was requested: " + playerID, this);

        return null;
    }
}
