using System;
using System.Text;

namespace BangServer
{
    public class Logger
    {
        public static void Log(params string[] messages)
        {
            StringBuilder sb = new StringBuilder();
            
            foreach (var message in messages)
            {
                sb.Append(message);
                sb.Append(" ");
            }
            
            Console.WriteLine("<Log>: {0}", sb);
        }

        public static void Warn(params string[] messages)
        {
            StringBuilder sb = new StringBuilder();
            
            foreach (var message in messages)
            {
                sb.Append(message);
                sb.Append(" ");
            }
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("<Warning>: {0}", sb);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void Error(params string[] messages)
        {
            StringBuilder sb = new StringBuilder();
            
            foreach (var message in messages)
            {
                sb.Append(message);
                sb.Append(" ");
            }
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("<Error>: {0}", sb);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}