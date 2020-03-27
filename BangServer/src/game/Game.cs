using System.Collections.Generic;
using System.Linq;
using BangServer.util;
using static BangServer.util.Logger;

namespace BangServer.game
{
    public class Game
    {
        private const int MinPlayers = 2;
        public List<Player> Players { get; set; }

        private List<string> Characters { get; set; }
        private List<string> Roles { get; set; }
        public Deck Deck { get; set; }
        public Stack<string> DiscardPile { get; set; }

        private bool _setup;

        public Game()
        {
            _setup = false;

            Players = new List<Player>();
            Characters = SetupCharacters();
            DiscardPile = new Stack<string>();
        }

        public void Start()
        {
            Highlight("Game starting..");

            Deck = new Deck();
            AssignRolesAndCharacters();

            Players[0].TheirTurn = true;

            Confirm("Game started..");
        }

        public void Restart()
        {
            Highlight("Game restarting..");

            // Constructor stuff
            _setup = false;

            Players = new List<Player>();
            Characters = SetupCharacters();
            DiscardPile = new Stack<string>();

            // Start stuff
            Start();
        }

        public void NextTurn()
        {
            Player currentTurn = Players.First(player => player.TheirTurn = true);
            currentTurn.TheirTurn = false;

            Player nextPlayer;

            if (!(Players.IndexOf(currentTurn) >= PlayersOnline() - 1))
            {
                nextPlayer = Players[Players.IndexOf(currentTurn) + 1];
                nextPlayer.TheirTurn = true;
            }
            else
            {
                nextPlayer = Players[0];
                nextPlayer.TheirTurn = true;
            }
            
            Log($"It's now {nextPlayer.Username}'s turn.");
        }

        public bool LobbyReady()
        {
            foreach (Player player in Players)
            {
                if (!player.ReadyCheck) return false;
            }

            return PlayersOnline() >= MinPlayers;
        }

        public void DealCardsAtStart()
        {
            for (int i = 0; i < 10; i++)
            {
                foreach (Player player in Players.Where(player => player.CardsInHand() <= player.MaxHealth))
                {
                    AddCardToHand(player, Deck.Draw());
                }
            }
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

            if (player != null)
            {
                Log($"{player.Username} disconnected.");
                Players.Remove(player);

                return player;
            }

            Error("Player couldn't be removed.");
            return null;
        }
        
        private void AddCardToHand(Player player, string card)
        {
            player.Hand?.Add(card);
        }
        
        private void AddCardsToHand(Player player, params string[] cards)
        {
            player.Hand?.AddRange(cards);
        }

        private void RemoveCardFromHand(Player player, string card)
        {
            player.Hand?.Remove(card);
            DiscardPile.Push(card);
        }
        
        private void RemoveCardsFromHand(Player player, params string[] cards)
        {
            foreach (var card in cards)
            {
                player.Hand?.Remove(card);
                DiscardPile.Push(card);
            }
        }

        private string FindCardInDeck(string card)
        {
            return Deck.First(c => c == card);
        }
        
        private string FindCardInDiscardPile(string card)
        {
            return DiscardPile.First(c => c == card);
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

        public Player FindPlayerByRole(string role)
        {
            return Players.Find(player => player.Role == role);
        }

        public Player FindPlayerByCharacter(string character)
        {
            return Players.Find(player => player.Character == character);
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

        public void AssignRolesAndCharacters()
        {
            if (_setup) return;

            Roles.Shuffle();
            Characters.Shuffle();

            foreach (Player player in Players)
            {
                player.AssignCharacter(Characters.Last());
                Characters.Remove(Characters.Last());
                player.AssignRole(Roles.Last());
                Roles.Remove(Roles.Last());

                Highlight($"{player.Username} is now known as {player.Character}, playing the {player.Role}.");
            }

            _setup = true;
        }

        public void SetupRoles(int playerNumber)
        {
            if (playerNumber >= MinPlayers)
            {
                Roles = new Dictionary<int, List<string>>
                {
                    // DEBUG
                    {
                        1, new List<string>
                        {
                            "Sheriff",
                        }
                    },
                    // DEBUG
                    {
                        2, new List<string>
                        {
                            "Sheriff",
                            "Outlaw",
                        }
                    },
                    // DEBUG
                    {
                        3, new List<string>
                        {
                            "Sheriff",
                            "Outlaw",
                            "Renegade"
                        }
                    },
                    // REAL STUFF DOWN HERE
                    {
                        4, new List<string>
                        {
                            "Sheriff",
                            "Outlaw",
                            "Outlaw",
                            "Renegade"
                        }
                    },
                    {
                        5, new List<string>
                        {
                            "Sheriff",
                            "Outlaw",
                            "Outlaw",
                            "Deputy",
                            "Renegade"
                        }
                    },
                    {
                        6, new List<string>
                        {
                            "Sheriff",
                            "Outlaw",
                            "Outlaw",
                            "Deputy",
                            "Deputy",
                            "Renegade"
                        }
                    },
                    {
                        7, new List<string>
                        {
                            "Sheriff",
                            "Outlaw",
                            "Outlaw",
                            "Outlaw",
                            "Deputy",
                            "Deputy",
                            "Renegade"
                        }
                    }
                }[playerNumber];
            }
            else
            {
                Error("Invalid number of players.");
            }
        }

        private List<string> SetupCharacters()
        {
            return new List<string>()
            {
                "Bart Cassidy",
                "Black Jack",
                "Calamity Janet",
                "El Gringo",
                "Jesse Jones",
                "Jourdonnais",
                "Kit Carlson",
                "Lucky Duke",
                "Paul Regret",
                "Pedro Ramirez",
                "Rose Doolan",
                "Sid Ketchum",
                "Slab the Killer",
                "Suzy Lafayette",
                "Vulture Sam",
                "Willy The Kid"
            };
        }
    }
}