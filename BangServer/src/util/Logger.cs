using System;
using System.Text;

namespace BangServer.util
{
    /// <summary>
    /// A utility class, printing to the console in different colours.
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// Simply prints any number of strings to the console in gray.
        /// Arguments are printed with a whitespace inbetween.
        /// </summary>
        /// <param name="messages">Any number of strings.</param>
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
        
        /// <summary>
        /// Highlights an important message in dark yellow.
        /// Arguments are printed with a whitespace inbetween.
        /// </summary>
        /// <param name="messages">Any number of strings.</param>
        public static void Highlight(params string[] messages)
        {
            StringBuilder sb = new StringBuilder();
            
            foreach (var message in messages)
            {
                sb.Append(message);
                sb.Append(" ");
            }
            
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("<Log>: {0}", sb);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        
        /// <summary>
        /// Prints any number of strings in green.
        /// Arguments are printed with a whitespace inbetween.
        /// </summary>
        /// <param name="messages">Any number of strings.</param>
        public static void Confirm (params string[] messages)
        {
            StringBuilder sb = new StringBuilder();
            
            foreach (var message in messages)
            {
                sb.Append(message);
                sb.Append(" ");
            }
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("<Log>: {0}", sb);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        /// <summary>
        /// Prints any number of strings in yellow and inserts "warning" at the start.
        /// Arguments are printed with a whitespace inbetween.
        /// </summary>
        /// <param name="messages">Any number of strings.</param>
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

        /// <summary>
        /// Prints any number of strings in red and inserts "error" at the start.
        /// Arguments are printed with a whitespace inbetween.
        /// Should be used in conjunction with exceptions.
        /// </summary>
        /// <param name="messages">Any number of strings.</param>
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