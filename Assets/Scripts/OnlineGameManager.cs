using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineGameManager : MonoBehaviour
{
    private static OnlineGameManager instance;

    public static bool IsOnlineGame => instance != null;

    public static int LocalPlayerID { get; private set; }

    string[] players;
     public static string myNameOnServer;

    private void Awake()
    {
        instance = this;
     
        ServerConnection.Instance.ReceivedCommandEvent.AddListener(ReceiveCommand);
        ServerConnection.Instance.ReceivedRoomPlayerListEvent.AddListener(OnReceivedRoomPlayerList);
        ServerConnection.Instance.ReceivedPlayerInfoEvent.AddListener(UpdateNameOnServer);
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
        myNameOnServer = myName;
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
        for (int i = 0; i < instance.players.Length; i++)
        {
            if (instance.players[i] == myNameOnServer)
                LocalPlayerID = i;
        }

        InputMessage message = InputMessageGenerator.CreateBasicMessage(ePlayeractionType.StartGame);

        SendCommand(message.ToString());
    }

    public static void PrepareGame()
    {
        for (int i = 0; i < instance.players.Length; i++)
        {
            if (instance.players[i] == myNameOnServer)
                LocalPlayerID = i;
        }

        Debug.Log($"My Name {myNameOnServer}, My localID {LocalPlayerID}");

        //LocalPlayerID = 

        SceneManager.LoadScene("NormanMapGeneration");
    }
}
