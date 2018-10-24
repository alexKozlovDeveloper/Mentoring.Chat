using MP.Chat.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP.Chat.Core
{
    public class ConsoleViewer
    {
        private Logger _logger;

        public ConsoleViewer(Logger logger)
        {
            _logger = logger;
        }

        public void WriteMessageToConsole(ChatMessage message, ConsoleColor color)
        {
            var oldColor = Console.ForegroundColor;

            Console.ForegroundColor = color;

            var str = GetStringMessage(message);

            Console.WriteLine();
            _logger.Info(str);

            Console.ForegroundColor = oldColor;
        }

        public void WriteMessageToConsole(ChatMessage message)
        {
            WriteMessageToConsole(message, ConsoleColor.White);
        }

        private string GetStringMessage(ChatMessage message)
        {
            return $"{message.Name}: {message.Content}";
        }
    }
}
