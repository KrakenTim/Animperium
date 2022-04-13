using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    struct PlayerValues
    {
        public int playerID;
        public int fractionID;
        public Color playerColor;
    }

    static GameManager instance;

    public static event System.Action<int> TurnStarted;

    [SerializeField] PlayerValues[] playerValueList;

    int activePlayerID = 1;
    public static int ActivePlayerID=> instance.activePlayerID;

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

        TurnStarted?.Invoke(instance.activePlayerID);
    }

    public static Color GetPlayerColor(int playerID)
    {
        foreach (var item in instance.playerValueList)
        {
            if (item.playerID == playerID)
                return item.playerColor;
        }

        Debug.LogError("Color not found for Player " + playerID, instance);

        return Color.magenta;
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
