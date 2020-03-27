using System;
using System.Collections.Generic;
using System.Threading;
using BangServer.game;
using LiteNetLib;
using LiteNetLib.Utils;
using static BangServer.util.Logger;

namespace BangServer
{
    /// <summary>
    /// An abstraction of a managed socket. The server takes care of managing a <see cref="Game"/> session.
    /// There can only be one game per server.
    /// </summary>
    public class Server
    {
        /// <summary>The server's socket manager.</summary>
        private NetManager Manager { get; }

        /// <summary>A thread to run the main loop on, so the main thread can still accept input.</summary>
        private Thread _workerThread;

        /// <summary>Alias for all the information the server needs to hold on the ongoing game.</summary>
        private readonly Game _game;

        /// <summary>
        /// A new Server. Takes care of registering listener events, binding the server's socket and so on.
        /// </summary>
        public Server()
        {
            _game = new Game();

            var listener = new EventBasedNetListener();
            Manager = new NetManager(listener);

            Manager.Start(5001);

            // Accepts incoming connection, if they share the same key.
            listener.ConnectionRequestEvent += request =>
            {
                if (Manager.PeersCount < 12)
                    request.AcceptIfKey("98io0u");
                else
                    request.Reject();
            };

            // Call this function, when the server shuts down.
            AppDomain.CurrentDomain.ProcessExit += OnServerShutdown;

            // Triggers the PlayerJoin function each time a peer connects successfully.
            listener.PeerConnectedEvent += OnPlayerJoin;

            // Triggers the PlayerQuit function each time a peer disconnects successfully.
            listener.PeerDisconnectedEvent += OnPlayerQuit;

            // Tries to process incoming messages.
            listener.NetworkReceiveEvent += Process;

            Confirm("Server online.");
            Log("Listening..");
            MainLoop();
        }

        /// <summary>
        /// Creates a new worker thread and then checks for incoming 'events' every 10 ms.
        /// </summary>
        private void MainLoop()
        {
            _workerThread = new Thread(() =>
            {
                while (!Console.KeyAvailable)
                {
                    Manager.PollEvents();
                    Thread.Sleep(10);
                }

                Manager.Stop();
            });

            _workerThread.Start();
        }

        /*
         * Incoming:
         * 0 -> Command
         * 1...n -> Data
         *
         * Outgoing:
         * 0 -> Type [INFO, CMD, DATA]
         * 1 -> Topic
         * 2...n -> Data
         */

        /// <summary>
        /// Processes any incoming messages to the server.
        /// </summary>
        /// <param name="peer">The peer (or client) sending data.</param>
        /// <param name="reader">Contains the data.</param>
        /// <param name="method"></param>
        private void Process(NetPeer peer, NetDataReader reader, DeliveryMethod method)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                var message = reader.GetStringArray();

