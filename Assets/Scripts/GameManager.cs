using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static bool InGame => instance != null;

    public static event System.Action<int> TurnStarted;

    [SerializeField] HexGrid myHexGrid;
    public static HexGrid HexGrid => instance.myHexGrid;

    [SerializeField] PlayerPawnData[] pawnDatas;

    [SerializeField] int activePlayerID = 1;
    public static int ActivePlayerID => instance.activePlayerID;

    [SerializeField] int activePlayerFactionID = 1;
    public static int CurrentFactionID => instance.activePlayerFactionID;

    private int localPlayerID;
    public static int LocalPlayerID => instance.localPlayerID;
    public static bool InputAllowed => ActivePlayerID == LocalPlayerID;

    private int turn;
    public static int Turn => instance ? instance.turn : -1;

    int spawnedPawnID = 0;
    public int maxPopulation = 5;
    public static int MaxPopulation => instance.maxPopulation;
    public int schoolNeededUpgrades;
    public static int SchoolNeededUpgrades => instance.schoolNeededUpgrades;

    [SerializeField] private PlayerValueProvider playerValueProvider;

    private void Awake()
    {
        instance = this;

        SettingsMenu.SetVolumeToPreference();

        if (OnlineGameManager.IsOnlineGame)
        {
            localPlayerID = OnlineGameManager.LocalPlayerID;
        }
        else
            localPlayerID = activePlayerID;

        playerValueProvider.SetupPlayerNames();
    }

    private void Start()
    {
        if (TryGetPlayerValues(localPlayerID, out PlayerValues player))
            HexMapCamera.SetPosition(player.GetTownHall().WorldPosition);

        playerValueProvider.SetupPawnFolders();
        StartNewPlayerTurn();
    }

    private void OnDestroy()
    {
        if (instance = this)
            instance = null;
    }

    public static void EndTurn()
    {
        Debug.Log("Ending Turn of Player " + ActivePlayerID + "\n");

        instance.EndOldPlayerTurn();

        instance.activePlayerID = instance.playerValueProvider.NextActivePlayer(ActivePlayerID);

        instance.StartNewPlayerTurn();

        Debug.Log("Ending Turn of Player " + ActivePlayerID + "\n");
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

    private void StartNewPlayerTurn()
    {
        if (!OnlineGameManager.IsOnlineGame)
            localPlayerID = activePlayerID;

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

    public static List<PlayerPawnData> GetBuildingDatas(bool withoutUpgrades, bool excludeTownHall = false)
    {
        List<PlayerPawnData> result = new List<PlayerPawnData>();

        foreach (var data in instance.pawnDatas)
        {
            if (data.IsBuilding)
            {
                if (withoutUpgrades && data.type.IsBuildingUpgrade()) continue;

                if (!excludeTownHall || data.type != ePlayerPawnType.TownHall)
                    result.Add(data);
            }
        }
        return result;
    }

    public static void UpgradeUnit(PlayerPawn upgraded, PlayerPawn school, ePlayerPawnType newPawn)
    {
        if (PawnUpgradeController.TryUpgradePawn(upgraded, newPawn, out GameResources costs)
            && instance.TryGetPlayerValues(upgraded.PlayerID, out PlayerValues playerValues))
        {
            playerValues.PayCosts(costs);
            PlayerHUD.UpdateHUD(instance.activePlayerID);
            playerValues.upgradeCounter++;
        }
    }

    public static void UpgradeBuilding(PlayerPawn builder, PlayerPawn building)
    {
        if (PawnUpgradeController.TryUpgradePawn(building, building.PawnData.linearUpgrade, out GameResources costs)
            && instance.TryGetPlayerValues(builder.PlayerID, out PlayerValues playerValues))
        {
            playerValues.PayCosts(costs);

            builder.UpgradedBuilding(building);

            PlayerHUD.UpdateHUD(instance.activePlayerID);
        }
    }

    public static int GetUpgradeCount(int playerID)
    {
        if (instance.TryGetPlayerValues(playerID, out PlayerValues result))
            return result.upgradeCounter;

        Debug.LogError("Upgrade Count failed for Player " + playerID, instance);

        return 0;
    }

    /// <summary>
    /// playes a new pawn according to the spawner, if the player has the needed resource, removes the costs and places the pawn 
    /// </summary>
    public static bool SpawnPawn(PlayerPawn spawner, HexCell spawnPoint, ePlayerPawnType newPawnType)
    {
        if (!spawner.CanAct)
        {
            Debug.LogError($"GameManager\tTried to place {newPawnType} at {spawnPoint}, but Spawner can't act\n\t\n{spawner}", spawner);
            return false;
        }

        if (spawnPoint.HasPawn)
        {
            Debug.LogError($"GameManager\tTried to place {newPawnType} at {spawnPoint}, " +
                           $"but there's a {spawnPoint.Pawn.PawnType} already.\n\t\n{spawner}\n\t\t {spawnPoint.Pawn}", spawner);
            return false;
        }

        PlayerPawnData spawnedPawnData = GetPawnData(newPawnType);

        if (spawnedPawnData == null) return false;

        // return if there's no playerdata or can't afford spawn
        if (!instance.TryGetPlayerValues(spawner.PlayerID, out PlayerValues playerResources)
            || !playerResources.HasResourcesToSpawn(spawnedPawnData))
            return false;

        playerResources.PaySpawnCosts(spawnedPawnData);
        PlayerHUD.UpdateHUD(instance.activePlayerID);

        PlaceNewPawn(spawnedPawnData, spawnPoint, spawner.PlayerID, spawner.HexCell);

        return true;
    }

    /// <summary>
    /// Places new Pawn onto grid, according to given data and position.
    /// </summary>
    public static PlayerPawn PlaceNewPawn(PlayerPawnData placedPawnData, HexCell spot, int playerID, HexCell origin = null)
    {
        PlayerPawn newPawn = Instantiate(placedPawnData.GetPawnPrefab(playerID), spot.transform.position,
                                         Quaternion.identity, instance.playerValueProvider.GetPlayerPawnParrent(playerID));

        // Pawn adds itself to the grid on the matching position.
        newPawn.SetPlayer(playerID);

        if (origin != null)
            newPawn.LookAway(origin);

        return newPawn;
    }

    public static bool IsEnemy(int otherPlayerID)
    {
        return instance.playerValueProvider.isEnemy(otherPlayerID);
    }

    private bool TryGetPlayerValues(int playerID, out PlayerValues result)
    {
        return playerValueProvider.TryGetPlayerValues(playerID, out result);
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

    public static int PlayerPopulation(int playerID)
    {
        if (instance.TryGetPlayerValues(playerID, out PlayerValues result))
            return result.populationCount;

        Debug.LogError("Population Count not found for Player " + playerID, instance);

        return MaxPopulation;
    }

    public static GameResources GetPlayerResources(int playerID)
    {
        if (instance.TryGetPlayerValues(playerID, out PlayerValues result))
            return result.playerResources;

        Debug.LogError("Food not found for Player " + playerID, instance);

        return new GameResources();
    }

    public static ColorableIconData GetPlayerIcon(int playerID)
    {
        if (instance.TryGetPlayerValues(playerID, out PlayerValues result))
            return result.PlayerIcon;

        Debug.LogError("Icon not found for Player " + playerID, instance);

        return new ColorableIconData();
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
            if (pawn.pawnID == 0)
            {
                instance.spawnedPawnID += 1;
                pawn.pawnID = instance.spawnedPawnID;
                pawn.gameObject.name = pawn.FriendlyName;
            }

            result.ownedPawns.Add(pawn);
            result.populationCount += pawn.PawnData.populationCount;
        }
    }

    /// <summary>
    /// Removes Pawn from owned pawns List of the player and takes it of the grid.
    /// </summary>
    public static void RemovePlayerPawn(PlayerPawn pawn)
    {
        if (instance.TryGetPlayerValues(pawn.PlayerID, out PlayerValues result))
        {
            result.ownedPawns.Remove(pawn);
            result.populationCount -= pawn.PawnData.populationCount;
        }
        pawn.SetHexCell(null);
        Destroy(pawn.gameObject);

        instance.CheckIfGameEnds(pawn.PlayerID);
    }

    public static void AddResource(eRessourceType resource, int amount)
    {
        if (instance.TryGetPlayerValues(ActivePlayerID, out PlayerValues result))
        {
            switch (resource)
            {
                case eRessourceType.Food:
                    result.playerResources.food += amount;
                    break;
                case eRessourceType.Wood:
                    result.playerResources.wood += amount;
                    break;
                case eRessourceType.Ore:
                    result.playerResources.ore += amount;
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
        if (instance.TryGetPlayerValues(ActivePlayerID, out PlayerValues result))
        {
            return result.CanAfford(costs);
        }
        return false;
    }

    public static void ResignTroughQuitting()
    {
        if (!OnlineGameManager.IsOnlineGame) return;

        InputMessage message = InputMessageGenerator.CreateBasicMessage(ePlayeractionType.Resign);
        InputMessageExecuter.Send(message);
    }

    public static void PlayerResigned(int playerID)
    {
        if (!instance.TryGetPlayerValues(playerID, out PlayerValues loserValues))
            return;

        loserValues.GiveUp();

        instance.CheckIfGameEnds(playerID);
    }

    private void CheckIfGameEnds(int potencialLoserPlayerID)
    {

        if (instance.playerValueProvider.CheckIfGameEnds(potencialLoserPlayerID, out List<PlayerValues> winners))
        {
            CallVictory(winners);
        }
    }

    private static void CallVictory(List<PlayerValues> winners)
    {
        VictoryLoseScreen.ShowVictory(winners);
    }
}
