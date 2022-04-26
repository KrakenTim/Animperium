using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace AnimperiumServer
{
    public class Room
    {
        public string Name = "";
        public List<Connection> ConnectionList = new List<Connection>();

        public string PlayerListAsMessage
        {
            get
            {
                string playerListMessage = "ROOM_PLAYERLIST";
                foreach (Connection connection in ConnectionList)
                {
                    playerListMessage += "|" + connection.PlayerName;
                }
                return playerListMessage;
            }
        }

        public Room(string _name, Connection _connection)
        {
            Name = _name;
            AddConnection(_connection);
            Console.WriteLine("{0} created a new room: {1}", _connection.PlayerName, _name);
        }

        public void AddConnection(Connection _connection)
        {
            ConnectionList.Add(_connection);
            Console.WriteLine("{0} joined room: {1}", _connection.PlayerName, Name);
            SendBroadcast("TEXTMESSAGE|" + _connection.PlayerName + " joined!");

            foreach (Connection connection in ConnectionList)
            {
                connection.Send(PlayerListAsMessage);
            }
        }

        public void RemoveConnection(Connection _connection)
        {
            ConnectionList.Remove(_connection);
            Console.WriteLine("Removed {1} from room: {0}", _connection.PlayerName, Name);
            SendBroadcast("TEXTMESSAGE|" + _connection.PlayerName + " left!");

            foreach (Connection connection in ConnectionList)
            {
                connection.Send(PlayerListAsMessage);
            }

            if (ConnectionList.Count == 0)
            {
                Server.Instance.DeleteRoom(this);
            }
        }

        public void SendBroadcast(string message)
        {
            foreach (Connection connection in ConnectionList)
            {
                connection.Send(message);
            }
        }
    }
}