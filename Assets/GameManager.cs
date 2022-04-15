using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    class PlayerValues
    {
        public int playerID;
        public int factionID;
        public Color playerColor;
        public Sprite playerIcon;

        public int food;

        public List<PlayerPawn> ownedPawns;
    }

    static GameManager instance;
    public bool InGame => instance != null;

    public static event System.Action<int> TurnStarted;

    [SerializeField] HexGrid myHexGrid;
    public static HexGrid HexGrid => instance.myHexGrid;

    [SerializeField] PlayerPawnData[] pawnDatas;

    [SerializeField] PlayerValues[] playerValueList;

    int activePlayerID = 1;
    public static int CurrentPlayerID => instance.activePlayerID;

    int activePlayerFactionID = 1;
    public static int CurrentFactionID => instance.activePlayerFactionID;

    private void Awake()
    {
        for (int i = 0; i < playerValueList.Length; i++)
        {
            if (playerValueList[i].ownedPawns == null)
                playerValueList[i].ownedPawns = new List<PlayerPawn>();
        }

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

        instance.activePlayerID++;

        if (instance.activePlayerID > instance.playerValueList.Length)
            instance.activePlayerID = 1;

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

    private void StartNewPlayerTurn()
    {
        if (TryGetPlayerValues(activePlayerID, out PlayerValues newPlayer))
            activePlayerFactionID = newPlayer.factionID;

        TurnStarted?.Invoke(activePlayerID);
    }

    public static void SpawnPawn(PlayerPawn spawner, HexCell spawnPoint)
    {
        ePlayerPawnType spawnPawnType = spawner.Spawn;

        PlayerPawnData spawnedPawn = null;

        foreach (var data in instance.pawnDatas)
        {
            if (data.type == spawnPawnType)
                spawnedPawn = data;
        }

        if (spawnedPawn == null) return;

        bool isPossible = true;
        // Check if the Player have enough resources
        if (instance.TryGetPlayerValues(CurrentPlayerID, out PlayerValues playerResources))
        {
            if (spawnedPawn.food > playerResources.food)
                isPossible = false;
        }
        if (isPossible == false) return;

        playerResources.food -= spawnedPawn.food;
        PlayerHUD.UpdateHUD(instance.activePlayerID);

        PlayerPawn newPawn = Instantiate(spawnedPawn.GetPawnPrefap(instance.activePlayerID),
            spawnPoint.transform.position, Quaternion.identity, instance.transform);
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

    public static int GetPlayerFood(int playerID)
    {
        if (instance.TryGetPlayerValues(playerID, out PlayerValues result))
            return result.food;

        Debug.LogError("Food not found for Player " + playerID, instance);

        return -1;
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

    public static void AddPawn(PlayerPawn pawn)
    {
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
                    result.food += amount;
                    break;
                default:
                    Debug.LogError("AddResource UNDEFINED for " + resource);
                    return;
            }

            PlayerHUD.UpdateHUD(instance.activePlayerID);

        }
    }

}
