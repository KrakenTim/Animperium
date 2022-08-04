using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MultiplayerMenu : MonoBehaviour
{
    private const string SCENE_MainMenu = "MainMenu";

    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject ServerMenu;
    [SerializeField] private GameObject LobbyMenu;
    [SerializeField] private TMP_InputField RoomNameInput;
    [SerializeField] private TMP_InputField MessageInput;
    [SerializeField] private TMP_InputField NameInput;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {            
            if (EventSystem.current.currentSelectedGameObject == MessageInput.gameObject)
            {
                SendMessage();
            }
            else if (EventSystem.current.currentSelectedGameObject == RoomNameInput.gameObject)
            {
                CreateRoom();
                OpenMenu(LobbyMenu);
            }
            else if (EventSystem.current.currentSelectedGameObject == NameInput.gameObject)
            {
                JoinServer();
            }
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(AI_Scene.SCENENAME_MainMenu);
    }

    #region Server Commands
    public void JoinServer()
    {
        if (ServerConnection.Instance.ConnectToServer() && NameInput.text != string.Empty)
        {
            OpenMenu(ServerMenu);
            ServerConnection.Instance.SendPlayerInfo(NameInput.text);
        }
    }

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

    private void OnReceivedTextMessageList(string _playerName, string _textMessage)
    {
        RoomChatList.AddItem(_playerName + ": " + _textMessage);
    }

    private void OnRoomCreated(string _roomName)
    {
        OpenMenu(LobbyMenu);
        RoomChatList.Clear();
    }

    private void OnRoomJoined(string _roomName)
    {
        OpenMenu(LobbyMenu);
        RoomChatList.Clear();
    }
    #endregion
}