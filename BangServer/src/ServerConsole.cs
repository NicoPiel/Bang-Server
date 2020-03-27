using static BangServer.util.Logger;

namespace BangServer
{
    internal class ServerConsole
    {
        public static void Main(string[] args)
        {
            Log("Starting server..");
            Highlight("Press any key to exit.");
            Server server = new Server();
        }
    }
}