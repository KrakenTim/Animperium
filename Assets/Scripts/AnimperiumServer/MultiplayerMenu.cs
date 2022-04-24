using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiplayerMenu : MonoBehaviour
{
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject ServerMenu;
    [SerializeField] private GameObject LobbyMenu;
    [SerializeField] private TMP_InputField RoomNameInput;
    [SerializeField] private TMP_InputField MessageInput;
    [SerializeField] private UIList ServerPlayerList;
    [SerializeField] private UIList ServerRoomList;
    [SerializeField] private UIList RoomPlayerList;
    [SerializeField] private UIList RoomChatList;

    void Awake()
    {
        ServerConnection.Instance.ReceivedServerPlayerListEvent.AddListener(OnReceivedServerPlayerList);
        ServerConnection.Instance.ReceivedServerRoomListEvent.AddListener(OnReceivedServerRoomList);
        ServerConnection.Instance.ReceivedRoomPlayerListEvent.AddListener(OnReceivedRoomPlayerList);
        ServerConnection.Instance.ReceivedTextMessageEvent.AddListener(OnReceivedTextMessageList);
        OpenMenu(MainMenu);
    }

    public void OpenMenu(GameObject _menu)
    {
        MainMenu.SetActive(_menu == MainMenu);
        ServerMenu.SetActive(_menu == ServerMenu);
        LobbyMenu.SetActive(_menu == LobbyMenu);
    }

    #region Server Commands
    public void CreateRoom()
    {
        ServerConnection.Instance.CreateRoom(RoomNameInput.text);
        RoomNameInput.text = "";
    }

    public void SendMessage()
    {
        ServerConnection.Instance.SendTextMessage(MessageInput.text);
        MessageInput.text = "";
    }
    #endregion

    #region Server Event Handler 
    private void OnReceivedServerPlayerList(string _serverPlayerList)
    {
        ServerPlayerList.RefreshList(_serverPlayerList);
    }

    private void OnReceivedServerRoomList(string _serverRoomList)
    {
        ServerRoomList.RefreshList(_serverRoomList);
    }

    private void OnReceivedRoomPlayerList(string _roomPlayerList)
    {
        RoomPlayerList.RefreshList(_roomPlayerList);
    }

    private void OnReceivedTextMessageList(string _textMessage)
    {
        RoomChatList.AddItem(_textMessage);
    }
    #endregion
}