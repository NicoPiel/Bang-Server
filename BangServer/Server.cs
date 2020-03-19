using System.Collections.Generic;
using static BangServer.Logger;

namespace BangServer
{
    public class Server
    {
        private List<string> players;

        public Server()
        {
            players = new List<string>();
        }

        public bool AddPlayer(string playername)
        {
            if (!players.Contains(playername)) {
                players.Add(playername);
                Log("Player " + playername + " connected.");
                return true;
            }
            
            Warn("Player " + playername + " already online.");
            return false;
        }

        public bool RemovePlayer(string playername)
        {
            if (IsOnline(playername))
            {
                Log("Player " + playername + " disconnected.");
                return true;
            }
            
            Warn("Player " + playername + " isn't online.");
            return false;
        }

        public bool IsOnline(string playername)
        {
            return players.Contains(playername);
        }
    }
}