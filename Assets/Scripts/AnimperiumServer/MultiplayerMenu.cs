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
        ServerConnection.Instance.RoomCreatedEvent.AddListener(OnRoomCreated);
        ServerConnection.Instance.RoomJoinedEvent.AddListener(OnRoomJoined);
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
        if (RoomNameInput.text == string.Empty)
            return;
        ServerConnection.Instance.CreateRoom(RoomNameInput.text);
        RoomNameInput.text = "";
    }

    public void SendMessage()
    {
        if (MessageInput.text == string.Empty)
            return;
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

    private void OnRoomCreated(string _roomName)
    {
        OpenMenu(LobbyMenu);
    }

    private void OnRoomJoined(string _roomName)
    {
        OpenMenu(LobbyMenu);
    }
    #endregion
}