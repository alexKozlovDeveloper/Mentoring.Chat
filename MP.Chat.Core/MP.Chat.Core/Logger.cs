using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP.Chat.Core
{
    public class Logger
    {
        private string _filePath;

        private string _lock = "lock";

        private bool _writeToConsole;

        public Logger(string filePath, bool writeToConsole = true)
        {
            _filePath = filePath;
            _writeToConsole = writeToConsole;
        }

        public void Info(string message)
        {
            LogToFile($"Info: '{message}'");
            LogToConsole($"Info: '{message}'", ConsoleColor.Green);
        }

        public void Error(string message)
        {
            LogToFile($"ERROR! '{message}'");
            LogToConsole($"ERROR! '{message}'", ConsoleColor.Red);
        }

        public void Error(Exception ex)
        {
            Error(ex.Message);
        }

        private void LogToConsole(string message, ConsoleColor color)
        {
            if (_writeToConsole == true)
            {
                var oldColor = Console.ForegroundColor;

                Console.ForegroundColor = color;

                Console.WriteLine(message);

                Console.ForegroundColor = oldColor;
            }
        }

        private void LogToFile(string message)
        {
            lock(_lock)
            {
                var str = $"{DateTime.Now.ToString("hh:mm:ss.ffff")} {message}";                

                str += Environment.NewLine;

                File.AppendAllText(_filePath, str);
            }            
        }
    }
}
