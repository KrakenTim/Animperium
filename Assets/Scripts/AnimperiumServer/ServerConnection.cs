using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;
using System.Threading;
using System.Text;

public class ServerConnection : MonoBehaviour
{
    public UnityEvent<string> ReceivedServerRoomListEvent = new UnityEvent<string>();
    public UnityEvent<string> ReceivedServerPlayerListEvent = new UnityEvent<string>();
    public UnityEvent<string> ReceivedRoomPlayerListEvent = new UnityEvent<string>();
    public UnityEvent<string> ReceivedTextMessageEvent = new UnityEvent<string>();
    public UnityEvent<string> ReceivedCommandEvent = new UnityEvent<string>();
    public UnityEvent<string> RoomCreatedEvent = new UnityEvent<string>();
    public UnityEvent<string> RoomJoinedEvent = new UnityEvent<string>();

    private TcpClient tcpClient;
    private NetworkStream networktStream;    
    private byte[] streamDataBuffer = new byte[256];
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

    public void ConnectToServer()
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
        }
        catch (ArgumentNullException e)
        {
            Debug.Log("ArgumentNullException: " + e);
        }
        catch (SocketException e)
        {
            Debug.Log("SocketException: " + e);
        }
    }

    private IEnumerator ListenToConnection()
    {
        while (true)
        {
            streamDataBuffer = new Byte[256];
            string receivedData = "";
            string[] receivedCommands = new string [0];
            try
            {
                if (networktStream.DataAvailable)
                {
                    int bytesData = this.networktStream.Read(streamDataBuffer, 0, streamDataBuffer.Length);
                    receivedData = Encoding.UTF8.GetString(streamDataBuffer, 0, streamDataBuffer.Length);


                    //receivedData = commandBuffer + receivedData;
                    receivedCommands = receivedData.Split(new char[] { ';' }, 2);
                    if (receivedCommands.Length > 1)
                        commandBuffer = receivedCommands[1];

                    string[] splittedMessage = receivedCommands[0].Split(new char[] { '|' }, 2);

                    if (splittedMessage.Length == 1)
                    {
                        Debug.Log("No parameter sent!\nCommand: " + splittedMessage[0]);
                        splittedMessage = new string[2] { splittedMessage[0], "" };
                    }

                    switch (splittedMessage[0])
                    {
                        case "ROOM_PLAYERLIST":
                            ReceivedRoomPlayerListEvent.Invoke(splittedMessage[1]);
                            Debug.Log("Received data: " + receivedData);
                            break;
                        case "SERVER_ROOMLIST":
                            ReceivedServerRoomListEvent.Invoke(splittedMessage[1]);
                            Debug.Log("Received data: " + receivedData);
                            break;
                        case "SERVER_PLAYERLIST":
                            ReceivedServerPlayerListEvent.Invoke(splittedMessage[1]);
                            Debug.Log("Received data: " + receivedData);
                            break;
                        case "TEXTMESSAGE":
                            ReceivedTextMessageEvent.Invoke(splittedMessage[1]);
                            Debug.Log("Received data: " + receivedData);
                            break;
                        case "COMMAND":
                            ReceivedCommandEvent.Invoke(splittedMessage[1]);
                            break;
                        case "HEARTBEAT":
                            break;
                        default:
                            Console.WriteLine("Can't interpret received data!\nReceived Data: " + receivedData + "\nCommand: " + receivedCommands[0]);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Reading Connection Error: " + e + "\nReceived Data: " + receivedData + "\nCommand: " + receivedCommands[0]);
            }
            yield return new WaitForSeconds(0.05f);
        }
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

        if (_message.Split('|')[0] != "HEARTBEAT")
        {
            Debug.Log("Sent to server:\n" + _message);
        }        
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
