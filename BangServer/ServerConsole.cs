using System;
using System.Threading;
using static BangServer.Logger;
using NetMQ;
using NetMQ.Sockets;

namespace BangServer
{
    internal class ServerConsole
    {
        public static void Main(string[] args)
        {
            Log("Starting server..");
            Server server = new Server();

            Random rand = new Random(50);


            new Thread(() =>
            {
                Log("Listening..");
                
                var timeout = new System.TimeSpan(0, 0, 5);

                bool stopThread = false;

                // Listen for requests
                while (!stopThread)
                {
                    // Build new response socket
                    using (ResponseSocket serverSocket = new ResponseSocket("@tcp://*:5001"))
                    {
                        // Wait for incoming messages
                        var message = serverSocket.ReceiveMultipartMessage();

                        // Check if message has content
                        if (message.FrameCount > 0)
                        {
                            string playername;
                            // Take apart header (index 0) and respond accordingly
                            switch (message[0].ConvertToString())
                            {
                                case "PlayerJoined":
                                    playername = message[1].ConvertToString();
                                    
                                    // Add the player to the server's playerlist
                                    if (server.AddPlayer(playername))
                                        serverSocket.SendFrame("join-200"); // OK - Response contains result
                                    else
                                        serverSocket.SendFrame("join-409"); // Conflict - User already known
                                    break;
                                case "PlayerQuit":
                                    playername = message[1].ConvertToString();
                                    
                                    // Add the player to the server's playerlist
                                    if (server.RemovePlayer(playername))
                                        serverSocket.SendFrame("quit-200"); // OK - Response contains result
                                    else
                                        serverSocket.SendFrame("quit-404"); // Conflict - User not found
                                    break;
                                default:
                                    serverSocket.SendFrame("quit-406"); // Not Acceptable - Unknown command
                                    break;
                            }
                        }
                        else
                        {
                            serverSocket.SendFrame("join-204"); // No content sent
                        }
                        
                        // Cleanup
                        serverSocket.Close();
                        NetMQConfig.Cleanup();
                    }
                }
            }).Start();

            Log("Press any key to exit.");
            Console.ReadLine();
        }
    }
}