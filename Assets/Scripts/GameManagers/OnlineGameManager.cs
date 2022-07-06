using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnlineGameManager : MonoBehaviour
{
    private static OnlineGameManager instance;

    [SerializeField] Button startButton;

    [SerializeField] PersistingMatchData matchData;

    public static bool IsOnlineGame => instance != null;

    public static int LocalPlayerID { get; private set; }

    /// <summary>
    /// first field is empty
    /// </summary>
    string[] players = new string[0];
    public static string NameOnServer { get; private set; }

    private void Awake()
    {
        instance = this;

        ServerConnection.Instance.ReceivedCommandEvent.AddListener(ReceiveCommand);
        ServerConnection.Instance.ReceivedRoomPlayerListEvent.AddListener(OnReceivedRoomPlayerList);
        ServerConnection.Instance.ReceivedPlayerInfoEvent.AddListener(UpdateNameOnServer);

        ServerConnection.Instance.ReceivedMapDataEvent.AddListener(CreateTemporaryMap);
    }

    private void OnDestroy()
    {
        ServerConnection.Instance.ReceivedCommandEvent.RemoveListener(ReceiveCommand);
        ServerConnection.Instance.ReceivedRoomPlayerListEvent.RemoveListener(OnReceivedRoomPlayerList);
        ServerConnection.Instance.ReceivedPlayerInfoEvent.RemoveListener(UpdateNameOnServer);

        if (instance == this) instance = null;
    }

    private void OnReceivedRoomPlayerList(string message)
    {
        players = message.Split(UIList.SPLITSYMBOL);
    }

    private void UpdateNameOnServer(string myName)
    {
        NameOnServer = myName;
    }

    public static void SendCommand(string message)
    {
        ServerConnection.Instance.SendCommand(message);
    }

    private void ReceiveCommand(string sender, string message)
    {
        InputMessageExecuter.Recieve(message);
    }

    public void Button_StartGame()
    {
        if (!matchData.IsMapPathValid) return;

        if (startButton)
            startButton.interactable = false;

        for (int i = 0; i < instance.players.Length; i++)
        {
            if (instance.players[i] == NameOnServer)
                LocalPlayerID = i;
        }

        InputMessage randomKeyMessage = InputMessageGenerator.CreateRandomKeyMessage();
        SendCommand(randomKeyMessage.ToString());

        ServerConnection.Instance.SendMapData(File.ReadAllText((matchData.MapPath)));
    }

    public static void PrepareGame()
    {
        if (instance.startButton)
            instance.startButton.interactable = false;

        for (int i = 0; i < instance.players.Length; i++)
        {
            if (instance.players[i] == NameOnServer)
                LocalPlayerID = i;
        }

        Debug.Log($"OnlineGameManager\tMy Name: {NameOnServer}, My localID: {LocalPlayerID}\n");

        SceneManager.LoadScene(AI_Scene.SCENENAME_Game);
    }

    public static void SetupPlayerNames(PlayerValues[] playerValueList)
    {
        string nextName;

        int max = Mathf.Min(instance.players.Length - 1, playerValueList.Length);

        // Add the online Name with capitalised first letter
        for (int i = 0; i < max; i++)
        {
            // replaces all whitespace with a single space, trims the ends.
            nextName = Regex.Replace(instance.players[i + 1], @"\s+", " ").Trim();

            Debug.Log(playerValueList[i].factionID + " = " + nextName);

            if (nextName.Length > 1)
                playerValueList[i].name = char.ToUpper(nextName[0]) + nextName.Substring(1);
            else if (nextName.Length == 1)
                playerValueList[i].name = nextName.ToUpper();
        }
    }

    private void CreateTemporaryMap(string mapData)
    {
        string mapPath = Path.Combine(AI_File.PathTempMaps, "OnlineMap.map");
        AI_File.WriteUTF8(mapData, mapPath);

        matchData.MapPath = mapPath;

        InputMessage startGameMessage = InputMessageGenerator.CreateBasicMessage(ePlayeractionType.StartGame);
        SendCommand(startGameMessage.ToString());
    }
}
