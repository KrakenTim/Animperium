using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AnimperiumServer
{
    public class Server
    {
        public List<Connection> ConnectionList = new List<Connection>();
        public List<Room> RoomList = new List<Room>();

        private TcpListener tcpListener;
        private Thread connectThread;
        private Thread heartbeatThread;
        private int heartbeatTick = 0;
        private int heartbeatIntervall = 3000; //in milliseconds
        private int playerID = 0;

        private const int serverPort = 2018;

        private static Server instance;
        public static Server Instance
        {
            get { return instance; }
        }

        public string RoomListAsString
        {
            get
            {
                string roomListMessage = "SERVER_ROOMLIST";
                foreach (Room room in RoomList)
                {
                    roomListMessage += "|" + room.Name;
                }
                return roomListMessage;
            }
        }

        public string PlayerListAsMessage
        {
            get
            {
                string playerListMessage = "SERVER_PLAYERLIST";
                foreach (Connection connection in ConnectionList)
                {
                    playerListMessage += "|" + connection.PlayerName;
                }
                return playerListMessage;
            }
        }

        public Server()
        {
            if (instance == null)
            {
                instance = this;
            }

            tcpListener = new TcpListener(IPAddress.Any, serverPort);
            
            try
            {
                tcpListener.Start(); 
                Console.WriteLine("Server started!");
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            connectThread = new Thread(ListenToConnectionRequests);
            connectThread.IsBackground = true;
            connectThread.Start();

            heartbeatThread = new Thread(Heartbeat);
            heartbeatThread.IsBackground = true;
            heartbeatThread.Start();
        }

        private void ListenToConnectionRequests()
        {
            Console.WriteLine("Awaiting connection requests...");
            while (true)
            {
                while (tcpListener.Pending())
                {
                    lock (ConnectionList)
                    {
                        TcpClient newTcpClient = tcpListener.AcceptTcpClient();
                        IPAddress clientIP = IPAddress.Parse(((IPEndPoint)newTcpClient.Client.RemoteEndPoint).Address.ToString());

                        Connection newConnection = new Connection(newTcpClient, clientIP, "Player" + playerID++);
                        ConnectionList.Add(newConnection);
                        newConnection.Send(PlayerListAsMessage);
                        newConnection.Send(RoomListAsString);

                        Console.WriteLine("New client connected! IP: {0} Name: {1}\n", newConnection.IP, newConnection.PlayerName);
                        Console.WriteLine("Connected player count: {0}", ConnectionList.Count);
                    }
                }
                Thread.Sleep(10);
            }
        }

        void Heartbeat()
        {
            while (true)
            {
                heartbeatTick++;
                for (int i = 0; i < ConnectionList.Count; i++)
                {
                    try
                    {
                        ConnectionList[i].Send("HEARTBEAT|" + heartbeatTick.ToString());
                    }
                    catch
                    {
                        DeleteConnection(ConnectionList[i]);
                        i--;
                    }
                }

                Thread.Sleep(heartbeatIntervall);
            }
        }

        public void DeleteConnection(Connection _connection)
        {
            Console.WriteLine("Client disconnected! Name: {0}", _connection.PlayerName);
            ConnectionList.Remove(_connection);
        }

        public void DeleteRoom(Room _room)
        {
            Console.WriteLine("Deleting room: {0}", _room.Name);
            RoomList.Remove(_room);
        }
    }
}