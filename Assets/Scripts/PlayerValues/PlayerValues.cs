using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data type holding the values related to a player.
/// </summary>
[System.Serializable]
public class PlayerValues
{
    public int playerID;
    public int factionID;
    public Color playerColor;

    private int populationCount;
    public int PopulationCount
    {
        get
        {
            return populationCount;
        }
        set
        {
            populationCount = value;
            OnValuesChanged?.Invoke(playerID);
        }
    }

    private int populationMax;
    public int PopulationMax
    {
        get
        {
            return populationMax;
        }
        set
        {
            populationMax = value;
            OnValuesChanged?.Invoke(playerID);
        }
    }

    public int upgradeCounter;
    [SerializeField] private ColorableIconData playerIcon;
    public ColorableIconData PlayerIcon => IconProvider.GetCheckedPlayer(playerIcon, name);

    [SerializeField] private GameResources playerResources;
    public GameResources PlayerResources => playerResources;

    public List<PlayerPawn> ownedPawns = new List<PlayerPawn>();

    private bool hasLost = false;
    public bool HasLost => hasLost;

    public string name;
    public string DefaultName => "Player " + playerID;

    public CameraValues lastCameraValues = new CameraValues();

    [SerializeField] private GameResources resourcesPerTurn = new GameResources();
    public GameResources RessourcesPerTurn => resourcesPerTurn;

    /// <summary>
    /// Is called if a relevant value of a player is changed, hands over the player's ID.
    /// </summary>
    public static System.Action<int> OnValuesChanged; // EventHandler oder EventData nutzen??? public event EventHandler<type> ist erweiterbar

    public void GiveUp()
    {
        hasLost = true;
    }

    /// <summary>
    /// True if the player doesn't have a townhall anymore or no units and can't spawn any.
    /// </summary>
    public bool CheckIfHasLost()
    {
        if (hasLost) return true;

        int units = 0;
        bool townHall = false;

        // Checklist of units the player might spawn
        HashSet<ePlayerPawnType> possibleSpawns = new HashSet<ePlayerPawnType>();

        foreach (PlayerPawn pawn in ownedPawns)
        {
            if (pawn.IsUnit)
            {
                units += 1;

                // if there's a townhall and a unit, we didn't lose yet.
                if (townHall) return false;
            }

            if (pawn.PawnType == ePlayerPawnType.TownHall)
            {
                // if there's a townhall and a unit, we didn't lose yet.
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
            if (HasResourcesToSpawn(spawn))
                return false;
        }

        hasLost = true;
        return true;
    }

    public bool CanBuildAnything(List<PlayerPawnData> spawnDatas)
    {
        foreach (var data in spawnDatas)
        {
            if (HasResourcesToSpawn(data)) return true;
        }
        return false;
    }

    public bool HasResourcesToSpawn(ePlayerPawnType pawnType)
    {
        return HasResourcesToSpawn(GameManager.GetPawnData(pawnType));
    }

    public bool HasResourcesToSpawn(PlayerPawnData spawnData)
    {
        // Check if the Player have enough resources
        return (CanAfford(spawnData.resourceCosts));
    }

    public void AddResource(eResourceType resource, int amount)
    {
        playerResources[resource] += amount;

        OnValuesChanged?.Invoke(playerID);
    }

    public bool CanAfford(GameResources resources)
    {
        if (PlayerResources.food < resources.food) return false;
        if (PlayerResources.wood < resources.wood) return false;
        if (PlayerResources.ore < resources.ore) return false;

        return true;
    }

    public void PayCosts(GameResources resources)
    {
        playerResources.food -= resources.food;
        playerResources.wood -= resources.wood;
        playerResources.ore -= resources.ore;

        OnValuesChanged?.Invoke(playerID);
    }

    public void PaySpawnCosts(PlayerPawnData spawnData)
    {
        PayCosts(spawnData.resourceCosts);
    }

    public PlayerPawn GetTownHall()
    {
        foreach (var pawn in ownedPawns)
        {
            if (pawn.PawnType == ePlayerPawnType.TownHall)
                return pawn;
        }
        return null;
    }

    public void ChangeRessourcesPerTurn(eResourceType type, int change)
    {
        resourcesPerTurn[type] = resourcesPerTurn[type] + change;

        OnValuesChanged?.Invoke(playerID);
    }

    public void AddResourcesPerTurn()
    {
        foreach (eResourceType resource in Enum.GetValues(typeof(eResourceType)))
        {
            if (resource == eResourceType.NONE) continue;

            if (resourcesPerTurn[resource] != 0)
                AddResource(resource, resourcesPerTurn[resource]);
        }
    }
}