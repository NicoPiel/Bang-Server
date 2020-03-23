using System.Collections.Generic;
using static BangServer.Logger;

namespace BangServer
{
    public class Game
    {
        public List<Player> Players { get; }

        public Game()
        {
            Players = new List<Player>();
        }

        public bool LobbyReady()
        {
            foreach (Player player in Players)
            {
                if (!player.ReadyCheck) return false;
            }

            return true;
        }
        
        /// <summary>
        /// Add a player to the <see cref="Players"/> list.
        /// </summary>
        public bool AddPlayer(int id)
        {
            Player player = new Player(id);

            if (!PlayerIsOnline(player))
            {
                Players.Add(player);
                return true;
            }
            
            Error("Player " + player.Id + " already online.");
            return false;
        }

        /// <summary>
        /// Removes a player from the <see cref="Players"/> list
        /// </summary>
        public Player RemovePlayer(int id)
        {
            Player player = FindPlayerById(id);
            
            Log($"Player {player.Id}:{player.Username} disconnected.");
            Players.Remove(player);

            return player;
        }

        /// <summary>
        /// Uses LINQ to find a player by their id.
        /// </summary>
        /// <param name="id">The player's id.</param>
        /// <returns>A new <see cref="Player"/> object.</returns>
        public Player FindPlayerById(int id)
        {
            return Players.Find(player => player.Id == id);
        }
        
        /// <summary>
        /// Uses LINQ to find a player by their username.
        /// </summary>
        /// <param name="username">The player's username.</param>
        /// <returns>A new <see cref="Player"/> object.</returns>
        public Player FindPlayerByName(string username)
        {
            return Players.Find(player => player.Username == username);
        }

        /// <summary>
        /// Checks if a player is online by using a <see cref="Player"/> object.
        /// </summary>
        /// <param name="player">Any player.</param>
        /// <returns>True, if the player's client is connected.</returns>
        public bool PlayerIsOnline(Player player)
        {
            return Players.Contains(player);
        }

        /// <summary>
        /// Checks if a player is online by using an ID. The method then creates a dummy <see cref="Player"/> object.
        /// </summary>
        /// <param name="id">Any integer.</param>
        /// <returns>True, if there is a player using said ID.</returns>
        public bool PlayerIsOnline(int id)
        {
            Player player = new Player(id);

            return Players.Contains(player);
        }

        /// <summary>
        /// Converts the <see cref="Players"/> list into an array.
        /// </summary>
        /// <returns></returns>
        public string[] PlayerListToArray()
        {
            List<string> playerNames = new List<string>();
            
            foreach (Player player in Players)
            {
                playerNames.Add(player.Username);
            }

            return playerNames.ToArray();
        }

        public int PlayersOnline()
        {
            return Players.Count;
        }
    }
}