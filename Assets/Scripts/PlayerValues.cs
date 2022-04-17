using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerValues
{
    public int playerID;
    public int factionID;
    public Color playerColor;
    public Sprite playerIcon;

    public int food;

    public List<PlayerPawn> ownedPawns = new List<PlayerPawn>();

    private bool hasLost = false;
    public bool HasLost => hasLost;

    public bool CheckIfHasLost()
    {
        int units = 0;
        bool townHall = false;

        // Checklist of units the player might spawn
        HashSet<ePlayerPawnType> possibleSpawns = new HashSet<ePlayerPawnType>();

        foreach (PlayerPawn pawn in ownedPawns)
        {
            if (pawn.IsUnit)
            {
                units += 1;

                // if there's a townhall an a unit, we didn't lose yet.
                if (townHall) return false;
            }

            if (pawn.PawnType == ePlayerPawnType.TownHall)
            {
                // if there's a townhall an a unit, we didn't lose yet.
                townHall = true;

                if (units > 0) return false;
            }
            // if we need to check for spawnable units, add possible spawn to cheklist
            if (units == 0 && pawn.Spawn != ePlayerPawnType.NONE && pawn.Spawn.IsUnit())
                possibleSpawns.Add(pawn.Spawn);

        }
        if (!townHall)
        {
            hasLost = true;
            return true;
        }

        foreach (ePlayerPawnType spawn in possibleSpawns)
        {
            // if we can spawn something we didn't lose yet
            if (HasResourcesToSpawn(GameManager.GetPawnData(spawn)))
                return false;
        }

        hasLost = true;
        return true;
    }

    public bool HasResourcesToSpawn(PlayerPawnData spawnData)
    {
        // Check if the Player have enough resources
        if (spawnData.food > food)
            return false;

        // all resources there
        return true;
    }

    public void RemoveSpawnCosts(PlayerPawnData spawnData)
    {
        food -= spawnData.food;

        if (food < 0)
            Debug.LogError($"PlayerValues\tPlayer{playerID} got negative food!\n");
    }
}