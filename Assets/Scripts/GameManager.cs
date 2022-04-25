using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public bool InGame => instance != null;

    public static event System.Action<int> TurnStarted;

    [SerializeField] HexGrid myHexGrid;
    public static HexGrid HexGrid => instance.myHexGrid;

    [SerializeField] PlayerPawnData[] pawnDatas;

    [SerializeField] PlayerValues[] playerValueList;

    [SerializeField] int activePlayerID = 1;
    public static int CurrentPlayerID => instance.activePlayerID;

    [SerializeField] int activePlayerFactionID = 1;
    public static int CurrentFactionID => instance.activePlayerFactionID;

    private int turn;
    public static int Turn => instance ? instance.turn : -1;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartNewPlayerTurn();
    }

    private void OnDestroy()
    {
        if (instance = this)
            instance = null;
    }

    public static void EndTurn()
    {
        instance.EndOldPlayerTurn();

        instance.SetNextPlayer();

        instance.StartNewPlayerTurn();
    }

    private void EndOldPlayerTurn()
    {
        if (TryGetPlayerValues(activePlayerID, out PlayerValues oldPlayer))
        {
            foreach (var pawn in oldPlayer.ownedPawns)
                pawn.RefreshTurn();
        }

        GameInputManager.DeselectPawn();
    }

    /// <summary>
    /// Increases the activePlayerID to the next player that didn't lose yet.
    /// </summary>
    private void SetNextPlayer()
    {
        instance.activePlayerID++;

        if (instance.activePlayerID > instance.playerValueList.Length)
            instance.activePlayerID = 1;

        // Skip players that lost or if there's no player values
        if (TryGetPlayerValues(instance.activePlayerID, out PlayerValues nextPlayer))
        {
            if (nextPlayer.HasLost) SetNextPlayer();
        }
        else
            SetNextPlayer();
    }

    private void StartNewPlayerTurn()
    {
        if (TryGetPlayerValues(activePlayerID, out PlayerValues newPlayer))
            activePlayerFactionID = newPlayer.factionID;

        turn += 1;

        TurnStarted?.Invoke(activePlayerID);
    }

    public static PlayerPawnData GetPawnData(ePlayerPawnType pawnType)
    {
        foreach (var data in instance.pawnDatas)
        {
            if (data.type == pawnType)
                return data;
        }

        Debug.LogError("GameManager\tCouldn't find PawnData for Type " + pawnType, instance);

        return null;
    }

    public static void UpgradePawn(PlayerPawn upgraded, PlayerPawn school)
    {
        if (PawnUpgradeController.TryUpgradeUnit(upgraded, school, out GameResources costs)
            && instance.TryGetPlayerValues(upgraded.PlayerID, out PlayerValues playerValues))
        {
            playerValues.PayCosts(costs);
            PlayerHUD.UpdateHUD(instance.activePlayerID);
        }
    }

    /// <summary>
    /// playes a new pawn according to the spawner, if the player has the needed resource, removes the costs and places the pawn 
    /// </summary>
    public static void SpawnPawn(PlayerPawn spawner, HexCell spawnPoint)
    {
        ePlayerPawnType spawnPawnType = spawner.Spawn;

        PlayerPawnData spawnedPawnData = GetPawnData(spawnPawnType);

        Debug.Log("1");

        if (spawnedPawnData == null) return;

        Debug.Log("2");

        // return if there's no playerdata or can't afford spawn
        if (!instance.TryGetPlayerValues(spawner.PlayerID, out PlayerValues playerResources)
            || !playerResources.HasResourcesToSpawn(spawnedPawnData))
            return;

        Debug.Log("3");

        playerResources.PaySpawnCosts(spawnedPawnData);
        PlayerHUD.UpdateHUD(instance.activePlayerID);

        PlaceNewPawn(spawnedPawnData, spawnPoint, spawner.PlayerID);
    }

    /// <summary>
    /// Places new Pawn onto grid, according to given data and position.
    /// </summary>
    public static PlayerPawn PlaceNewPawn(PlayerPawnData placedPawnData, HexCell spot, int playerID)
    {
        PlayerPawn newPawn = Instantiate(placedPawnData.GetPawnPrefab(playerID),
                             spot.transform.position, Quaternion.identity, instance.transform);

        // Pawn adds itself to the grid on the matching position.

        return newPawn;
    }

    public static bool IsEnemy(int otherPlayerID)
    {
        if (instance.TryGetPlayerValues(CurrentPlayerID, out PlayerValues currentPlayer)
            && instance.TryGetPlayerValues(otherPlayerID, out PlayerValues otherPlayer))
        {
            return currentPlayer.factionID != otherPlayer.factionID;
        }
        return false;
    }
    
    private bool TryGetPlayerValues(int playerID, out PlayerValues result)
    {
        foreach (var item in instance.playerValueList)
        {
            if (item.playerID == playerID)
            {
                result = item;
                return true;
            }

        }

        Debug.LogError("Values not found for Player " + playerID, instance);

        result = new PlayerValues();

        return false;
    }

    public static int GetPlayerFactionID(int playerID)
    {
        if (instance.TryGetPlayerValues(playerID, out PlayerValues result))
            return result.factionID;

        Debug.LogError("Faction not found for Player " + playerID, instance);

        return 0;
    }
    public static Color GetPlayerColor(int playerID)
    {
        if (instance.TryGetPlayerValues(playerID, out PlayerValues result))
            return result.playerColor;

        Debug.LogError("Color not found for Player " + playerID, instance);

        return Color.cyan;
    }

    public static GameResources GetPlayerResources (int playerID)
    {
        if (instance.TryGetPlayerValues(playerID, out PlayerValues result))
            return result.playerResources;

        Debug.LogError("Food not found for Player " + playerID, instance);

        return new GameResources();
    }

    public static Sprite GetPlayerIcon(int playerID)
    {
        if (instance.TryGetPlayerValues(playerID, out PlayerValues result))
            return result.playerIcon;

        Debug.LogError("Icon not found for Player " + playerID, instance);

        return null;
    }

    public static HexCell GetHexCell(Vector3 worldPosition)
    {
        return instance.myHexGrid.GetHexCell(worldPosition);
    }

    public static void AddPlayerPawn(PlayerPawn pawn)
    {
        if (pawn.PawnType.IsNonPlayer())
        {
            Debug.LogError("Tried to add non Player Pawn unexpectedly!");
            return;
        }

        if (instance.TryGetPlayerValues(pawn.PlayerID, out PlayerValues result)
            && !result.ownedPawns.Contains(pawn))
        {
            result.ownedPawns.Add(pawn);
        }
    }

    /// <summary>
    /// Removes Pawn from owned pawns List of the player and takes it of the grid.
    /// </summary>
    public static void RemovePawn(PlayerPawn pawn)
    {
        if (instance.TryGetPlayerValues(pawn.PlayerID, out PlayerValues result))
        {
            result.ownedPawns.Remove(pawn);
        }
        pawn.SetHexCell(null);
        Destroy(pawn.gameObject);
    }

    public static void AddResource(eRessourceType resource, int amount)
    {
        if (instance.TryGetPlayerValues(CurrentPlayerID, out PlayerValues result))
        {
            switch (resource)
            {
                case eRessourceType.Food:
                    result.playerResources.food += amount;
                    break;
                default:
                    Debug.LogError("AddResource UNDEFINED for " + resource);
                    return;
            }

            PlayerHUD.UpdateHUD(instance.activePlayerID);

        }
    }

    public static bool CanAfford(int playerID, GameResources costs)
    {
        if (instance.TryGetPlayerValues(CurrentPlayerID, out PlayerValues result))
        {
            return result.CanAfford(costs);
        }
        return false;
    }

    public static void CheckIfGameEnds(int potencialLoserPlayerID)
    {
        if (!instance.TryGetPlayerValues(potencialLoserPlayerID, out PlayerValues loserValues))
            return;

        if (!loserValues.CheckIfHasLost()) return;

        // int: fractionId, List<PlayerValues>: surviving Players
        Dictionary<int, List<PlayerValues>> remainingFactions = new Dictionary<int, List<PlayerValues>>();

        foreach (PlayerValues player in instance.playerValueList)
        {
            if (player.HasLost) continue;

            if(!remainingFactions.ContainsKey(player.factionID))
                remainingFactions.Add(player.factionID, new List<PlayerValues>());

            remainingFactions[player.factionID].Add(player);
        }

        Debug.Log("factions left:" + remainingFactions.Count);

        if (remainingFactions.Count < 2)
        {
            foreach (var faction in remainingFactions)
            CallVictory(faction.Value);
        }
    }

    private static void CallVictory(List<PlayerValues> winners)
    {
        VictoryLoseScreen.ShowVictory(winners);
    }

}
