using System;

namespace BangServer
{
    public class Logger
    {
        public static void Log(string message)
        {
            Console.WriteLine("<Log>: {0}", message);
        }

        public static void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("<Warning>: {0}", message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("<Error>: {0}", message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}