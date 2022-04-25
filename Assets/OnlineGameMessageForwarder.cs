using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineGameMessageForwarder : MonoBehaviour
{
    private static OnlineGameMessageForwarder instance;

    public static bool IsOnlineGame => instance != null;

    private void Awake()
    {
        instance = this;

        ServerConnection.Instance.ReceivedCommandEvent.AddListener(ReceiveCommand);
    }

    private void OnDestroy()
    {
        ServerConnection.Instance.ReceivedCommandEvent.RemoveListener(ReceiveCommand);

        if (instance == this) instance = null;
    }

    public static void SendCommand(string message)
    {
        ServerConnection.Instance.SendCommand(message);
    }

    private void ReceiveCommand(string message)
    {
        InputMessageExecuter.Recieve(message);
    }

    public void Button_StartGame()
    {
        InputMessage message = InputMessageGenerator.CreateBasicMessage(ePlayeractionType.StartGame);

        SendCommand(message.ToString());
    }

    public static void PrepareGame()
    {
        SceneManager.LoadScene("NormanMapGeneration");
    }
}
