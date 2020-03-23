using System;
using System.Collections.Generic;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;
using static BangServer.Logger;

namespace BangServer
{
    public class Server
    {
        /// <summary></summary>
        public NetManager Manager { get; }
        private Thread _workerThread;
        private Game _game;

        /// <summary>
        /// A new Server. Takes care of registering listener events, binding the server's socket and so on.
        /// </summary>
        public Server()
        {
            _game = new Game();

            var listener = new EventBasedNetListener();
            Manager = new NetManager(listener);

            Manager.Start(5001);

            listener.ConnectionRequestEvent += request =>
            {
                if (Manager.PeersCount < 12)
                    request.AcceptIfKey("98io0u");
                else
                    request.Reject();
            };

            listener.PeerConnectedEvent += PlayerJoin;

            listener.PeerDisconnectedEvent += PlayerQuit;

            listener.NetworkReceiveEvent += Process;

            Log("Listening..");
            MainLoop();
        }

        /// <summary>
        /// Creates a new worker thread and then checks for incoming 'events' every 20 ms.
        /// </summary>
        private void MainLoop()
        {
            _workerThread = new Thread(() =>
            {
                while (!Console.KeyAvailable)
                {
                    Manager.PollEvents();
                    Thread.Sleep(20);
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
            var message = reader.GetStringArray();
            
            switch (message[0])
            {
                case "PlayerJoin":
                    FinishPlayerJoinForPeer(peer, message[1]);
                    break;
                case "SendPlayerList":
                    BroadcastPlayerlist();
                    break;
                case "LobbyReady":
                    ReadyPlayerAndBroadcast(peer, message[1]);
                    break;
                default:
                    SendMessage(peer, "INFO", "ERROR", "Unknown Command");
                    break;
            }
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
                Log($"{username} logged in successfully.");
                
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
            
            if (_game.LobbyReady()) Log("Lobby is ready.");
            else Log("Lobby isn't ready.");
        }

        // EVENTS

        private void PlayerJoin(NetPeer peer)
        {
            _game.AddPlayer(peer.Id);
        }

        private void PlayerQuit(NetPeer peer, DisconnectInfo info)
        {
            Broadcast("DATA", "PLAYERDC", _game.RemovePlayer(peer.Id).Username);
        }
    }
}