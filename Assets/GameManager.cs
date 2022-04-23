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

    private void SetNextPlayer()
    {
        instance.activePlayerID++;

        if (instance.activePlayerID > instance.playerValueList.Length)
            instance.activePlayerID = 1;

        if (TryGetPlayerValues(instance.activePlayerID, out PlayerValues nextPlayer))
        {
            if (nextPlayer.HasLost) SetNextPlayer();
        }
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
    public static void SpawnPawn(PlayerPawn spawner, HexCell spawnPoint)
    {
        ePlayerPawnType spawnPawnType = spawner.Spawn;

        PlayerPawnData spawnedPawnData = GetPawnData(spawnPawnType);

        if (spawnedPawnData == null) return;

        // return if there's no playerdata or can't afford spawn
        if (!instance.TryGetPlayerValues(CurrentPlayerID, out PlayerValues playerResources)
            || !playerResources.HasResourcesToSpawn(spawnedPawnData))
            return;

        playerResources.RemoveSpawnCosts(spawnedPawnData);
        PlayerHUD.UpdateHUD(instance.activePlayerID);

        PlayerPawn newPawn = Instantiate(spawnedPawnData.GetPawnPrefap(instance.activePlayerID),
            spawnPoint.transform.position, Quaternion.identity, instance.transform);
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


    public static void RemovePawn(PlayerPawn pawn)
    {
        if (instance.TryGetPlayerValues(pawn.PlayerID, out PlayerValues result))
        {
            result.ownedPawns.Remove(pawn);
        }
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

    public static void CheckIFGameEnds(int potencialLoserPlayerID)
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
