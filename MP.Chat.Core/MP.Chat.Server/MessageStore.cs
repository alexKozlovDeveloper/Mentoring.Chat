using MP.Chat.Core;
using MP.Chat.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP.Chat.Server
{
    public class MessageStore
    {
        public List<ChatMessage> Messages { get; private set; }

        public delegate void NewMessageDelegate(ChatMessage message);
        public event NewMessageDelegate NewMessage;

        private Logger _logger;

        private object locker = new object();

        private ConsoleViewer _consoleViewer;

        public MessageStore(Logger logger)
        {
            _logger = logger;

            Messages = new List<ChatMessage>();

            _consoleViewer = new ConsoleViewer(logger);
        }

        public void AddNewMessage(ChatMessage message)
        {
            lock (locker)
            {
                Messages.Add(message);

                if(NewMessage != null)
                {
                    NewMessage(message);
                }                
            }

            _consoleViewer.WriteMessageToConsole(message);
        }
    }
}
