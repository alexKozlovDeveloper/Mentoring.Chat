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
        }

        public void Error(string message)
        {
            LogToFile($"ERROR! '{message}'");
        }

        public void Error(Exception ex)
        {
            LogToFile($"ERROR! '{ex.Message}'");
        }

        private void LogToFile(string message)
        {
            lock(_lock)
            {
                var str = $"{DateTime.Now.ToString("hh:mm:ss.ffff")} {message}";

                if(_writeToConsole == true)
                {
                    Console.WriteLine(str);
                }

                str += Environment.NewLine;

                File.AppendAllText(_filePath, str);
            }            
        }
    }
}
