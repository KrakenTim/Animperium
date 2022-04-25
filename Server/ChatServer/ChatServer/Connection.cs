using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnimperiumServer
{
    public class Connection
    {
        private Thread clientThread;
        private byte[] streamDataBuffer = new byte[256];

        private IPAddress ip;
        public IPAddress IP
        {
            get { return ip; }
        }

        private TcpClient tcpClient;
        public TcpClient TcpClient
        {
            get { return tcpClient; }
        }

        private string playerName;
        public string PlayerName
        {
            get { return playerName; }
        }

        private NetworkStream networktStream;
        public NetworkStream NetworkStream
        {
            get { return networktStream; }
        }

        private Room room;
        public Room Room
        {
            get { return room; }
        }

        public Connection(TcpClient _tcpClient, IPAddress _ip, string _name)
        {
            tcpClient = _tcpClient;
            networktStream = _tcpClient.GetStream();
            ip = _ip;
            playerName = _name;

            clientThread = new Thread(ListenToConnection);
            clientThread.IsBackground = true;
            clientThread.Start();
        }

        ~Connection ()
        {
            LeaveRoom();
        }

        private void ListenToConnection()
        {
            while (true)
            {
                streamDataBuffer = new Byte[256];
                try
                {
                    if (networktStream.DataAvailable)
                    {
                        int bytesData = this.networktStream.Read(streamDataBuffer, 0, streamDataBuffer.Length);
                        string receivedData = Encoding.ASCII.GetString(streamDataBuffer, 0, streamDataBuffer.Length);
                        Console.WriteLine(ip + ": " + receivedData + "\n");
                        InterpretMessage(receivedData);
                    }
                    Thread.Sleep(50);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Stopped listening to server connection! Error: {0}", e);
                    break;
                }

            }
        }

        private void InterpretMessage(string _receivedData)
        {
            string[] splittedData = _receivedData.Split('|');

            switch (splittedData[0])
            {
                case "ROOM_CREATE":
                    CreateRoom(splittedData[1]);
                    break;
                case "ROOM_JOIN":
                    JoinRoom(splittedData[1]);
                    break;
                case "ROOM_LEAVE":
                    LeaveRoom();
                    break;
                case "SERVER_LEAVE":
                    Server.Instance.DeleteConnection(this);
                    break;
                case "TEXTMESSAGE":
                case "COMMAND":
                    BroadcastToRoom(_receivedData);
                    break;
                //case "SERVER_REQUEST_PLAYERLIST":
                //    Send(Server.Instance.PlayerListAsMessage);
                //    break;
                //case "ROOM_REQUEST_PLAYERLIST":
                //    Send(Room.PlayerListAsMessage);
                //    break;
                //case "SERVER_REQUEST_ROOMLIST":
                //    Send(Server.Instance.RoomListAsString);
                //    break;
                default:
                    break;
            }
        }

        private void CreateRoom(string _roomName)
        {
            room = new Room(_roomName, this);
        }

        private void JoinRoom(string _roomName)
        {
            room = Server.Instance.RoomList.FirstOrDefault(x => x.Name == _roomName);
            if (room == null)
            {
                Console.WriteLine("{1] can't join room! Room \"{0}\" does not exist!", _roomName, playerName);
                return;
            }
            room.AddConnection(this);
        }

        private void LeaveRoom()
        {
            if (Room == null)
                return;
            room.RemoveConnection(this);
        }

        private void BroadcastToRoom(string _message)
        {
            if (Room == null)
                return;
            room.SendBroadcast(_message);
        }

        public void Send(string _message)
        {
            byte[] msg = Encoding.ASCII.GetBytes(_message);

            networktStream.Write(msg, 0, msg.Length);
            Console.WriteLine("Sent to {0}: {1}\n", ip, _message);
        }
    }
}
