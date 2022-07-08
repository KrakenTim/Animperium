using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;
using System.Threading;
using System.Text;
using System.Linq;

public class ServerConnection : MonoBehaviour
{
    public UnityEvent<string> ReceivedPlayerInfoEvent = new UnityEvent<string>();
    public UnityEvent<string> ReceivedServerRoomListEvent = new UnityEvent<string>();
    public UnityEvent<string> ReceivedServerPlayerListEvent = new UnityEvent<string>();
    public UnityEvent<string> ReceivedRoomPlayerListEvent = new UnityEvent<string>();
    public UnityEvent<string, string> ReceivedTextMessageEvent = new UnityEvent<string, string>();
    public UnityEvent<string, string> ReceivedCommandEvent = new UnityEvent<string, string>();
    public UnityEvent<string> RoomCreatedEvent = new UnityEvent<string>();
    public UnityEvent<string> RoomJoinedEvent = new UnityEvent<string>();
    //public UnityEvent<byte[]> ReceivedMapDataEvent = new UnityEvent<byte[]>();
    public UnityEvent<string> ReceivedMapDataEvent = new UnityEvent<string>();

    private TcpClient tcpClient;
    private NetworkStream networktStream;    
    private byte[] streamDataBuffer = new byte[8192];
    private Thread heartbeatThread;
    private int heartbeatTick = 0;
    private int heartbeatIntervall = 3000; //in milliseconds
    private string commandBuffer = "";

    private const string serverIP = "85.215.200.62";
    private const int serverPort = 2018;

    private static ServerConnection instance;
    public static ServerConnection Instance { get { return instance; } }

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public bool ConnectToServer()
    {
        try
        {
            tcpClient = new TcpClient(serverIP, serverPort);
            networktStream = tcpClient.GetStream();
            Debug.Log("Connected to server!");

            StartCoroutine(ListenToConnection());

            heartbeatThread = new Thread(Heartbeat);
            heartbeatThread.IsBackground = true;
            heartbeatThread.Start();
            return true;
        }
        catch (ArgumentNullException e)
        {
            Debug.Log("ArgumentNullException: " + e);
            return false;
        }
        catch (SocketException e)
        {
            Debug.Log("SocketException: " + e);
            return false;
        }
    }

