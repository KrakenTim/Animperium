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

    [SerializeField] PlayerValues[] playerValueList;

    int activePlayerID = 1;
    public static int CurrentPlayerID=> instance.activePlayerID;

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

    private void OnDestroy()
    {
        if (instance = this)
            instance = null;
    }

    public static void EndTurn()
    {
        // End old Players turn
        if (instance.TryGetPlayerValues(instance.activePlayerID, out PlayerValues oldPlayer))
        {
            foreach (var pawn in oldPlayer.ownedPawns)
                pawn.RefreshTurn();
        }

        GameInputManager.DeselectPawn();

        if (instance.activePlayerID ==1)
            instance.activePlayerID = 2;
        else
            instance.activePlayerID = 1;

        // Start new Players Turn
        if (instance.TryGetPlayerValues(instance.activePlayerID, out PlayerValues newPlayer))
            instance.activePlayerFactionID = newPlayer.factionID;

        TurnStarted?.Invoke(instance.activePlayerID);
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

}
