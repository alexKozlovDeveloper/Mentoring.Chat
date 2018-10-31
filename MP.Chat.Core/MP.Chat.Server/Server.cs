using MP.Chat.Core;
using MP.Chat.Core.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MP.Chat.Server
{
    public class Server
    {
        private readonly MessageStore _messageStore;
        private readonly List<ClientHandler> _clientHandlers;

        private Logger _logger;

        private object locker = new object();
        private Thread _removeDisconnectedClientsThread;

        public Server(Logger logger)
        {
            _logger = logger;

            _messageStore = new MessageStore(_logger);
            _clientHandlers = new List<ClientHandler>();

            _removeDisconnectedClientsThread = new Thread(RemoveDisconnectedClientsFunc);
        }

        public void StartListening()
        {
            _removeDisconnectedClientsThread.Start();

            _logger.Info("Creating listening pipe...");
            NamedPipeServerStream pipeServer = new NamedPipeServerStream(Constant.ServerListeningPipeName, PipeDirection.InOut);

            while (true)
            {
                _logger.Info("Waiting for new connection...");
                pipeServer.WaitForConnection();

                _logger.Info("New client connected");

                try
                {
                    StreamController streamController = new StreamController(pipeServer);

                    _logger.Info("Reading message from user...");
                    var chatMessage = streamController.ReceiveMessage();

                    _logger.Info($"User name '{chatMessage.Name}', command '{chatMessage.Command}'");

                    if (chatMessage.Command == ChatCommand.Login)
                    {
                        _logger.Info($"[{chatMessage.Name}] Creating user id...");
                        var id = RandomHelper.Random.Next(0, 1000).ToString();

                        _logger.Info($"[{chatMessage.Name}] User Id '{id}'");
                        _logger.Info($"[{chatMessage.Name}|{id}] Creating client handler...");

                        _messageStore.AddNewMessage(new ChatMessage
                        {
                            Name = "Server",
                            Content = $"A new user '{chatMessage.Name}' was connected."
                        });

                        var clientHandler = new ClientHandler(_logger, id, chatMessage.Name, _messageStore);

                        _logger.Info($"[{chatMessage.Name}|{id}] Starting client handler...");
                        clientHandler.Start();

                        lock (locker)
                        {
                            _clientHandlers.Add(clientHandler);
                        }

                        _logger.Info($"[{chatMessage.Name}|{id}] Sending approve message to client with user id...");

                        var infoMessageToClient = new ChatMessage()
                        {
                            Content = id
                        };

                        streamController.SendMessage(infoMessageToClient);
                        _logger.Info($"[{chatMessage.Name}|{id}] Disconnecting...");

                        pipeServer.Disconnect();
                    }

                    if (chatMessage.Command == ChatCommand.GetMessagesHistory)
                    {
                        _logger.Info($"[{chatMessage.Name}] Sending messages history to client...");

                        var infoMessageToClient = new ChatMessage()
                        {
                            Command = ChatCommand.GetMessagesHistory,
                            Content = JsonConvert.SerializeObject(_messageStore.Messages)
                        };

                        streamController.SendMessage(infoMessageToClient);
                        _logger.Info($"[{chatMessage.Name}] Disconnecting...");

                        pipeServer.Disconnect();
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("ERROR: {0}", e.Message);
                }
            }
        }

        private void RemoveDisconnectedClientsFunc()
        {
            while (true)
            {
                var messages = new List<ChatMessage>();

                //_logger.Info("Check disconnected clients");
                lock (locker)
                {
                    var disconnectedClients = _clientHandlers.Where(a => a.IsAlive == false).ToList();

                    foreach (var item in disconnectedClients)
                    {
                        item.Stop();
                        _clientHandlers.Remove(item);
                        messages.Add(new ChatMessage("Server", $"Client '{item.ClientName}' has been disconnected"));                        
                    }
                }

                foreach (var message in messages)
                {
                    _messageStore.AddNewMessage(message);
                }

                Thread.Sleep(100);
            }
        }
    }
}
