using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    struct PlayerValues
    {
        public int playerID;
        public int factionID;
        public Color playerColor;

        public int food;
    }

    static GameManager instance;

    public static event System.Action<int> TurnStarted;

    [SerializeField] HexGrid myHexGrid;

    [SerializeField] PlayerValues[] playerValueList;

    int activePlayerID = 1;
    public static int ActivePlayerID=> instance.activePlayerID;

    int activePlayerFactionID = 1;
    public static int ActivePlayerFactionID => instance.activePlayerFactionID;

    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        if (instance = this)
            instance = null;
    }

    public static void EndTurn()
    {
        if (instance.activePlayerID ==1)
            instance.activePlayerID = 2;
        else
            instance.activePlayerID = 1;

        if (instance.TryGetPlayerValues(instance.activePlayerID, out PlayerValues result))
            instance.activePlayerFactionID = result.factionID;

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

        return Color.magenta;
    }

    public static HexCell GetHexCell(Vector3 worldPosition)
    {
        return instance.myHexGrid.GetHexCell(worldPosition);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
