using System;
using static BangServer.Logger;

namespace BangServer
{
    internal class ServerConsole
    {
        public static void Main(string[] args)
        {
            Log("Starting server..");
            Server server = new Server();
        }
    }
}