    private IEnumerator ListenToConnection()
    {
        while (true)
        {
            streamDataBuffer = new Byte[8192];
            string receivedData = "";
            try
            {
                if (networktStream.DataAvailable)
                {
                    int bytesData = this.networktStream.Read(streamDataBuffer, 0, streamDataBuffer.Length);
                    receivedData = Encoding.UTF8.GetString(streamDataBuffer, 0, bytesData);

                    //receivedData = commandBuffer + receivedData;
                    string[] receivedCommands = receivedData.Split(';');

                    //if (receivedCommands.Length > 1)
                    //    commandBuffer = receivedCommands[1];

                    for (int i = 0; i < receivedCommands.Length; i++)
                    {
                        if (receivedCommands[i] == string.Empty)
                            continue;
                        string[] splittedData = receivedCommands[i].Split('|');
                        if (splittedData[0] != "HEARTBEAT")
                        {
                            Debug.Log("Received command: " + receivedCommands[i]);
                        }
                        //if (splittedData[0] == "MAPDATA")
                        //{
                        //    Debug.Log("Received MAPDATA");
                        //    InterpretMapData(streamDataBuffer);
                        //}
                        //else
                        //{
                            InterpretMessage(splittedData);
                        //}
                    }

                    //if (splittedMessage.Length == 1)
                    //{
                    //    Debug.Log("No parameter sent!\nCommand: " + splittedMessage[0]);
                    //    splittedMessage = new string[3] { splittedMessage[0], "", "" };
                    //}


                }
            }
            catch (Exception e)
            {
                //Debug.Log("Reading Connection Error: " + e + "\nReceived Data: " + receivedData + "\nCommand: " + receivedCommands[0]);
                Debug.Log("Reading Connection Error: " + e + "\nReceived Data: " + receivedData);
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void InterpretMessage(string[] _message)
    {
        switch (_message[0])
        {
            case "PLAYER_INFO":
                ReceivedPlayerInfoEvent.Invoke(_message[1]);
                break;
            case "ROOM_PLAYERLIST":
                ReceivedRoomPlayerListEvent.Invoke(_message[1]);
                break;
            case "SERVER_ROOMLIST":
                ReceivedServerRoomListEvent.Invoke(_message[1]);
                break;
            case "SERVER_PLAYERLIST":
                ReceivedServerPlayerListEvent.Invoke(_message[1]);
                break;
            case "TEXTMESSAGE": //1: Sender Player Name, 2: Text Message
                ReceivedTextMessageEvent.Invoke(_message[1], _message[2]);
                break;
            case "COMMAND": //1: Sender Player Name, 2: Command
                ReceivedCommandEvent.Invoke(_message[1], _message[2]);
                break;
            case "HEARTBEAT":
                break;
            case "MAPDATA":
                ReceivedMapDataEvent.Invoke(_message[1]);
                break;
            default:
                Console.WriteLine("Can't interpret received message!\nMessage: " + _message);
                break;
        }
    }

    private void InterpretMapData(byte [] _bytes)
    {
        byte[] messageType = Encoding.ASCII.GetBytes("MAPDATA|");
        byte[] messageEndIndicator = Encoding.UTF8.GetBytes(";");
        List<byte> mapData = new List<byte>();
        for (int i = messageType.Length; i < _bytes.Length - messageEndIndicator.Length; i++)
        {
            mapData.Add(_bytes[i]);
        }
        //ReceivedMapDataEvent.Invoke(mapData.ToArray());
    }

    void Heartbeat()
    {
        while (true)
        {
            heartbeatTick++;
            try
            {
                Send("HEARTBEAT|" + heartbeatTick.ToString());
            }
            catch
            {
                Debug.Log("No server connection!");
                CloseConnection();
                break;
            }

            Thread.Sleep(heartbeatIntervall);
        }
    }


    private void OnDisable()
    {
        CloseConnection();
    }

    private void Send(string _message)
    {
        byte[] msg = Encoding.UTF8.GetBytes(_message + ';');

        networktStream.Write(msg, 0, msg.Length);

        //if (_message.Split('|')[0] != "HEARTBEAT")
        //{
        //    Debug.Log("Sent to server:\n" + _message);
        //}        
    }

    private void SendByteArray(string _message, byte [] _bytes)
    {
        byte[] messageType = Encoding.UTF8.GetBytes(_message);
        byte[] messageEndIndicator = Encoding.UTF8.GetBytes(";");

        byte[] msg = new byte[messageType.Length + _bytes.Length + messageEndIndicator.Length];

        messageType.CopyTo(msg, 0);
        _bytes.CopyTo(msg, messageType.Length);
        messageEndIndicator.CopyTo(msg, messageType.Length + _bytes.Length);

        networktStream.Write(msg, 0, msg.Length);

        //if (_message.Split('|')[0] != "HEARTBEAT")
        //{
        //    Debug.Log("Sent to server:\n" + _message);
        //}        
    }

    #region Sender
    public void CreateRoom(string _roomName)
    {
        Send("ROOM_CREATE|" + _roomName);
        RoomCreatedEvent.Invoke(_roomName);
    }

    public void LeaveRoom()
    {
        Send("ROOM_LEAVE");
    }

    public void JoinRoom(string _roomName)
    {
        Send("ROOM_JOIN|" + _roomName);
        RoomJoinedEvent.Invoke(_roomName);
    }

    public void LeaveServer()
    {
        Send("SERVER_LEAVE");
        CloseConnection();
    }

    public void SendTextMessage(string _textMessage)
    {
        Send("TEXTMESSAGE|" + _textMessage);
    }

    public void SendCommand(string _command)
    {
        Send("COMMAND|" + _command);
    }

    public void SendPlayerInfo(string _name)
    {
        Send("PLAYER_INFO|" + _name);
    }

    public void SendMapData(byte [] _mapdata)
    {
        SendByteArray("MAPDATA|", _mapdata);
    }

    public void SendMapData(string _mapdata)
    {
        Send("MAPDATA|" + _mapdata);
    }
    #endregion

    public void CloseConnection()
    {
        try
        {
            StopAllCoroutines();
            heartbeatThread.Abort();
            networktStream.Close();
            tcpClient.Close();
            Debug.Log("Server connection closed!");
        }
        catch (Exception e)
        {
            Debug.Log("Failed to close server connection!\n" + e);
        }

    }
}
