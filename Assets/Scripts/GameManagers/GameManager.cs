using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool InGame => Instance != null;

    public static event System.Action<int> TurnStarted;
    public static event System.Action LocalPlayerChanged;

    [SerializeField] public HexGridManager hexGridManager;

    [SerializeField] PlayerPawnData[] pawnDatas;

    [SerializeField] int activePlayerID = 1;
    public static int ActivePlayerID => Instance.activePlayerID;
    
    private int localPlayerID;
    public static int LocalPlayerID
    {
        get => Instance.localPlayerID;
        private set
        {
            Instance.localPlayerID = value;
            LocalPlayerChanged?.Invoke();
        }
    }
    public static bool InputAllowed => ActivePlayerID == LocalPlayerID;

    private int turn = 1;
    public static int Turn => Instance ? Instance.turn : -1;

    int spawnedPawnID = 0;
    
    [SerializeField] private PlayerValueProvider playerValueProvider;
    public static PlayerValueProvider PlayerValueProvider => Instance.playerValueProvider;

    public static int RandomGenerationKey;

    private void Awake()
    {
        Instance = this;

        SettingsMenu.SetVolumeToPreference();

        if (OnlineGameManager.IsOnlineGame)
            LocalPlayerID = OnlineGameManager.LocalPlayerID;
        else
        {
            RandomGenerationKey = InputMessageGenerator.GetRandomKey(InputMessageGenerator.CreateRandomKeyMessage());
            LocalPlayerID = activePlayerID;
        }
    }

    private void Start()
    {
        playerValueProvider.SetupPlayerStart();

        if (TryGetPlayerValues(localPlayerID, out PlayerValues player))
            HexMapCamera.SetPosition(player.lastCameraValues.localPosition);

        playerValueProvider.SetupPawnFolders();
        StartNewPlayerTurn();
    }

    private void OnDestroy()
    {
        if (Instance = this)
            Instance = null;
    }

    public static void EndTurn()
    {
        Instance.EndOldPlayerTurn();

        int nextActivePlayerID = Instance.playerValueProvider.NextActivePlayer(ActivePlayerID);

        if (nextActivePlayerID < Instance.activePlayerID)
            Instance.turn += 1;

        Instance.activePlayerID = nextActivePlayerID;

        Instance.StartNewPlayerTurn();
    }

    private void EndOldPlayerTurn()
    {
        if (TryGetPlayerValues(activePlayerID, out PlayerValues oldPlayer))
        {
            foreach (var pawn in oldPlayer.ownedPawns)
                pawn.RefreshTurn();

            if (!OnlineGameManager.IsOnlineGame)
                oldPlayer.lastCameraValues = HexMapCamera.GetCurrentCameraValues();
        }

        GameInputManager.DeselectPawn();
    }

    private void StartNewPlayerTurn()
    {
        if (!OnlineGameManager.IsOnlineGame)
            LocalPlayerID = activePlayerID;

        if (TryGetPlayerValues(activePlayerID, out PlayerValues newPlayer))
        {
            if (!OnlineGameManager.IsOnlineGame)
                HexMapCamera.SetCameraValues(newPlayer.lastCameraValues);
        }

        TurnStarted?.Invoke(activePlayerID);
    }

    public static PlayerPawnData GetPawnData(ePlayerPawnType pawnType)
    {
        foreach (var data in Instance.pawnDatas)
        {
            if (data.type == pawnType)
                return data;
        }

        Debug.LogError("GameManager\tCouldn't find PawnData for Type " + pawnType, Instance);

        return null;
    }

    public static List<PlayerPawnData> GetBuildingDatas(bool withoutUpgrades, bool excludeTownHall = false, bool excludeTunnelEntry = false)
    {
        List<PlayerPawnData> result = new List<PlayerPawnData>();

        foreach (var data in Instance.pawnDatas)
        {
            if (data.IsBuilding)
            {
                if (withoutUpgrades && data.type.IsBuildingUpgrade()) continue;

                if (excludeTownHall && data.type == ePlayerPawnType.TownHall)
                    continue;

                if (excludeTunnelEntry && data.type == ePlayerPawnType.TunnelEntry)
                    continue;

                result.Add(data);
            }
        }
        return result;
    }

    public static void UpgradeUnit(PlayerPawn upgraded, PlayerPawn school, ePlayerPawnType newPawn)
    {
        if (PawnUpgradeController.TryUpgradePawn(upgraded, newPawn, out GameResources costs)
            && Instance.TryGetPlayerValues(upgraded.PlayerID, out PlayerValues playerValues))
        {
            playerValues.PayCosts(costs);
            playerValues.upgradeCounter++;
        }
    }

    public static void UpgradeBuilding(PlayerPawn builder, PlayerPawn building)
    {
        if (PawnUpgradeController.TryUpgradePawn(building, building.PawnData.linearUpgrade, out GameResources costs)
            && Instance.TryGetPlayerValues(builder.PlayerID, out PlayerValues playerValues))
        {
            playerValues.PayCosts(costs);

            builder.UpgradedBuilding(building);
        }
    }

    public static int GetUpgradeCount(int playerID)
    {
        if (Instance.TryGetPlayerValues(playerID, out PlayerValues result))
            return result.upgradeCounter;

        Debug.LogError("Upgrade Count failed for Player " + playerID, Instance);

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
        if (!Instance.TryGetPlayerValues(spawner.PlayerID, out PlayerValues playerResources)
            || !playerResources.HasResourcesToSpawn(spawnedPawnData))
            return false;

        playerResources.PaySpawnCosts(spawnedPawnData);

        if (spawnedPawnData.type != ePlayerPawnType.TunnelEntry)
            PlaceNewPawn(spawnedPawnData, spawnPoint, spawner.PlayerID, spawner.HexCell);
        else
        {
            HexGridManager.Current.DigAwayCircle(spawnPoint);

            foreach (var hexCell in HexGridManager.Current.GetHexCells(spawnPoint.coordinates))
                PlaceNewPawn(spawnedPawnData, hexCell, spawner.PlayerID, spawner.HexCell);
        }

        return true;
    }

    /// <summary>
    /// Places new Pawn onto grid, according to given data and position.
    /// </summary>
    public static PlayerPawn PlaceNewPawn(PlayerPawnData placedPawnData, HexCell spot, int playerID, HexCell origin = null)
    {
        PlayerPawn newPawn = Instantiate(placedPawnData.GetPawnPrefab(playerID), spot.transform.position,
                                         Quaternion.identity, Instance.playerValueProvider.GetPlayerPawnParent(playerID));

        // Pawn adds itself to the grid on the matching position.
        newPawn.SetPlayer(playerID);

        if (origin != null)
            newPawn.LookAway(origin);

        return newPawn;
    }

    private bool TryGetPlayerValues(int playerID, out PlayerValues result)
    {
        return playerValueProvider.TryGetPlayerValues(playerID, out result);
    }
    
    public static Color GetPlayerColor(int playerID)
    {
        if (Instance.TryGetPlayerValues(playerID, out PlayerValues result))
            return result.playerColor;

        Debug.LogError("Color not found for Player " + playerID, Instance);

        return Color.cyan;
    }

    public static int PlayerPopulation(int playerID)
    {
        if (Instance.TryGetPlayerValues(playerID, out PlayerValues result))
            return result.PopulationCount;

        Debug.LogError("Population Count not found for Player " + playerID, Instance);

        return 10;
    }

    public static int PlayerPopulationMax(int playerID)
    {
        if (Instance.TryGetPlayerValues(playerID, out PlayerValues result))
            return result.PopulationMax;

        Debug.LogError("Population Max not found for Player " + playerID, Instance);

        return 10;
    }
    
    public static ColorableIconData GetPlayerIcon(int playerID)
    {
        if (Instance.TryGetPlayerValues(playerID, out PlayerValues result))
            return result.PlayerIcon;

        Debug.LogError("Icon not found for Player " + playerID, Instance);

        return new ColorableIconData();
    }

    public static void AddPlayerPawn(PlayerPawn pawn)
    {
        if (pawn.PawnType.IsNonPlayer())
        {
            Debug.LogError("Tried to add non Player Pawn unexpectedly!");
            return;
        }

        if (Instance.TryGetPlayerValues(pawn.PlayerID, out PlayerValues result)
            && !result.ownedPawns.Contains(pawn))
        {
            if (pawn.pawnID == 0)
            {
                Instance.spawnedPawnID += 1;
                pawn.pawnID = Instance.spawnedPawnID;
                pawn.gameObject.name = pawn.FriendlyName;
            }

            result.ownedPawns.Add(pawn);
            if (pawn.PawnData.populationCount > 0)
                result.PopulationCount += pawn.PawnData.populationCount;

            // negative population costs increase max population
            else if (pawn.PawnData.populationCount < 0)
                result.PopulationMax += -pawn.PawnData.populationCount;
        }
    }

    /// <summary>
    /// Removes Pawn from owned pawns List of the player and takes it of the grid.
    /// </summary>
    public static void RemovePlayerPawn(PlayerPawn pawn)
    {
        if (Instance.TryGetPlayerValues(pawn.PlayerID, out PlayerValues result))
        {
            result.ownedPawns.Remove(pawn);

            if (pawn.PawnData.populationCount > 0)
                result.PopulationCount -= pawn.PawnData.populationCount;

            // negative population costs increase max population
            else if (pawn.PawnData.populationCount < 0)
                result.PopulationMax -= -pawn.PawnData.populationCount;
        }
        pawn.SetHexCell(null);
        Destroy(pawn.gameObject);

        Instance.CheckIfGameEnds(pawn.PlayerID);

        GameInputManager.DeselectPawn(pawn);

        PlayerHUD.UpdateExistingPawns();
    }

    public static void AddResource(eResourceType resource, int amount)
    {
        if (Instance.TryGetPlayerValues(ActivePlayerID, out PlayerValues result))
        {
            result.AddResource(resource, amount);
        }
    }

    public static bool CanAfford(int playerID, GameResources costs)
    {
        if (Instance.TryGetPlayerValues(ActivePlayerID, out PlayerValues result))
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
        if (!Instance.TryGetPlayerValues(playerID, out PlayerValues loserValues))
            return;

        loserValues.GiveUp();

        Instance.CheckIfGameEnds(playerID);
    }

    private bool CheckIfGameEnds(int potencialLoserPlayerID)
    {
        if (Instance.playerValueProvider.CheckIfGameEnds(potencialLoserPlayerID, out List<PlayerValues> winners))
        {
            VictoryLoseScreen.ShowVictory(winners);
            return true;
        }
        return false;
    }
}
