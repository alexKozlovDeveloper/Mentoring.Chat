﻿using MP.Chat.Core;
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
        private MessageStore _messageStore;
        private List<ClientHandler> _clientHandlers;

        private Logger _logger;

        public Server(Logger logger)
        {
            _logger = logger;

            _messageStore = new MessageStore(_logger);
            _clientHandlers = new List<ClientHandler>();
        }

        public void StartListening()
        {
            _logger.Info("Creating listening pipe...");
            NamedPipeServerStream pipeServer = new NamedPipeServerStream(Constant.ServerListeningPipeName, PipeDirection.InOut, 1);

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

                    _logger.Info($"User name '{chatMessage.Name}'");

                    if (chatMessage.Command == ChatCommand.RegisterUser)
                    {
                        _logger.Info($"[{chatMessage.Name}] 'RegisterUser' command received");
                        _logger.Info($"[{chatMessage.Name}] Creating user id...");
                        var id = RandomHelper.Random.Next(0, 1000).ToString();

                        _logger.Info($"[{chatMessage.Name}] User Id '{id}'");
                        _logger.Info($"[{chatMessage.Name}|{id}] Creating client handler...");

                        var clientHandler = new ClientHandler(_logger, id, chatMessage.Name, _messageStore);

                        _logger.Info($"[{chatMessage.Name}|{id}] Starting client handler...");
                        clientHandler.Start();

                        _logger.Info($"[{chatMessage.Name}|{id}] Sending approve message to client with user id...");

                        var infoMessageToClient = new ChatMessage()
                        {
                            Command = ChatCommand.RegisterApproved,
                            Content = id
                        };

                        streamController.SendMessage(infoMessageToClient);
                        _logger.Info($"[{chatMessage.Name}|{id}] Disconnecting...");

                        pipeServer.Disconnect();
                    }

                    if (chatMessage.Command == ChatCommand.GetMessagesStory)
                    {
                        _logger.Info($"[{chatMessage.Name}] 'GetMessagesStory' command received");

                        _logger.Info($"[{chatMessage.Name}] Sending messages history to client...");

                        var infoMessageToClient = new ChatMessage()
                        {
                            Command = ChatCommand.GetMessagesStory,
                            Content = JsonConvert.SerializeObject(_messageStore.Messages)
                        };

                        streamController.SendMessage(infoMessageToClient);
                        _logger.Info($"[{chatMessage.Name}] Disconnecting...");

                        pipeServer.Disconnect();
                    }
                }
                // Catch the IOException that is raised if the pipe is broken
                // or disconnected.
                catch (IOException e)
                {
                    Console.WriteLine("ERROR: {0}", e.Message);
                }
                // Console.WriteLine("Closing...");
                //pipeServer.Close();

            }
        }

        public void ClientThreadFunc()
        {

        }
    }
}
