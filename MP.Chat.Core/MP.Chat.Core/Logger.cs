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

        public Logger(string filePath)
        {
            _filePath = filePath;
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
                var str = $"{DateTime.Now.ToString("hh:mm:ss.ffff")} {message}{Environment.NewLine}";
                Console.WriteLine(str);
                File.AppendAllText(_filePath, str);
            }            
        }
    }
}
