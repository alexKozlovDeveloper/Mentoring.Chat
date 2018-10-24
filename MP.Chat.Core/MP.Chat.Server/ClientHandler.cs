using MP.Chat.Core;
using MP.Chat.Core.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MP.Chat.Server
{
    public class ClientHandler
    {
        public string Id { get; private set; }
        public string ClientName { get; private set; }

        private bool _isAlive;

        private MessageStore _store;

        private Thread _messagesFromUserThread;
        private Thread _messagesToUserThread;

        private NamedPipeServerStream _messagesFromUserPipe;
        private NamedPipeServerStream _messagesToUserPipe;

        private Logger _logger;

        private ConsoleViewer _consoleViewer;

        public ClientHandler(Logger logger,string id, string clientName, MessageStore store)
        {
            _logger = logger;

            _consoleViewer = new ConsoleViewer(_logger);

            Id = id;
            ClientName = clientName;

            _store = store;
            _store.NewMessage += Store_NewMessage;

            _isAlive = true;
        }

        private void Store_NewMessage(ChatMessage message)
        {
            _logger.Info($"[{ClientName}] New message received '{message}'");

            //if(message.Name == ClientName)
            //{
            //    _logger.Info($"[{ClientName}] It is client's message, send message to client is not need");
            //    return;
            //}

            try
            {
                _logger.Info($"[{ClientName}] Sending message to clien...");
                StreamController streamController = new StreamController(_messagesToUserPipe);

                streamController.SendMessage(message);

                _consoleViewer.WriteMessageToConsole(message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                _isAlive = false;
            }
        }

        public void Start()
        {
            _logger.Info($"[{ClientName}] Starting MessagesFromUser thread");
            _messagesFromUserThread = new Thread(MessagesFromUserThreadFunc);
            _messagesFromUserThread.Start();

            _logger.Info($"[{ClientName}] Starting MessagesToUser thread");
            _messagesToUserThread = new Thread(MessagesToUserThreadFunc);
            _messagesToUserThread.Start();
        }

        public void Stop()
        {
            _isAlive = false;
        }

        public void MessagesFromUserThreadFunc()
        {
            _logger.Info($"[{ClientName}] Initializing messagesFromUser pipe...");
            _messagesFromUserPipe = new NamedPipeServerStream(PipeNameHelper.GetMessagesFromUserPipeName(Id), PipeDirection.In);

            _logger.Info($"[{ClientName}] Waiting for connection to messagesFromUser pipe...");
            _messagesFromUserPipe.WaitForConnection();
            _logger.Info($"[{ClientName}] Client connected to messagesFromUser pipe...");

            try
            {
                while (_isAlive)
                {
                    StreamController streamController = new StreamController(_messagesFromUserPipe);

                    _logger.Info($"[{ClientName}] Waiting message from client...");
                    var chatMessage = streamController.ReceiveMessage();

                    _logger.Info($"[{ClientName}] Message from client '{chatMessage}' received");
                    _logger.Info($"[{ClientName}] Add message to store");

                    _store.AddNewMessage(chatMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                _isAlive = false;
            }           
        }

        public void MessagesToUserThreadFunc()
        {
            _logger.Info($"[{ClientName}] Initializing messagesToUser pipe...");
            _messagesToUserPipe = new NamedPipeServerStream(PipeNameHelper.GetMessagesToUserPipeName(Id), PipeDirection.Out);

            _logger.Info($"[{ClientName}] Waiting for connection to messagesToUser pipe...");
            _messagesToUserPipe.WaitForConnection();
            _logger.Info($"[{ClientName}] Client connected to messagesToUser pipe...");
        }
    }
}