                switch (message[0])
                {
                    case "PlayerJoin": // Means a new player has joined the lobby and should be added to the server.
                        FinishPlayerJoinForPeer(peer, message[1]);
                        break;
                    case "SendPlayerList": // Means the client has requested an update of their playerlist.
                        BroadcastPlayerlist();
                        break;
                    case "LobbyReady": // Means any player has performed a ready check, so the server can check if all the players have done so.
                        ReadyPlayerAndBroadcast(peer, message[1]);
                        break;
                    case "GameStarted": // Means the game scene has been loaded, so the server can distribute roles and chars.
                        _game.Start();
                        SendRoleAndCharacters(peer);
                        break;
                    case "SetMaxHealth": // Means the player is set up and ready to receive data relevant to gameplay.
                        _game.DealCardsAtStart();
                        SendHand(peer, int.Parse(message[1]));
                        break;
                    case "DrawCard":
                        DealCardToPlayer(peer);
                        break;
                    case "NextTurn":
                        _game.NextTurn();
                        break;
                    default: // Means the client sent an incomprehensible message.
                        SendMessage(peer, "INFO", "ERROR", "Unknown Command");
                        break;
                }
            });
        }

        /// <summary>
        /// Add remaining data to player object. Then send ACK and the playerlist to the new player, 
        /// and <see cref="Broadcast"/> the new player to the rest.
        /// </summary>
        /// <param name="peer">The connection information required to identify the player.</param>
        /// <param name="username">Alias for the player.</param>
        private void FinishPlayerJoinForPeer(NetPeer peer, string username)
        {
            Player player = new Player(peer.Id);

            if (_game.PlayerIsOnline(player))
            {
                player = _game.FindPlayerById(peer.Id);
                player.SetUsername(username);
                SendMessage(peer, "DATA", "JOIN", "ACK"); // Accept connection
                SendMessage(peer, "DATA", "PLAYERLIST", _game.PlayerListToArray());
                Log($"{username} connected.");

                Broadcast("DATA", "PLAYERJOINED", username);
            }
            else
            {
                SendMessage(peer, "DATA", "JOIN", "REJ"); // Reject connection
            }
        }

        /// <summary>
        /// Sends one message to a specific connected peer.
        /// </summary>
        /// <param name="peer">The connection information required to identify the player.</param>
        /// <param name="type">The type of message [INFO, CMD, DATA]</param>
        /// <param name="topic">The topic of this message.</param>
        /// <param name="data">Any number of strings</param>
        private void SendMessage(NetPeer peer, string type, string topic, params string[] data)
        {
            NetDataWriter output = new NetDataWriter();
            List<string> message = new List<string>();
            message.Add(type);
            message.Add(topic);
            message.AddRange(data);

            output.PutArray(message.ToArray());
            if ((peer?.ConnectionState & ConnectionState.Connected) != 0)
            {
                peer?.Send(output, DeliveryMethod.ReliableOrdered);
            }
        }

        /// <summary>
        /// Sends one message to all connected clients.
        /// </summary>
        /// <param name="type">The type of message [INFO, CMD, DATA]</param>
        /// <param name="topic">The topic of this message.</param>
        /// <param name="data">Any number of strings</param>
        private void Broadcast(string type, string topic, params string[] data)
        {
            NetDataWriter output = new NetDataWriter();
            List<string> message = new List<string>();
            message.Add(type);
            message.Add(topic);
            message.AddRange(data);

            output.PutArray(message.ToArray());
            Manager?.SendToAll(output, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Uses <see cref="Broadcast"/> to update the playerlists of all clients.
        /// </summary>
        private void BroadcastPlayerlist()
        {
            Broadcast("DATA", "PLAYERLIST", _game.PlayerListToArray());
        }

        private void ReadyPlayerAndBroadcast(NetPeer peer, string message)
        {
            switch (message)
            {
                case "Ready":
                {
                    Player player = _game.FindPlayerById(peer.Id);
                    player.Ready();
                    Broadcast("DATA", "LOBBYREADY", "READY", player.Username);
                    break;
                }
                case "Unready":
                {
                    Player player = _game.FindPlayerById(peer.Id);
                    player.Unready();
                    Broadcast("DATA", "LOBBYREADY", "UNREADY", player.Username);
                    break;
                }
            }

            if (_game.LobbyReady())
            {
                Log("Lobby is ready.");
                Broadcast("DATA", "GAMESTART");
                _game.SetupRoles(_game.PlayersOnline());
            }

            Log("Lobby isn't ready.");
        }

        private void SendRoleAndCharacters(NetPeer peer)
        {
            Player player = _game.FindPlayerById(peer.Id);
            Player sheriff = _game.FindPlayerByRole("Sheriff");

            SendMessage(peer, "DATA", "ROLEINFO", player.Username, player.Role);
            SendMessage(peer, "DATA", "ROLEINFO", sheriff.Username, sheriff.Role);

            foreach (Player p in _game.Players)
            {
                SendMessage(peer, "DATA", "CHARACTERINFO", p.Username, p.Character);
            }

            SendMessage(peer, "DATA", "ALLSENT");
        }

        private void SendHand(NetPeer peer, int maxHealth)
        {
            Player player = _game.FindPlayerById(peer.Id);

            player.MaxHealth = maxHealth;

            SendMessage(peer, "DATA", "RECEIVEHANDCARDS", player.Hand.ToArray());
            Log($"Sent cards to {player.Username}");
        }

        private void DealCardToPlayer(NetPeer peer)
        {
            Player player = _game.FindPlayerById(peer.Id);
            var card = _game.Deck.Draw();
            
            SendMessage(peer, "DATA", "CARD", card);
            Log($"{player.Username} drew a {card}.");
        }

        // EVENTS

        private void OnPlayerJoin(NetPeer peer)
        {
            _game.AddPlayer(peer.Id);
        }

        private void OnPlayerQuit(NetPeer peer, DisconnectInfo info)
        {
            Broadcast("DATA", "PLAYERDC", _game.RemovePlayer(peer.Id).Username);

            if (_game?.PlayersOnline() <= 0)
            {
                _game.Restart();
            }
        }

        private void OnServerShutdown(object sender, EventArgs e)
        {
            Broadcast("INFO", "SERVERQUIT");
            _workerThread?.Join();
        }
    }
}