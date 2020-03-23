using static BangServer.Logger;

namespace BangServer
{
    internal class ServerConsole
    {
        public static void Main(string[] args)
        {
            Log("Starting server..");
            Warn("Press any key to exit.");
            Server server = new Server();
        }
    }
}